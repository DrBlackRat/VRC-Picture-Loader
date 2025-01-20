using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Image;
using VRC.Udon;
using UnityEngine.UI;

namespace DrBlackRat.VRC.PictureLoader
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [DefaultExecutionOrder(100)]
    public class LitePictureDownloader : UdonSharpBehaviour
    {
        [Header("Download Link & Settings")]
        [Tooltip("The Link to the Picture you want to download.")]
        public VRCUrl url;
        [Tooltip("Load Picture when you enter the World.")]
        [SerializeField] private bool loadOnStart = true;
        [Space(10)]
        [Tooltip("Automatically reload Picture after a certain amount of time. Will be auto disabled if using a URL Input (Load On Start should be enabled for this).")]
        public bool autoReload = false;
        [Tooltip("Time in minutes after which the Picture should be downloaded again.")]
        [Range(1, 60)]
        [SerializeField] private int autoReloadTime = 10;

        [Header("Texture Settings")]
        [SerializeField] private bool generateMipMaps = true;
        [SerializeField] private int anisoLevel = 9;
        [SerializeField] private FilterMode filterMode = FilterMode.Bilinear;
        [Space(10)]
        [Tooltip("Texture Wrap Mode along the Horizontal Axis.")]
        [SerializeField] private TextureWrapMode wrapModeU = TextureWrapMode.Repeat;
        [Tooltip("Texture Wrap Mode along the Vertical Axis.")]
        [SerializeField] private TextureWrapMode wrapModeV = TextureWrapMode.Repeat;
        [Tooltip("Texture Wrap Mode for depth (only relevant for Texture3D).")]
        [SerializeField] private TextureWrapMode wrapModeW = TextureWrapMode.Repeat;

        [Header("Material & Raw Image Settings")]
        [Tooltip("The Material the Textures should be applied to, if left empty it tries use the one it's attached to.")]
        [SerializeField] private Material material;
        [Tooltip("List of Material Properties you want to apply the downloaded Picture to.")]
        [SerializeField] private string[] materialProperties = { "_MainTex" };
        [Space(10)]
        [Tooltip("List of UI Raw Images the texture should be applied to, if left empty it tires to use the one it's attached to.")]
        [SerializeField] private RawImage[] uiRawImages;
        [Tooltip("List of Aspect Ratio Filters for UI Raw Images, can be used automatically adjust the aspect ratio. If left empty it tires to use the one it's attached to.")]
        [SerializeField] private AspectRatioFitter[] aspectRatioFilters;

        [Header("Loading & Error Texture")]
        [Tooltip("Use the Loading Texture while it waits for the Picture to Load.")]
        [SerializeField] private bool useLoadingTexture = true;
        [Tooltip("Skips the Loading Texture when reloading the Picture (e.g. Auto Reload or Manually Loading it again).")]
        [SerializeField] private bool skipLoadingTextureOnReload = true;
        [Tooltip("Texture used while the Picture is Loading.")]
        [SerializeField] private Texture2D loadingTexture;
        [Space(10)]
        [Tooltip("Use the Error Texture when the Picture couldn't be Loaded.")]
        [SerializeField] private bool useErrorTexture = true;
        [Tooltip("Texture used when the Picture couldn't be Loaded.")]
        [SerializeField] private Texture2D errorTexture;
        
        private VRCImageDownloader pictureDL;
        private VRCImageDownloader oldPictureDL;
        [HideInInspector]
        public TextureInfo textureInfo;
        private Texture2D picture;
        private int timesRun;
        private bool loading;
        private bool errorShown;
        
        
        [HideInInspector] 
        public PictureLoaderURLInput urlInput;
        private void Start()
        {
            // Sets Material
            var renderer = GetComponent<Renderer>();
            if (material == null && renderer != null) material = renderer.material;
            // Sets Raw Image
            var rawImage = GetComponent<RawImage>();
            if (uiRawImages.Length == 0 && rawImage != null) uiRawImages = new[] { rawImage };
            // Set Aspect Ratio Filter
            var aspectRatioFilter = GetComponent<AspectRatioFitter>();
            if (aspectRatioFilters.Length == 0 && aspectRatioFilter != null) aspectRatioFilters = new[] { aspectRatioFilter };
            // Texture Info Setup
            textureInfo.MaterialProperty = null;
            textureInfo.GenerateMipMaps = generateMipMaps;
            textureInfo.AnisoLevel = anisoLevel;
            textureInfo.WrapModeU = wrapModeU;
            textureInfo.WrapModeV = wrapModeV;
            textureInfo.WrapModeW = wrapModeW;
            textureInfo.FilterMode = filterMode;
            if (urlInput != null) autoReload = false;
            if (loadOnStart) _DownloadPicture();
        }
        private void OnDestroy()
        {
            if (pictureDL != null) pictureDL.Dispose();
            if (oldPictureDL != null) oldPictureDL.Dispose();
        }
        public void _DownloadPicture()
        {
            if (!loading)
            {
                loading = true;
                // Talk to URL Input if available
                if (urlInput != null) urlInput._Loading();
                // Sets Loading Texture & Removes Error Texture
                if (useLoadingTexture && timesRun == 0 || useLoadingTexture && timesRun >= 1 && !skipLoadingTextureOnReload || useLoadingTexture && errorShown) ApplyTexture(loadingTexture);
                errorShown = false;
                // Remove Old Loader
                if (timesRun >= 1) oldPictureDL = pictureDL;
                // Loads new Picture
                pictureDL = new VRCImageDownloader();
                pictureDL.DownloadImage(url, null, GetComponent<UdonBehaviour>(), textureInfo);
                PLDebug.LiteLog($"Started Loading Picture from [{url}]");
            }
            else
            {
                PLDebug.LiteLogWarning($"Picture from [{url}] is currently being downloaded, wait for it to be done before trying again!");
            }
        }
        private void ApplyTexture(Texture2D newTexture)
        {
            if (material != null)
            {
                foreach (string materialProperty in materialProperties)
                {
                    if (material == null) continue;
                    material.SetTexture(materialProperty, newTexture);
                }
            }
            if (uiRawImages != null && uiRawImages.Length != 0)
            {
                foreach (RawImage uiRawImage in uiRawImages)
                {
                    if (uiRawImage == null) continue;
                    uiRawImage.texture = newTexture;
                }
            }
            // Change Aspect Ratio for Raw Images
            if (aspectRatioFilters == null || aspectRatioFilters.Length == 0 || newTexture == null) return;
            var aspectRatio = newTexture.width / (float)newTexture.height;
            foreach (var aspectRatioFilter in aspectRatioFilters)
            {
                if (aspectRatioFilter == null) continue;
                aspectRatioFilter.aspectRatio = aspectRatio;
            }
        }
        
        private void AutoReload()
        {
            if (!autoReload) return;
            SendCustomEventDelayedSeconds("_DownloadPicture", autoReloadTime * 60);
            PLDebug.LiteLog($"Next Auto Reload for [{url}] in {autoReloadTime} minute(s)");
        }
        public override void OnImageLoadSuccess(IVRCImageDownload result)
        {
            loading = false;
            PLDebug.LiteLog($"Picture from [{url}] Loaded Successfully");
            // Set result as Texture
            picture = result.Result;
            ApplyTexture(picture);
            // Dispose Old Loader
            if (timesRun >= 1) oldPictureDL.Dispose();
            timesRun++;
            AutoReload();
            // Talk to URL Input if available
            if (urlInput != null) urlInput._Finished();
        }
        public override void OnImageLoadError(IVRCImageDownload result)
        {
            loading = false;
            PLDebug.LiteLogError($"Could not Load Picture from [{url}] because: {result.Error}");
            // Sets Error Texture
            if (useErrorTexture) {ApplyTexture(errorTexture);}
            errorShown = useErrorTexture;
            // Dispose Loaders
            if (timesRun >= 1) oldPictureDL.Dispose();
            pictureDL.Dispose();
            timesRun++;
            AutoReload();
            // Talk to URL Input if available
            if (urlInput != null) urlInput._Error();
        }
    }
}
