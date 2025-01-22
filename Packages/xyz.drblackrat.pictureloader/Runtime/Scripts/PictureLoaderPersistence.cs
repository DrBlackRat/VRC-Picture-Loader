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
        [Tooltip("Use Networking Master instead of Instance Owner. Instance Owner only works in Invite, Invite+, Friends and Friends+, but has some usage benefits.")]
        [SerializeField] private bool useNetMaster;
        [Tooltip("If the Master changes the new Master will be able to save URL's to their Player Object. (Only for Net Master Mode)")]
        [SerializeField] private bool allowNewMasterToSave;
        [Space(10)]
        [Tooltip("List of Picture Loader URL Inputs that you want to use Persistence for")]
        [SerializeField] private PictureLoaderURLInput[] urlInputs;
        [UdonSynced] private VRCUrl[] urls;

        private bool isLocalOwner;
        private bool setupCorrect;
        
        private void Start()
        {
            var owner = Networking.GetOwner(gameObject);
            if (!useNetMaster)
            {
                PLDebug.PersistenceLog(owner.isInstanceOwner
                    ? $"Instance Onwer Mode | Running Persistence on {owner.displayName}'s client."
                    : $"Skipping Persistence for {owner.displayName} as they are not the Instance Owner.");
                isLocalOwner = Networking.LocalPlayer.isInstanceOwner && owner.isLocal;
            }
            else
            {
                PLDebug.PersistenceLog(owner.isMaster
                    ? $"Network Master Mode | {owner.displayName} is the current Master."
                    : $"Skipping Persistence for {owner.displayName} as they are not the Master.");
                isLocalOwner = Networking.LocalPlayer.isMaster && owner.isLocal;
            }
            SetupPersistence();
        }

        private void SetupPersistence()
        {
            if (!isLocalOwner || setupCorrect) return;
            
            if (urlInputs.Length == 0)
            {
                PLDebug.PersistenceLogError("No URL Inputs provided!");
                return;
            }
            foreach (var urlInput in urlInputs)
            {
                if (urlInput == null)
                {
                    PLDebug.PersistenceLogError("URL Input slot is empty!");
                    return;
                }
            }
            setupCorrect = true;
            
            for (int i = 0; i < urlInputs.Length; i++)
            {
                urlInputs[i]._SetPersistenceReference(i, this);
            }
        }
        
        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (!useNetMaster || !allowNewMasterToSave) return;
            if (player.isLocal) return;
            
            var owner = Networking.GetOwner(gameObject);
            if (!owner.isMaster) return;
            isLocalOwner = owner.isLocal;
            PLDebug.PersistenceLog($"{owner.displayName} is now Master. They can now save URLs.");
            
            SetupPersistence();
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
