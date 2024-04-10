using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DrBlackRat
{
    public class Menu : MonoBehaviour
    {
        [MenuItem("Tools/Picture Loader/Open Example Scene")]
        public static void OpenExampleScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Packages/xyz.drblackrat.pictureloader/Runtime/Example Scene.unity");
            }
        }
        [MenuItem("Tools/Picture Loader/ Add Manager Prefab to Scene")]
        public static void AddManagerPrefab()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/xyz.drblackrat.pictureloader/Runtime/Picture Loader.prefab");
            PrefabUtility.InstantiatePrefab(prefab);
        }        
        [MenuItem("Tools/Picture Loader/ Add (Dark) Manager Prefab to Scene")]
        public static void AddDarkManagerPrefab()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/xyz.drblackrat.pictureloader/Runtime/(Dark) Picture Loader.prefab");
            PrefabUtility.InstantiatePrefab(prefab);
        }
        [MenuItem("Tools/Picture Loader/ Add Url Input Prefab to Scene")]
        public static void AddUrlPrefab()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/xyz.drblackrat.pictureloader/Runtime/Picture Loader URL Input.prefab");
            PrefabUtility.InstantiatePrefab(prefab);
        }        
        [MenuItem("Tools/Picture Loader/ Add (Dark) Url Input Prefab to Scene")]
        public static void AddDarkUrlPrefab()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/xyz.drblackrat.pictureloader/Runtime/(Dark) Picture Loader URL Input.prefab");
            PrefabUtility.InstantiatePrefab(prefab);
        }
    }
}

