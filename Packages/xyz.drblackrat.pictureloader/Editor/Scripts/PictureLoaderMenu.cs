using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DrBlackRat.VRC.PictureLoader
{
    public class Menu : MonoBehaviour
    {
        [MenuItem("Tools/Picture Loader/Open Example Scene", false, 1)]
        public static void OpenExampleScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Packages/xyz.drblackrat.pictureloader/Runtime/Example Scene.unity");
            }
        }
        [MenuItem("Tools/Picture Loader/ Add Manager Prefab to Scene", false, 2)]
        public static void AddManagerPrefab()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/xyz.drblackrat.pictureloader/Runtime/Prefabs/Picture Loader.prefab");
            PrefabUtility.InstantiatePrefab(prefab);
        }        
        [MenuItem("Tools/Picture Loader/ Add (Dark) Manager Prefab to Scene", false, 3)]
        public static void AddDarkManagerPrefab()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/xyz.drblackrat.pictureloader/Runtime/Prefabs/(Dark) Picture Loader.prefab");
            PrefabUtility.InstantiatePrefab(prefab);
        }
        [MenuItem("Tools/Picture Loader/ Add Url Input Prefab to Scene", false, 4)]
        public static void AddUrlPrefab()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/xyz.drblackrat.pictureloader/Runtime/Prefabs/Picture Loader URL Input.prefab");
            PrefabUtility.InstantiatePrefab(prefab);
        }        
        [MenuItem("Tools/Picture Loader/ Add (Dark) Url Input Prefab to Scene", false, 5)]
        public static void AddDarkUrlPrefab()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/xyz.drblackrat.pictureloader/Runtime/Prefabs/(Dark) Picture Loader URL Input.prefab");
            PrefabUtility.InstantiatePrefab(prefab);
        }
        [MenuItem("Tools/Picture Loader/ Add Persistence Prefab to Scene", false, 6)]
        public static void AddPersistencePrefab()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/xyz.drblackrat.pictureloader/Runtime/Prefabs/Picture Loader Persistence.prefab");
            PrefabUtility.InstantiatePrefab(prefab);
        }
        [MenuItem("Tools/Picture Loader/ Add Tablet Downloader Prefab to Scene", false, 6)]
        public static void AddTabletPrefab()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/xyz.drblackrat.pictureloader/Runtime/Prefabs/Tablet Downloader.prefab");
            PrefabUtility.InstantiatePrefab(prefab);
        }
    }
}

