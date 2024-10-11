using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace DrBlackRat.VRC.PictureLoader
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    [DefaultExecutionOrder(200)]
    public class PictureLoaderPersistence : UdonSharpBehaviour
    {
        [Header("Settings")]
        [Tooltip("List of Picture Loader URL Inputs that you want to use Persistence for")]
        [SerializeField] private PictureLoaderURLInput[] urlInputs;
        [UdonSynced] private VRCUrl[] urls;

        private bool isLocalOwner;
        private bool setupCorrect;
        private void Start()
        {
            // Cache Networking Owner
            var owner = Networking.GetOwner(gameObject);
            // Check if Persistence should be used for the owner etc
            PLDebug.PersistenceLog(owner.isInstanceOwner
                ? $"Running Persistence on {owner.displayName}'s client."
                : $"Skipping Persistence for {owner.displayName} as they are not the Instance Owner.");
            isLocalOwner = Networking.LocalPlayer.isInstanceOwner && owner.isLocal;
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
                PLDebug.PersistenceLogError("No URL Inputs provided!");
                return false;
            }
            foreach (var urlInput in urlInputs)
            {
                if (urlInput == null)
                {
                    PLDebug.PersistenceLogError("URL Input slot is empty!");
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
            if (urls == null || urlInputs.Length != urls.Length)
            {
                PLDebug.PersistenceLogError("Incompatible data found! Resetting saved data.");
                ResetUrls();
                RequestSerialization();
                return;
            }
            // Load Images
            for (int i = 0; i < urlInputs.Length; i++)
            {
                if (urls[i].Equals(VRCUrl.Empty)) continue;
                urlInputs[i]._LoadSavedImage(urls[i]);
            }
        }
        private void ResetUrls()
        {
            urls = new VRCUrl[urlInputs.Length];
            for (int i = 0; i < urls.Length; i++)
            {
                urls[i] = VRCUrl.Empty;
            }
        }
        public void _SaveUrl(int id, VRCUrl url)
        {
            if (!isLocalOwner) return;
            if (!setupCorrect) return;
            if (urls == null || urlInputs.Length != urls.Length) ResetUrls();
            urls[id] = url;
            RequestSerialization();
            PLDebug.PersistenceLog("Saved URL");
        }
    }
}
