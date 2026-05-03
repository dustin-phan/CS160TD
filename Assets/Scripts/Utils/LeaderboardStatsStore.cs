using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int round;
}

[Serializable]
public class LeaderboardStatsFile
{
    public LeaderboardEntry[] entries;
}

public static class LeaderboardStatsStore
{
    public const string FileName = "leaderboard_stats.json";

    private static string PersistentPath => System.IO.Path.Combine(Application.persistentDataPath, FileName);
    private static string SeedPath => System.IO.Path.Combine(Application.streamingAssetsPath, FileName);

    /// <summary>Copies seed from StreamingAssets to persistent data on first run (same file we read/write later).</summary>
    public static void EnsureFileExists()
    {
        if (File.Exists(PersistentPath))
            return;

        try
        {
            if (File.Exists(SeedPath))
            {
                string dir = System.IO.Path.GetDirectoryName(PersistentPath);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);
                File.Copy(SeedPath, PersistentPath);
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Leaderboard seed copy failed: {e.Message}");
        }

        WriteFile(CreateDefaultFile());
    }

    public static LeaderboardStatsFile Load()
    {
        EnsureFileExists();
        try
        {
            string json = File.ReadAllText(PersistentPath);
            if (string.IsNullOrWhiteSpace(json))
                return CreateDefaultFile();
            var file = JsonUtility.FromJson<LeaderboardStatsFile>(json);
            if (file == null || file.entries == null)
                return CreateDefaultFile();
            return file;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Leaderboard load failed, using defaults: {e.Message}");
            return CreateDefaultFile();
        }
    }

    public static List<LeaderboardEntry> GetTopEntries(int count)
    {
        var file = Load();
        return file.entries
            .OrderByDescending(e => e.round)
            .ThenBy(e => e.playerName, StringComparer.OrdinalIgnoreCase)
            .Take(count)
            .ToList();
    }

    public static void AppendGameOverEntry(string playerName, int round)
    {
        EnsureFileExists();
        string name = string.IsNullOrWhiteSpace(playerName) ? "Default Player" : playerName.Trim();

        var file = Load();
        var list = new List<LeaderboardEntry>(
            file.entries ?? Array.Empty<LeaderboardEntry>());
        list.Add(new LeaderboardEntry { playerName = name, round = round });

        list = list
            .OrderByDescending(e => e.round)
            .ThenBy(e => e.playerName, StringComparer.OrdinalIgnoreCase)
            .Take(100)
            .ToList();

        file.entries = list.ToArray();
        WriteFile(file);
    }

    private static void WriteFile(LeaderboardStatsFile file)
    {
        if (file.entries == null)
            file.entries = Array.Empty<LeaderboardEntry>();
        string dir = System.IO.Path.GetDirectoryName(PersistentPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllText(PersistentPath, JsonUtility.ToJson(file, true));
    }

    private static LeaderboardStatsFile CreateDefaultFile()
    {
        return new LeaderboardStatsFile
        {
            entries = new[]
            {
                new LeaderboardEntry { playerName = "TowerMaster", round = 24 },
                new LeaderboardEntry { playerName = "OrcSlayer99", round = 21 },
                new LeaderboardEntry { playerName = "PathFinder", round = 18 },
            }
        };
    }
}
