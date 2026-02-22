using System.Collections.Generic;
using UnityEngine;

public class MeshAfterimagePool : MonoBehaviour
{
    // Renderitzadors "font" des d'on es cou la pose animada a una malla estàtica.
    [Header("Fonts (auto)")]
    [Tooltip("Si ho deixes buit, buscarà SkinnedMeshRenderer en aquest GameObject o en els fills.")]
    [SerializeField] private SkinnedMeshRenderer[] sources;

    [Header("Visual del fantasma")]
    [SerializeField] private Material ghostMaterial;
    [Tooltip("Nom del paràmetre de color del shader. A URP sol ser _BaseColor. A Standard, _Color.")]
    [SerializeField] private string colorProperty = "_BaseColor";

    [Header("Temps")]
    [Min(0.001f)] [SerializeField] private float spawnInterval = 0.05f;
    [Min(0.01f)] [SerializeField] private float lifeTime = 0.4f;

    [Header("Pool")]
    [Min(1)] [SerializeField] private int initialPoolSize = 16;
    [SerializeField] private bool canExpandPool = true;

    [Header("Esvaïment")]
    [Range(0f, 1f)] [SerializeField] private float startAlpha = 0.75f;
    [Tooltip("Corba d'alpha (t=0->1). Si està buida, esvaïment lineal.")]
    [SerializeField] private AnimationCurve alphaCurve;

    [Header("Opcions")]
    [SerializeField] private bool emitWhenMovingOnly = false;
    [SerializeField] private float minDistanceToEmit = 0.02f;

    private float timer;
    private int colorId;
    private Vector3 lastEmitPosition;
    private float minDistanceToEmitSqr;
    private Material cachedMaterialForColor;
    private Color cachedBaseColor = Color.white;

    private readonly Queue<GhostInstance> pool = new();
    private readonly List<GhostInstance> active = new();

    private static readonly int DefaultColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int LegacyColorId = Shader.PropertyToID("_Color");
    private const float UnitScaleEpsilon = 0.0005f;
    private const float CompensationDeadZone = 0.05f;
    private const float CompensationMin = 0.05f;
    private const float CompensationMax = 20f;

    private void Awake()
    {
        // Si no hi ha fonts definides a l'inspector, les busquem automàticament.
        if (sources == null || sources.Length == 0)
        {
            sources = GetComponentsInChildren<SkinnedMeshRenderer>(true);
        }

        RefreshCachedSettings();

        lastEmitPosition = transform.position;
        // Preescalfem el pool per evitar allocs en moments de joc.
        WarmPool();
    }

    private void OnValidate()
    {
        if (spawnInterval < 0.001f) spawnInterval = 0.001f;
        if (lifeTime < 0.01f) lifeTime = 0.01f;
        if (initialPoolSize < 1) initialPoolSize = 1;
        if (minDistanceToEmit < 0f) minDistanceToEmit = 0f;

        RefreshCachedSettings();
    }

    private void RefreshCachedSettings()
    {
        // Resol l'ID del color una sola vegada per evitar lookups per frame.
        colorId = Shader.PropertyToID(colorProperty);
        if (colorId == 0)
        {
            colorId = DefaultColorId;
        }

        minDistanceToEmitSqr = minDistanceToEmit * minDistanceToEmit;
        RefreshCachedBaseColor();
    }

    private void RefreshCachedBaseColor()
    {
        cachedMaterialForColor = ghostMaterial;
        cachedBaseColor = Color.white;

        if (cachedMaterialForColor == null)
        {
            return;
        }

        if (cachedMaterialForColor.HasProperty(colorId))
        {
            cachedBaseColor = cachedMaterialForColor.GetColor(colorId);
        }
        else if (cachedMaterialForColor.HasProperty(LegacyColorId))
        {
            cachedBaseColor = cachedMaterialForColor.GetColor(LegacyColorId);
        }
    }

    private void OnDisable()
    {
        // Retorna tots els "ghosts" actius al pool quan es desactiva el component.
        for (int i = active.Count - 1; i >= 0; i--)
        {
            ReturnToPool(active[i]);
        }
        active.Clear();
        timer = 0f;
    }

    private void OnDestroy()
    {
        // Allibera explícitament malles i gameobjects del pool.
        foreach (GhostInstance g in active)
        {
            DisposeGhost(g);
        }
        active.Clear();

        while (pool.Count > 0)
        {
            DisposeGhost(pool.Dequeue());
        }
    }

    private void Update()
    {
        // Primer actualitzem esvaïment i lifetime dels ghosts ja actius.
        UpdateActiveGhosts();

        if (ghostMaterial == null || sources == null || sources.Length == 0)
        {
            return;
        }

        if (emitWhenMovingOnly)
        {
            float sqrDist = (transform.position - lastEmitPosition).sqrMagnitude;
            if (sqrDist < minDistanceToEmitSqr)
            {
                return;
            }
        }

        timer += Time.deltaTime;
        while (timer >= spawnInterval)
        {
            timer -= spawnInterval;
            SpawnSnapshot();
            lastEmitPosition = transform.position;
        }
    }

    private void WarmPool()
    {
        // Crea instàncies inicials per minimitzar GC i pics de CPU.
        for (int i = 0; i < initialPoolSize; i++)
        {
            pool.Enqueue(CreateGhost());
        }
    }

    private GhostInstance CreateGhost()
    {
        GameObject go = new("AfterimageGhost");
        go.hideFlags = HideFlags.DontSave;
        go.SetActive(false);

        MeshFilter meshFilter = go.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
        meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

        Mesh bakedMesh = new();
        bakedMesh.name = "AfterimageBakedMesh";
        bakedMesh.MarkDynamic();

        meshFilter.sharedMesh = bakedMesh;
        meshRenderer.sharedMaterial = ghostMaterial;

        return new GhostInstance
        {
            go = go,
            meshFilter = meshFilter,
            meshRenderer = meshRenderer,
            bakedMesh = bakedMesh,
            mpb = new MaterialPropertyBlock(),
            timeLeft = 0f
        };
    }

    private GhostInstance GetFromPool()
    {
        // Política: pool -> expandir -> reciclar el més antic.
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }

        if (canExpandPool)
        {
            return CreateGhost();
        }

        if (active.Count > 0)
        {
            GhostInstance oldest = active[0];
            active.RemoveAt(0);
            return oldest;
        }

        return CreateGhost();
    }

    private void ReturnToPool(GhostInstance ghost)
    {
        ghost.go.SetActive(false);
        ghost.timeLeft = 0f;
        pool.Enqueue(ghost);
    }

    private void SpawnSnapshot()
    {
        for (int i = 0; i < sources.Length; i++)
        {
            SkinnedMeshRenderer smr = sources[i];
            if (smr == null || !smr.enabled || !smr.gameObject.activeInHierarchy)
            {
                continue;
            }

            GhostInstance ghost = GetFromPool();

            // Copiem transform global i ajustem escala de manera robusta segons com hagi "bakejat" Unity.
            Transform t = smr.transform;
            ghost.go.transform.SetParent(null, false);
            ghost.go.transform.SetPositionAndRotation(t.position, t.rotation);
            ghost.go.transform.localScale = t.lossyScale;

            ghost.bakedMesh.Clear(false);
            smr.BakeMesh(ghost.bakedMesh, false);

            // Només compensa quan el source té escala global no unitària.
            if (!IsApproxUnitScale(t.lossyScale))
            {
                if (ApplyScaleCompensation(ghost, smr))
                {
                    AlignCenters(ghost, smr);
                }
            }

            if (ghost.meshRenderer.sharedMaterial != ghostMaterial)
            {
                ghost.meshRenderer.sharedMaterial = ghostMaterial;
            }
            ghost.timeLeft = lifeTime;

            ApplyAlpha(ghost, startAlpha);
            ghost.go.SetActive(true);

            active.Add(ghost);
        }
    }

    private static bool ApplyScaleCompensation(GhostInstance ghost, SkinnedMeshRenderer source)
    {
        const float epsilon = 0.0001f;
        float sourceWorldRadius = source.bounds.extents.magnitude;
        float ghostWorldRadius = GetWorldBoundsRadius(ghost.bakedMesh.bounds, ghost.go.transform.localToWorldMatrix);

        if (sourceWorldRadius <= epsilon || ghostWorldRadius <= epsilon)
        {
            return false;
        }

        float compensation = sourceWorldRadius / ghostWorldRadius;
        if (Mathf.Abs(compensation - 1f) <= CompensationDeadZone)
        {
            return false;
        }

        compensation = Mathf.Clamp(compensation, CompensationMin, CompensationMax);
        ghost.go.transform.localScale *= compensation;
        return true;
    }

    private static bool IsApproxUnitScale(Vector3 scale)
    {
        return Mathf.Abs(scale.x - 1f) <= UnitScaleEpsilon
            && Mathf.Abs(scale.y - 1f) <= UnitScaleEpsilon
            && Mathf.Abs(scale.z - 1f) <= UnitScaleEpsilon;
    }

    private static float GetWorldBoundsRadius(Bounds localBounds, Matrix4x4 localToWorld)
    {
        Vector3 e = localBounds.extents;

        float ex = Mathf.Abs(localToWorld.m00) * e.x + Mathf.Abs(localToWorld.m01) * e.y + Mathf.Abs(localToWorld.m02) * e.z;
        float ey = Mathf.Abs(localToWorld.m10) * e.x + Mathf.Abs(localToWorld.m11) * e.y + Mathf.Abs(localToWorld.m12) * e.z;
        float ez = Mathf.Abs(localToWorld.m20) * e.x + Mathf.Abs(localToWorld.m21) * e.y + Mathf.Abs(localToWorld.m22) * e.z;

        return new Vector3(ex, ey, ez).magnitude;
    }

    private static void AlignCenters(GhostInstance ghost, SkinnedMeshRenderer source)
    {
        Vector3 ghostCenterWorld = ghost.go.transform.TransformPoint(ghost.bakedMesh.bounds.center);
        Vector3 delta = source.bounds.center - ghostCenterWorld;
        ghost.go.transform.position += delta;
    }

    private void UpdateActiveGhosts()
    {
        if (active.Count == 0)
        {
            return;
        }

        float dt = Time.deltaTime;

        for (int i = active.Count - 1; i >= 0; i--)
        {
            GhostInstance ghost = active[i];
            ghost.timeLeft -= dt;

            // Quan expira, es recicla cap al pool.
            if (ghost.timeLeft <= 0f)
            {
                active.RemoveAt(i);
                ReturnToPool(ghost);
                continue;
            }

            float t01 = 1f - (ghost.timeLeft / lifeTime);
            float curve = (alphaCurve != null && alphaCurve.length > 0)
                ? alphaCurve.Evaluate(t01)
                : (1f - t01);
            float alpha = Mathf.Clamp01(startAlpha * curve);

            ApplyAlpha(ghost, alpha);
        }
    }

    private void ApplyAlpha(GhostInstance ghost, float alpha)
    {
        // Manté el color base del material i només modifica l'alpha via MPB.
        if (cachedMaterialForColor != ghostMaterial)
        {
            RefreshCachedBaseColor();
        }

        Color baseColor = cachedBaseColor;
        baseColor.a = Mathf.Clamp01(alpha);

        ghost.mpb.SetColor(colorId, baseColor);
        ghost.mpb.SetColor(LegacyColorId, baseColor);
        ghost.meshRenderer.SetPropertyBlock(ghost.mpb);
    }

    private static void DisposeGhost(GhostInstance ghost)
    {
        if (ghost == null)
        {
            return;
        }

        if (ghost.bakedMesh != null)
        {
            Object.Destroy(ghost.bakedMesh);
        }

        if (ghost.go != null)
        {
            Object.Destroy(ghost.go);
        }
    }

    private class GhostInstance
    {
        public GameObject go;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public Mesh bakedMesh;
        public MaterialPropertyBlock mpb;
        public float timeLeft;
    }
}
