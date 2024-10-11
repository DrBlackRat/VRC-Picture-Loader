using UnityEditor.Callbacks;
using UnityEngine;

namespace DrBlackRat.VRC.PictureLoader
{
    public static class PictureLoaderCallback
    {
        [PostProcessScene(-100)]
        public static void OnPostProcessScene()
        {
            PictureLoaderManager manager = Object.FindObjectOfType<PictureLoaderManager>();
            if (manager != null)
            {
                manager.downloaders = Object.FindObjectsOfType<PictureDownloader>();
                foreach (PictureDownloader downloader in manager.downloaders)
                {
                    downloader.manager = manager;
                }
            }
        }
    }
}