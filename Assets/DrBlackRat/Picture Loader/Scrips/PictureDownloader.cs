
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Image;
using VRC.Udon;

namespace DrBlackRat
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [DefaultExecutionOrder(0)]
    public class PictureDownloader : UdonSharpBehaviour
    {
        [Header("Download Link")]        
        [Tooltip("The Link to the Picture you want to download")]
        public VRCUrl url;
        
        [Space(2)]
        [Tooltip("List of Material Properties you want to apply the downloaded Picture to")]
        public string[] materialProperties = {"_MainTex"};

        private Material material;

        [HideInInspector]
        public PictureLoaderManager manager;

        private void Start()
        {
            material = GetComponent<Renderer>().material;
            if (manager == null)
            {
                Debug.LogWarning("No Picture Loader Manager Found");
            }
        }

        public void DownloadPicture()
        {
            VRCImageDownloader pictureDL = new VRCImageDownloader();
            pictureDL.DownloadImage(url, null, gameObject.GetComponent<UdonBehaviour>(), null);
        }

        public override void OnImageLoadSuccess(IVRCImageDownload result)
        {
            foreach (string materialProperty in materialProperties)
            {
                material.SetTexture(materialProperty, result.Result);
            }
            manager.PictureLoaded();
            Debug.Log("Picture Loaded Successfully");
        }
        public override void OnImageLoadError(IVRCImageDownload result)
        {
            manager.PictureFailed();
            Debug.Log($"Could not Load Picture {result.Error}");
        }
    }
}
