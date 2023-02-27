using UnityEditor.Callbacks;
using UnityEngine;

namespace DrBlackRat
{
    public class PictureLoaderCallback : MonoBehaviour
    {
        [PostProcessScene(-100)]
        public static void OnPostProcessScene()
        {
            PictureLoaderManager manager = FindObjectOfType<PictureLoaderManager>();
            if (manager != null)
            {
                manager.downloaders = FindObjectsOfType<PictureDownloader>();
                foreach (PictureDownloader downloader in manager.downloaders)
                {
                    downloader.manager = manager;
                }
            }
        }
    }
}