using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DungeonMenuSetup
{
    private const string MainPath = "Assets/Scenes/MainMenu.unity";
    private const string GamePath = "Assets/Scenes/Dungeon_PrototypeUnityAI.unity";

    [MenuItem("Tools/Dungeon/Set Up Menus")]
    public static void Setup()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        { Debug.LogWarning("Stop Play Mode before setting up menus."); return; }
        if (!File.Exists(GamePath))
        { Debug.LogError("Required gameplay scene missing: " + GamePath); return; }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        ConfigureBackground();
        PlayerSettings.productName = DungeonMenuController.GameTitle;
        if (!File.Exists(MainPath))
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var camera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            camera.tag = "MainCamera";
            camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            camera.GetComponent<Camera>().backgroundColor = Color.black;
            camera.transform.position = new Vector3(0, 0, -10);
            EditorSceneManager.SaveScene(scene, MainPath);
        }
        else EditorSceneManager.OpenScene(MainPath, OpenSceneMode.Single);

        var scenes = new List<EditorBuildSettingsScene>
        {
            new EditorBuildSettingsScene(MainPath, true),
            new EditorBuildSettingsScene(GamePath, true)
        };
        foreach (var scene in EditorBuildSettings.scenes)
            if (scene.path != MainPath && scene.path != GamePath) scenes.Add(scene);
        EditorBuildSettings.scenes = scenes.ToArray();
        AssetDatabase.SaveAssets();
        Debug.Log("Menus configured. Press Play in MainMenu. For a Build Profile with an overridden scene list, include MainMenu first and Dungeon_PrototypeUnityAI second there too.");
    }

    private static void ConfigureBackground()
    {
        const string path = "Assets/Resources/DungeonMenus/main_menu_background.png";
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) { Debug.LogWarning("Menu background missing: " + path); return; }
        importer.textureType = TextureImporterType.Default;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.maxTextureSize = 2048;
        importer.SaveAndReimport();
    }
}
