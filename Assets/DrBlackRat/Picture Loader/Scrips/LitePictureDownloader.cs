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
    public class LitePictureDownloader : UdonSharpBehaviour
    {
        [Header("Download Link & Settings")]
        [Tooltip("The Link to the Picture you want to download")]
        public VRCUrl url;
        [Tooltip("Load Picture when you enter the World")]
        public bool loadOnStart = true;
        [Space(10)]
        [Tooltip("Automaically reload Picture after a certain ammount of time (Load On Start should be enabled for this)")]
        public bool autoReload = false;
        [Tooltip("Time in minutes after which the Picture should be redownloaded")]
        [Range(1, 60)]
        public int autoReloadTime = 10;

        [Header("Texture Settings")]
        public bool generateMipMaps = true;
        public int anisoLevel = 9;
        public PFilterMode filterMode = PFilterMode.Bilinear;

        [Header("Material & Raw Image Settings")]
        [Tooltip("The Material the Textures should be applied to, if left empty it tries use the one it's attached to")]
        public Material material;
        [Tooltip("List of Material Properties you want to apply the downloaded Picture to")]
        public string[] materialProperties = { "_MainTex" };
        [Tooltip("List of UI Raw Images the texture should be applied to, if left empty it tires to use the one it's attached to")]
        public RawImage[] uiRawImages;

        [Header("Loadig & Error Texture")]
        [Tooltip("Use the Loading Texture while it waits for the Picture to Load")]
        public bool useLoadingTexture = true;
        [Tooltip("Skips the Loading Texture when reloading the Picture (e.g. Auto Reload or Manually Loading it again)")]
        public bool skipLoadingTextureOnReload = false;
        [Tooltip("Texture used while the Picture is Loading")]
        public Texture2D loadingTexture;
        [Space(10)]
        [Tooltip("Use the Error Texture when the Picture couldn't be Loaded")]
        public bool useErrorTexture = true;
        [Tooltip("Texture used when the Picture couldn't be Loaded")]
        public Texture2D errorTexture;

        private VRCImageDownloader pictureDL;
        private VRCImageDownloader oldPictureDL;
        [HideInInspector]
        public TextureInfo textureInfo;
        [HideInInspector]
        public Texture2D picture;
        private int timesRun;
        private bool loading;

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
                uiRawImages = new RawImage[1] { rawImage };
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
            if (loadOnStart)
            {
                DownloadPicture();
            }
        }
        public void DownloadPicture()
        {
            if (!loading)
            {
                loading = true;
                // Sets Loading Texture
                if (useLoadingTexture && timesRun == 0 || useLoadingTexture && timesRun >= 1 && !skipLoadingTextureOnReload)
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
                if (timesRun >= 1)
                {
                    oldPictureDL = pictureDL;
                }
                // Loads new Picture
                pictureDL = new VRCImageDownloader();
                pictureDL.DownloadImage(url, null, gameObject.GetComponent<UdonBehaviour>(), textureInfo);
                Debug.Log("[VRC Picture Loader Lite] Started Loading Picture");
            }
            else
            {
                Debug.LogWarning("[VRC Picture Loader Lite] Picture is currently being downloaded, wait for it to be done before trying again!");
            }
      
        }
        public override void OnImageLoadSuccess(IVRCImageDownload result)
        {
            // Set result as Texture
            picture = result.Result;
            // Sets Downloaded Picture as Texture
            if (material != null)
            {
                foreach (string materialProperty in materialProperties)
                {
                    material.SetTexture(materialProperty, picture);
                }
            }
            if (uiRawImages != null && uiRawImages.Length != 0)
            {
                foreach (RawImage uiRawImage in uiRawImages)
                {
                    uiRawImage.texture = picture;
                }
            }
            Debug.Log("[VRC Picture Loader Lite] Picture Loaded Successfully");
            // Dispose Old Loader
            if (timesRun >= 1)
            {
                oldPictureDL.Dispose();
            }
            loading = false;
            timesRun++;
            if (autoReload)
            {
                SendCustomEventDelayedSeconds("DownloadPicture", autoReloadTime * 60);
                Debug.Log($"[VRC Picture Loader Lite] Next Auto Reload in {autoReloadTime} minute(s)");
            }
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
                if (uiRawImages != null && uiRawImages.Length != 0)
                {
                    foreach (RawImage uiRawImage in uiRawImages)
                    {
                        uiRawImage.texture = errorTexture;
                    }
                }
            }
            Debug.Log($"[VRC Picture Loader Lite] Could not Load Picture: {result.Error}");
            // Dispose Loaders
            if (timesRun >= 1)
            {
                oldPictureDL.Dispose();
            }
            pictureDL.Dispose();
            loading = false;
            timesRun++;
            if (autoReload)
            {
                SendCustomEventDelayedSeconds("DownloadPicture", autoReloadTime * 60);
                Debug.Log($"[VRC Picture Loader Lite] Next Auto Reload in {autoReloadTime} minute(s)");
            }
        }
    }
}
