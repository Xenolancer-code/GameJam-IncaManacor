using UnityEngine;

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "SCO/SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    public SoundClip[] soundClips;

    private void OnValidate()
    {
        foreach (SoundClip soundClip in soundClips)
        {
            if (soundClip.hasPlayTimer && soundClip.playTimer == 0)
            {
                soundClip.playTimer = soundClip.audioClip.length;
            }
        }
    }
}

/// <summary>
/// enum all the clips that you will need to enumerate in your game
/// </summary>
public enum SoundName
{
    SlashPlayer,
    AoEAtkPlayer,
    DashPlayer,
    PlayerDie,
    PlayerInjured,
    PlayerRecover,
    FootSteps,
    FootSteps2,
    Jump,
    JumpAir,
    AmbientMusic,
    AmbientMusic2,
    AmbientMusic3,
    HomeMenuMusic,
    GameMusic,
    GameMusic2,
    GameMusic3,
    SpawnerAppear,
    EliminateSmoke,
    Smoke,
    Portal,
    FireBall,
    EnemyAwarness,
    EnemyAwarness2,
    EnemyAwarness3,
    BossAwarness,
    EnemySlash,
    EnemyInjured,
    EnemyInjured2,
    EnemyInjured3,
    EnemyDie,
    EnemyDie2,
    EnemyDie3,
    BossAtk,
    BossAtk2,
    BossAtk3,
    BossAtk4,
    BossAtk5,
    BossAtk6,
    BossDie,
    BossInjured,
    BossInjured2,
    GameOver,
    Win
}

[System.Serializable]
public class SoundClip
{
    public SoundName soundName;
    public AudioClip audioClip;
    public bool loop;
    [Range(0f, 1f)]
    public float volume = 1f;
    public bool hasPlayTimer;
    public float playTimer;
    [Range(0f, 1f)]
    public float spacialBlend;
    [Range(-3f, 3f)]
    public float pitch = 1f;

    public SoundClip()
    {
        volume = 1f;
        pitch = 1f;
    }
}