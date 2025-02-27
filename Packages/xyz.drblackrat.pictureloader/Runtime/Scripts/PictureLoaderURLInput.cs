﻿using DrBlackRat;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace DrBlackRat.VRC.PictureLoader
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    [DefaultExecutionOrder(0)]
    public class PictureLoaderURLInput : UdonSharpBehaviour
    {
        [Header("Settings")]
        [Tooltip("The URL Input uses a Lite Downloader, please provide the one that you want to use the URL Input for")]
        public LitePictureDownloader downloader;
        [Tooltip("Set if the Input Field should be locked or unlocked by default")]
        [SerializeField] [UdonSynced] private bool locked = true;
        [Header("Internals")]
        [SerializeField] private VRCUrlInputField inputField;
        [SerializeField] private TextMeshProUGUI bgText;
        [Space(10)]
        [SerializeField] private Button lockButton;
        [SerializeField] private GameObject lockedIcon;
        [SerializeField] private GameObject unlockedIcon;
        [Space(10)]
        [SerializeField] private GameObject persistenceInfo;
        [SerializeField] private TextMeshProUGUI persistenceText;
        
        private PLState state;
        private bool isOwner;

        private VRCUrl url = VRCUrl.Empty;
        [UdonSynced] private VRCUrl netUrl = VRCUrl.Empty;

        private string lastError;
    
        private PictureLoaderPersistence persistence;
        private int persistenceID;
        private bool newImageSaved;
        [UdonSynced] private bool savedImageLoaded;
        private VRCUrl persistenceUrl = VRCUrl.Empty;

        [HideInInspector] public TabletMenu tabletMenu;
        private void Start()
        {
            if (downloader == null)
            {
                PLDebug.UrlLogError("No Lite Picture Downloader provided!");
                bgText.text = "Error! No Downloader Provided!";
                inputField.interactable = false;
                lockButton.interactable = false;
                return;
            }
            _Wait();
            downloader.urlInput = this;
            downloader.autoReload = false;
            CheckOwner();
            PLDebug.UrlLog("Connected to Lite Picture Downloader");
        }
        #region UI
        // UI Events
        public void _LockPressed()
        {
            if (!isOwner) return;
            locked = !locked;
            UpdateUI();
            // Networking
            RequestSerialization();
        }
        public void _UrlEntered()
        {
            if (!AllowInput())
            {
                // This should only occour if a user is using client modifcations
                PLDebug.UrlLogError($"You are not permitted to change the URL!");
                bgText.text = "Error! Not Permitted!";
                inputField.SetUrl(VRCUrl.Empty);
                return;
            }
            var newUrl = inputField.GetUrl();
            url = newUrl;
            netUrl = newUrl;
            SavePersistenceUrl(newUrl);
            inputField.SetUrl(VRCUrl.Empty);
            _TryLoadingImage();
            // Networking
            if (!isOwner) Networking.SetOwner(Networking.LocalPlayer, gameObject);
            RequestSerialization();
        }
        private void UpdateUI()
        {
            bool allowInput = AllowInput();
            // Update Lock Button
            lockButton.interactable = isOwner;
            unlockedIcon.SetActive(!locked);
            lockedIcon.SetActive(locked);
            // Update Input Field
            inputField.interactable = allowInput;
            switch (state) 
            {
                case PLState.Waiting:
                    bgText.text = allowInput ? "Enter URL" : "Input Locked";
                    break;
                case PLState.Loading:
                    bgText.text = "Loading...";
                    break;
                case PLState.Finished:
                    bgText.text = allowInput ? "Finished! Enter New URL" : "Finished! Input Locked";
                    break;
                case PLState.Error:
                    bgText.text = allowInput ? $"{lastError}! Enter New URL" : $"{lastError}! Input Locked";
                    break;
            }
            // Update Persistence Info
            if (isOwner && persistence != null && url.Equals(persistenceUrl) && savedImageLoaded && !newImageSaved)
            {
                persistenceInfo.SetActive(true);
                persistenceText.text = "Saved URL Loaded";
            }
            else if(isOwner && persistence != null && url.Equals(persistenceUrl) && newImageSaved)
            {
                persistenceInfo.SetActive(true);
                persistenceText.text = "New URL Saved";
            }
            else
            {
                persistenceInfo.SetActive(false);
                persistenceText.text = "If you see this something exploded";
            }
        }
        // Check if you can enter a URL
        private bool AllowInput()
        {
            return (isOwner || !locked) && state != PLState.Loading;
        }
        #endregion
        #region Networking
        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            CheckOwner();
        }
        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            CheckOwner();
        }

        public override bool OnOwnershipRequest(VRCPlayerApi requester, VRCPlayerApi newOwner)
        {
            if (!locked || isOwner || newOwner.isInstanceOwner || newOwner.isMaster)
            {
                PLDebug.UrlLog($"{requester.displayName} has made {newOwner.displayName} the new Network Owner.");
                return true;
            }
            else
            {
                PLDebug.UrlLogError($"{requester.displayName} has tried to make {newOwner.displayName} the Network Owner, but isn't allowed to!");
                return false;
            }
        }
        public override void OnDeserialization()
        {
            UpdateUI();
            // Check if new url is different from previous URL
            if (netUrl.Equals(url)) return;
            url = netUrl;
            // Load Image
            _TryLoadingImage();
        }
        private void CheckOwner()
        {
            isOwner = Networking.LocalPlayer == Networking.GetOwner(gameObject);
            UpdateUI();
        }
        #endregion
        #region Persistence
        public void _SetPersistenceReference(int id, PictureLoaderPersistence persistenceRef)
        {
            persistenceID = id;
            persistence = persistenceRef;
            PLDebug.UrlLog($"Connected to Picture Loader Persistence with ID {id}");
        }
        public void _LoadSavedImage(VRCUrl savedUrl)
        {
            if (downloader == null) return;
            if (savedImageLoaded)
            {
                PLDebug.PersistenceLog("Already loaded saved URL once, skipping this time.");
                return;
            }
            PLDebug.PersistenceLog("Loading saved URL");
            savedImageLoaded = true;
            url = savedUrl;
            netUrl = savedUrl;
            persistenceUrl = savedUrl;
            _TryLoadingImage();
            // Networking
            if (!isOwner) Networking.SetOwner(Networking.LocalPlayer, gameObject);
            RequestSerialization();
        }
        private void SavePersistenceUrl(VRCUrl newUrl)
        {
            if (persistence == null) return;
            newImageSaved = true;
            persistenceUrl = newUrl;
            persistence._SaveUrl(persistenceID, newUrl);
        }
    
        #endregion
        #region Image Loading
        public void _TryLoadingImage()
        {
            if (state == PLState.Loading)
            {
                PLDebug.UrlLog("An Image is currently being loaded, will try again in 5s.");
                SendCustomEventDelayedSeconds("_TryLoadingImage", 5f);
            }
            else
            {
                downloader.url = url;
                downloader._DownloadPicture();
            }
        }
        // Lite Picture Loader Interface
        public void _Wait()
        {
            state = PLState.Waiting;
            UpdateUI();
        }
        public void _Loading()
        {
            state = PLState.Loading;
            UpdateUI();
        }
        public void _Finished()
        {
            state = PLState.Finished;
            if(tabletMenu != null) tabletMenu._HiddeUI();
            UpdateUI();
        }
        public void _Error(string errorMessage)
        {
            state = PLState.Error;
            lastError = errorMessage;
            UpdateUI();
        }
        #endregion
    }
}

