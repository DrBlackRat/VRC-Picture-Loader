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

        private bool isMasterOwner;
        public void Start()
        {
            // Initial Setup
            isMasterOwner = Networking.LocalPlayer.isInstanceOwner && Networking.GetOwner(gameObject).isLocal;
            if (!isMasterOwner) return;
            for (int i = 0; i < urlInputs.Length; i++)
            {
                urlInputs[i]._SetPersistenceReference(i, this);
            }
        }
        public override void OnDeserialization()
        {
            if (!isMasterOwner) return;
            // Check for old data
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
            if (!isMasterOwner) return;
            if (urlInputs.Length != urls.Length) ResetUrls();
            urls[id] = url;
            RequestSerialization();
            PLDebug.UrlLog("Persistence: Saved Url");
        }
    }
}
