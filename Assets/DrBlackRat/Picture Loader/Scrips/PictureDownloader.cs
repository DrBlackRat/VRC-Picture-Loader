using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Image;
using VRC.Udon;
using UnityEditor;

namespace DrBlackRat
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [DefaultExecutionOrder(0)]
    public class PictureDownloader : UdonSharpBehaviour
    {
        [Header("Download Link")]        
        [Tooltip("The Link to the Picture you want to download")]
        public VRCUrl url;

        [Header("Texture Settings")]
        public bool generateMipMaps = true;
        public int anisoLevel = 9;
        public PFilterMode filterMode = PFilterMode.Bilinear;

        [Header("Material Settings")]
        [Tooltip("List of Material Properties you want to apply the downloaded Picture to")]
        public string[] materialProperties = {"_MainTex"};

        [Header("Loadig & Error Texture")]
        [Tooltip("Use the Loading Texture while it waits for the Picture to Load")]
        public bool useLoadingTexture = true;
        [Tooltip("Texture used while the Picture is Loading")]
        public Texture2D loadingTexture;
        [Space(10)]
        [Tooltip("Use the Error Texture when the Picture couldn't be Loaded")]
        public bool useErrorTexture = true;
        [Tooltip("Texture used when the Picture couldn't be Loaded")]
        public Texture2D errorTexture;

        private Material material;
        private VRCImageDownloader pictureDL;
        [HideInInspector]
        public TextureInfo textureInfo;
        private int timesRun;

        [HideInInspector]
        public PictureLoaderManager manager;

        private void Start()
        {
            // Sets Material
            if (material == null)
            {
                material = GetComponent<Renderer>().material;
            }

            // Error when no Manager was found
            if (manager == null)
            {
                Debug.LogError("No Picture Loader Manager Found");
            }
            // Texture Info Setup
            textureInfo.MaterialProperty = null;
            textureInfo.GenerateMipMaps = generateMipMaps;
            textureInfo.AnisoLevel = anisoLevel;
            switch (filterMode)
            {
                case PFilterMode.Point:
                    textureInfo.FilterMode = FilterMode.Point; 
                    break;
                case PFilterMode.Bilinear:
                    textureInfo.FilterMode = FilterMode.Bilinear;
                    break;
                case PFilterMode.Trilinear:
                    textureInfo.FilterMode = FilterMode.Trilinear;
                    break;
            }
        }
        public void DownloadPicture()
        {
            // Dispose old Loader
            if (timesRun >= 1)
            {
                pictureDL.Dispose();
            }
            // Sets Loading Texture
            if (useLoadingTexture)
            {
                foreach (string materialProperty in materialProperties)
                {
                    material.SetTexture(materialProperty, loadingTexture);
                }
            }
            // Loads new Picture
            pictureDL = new VRCImageDownloader();
            pictureDL.DownloadImage(url, null, gameObject.GetComponent<UdonBehaviour>(), textureInfo);
            
            timesRun++;
        }
        public override void OnImageLoadSuccess(IVRCImageDownload result)
        {
            // Sets Downloaded Picture as Texture
            foreach (string materialProperty in materialProperties)
            {
                material.SetTexture(materialProperty, result.Result);
            }
            manager.PictureLoaded();
            Debug.Log("Picture Loaded Successfully");
        }
        public override void OnImageLoadError(IVRCImageDownload result)
        {   
            // Sets Error Texture
            if (useErrorTexture)
            {
                foreach (string materialProperty in materialProperties)
                {
                    material.SetTexture(materialProperty, errorTexture);
                }
            }
            manager.PictureFailed();
            Debug.Log($"Could not Load Picture {result.Error}");
        }
    }
    public enum PFilterMode
    {
        Point,
        Bilinear,
        Trilinear
    }
}
