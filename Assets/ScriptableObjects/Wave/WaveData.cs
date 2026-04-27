using System;
using UnityEngine;

[Serializable]
public struct WaveSpawnEntry
{
    public EnemyType enemyType;
    [Min(0)] public int count;
}

[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{
    public float spawnInterval;

    [Tooltip("How many of each enemy type appear this wave. Multiple rows mix types in one wave.")]
    public WaveSpawnEntry[] spawnComposition = Array.Empty<WaveSpawnEntry>();

    [Tooltip("When enabled, the order enemies appear is randomized each time this wave runs.")]
    public bool shuffleSpawnOrder = true;

    public int GetTotalEnemyCount()
    {
        if (spawnComposition == null) return 0;
        int sum = 0;
        foreach (var e in spawnComposition)
            sum += Mathf.Max(0, e.count);
        return sum;
    }
}
