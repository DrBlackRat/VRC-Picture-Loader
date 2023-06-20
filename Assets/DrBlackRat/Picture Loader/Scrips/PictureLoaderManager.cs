
using UdonSharp;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DrBlackRat
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [DefaultExecutionOrder(100)]
    public class PictureLoaderManager : UdonSharpBehaviour
    {
        [Header("Settings")]
        [Tooltip("Load Pictures when you enter the World (required for Auto Reloading)")]
        public bool loadOnStart = true;
        [Space(10)]
        [Tooltip("Automaically reload Pictures after a certain ammount of time (requires Load On Start to be enabled & disables Manual Loading)")]
        public bool autoReload = false;
        [Tooltip("Time in minutes after which Pictures should be redownloaded")]
        [Range(1, 60)]
        public int autoReloadTime = 10;
        [Space(10)]
        [Tooltip("Adds a button to Manually Load the Pictures (will be disabled if Auto Reload is enabled)")]
        public bool manualLoadButton = true;

        [Header("Internals")]
        public TextMeshProUGUI status;
        public TextMeshProUGUI indicator;
        public GameObject loadButtonObj;
        public RectTransform uiRect;
        public BoxCollider uiCollider;

        [HideInInspector]
        public PictureDownloader[] downloaders;
        [HideInInspector]
        public PictureLoaderState state;

        private Button loadButton;
        private int picturesToLoad;
        private int picturesLoaded;
        private int errors;
        private int timesRun;
        
        private void Start()
        {
            loadButton = loadButtonObj.GetComponent<Button>();
            // Inital set of Variables
            picturesLoaded = 0;
            picturesToLoad = downloaders.Length;
            indicator.text = $"{picturesLoaded} / {picturesToLoad}";
            // Manual Load Button
           if (!manualLoadButton || autoReload)
            {
                manualLoadButton = false;
                loadButtonObj.SetActive(false);
                uiRect.sizeDelta = new Vector2(105f, 46.25f);
                uiCollider.enabled = false;
            }
           // Enables Auto Loading when Reloading Is enabled
           if (autoReload)
            {
                loadOnStart = true;
            }
            // Picture Loading
            if (picturesToLoad == 0)
            {
                status.text = "Status: Error, no Pictures found";
                Debug.Log("[VRC Picture Loader] Error, no Pictures found");
            }
            else if (loadOnStart)
            {
                Debug.Log($"[VRC Picture Loader] Found {downloaders.Length} Picture Downloader(s)");
                LoadPictures();
            }
            else
            {
                Debug.Log($"[VRC Picture Loader] Found {downloaders.Length} Picture Downloader(s)");
                Wait();
            }
        }
        public void _ButtonLoad()
        {
            LoadPictures();
        }

        // Different States the Loader can be in
        // What happens in each state
        private void Wait()
        {
            state = PictureLoaderState.Waiting;
            status.text = "Status: Waiting";
        }
        public void LoadPictures()
        {
            if (state != PictureLoaderState.Loading)
            {
                state = PictureLoaderState.Loading;
                status.text = "Status: Loading";
                picturesLoaded = 0;
                errors = 0;
                indicator.text = $"{picturesLoaded} / {picturesToLoad}";
                loadButton.interactable = false;
                Debug.Log($"[VRC Picture Loader] Started Loading {picturesToLoad} Picture(s)");
                foreach (PictureDownloader downloader in downloaders)
                {
                    downloader.DownloadPicture();
                }
            }
            else
            {
                Debug.LogWarning("[VRC Picture Loader] Pictures are currently being downloaded, wait for it to be done before trying again!");
            }

        }
        private void FinishedLoading()
        {
            timesRun++;
            state = PictureLoaderState.Finished;
            status.text = "Status: Finished";
            Debug.Log($"[VRC Picture Loader] Finished Loading {picturesLoaded} Picture(s)");
            if (manualLoadButton || !autoReload)
            {
                loadButton.interactable = true;
            }
            if (autoReload)
            {
                SendCustomEventDelayedSeconds("LoadPictures", autoReloadTime*60);
                Debug.Log($"[VRC Picture Loader] Next Auto Reload in {autoReloadTime} minute(s)");
                
            }
        }

        private void FinishedLoadingError() 
        {
            timesRun++;
            state = PictureLoaderState.FinishedError;
            if (errors == 1)
            {
                status.text = $"Status: Finished with an Error";
                Debug.Log($"[VRC Picture Loader] Finished Loading {picturesLoaded} out of {picturesToLoad} Picture(s) with 1 Error");
            }
            else
            {
                status.text = $"Status: Finished with Errors";
                Debug.Log($"[VRC Picture Loader] Finished Loading {picturesLoaded} out of {picturesToLoad} Picture(s) with {errors} Errors");
            }
            if (manualLoadButton || !autoReload)
            {
                loadButton.interactable = true;
            }
            if (autoReload)
            {
                SendCustomEventDelayedSeconds("LoadPictures", autoReloadTime * 60);
                Debug.Log($"[VRC Picture Loader] Next Auto Reload in {autoReloadTime} minute(s)");
            }
        }
        
        // Callbacks from the PictureDonwloaders
        public void PictureLoaded()
        {
            picturesLoaded++;
            if (errors == 0)
            {
                indicator.text = $"{picturesLoaded} / {picturesToLoad}";
            }
            else
            {
                indicator.text = $"{picturesLoaded} / {picturesToLoad} | Errors: {errors}";
            }

            // What do do once it's done
            if (picturesLoaded == picturesToLoad)
            { 
                FinishedLoading();
            }
            else if (picturesLoaded + errors == picturesToLoad)
            {
                FinishedLoadingError();
            }
        }
        public void PictureFailed() 
        {
            errors++;
            indicator.text = $"{picturesLoaded} / {picturesToLoad} | Errors: {errors}";

            // What do do once it's done
            if (picturesLoaded == picturesToLoad)
            {
                FinishedLoading();
            }
            else if (picturesLoaded + errors == picturesToLoad)
            {
                FinishedLoadingError();
            }
        }
    }
    public enum PictureLoaderState
    {
        Waiting,
        Loading,
        Finished,
        FinishedError,
    }
}
