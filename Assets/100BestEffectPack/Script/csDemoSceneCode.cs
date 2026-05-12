using UnityEngine;
using UnityEngine.UI;  // <-- necesario para UI.Text
using System.Collections;

public class csDemoSceneCode : MonoBehaviour
{
    public string[] EffectNames;
    public string[] Effect2Names;
    public Transform[] Effect;
    public Text Text1;  // <-- cambiado de GUIText a Text
    int i = 0;
    int a = 0;

    void Start()
    {
        Instantiate(Effect[i], new Vector3(0, 5, 0), Quaternion.identity);
    }

    void Update()
    {
        Text1.text = i + 1 + ":" + EffectNames[i];

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (i <= 0)
                i = 99;
            else
                i--;

            InstantiateEffect();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (i < 99)
                i++;
            else
                i = 0;

            InstantiateEffect();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            InstantiateEffect();
        }
    }

    void InstantiateEffect()
    {
        for (a = 0; a < Effect2Names.Length; a++)
        {
            if (EffectNames[i] == Effect2Names[a])
            {
                Instantiate(Effect[i], new Vector3(0, 0.2f, 0), Quaternion.identity);
                return;  // <-- corregido: usaba break + if incorrecto
            }
        }
        // Si no encontró coincidencia, instancia arriba
        Instantiate(Effect[i], new Vector3(0, 5, 0), Quaternion.identity);
    }
}