using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Image;
using VRC.Udon;
using UnityEngine.UI;

namespace DrBlackRat
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [DefaultExecutionOrder(0)]
    public class PictureDownloader : UdonSharpBehaviour
    {
        [Header("Download Link")]        
        [Tooltip("The Link to the Picture you want to download.")]
        public VRCUrl url;

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
        [Tooltip("List of Aspect Ratio Filters for UI Ram Images, can be used automatically Adjust the Aspect Ratio. If left empty it tires to use the one it's attached to.")]
        [SerializeField] private AspectRatioFitter[] aspectRatioFilters;

        [Header("Loading & Error Texture")]
        [Tooltip("Use the Loading Texture while it waits for the Picture to Load.")]
        [SerializeField] private bool useLoadingTexture = true;
        [Tooltip("Skips the Loading Texture when reloading the Picture (e.g. Auto Reload or Manually Loading it again).")]
        [SerializeField] private bool skipLoadingTextureOnReload = false;
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

        [HideInInspector]
        public PictureLoaderManager manager;

        private void Start()
        {
            // Sets Material
            var renderer = GetComponent<Renderer>();
            if (material == null && renderer != null) material = renderer.material;
            // Sets Raw Image
            var rawImage = GetComponent<RawImage>();
            if (uiRawImages.Length == 0 && rawImage != null) uiRawImages = new[] { rawImage };
            // Set Aspect Ratio Filter
            var aspectFilter = GetComponent<AspectRatioFitter>();
            if (aspectRatioFilters.Length == 0 && aspectFilter != null) aspectRatioFilters = new[] { aspectFilter };
            // Error when no Manager was found
            if (manager == null) PLDebug.LogError($"No Picture Loader Manager Found!");
            // Texture Info Setup
            textureInfo.MaterialProperty = null;
            textureInfo.GenerateMipMaps = generateMipMaps;
            textureInfo.AnisoLevel = anisoLevel;
            textureInfo.WrapModeU = wrapModeU;
            textureInfo.WrapModeV = wrapModeV;
            textureInfo.WrapModeW = wrapModeW;
            textureInfo.FilterMode = filterMode;
        }
        public void _DownloadPicture()
        {
            // Sets Loading Texture
            if (useLoadingTexture && timesRun == 0 || useLoadingTexture && timesRun >= 1 && !skipLoadingTextureOnReload) ApplyTexture(loadingTexture);
            if (timesRun >= 1) oldPictureDL = pictureDL;
            // Loads new Picture
            pictureDL = new VRCImageDownloader();
            pictureDL.DownloadImage(url, null, gameObject.GetComponent<UdonBehaviour>(), textureInfo);
        }
        private void ApplyTexture(Texture2D newTexture)
        {
            if (material != null)
            {
                foreach (string materialProperty in materialProperties) material.SetTexture(materialProperty, newTexture);
            }
            if (uiRawImages != null && uiRawImages.Length != 0)
            {
                foreach (RawImage uiRawImage in uiRawImages) uiRawImage.texture = newTexture;
            }
            // Change Aspect Ratio for Raw Images
            if (aspectRatioFilters == null || aspectRatioFilters.Length == 0) return;
            var aspectRatio = newTexture.width / (float)newTexture.height;
            foreach (var aspectFilter in aspectRatioFilters)
            {
                aspectFilter.aspectRatio = aspectRatio;
            }
        }
        public override void OnImageLoadSuccess(IVRCImageDownload result)
        {
            PLDebug.Log($"Picture from [{url}] Loaded Successfully");
            // Set result as Texture
            picture = result.Result;
            ApplyTexture(picture);
            // Dispose Old Loader
            if (timesRun >= 1) oldPictureDL.Dispose();
            timesRun++;
            // Tell Manager that Picture was loaded
            manager._PictureLoaded();
        }
        public override void OnImageLoadError(IVRCImageDownload result)
        {
            PLDebug.LogError($"Could not Load Picture from [{url}] because: {result.Error}");
            // Sets Error Texture
            if (useErrorTexture) ApplyTexture(errorTexture);
            // Dispose Loaders
            if (timesRun >= 1) oldPictureDL.Dispose();
            pictureDL.Dispose();
            timesRun++;
            // Tell Manager that Picture was loaded
            manager._PictureFailed();
        }
    }
}
