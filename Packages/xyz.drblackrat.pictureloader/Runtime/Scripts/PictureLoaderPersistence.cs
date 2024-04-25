using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace DrBlackRat
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PictureLoaderPersistence : UdonSharpBehaviour
    {
        [Header("Settings")]
        [Tooltip("List of Picture Loader URL Inputs that you want to use Persistence for")]
        [SerializeField] private PictureLoaderURLInput[] urlInputs;
        [UdonSynced] private VRCUrl[] urls;

        private readonly VRCUrl emptyUrl = new VRCUrl("");

        private bool isLocalOwner;
        private bool setupCorrect;
        private void Start()
        {
            isLocalOwner = Networking.LocalPlayer.isInstanceOwner && Networking.GetOwner(gameObject).isLocal;
            if (!isLocalOwner) return;
            setupCorrect = CheckSetup();
            if (!setupCorrect) return;
            // Initial Setup
            for (int i = 0; i < urlInputs.Length; i++)
            {
                urlInputs[i]._SetPersistenceReference(i, this);
            }
        }
        private bool CheckSetup()
        {
            if (urlInputs.Length == 0)
            {
                PLDebug.UrlLogError("Persistence: No URL Inputs provided!");
                return false;
            }
            foreach (var urlInput in urlInputs)
            {
                if (urlInput == null)
                {
                    PLDebug.UrlLogError("Persistence: URL Input slot is empty!");
                    return false;
                }
            }
            return true;
        }
        
        public override void OnDeserialization()
        {
            if (!isLocalOwner) return;
            if (!setupCorrect) return;
            // Check for old / wrong data
            if (urlInputs.Length != urls.Length)
            {
                PLDebug.UrlLogError("Persistence: Incompatible data found! Resetting saved data.");
                ResetUrls();
                return;
            }
            // Load Images
            for (int i = 0; i < urlInputs.Length; i++)
            {
                if (urls[i].Equals(emptyUrl)) continue;
                urlInputs[i]._LoadSavedImage(urls[i]);
            }
        }
        private void ResetUrls()
        {
            urls = new VRCUrl[urlInputs.Length];
            for (int i = 0; i < urls.Length; i++)
            {
                urls[i] = emptyUrl;
            }
            RequestSerialization();
        }
        public void _SaveUrl(int id, VRCUrl url)
        {
            if (!isLocalOwner) return;
            if (!setupCorrect) return;
            if (urls == null || urlInputs.Length != urls.Length) ResetUrls();
            urls[id] = url;
            RequestSerialization();
            PLDebug.UrlLog("Persistence: Saved Url");
        }
    }
}
