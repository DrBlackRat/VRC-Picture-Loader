using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Economy;

namespace DrBlackRat.VRC.Economy
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DonationSystem : UdonSharpBehaviour
    {
        [Header("Settings:")] 
        [SerializeField] private UdonProduct product;
        [SerializeField] private string listingId;
        [Space(10)]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip purchaseSound;
        [SerializeField] private AudioClip buttonSound;
        
        public override void OnPurchaseConfirmed(IProduct result, VRCPlayerApi player, bool purchased)
        {
            if (!player.isLocal) return;
            
            if (product == null) return;
            if (result.ID != product.ID) return;
            
            if (purchased && audioSource != null && purchaseSound != null) audioSource.PlayOneShot(purchaseSound);
        }

        public void _OpenStore()
        {
            Store.OpenListing(listingId);
            if (audioSource != null && buttonSound != null) audioSource.PlayOneShot(buttonSound);
        }
    }
}
