
using System;
using DrBlackRat;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;

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
    
    private PLState state;
    private bool isOwner;
    private bool startedLoading;
    
    private VRCUrl url = new VRCUrl("");
    [UdonSynced] private VRCUrl netUrl = new VRCUrl("");
    private readonly VRCUrl emptyUrl = new VRCUrl("");
    private void Start()
    {
        // Initial Setup
        if (downloader == null)
        {
            PLDebug.UrlLogError("No Lite Picture Downloader provided!");
            return;
        }
        downloader.urlInput = this;
        downloader.autoReload = false;
        CheckOwner();
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
        if (!AllowInput()) return;
        url = inputField.GetUrl();
        netUrl = url;
        // Empty Input Field
        inputField.SetUrl(emptyUrl);
        // Load Image
        startedLoading = false;
        _TryLoadingImage();
        // Networking
        if (!isOwner) Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        RequestSerialization();
    }
    // Update UI
    private void UpdateUI()
    {
        // Update Lock Button
        lockButton.interactable = isOwner;
        unlockedIcon.SetActive(!locked);
        lockedIcon.SetActive(locked);
        // Update Input Field
        inputField.interactable = AllowInput();
        // Change Input Field Text
        bool allowInput = AllowInput();
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
                bgText.text = allowInput ? "Error! Enter New URL" : "Error! Input Locked";
                break;
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
    public override void OnDeserialization()
    {
        if (netUrl.Equals(url)) return;
        url = netUrl;
        // Load Image
        startedLoading = false;
        _TryLoadingImage();
    }
    private void CheckOwner()
    {
        isOwner = Networking.LocalPlayer == Networking.GetOwner(this.gameObject);
        UpdateUI();
    }
    #endregion
    #region Image Loading
    public void _TryLoadingImage()
    {
        if (state == PLState.Loading && !startedLoading)
        {
            PLDebug.UrlLog("An Image is currently being loaded, will try again in 5s");
            SendCustomEventDelayedSeconds("_TryLoadingImage", 5f);
        }
        else if (state == PLState.Loading && startedLoading)
        {
            return;
        }
        else
        {
            startedLoading = true;
            downloader.url = url;
            downloader._DownloadPicture();
        }
    }
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
        UpdateUI();
    }
    public void _Error()
    {
        state = PLState.Error;
        UpdateUI();
    }
    #endregion
}
