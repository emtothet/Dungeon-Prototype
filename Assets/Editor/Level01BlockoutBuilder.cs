using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Level01BlockoutBuilder
{
    private const string ScenePath = "Assets/Scenes/Cendrelith_Level01_Blockout.unity";
    private const string ArtPath = "Assets/Art/Blockout";
    private static readonly RectInt[] Rooms = {
        new RectInt(0,0,14,12), new RectInt(0,16,12,10), new RectInt(0,30,22,10),
        new RectInt(26,32,8,8), new RectInt(4,46,16,12), new RectInt(26,16,10,8)
    };
    private static readonly RectInt[] Corridors = {
        new RectInt(4,12,4,4), new RectInt(4,26,4,4), new RectInt(22,34,4,4),
        new RectInt(8,40,4,6), new RectInt(20,48,22,4), new RectInt(38,18,4,34),
        new RectInt(36,18,6,4), new RectInt(18,18,8,4), new RectInt(16,6,4,16), new RectInt(14,6,6,4)
    };
    private static Sprite square;
    private static Transform geometry;

    [MenuItem("Tools/Cendrelith/Create Level 01 Blockout")]
    public static void Build()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (File.Exists(ScenePath))
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) EditorSceneManager.OpenScene(ScenePath);
            Debug.Log("Opened existing blockout. No changes overwritten.");
            return;
        }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        square = GetSquare();
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        geometry = new GameObject("Blockout geometry").transform;
        var walkable = new HashSet<Vector2Int>();
        string[] names = {"REFUGE", "SEALED VESTIBULE", "GUARD GALLERY", "SUPPLY STORE", "INFIRMARY", "OBSERVATION"};
        for (int i=0;i<Rooms.Length;i++)
        {
            var r=Rooms[i];
            Fill(walkable,r);
            Color color = i==0 ? new Color(0.25f,0.21f,0.16f) : new Color(0.15f+i*0.012f,0.17f,0.19f);
            Visual(names[i]+" floor",r.center,r.size,color,-10);
            Label(names[i],new Vector2(r.center.x,r.yMax-1.1f));
        }
        foreach(var r in Corridors) { Fill(walkable,r); Visual("Passage",r.center,r.size,new Color(0.12f,0.14f,0.16f),-11); }
        // Boundary edges of the floor union form walls with actual door openings.
        foreach(var cell in walkable)
        {
            if(!walkable.Contains(cell+Vector2Int.left)) Wall(new Vector2(cell.x,cell.y+0.5f),new Vector2(0.24f,1.24f));
            if(!walkable.Contains(cell+Vector2Int.right)) Wall(new Vector2(cell.x+1,cell.y+0.5f),new Vector2(0.24f,1.24f));
            if(!walkable.Contains(cell+Vector2Int.down)) Wall(new Vector2(cell.x+0.5f,cell.y),new Vector2(1.24f,0.24f));
            if(!walkable.Contains(cell+Vector2Int.up)) Wall(new Vector2(cell.x+0.5f,cell.y+1),new Vector2(1.24f,0.24f));
        }
        Label("DOUBLE DOOR",new Vector2(6,14));
        Visual("Door leaf left",new Vector2(4.2f,13),new Vector2(0.3f,1.8f),new Color(0.45f,0.3f,0.12f),1);
        Visual("Door leaf right",new Vector2(7.8f,13),new Vector2(0.3f,1.8f),new Color(0.45f,0.3f,0.12f),1);

        var hero = Visual("Blockout Hero",new Vector2(6,6),new Vector2(0.8f,0.8f),new Color(0.7f,0.8f,0.9f),5);
        Body(hero);
        var hp=hero.AddComponent<PlayerHealth>(); hp.maxHealth=hp.health=10;
        hero.AddComponent<PlayerMovement>();
        hero.AddComponent<PlayerAttack>();
        var manager = new GameObject("Game Manager").AddComponent<GameManager>();
        var logic = new GameObject("Expedition test").AddComponent<Level01Blockout>();
        logic.player=hp;
        logic.caregiver=Visual("Caregiver interaction",new Vector2(4,6),Vector2.one,new Color(0.85f,0.7f,0.3f),3).transform;
        logic.chest=Visual("Test weapon chest",new Vector2(31,38),Vector2.one,new Color(0.7f,0.43f,0.16f),3).transform;
        logic.remedy=Visual("Remedy and oracle note",new Vector2(30,20),Vector2.one,new Color(0.35f,0.8f,0.55f),3).transform;
        logic.shortcutGate=Visual("Return gate - opens from east",new Vector2(25,20),new Vector2(0.4f,4),new Color(0.8f,0.48f,0.15f),3);
        logic.shortcutGate.AddComponent<BoxCollider2D>();
        Enemy("Gallery skeleton 1",new Vector2(6,33),Rooms[2],hp);
        Enemy("Gallery skeleton 2",new Vector2(15,35),Rooms[2],hp);
        Enemy("Gallery skeleton 3",new Vector2(18,36),Rooms[2],hp);
        logic.storeGuard=Enemy("Store skeleton",new Vector2(29,35),Rooms[3],hp);
        logic.infirmaryEnemies=new[] {
            Enemy("Infirmary skeleton 1",new Vector2(8,49),Rooms[4],hp),
            Enemy("Infirmary skeleton 2",new Vector2(16,50),Rooms[4],hp),
            Enemy("Necromancer melee proxy",new Vector2(12,54),Rooms[4],hp,true)
        };
        var cameraObject=new GameObject("Main Camera",typeof(Camera),typeof(AudioListener));
        cameraObject.tag="MainCamera";
        var camera=cameraObject.GetComponent<Camera>();
        camera.orthographic=true; camera.orthographicSize=8;
        camera.clearFlags=CameraClearFlags.SolidColor;
        camera.backgroundColor=new Color(0.035f,0.035f,0.04f);
        camera.transform.position=new Vector3(6,6,-10);
        logic.viewCamera=camera;
        EditorSceneManager.SaveScene(scene,ScenePath);
        var builds=new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        builds.RemoveAll(s=>s.path==ScenePath);
        builds.Add(new EditorBuildSettingsScene(ScenePath,true));
        EditorBuildSettings.scenes=builds.ToArray();
        Selection.activeGameObject=hero;
        Debug.Log("Level 01 blockout created. Press Play; E at caregiver; WASD, Space, M overview. Scene is added to the shared build list for Restart. Check any Build Profile override separately.");
    }

    private static void Fill(HashSet<Vector2Int> cells,RectInt rect)
    { foreach(var position in rect.allPositionsWithin) cells.Add(position); }

    private static GameObject Visual(string name,Vector2 position,Vector2 size,Color color,int order)
    {
        var obj=new GameObject(name);
        obj.transform.SetParent(geometry,false);
        obj.transform.position=position;
        obj.transform.localScale=new Vector3(size.x,size.y,1);
        var sr=obj.AddComponent<SpriteRenderer>(); sr.sprite=square; sr.color=color; sr.sortingOrder=order;
        return obj;
    }

    private static void Wall(Vector2 position,Vector2 size)
    { Visual("Wall",position,size,new Color(0.34f,0.35f,0.37f),1).AddComponent<BoxCollider2D>(); }

    private static void Body(GameObject obj)
    {
        var body=obj.AddComponent<Rigidbody2D>();
        body.gravityScale=0; body.constraints=RigidbodyConstraints2D.FreezeRotation;
        body.collisionDetectionMode=CollisionDetectionMode2D.Continuous;
        body.interpolation=RigidbodyInterpolation2D.Interpolate;
        obj.AddComponent<BoxCollider2D>();
    }

    private static EnemyHealth Enemy(string name,Vector2 position,RectInt room,PlayerHealth player,bool boss=false)
    {
        var obj=Visual(name,position,boss ? new Vector2(1,1) : new Vector2(0.8f,0.8f),
            boss ? new Color(0.5f,0.25f,0.6f) : new Color(0.7f,0.65f,0.5f),4);
        Body(obj);
        var health=obj.AddComponent<EnemyHealth>(); health.maxHealth=boss?7:3; health.experienceReward=boss?30:10;
        var ai=obj.AddComponent<BlockoutEnemy>(); ai.player=player; ai.homeRoom=new Rect(room.x,room.y,room.width,room.height);
        if(boss) { ai.speed=1.1f; ai.detectionRange=8; ai.attackInterval=1.8f; }
        return health;
    }

    private static void Label(string text,Vector2 position)
    {
        var obj=new GameObject(text); obj.transform.SetParent(geometry,false); obj.transform.position=position;
        var mesh=obj.AddComponent<TextMesh>(); mesh.text=text; mesh.characterSize=0.14f; mesh.fontSize=40;
        mesh.anchor=TextAnchor.MiddleCenter; mesh.color=new Color(0.8f,0.8f,0.8f);
        obj.GetComponent<MeshRenderer>().sortingOrder=2;
    }

    private static Sprite GetSquare()
    {
        Directory.CreateDirectory(ArtPath);
        const string path=ArtPath+"/blockout_square.png";
        if(!File.Exists(path))
        {
            var texture=new Texture2D(2,2,TextureFormat.RGBA32,false);
            texture.SetPixels(new[]{Color.white,Color.white,Color.white,Color.white}); texture.Apply();
            File.WriteAllBytes(path,texture.EncodeToPNG()); Object.DestroyImmediate(texture);
        }
        AssetDatabase.ImportAsset(path);
        var importer=(TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType=TextureImporterType.Sprite; importer.spriteImportMode=SpriteImportMode.Single;
        importer.spritePixelsPerUnit=2; importer.filterMode=FilterMode.Point; importer.mipmapEnabled=false;
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
}
