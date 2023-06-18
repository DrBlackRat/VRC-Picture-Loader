using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Image;
using VRC.Udon;
using Unity;
using UnityEngine.UI;

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

        [Header("Material & Raw Image Settings")]
        [Tooltip("The Material the Textures should be applied to, if left empty it tries use the one it's attached to")]
        public Material material;
        [Tooltip("List of Material Properties you want to apply the downloaded Picture to")]
        public string[] materialProperties = {"_MainTex"};
        [Tooltip("List of UI Raw Images the texture should be applied to, if left empty it tires to use the one it's attached to")]
        public RawImage[] uiRawImages;

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

        private VRCImageDownloader pictureDL;
        [HideInInspector]
        public TextureInfo textureInfo;
        private int timesRun;

        [HideInInspector]
        public PictureLoaderManager manager;

        private void Start()
        {
            // Sets Material
            var renderer = GetComponent<Renderer>();
            if (material == null && renderer != null)
            {
                material = renderer.material;
            }
            // Sets Raw Image
            var rawImage = GetComponent<RawImage>();
            if (uiRawImages.Length == 0 && rawImage != null)
            {
                uiRawImages = new RawImage[1] {rawImage};
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
                if (material != null)
                {
                    foreach (string materialProperty in materialProperties)
                    {
                        material.SetTexture(materialProperty, loadingTexture);
                    }
                }
                if (uiRawImages.Length != 0 && uiRawImages != null)
                {
                    foreach (RawImage uiRawImage in uiRawImages)
                    {
                        uiRawImage.texture = loadingTexture;
                    }
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
            if (material != null)
            {
                foreach (string materialProperty in materialProperties)
                {
                    material.SetTexture(materialProperty, result.Result);
                }
            }
            if (uiRawImages.Length != 0 && uiRawImages != null)
            {
                foreach (RawImage uiRawImage in uiRawImages)
                {
                    uiRawImage.texture = result.Result;
                }
            }
            Debug.Log("[VRC Picture Loader] Picture Loaded Successfully");
            manager.PictureLoaded();
        }
        public override void OnImageLoadError(IVRCImageDownload result)
        {   
            // Sets Error Texture
            if (useErrorTexture)
            {
                if (material != null)
                {
                    foreach (string materialProperty in materialProperties)
                    {
                        material.SetTexture(materialProperty, errorTexture);
                    }
                }
                if (uiRawImages.Length != 0 && uiRawImages != null)
                {
                    foreach (RawImage uiRawImage in uiRawImages)
                    {
                        uiRawImage.texture = errorTexture;
                    }
                }
            }
            Debug.Log($"[VRC Picture Loader] Could not Load Picture: {result.Error}");
            manager.PictureFailed();
        }
    }
    public enum PFilterMode
    {
        Point,
        Bilinear,
        Trilinear
    }
}
