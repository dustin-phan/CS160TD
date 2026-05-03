using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioSetup : Editor
{
    [MenuItem("Tools/Setup Audio")]
    public static void SetupAudio()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            bool proceed = EditorUtility.DisplayDialog(
                "Wrong Scene",
                "SoundManager and MusicManager should live in the MainMenu scene so they persist across all levels.\n\nOpen MainMenu scene first, then run this again.\n\nProceed anyway in the current scene?",
                "Proceed Anyway", "Cancel");
            if (!proceed) return;
        }

        // --- Load all audio clips ---
        var archerShoot  = Load("Assets/Audio/archer_shoot.ogg");
        var knightShoot  = Load("Assets/Audio/knight_shoot.wav");
        var wizardShoot  = Load("Assets/Audio/wizard_shoot.ogg");
        var enemyDeath   = Load("Assets/Audio/enemy_death.ogg");
        var enemyBreach  = Load("Assets/Audio/enemy_breach.ogg");
        var towerPlace   = Load("Assets/Audio/tower_place.ogg");
        var towerDestroy = Load("Assets/Audio/tower_destroy.ogg");
        var waveStart    = Load("Assets/Audio/wave_start.ogg");
        var gameOver     = Load("Assets/Audio/game_over.ogg");
        var victory      = Load("Assets/Audio/victory.ogg");
        var musicBg      = Load("Assets/Audio/music_bg.ogg");
        var buttonClick  = Load("Assets/Audio/button_click.wav");

        // --- SoundManager ---
        SoundManager sm = SetupManager<SoundManager>("SoundManager");
        var smSO = new SerializedObject(sm);
        smSO.FindProperty("enemyDeathSound").objectReferenceValue   = enemyDeath;
        smSO.FindProperty("enemyBreachSound").objectReferenceValue  = enemyBreach;
        smSO.FindProperty("towerPlaceSound").objectReferenceValue   = towerPlace;
        smSO.FindProperty("towerDestroySound").objectReferenceValue = towerDestroy;
        smSO.FindProperty("waveStartSound").objectReferenceValue    = waveStart;
        smSO.FindProperty("gameOverSound").objectReferenceValue     = gameOver;
        smSO.FindProperty("victorySound").objectReferenceValue      = victory;
        smSO.FindProperty("buttonClickSound").objectReferenceValue  = buttonClick;
        smSO.ApplyModifiedProperties();

        // --- MusicManager ---
        MusicManager mm = SetupManager<MusicManager>("MusicManager");
        var mmSO = new SerializedObject(mm);
        mmSO.FindProperty("backgroundMusic").objectReferenceValue = musicBg;
        mmSO.ApplyModifiedProperties();

        // --- TowerData shoot sounds ---
        SetShootSound("Assets/ScriptableObjects/Tower/ArcherTower/ArcherTower.asset",  archerShoot);
        SetShootSound("Assets/ScriptableObjects/Tower/ArcherTower/ArcherTower2.asset", archerShoot);
        SetShootSound("Assets/ScriptableObjects/Tower/KnightTree/KnightTower.asset",   knightShoot);
        SetShootSound("Assets/ScriptableObjects/Tower/KnightTree/KnightTower2.asset",  knightShoot);
        SetShootSound("Assets/ScriptableObjects/Tower/KnightTree/KnightTower3.asset",  knightShoot);
        SetShootSound("Assets/ScriptableObjects/Tower/WizardTower/WizardTower.asset",  wizardShoot);
        SetShootSound("Assets/ScriptableObjects/Tower/WizardTower/WizardTower2.asset", wizardShoot);
        SetShootSound("Assets/ScriptableObjects/Tower/WizardTower/WizardTower3.asset", wizardShoot);

        // --- Add SoundButton to all buttons in the open scene ---
        int sceneCount = AddSoundButtonsToScene();

        // --- Add SoundButton to buttons inside UI prefabs ---
        int prefabCount = 0;
        prefabCount += AddSoundButtonsToPrefab("Assets/Prefabs/UI/Button.prefab");
        prefabCount += AddSoundButtonsToPrefab("Assets/Prefabs/UI/TowerCard.prefab");
        prefabCount += AddSoundButtonsToPrefab("Assets/Prefabs/UI/DestroyCard.prefab");

        // --- Save ---
        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        EditorUtility.DisplayDialog("Audio Setup Complete",
            $"SoundManager and MusicManager created.\n" +
            $"All audio clips assigned.\n" +
            $"TowerData shoot sounds wired up.\n" +
            $"SoundButton added to {sceneCount} scene buttons and {prefabCount} prefab buttons.\n\n" +
            $"Press Play from MainMenu to test!", "OK");
    }

    private static int AddSoundButtonsToScene()
    {
        int count = 0;
        foreach (Button btn in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
        {
            if (btn.GetComponent<SoundButton>() == null)
            {
                btn.gameObject.AddComponent<SoundButton>();
                count++;
            }
        }
        return count;
    }

    private static int AddSoundButtonsToPrefab(string prefabPath)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogWarning($"[AudioSetup] Could not find prefab at: {prefabPath}");
            return 0;
        }

        int count = 0;
        foreach (Button btn in prefab.GetComponentsInChildren<Button>(true))
        {
            if (btn.GetComponent<SoundButton>() == null)
            {
                btn.gameObject.AddComponent<SoundButton>();
                count++;
            }
        }

        if (count > 0)
            PrefabUtility.SavePrefabAsset(prefab);

        return count;
    }

    private static T SetupManager<T>(string name) where T : Component
    {
        T existing = Object.FindFirstObjectByType<T>();
        GameObject go = existing != null ? existing.gameObject : new GameObject(name);
        if (go.GetComponent<T>() == null)
            go.AddComponent<T>();
        return go.GetComponent<T>();
    }

    private static AudioClip Load(string path)
    {
        AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        if (clip == null)
            Debug.LogWarning($"[AudioSetup] Could not find clip at: {path}");
        return clip;
    }

    private static void SetShootSound(string assetPath, AudioClip clip)
    {
        TowerData td = AssetDatabase.LoadAssetAtPath<TowerData>(assetPath);
        if (td == null) { Debug.LogWarning($"[AudioSetup] Could not find TowerData at: {assetPath}"); return; }
        var so = new SerializedObject(td);
        so.FindProperty("shootSound").objectReferenceValue = clip;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(td);
    }
}
