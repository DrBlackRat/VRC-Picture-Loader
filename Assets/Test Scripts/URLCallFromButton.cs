using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Components;
using VRC.Udon;
using DrBlackRat.VRC.PictureLoader;

public class URLCallFromButton : UdonSharpBehaviour
{
    public VRCUrl url = VRCUrl.Empty;
    public PictureLoaderURLInput plUrlInputField;
    public VRCUrlInputField inputField;

    public void _ButtonPress()
    {
        inputField.SetUrl(url);
        plUrlInputField._UrlEntered();
    }
}
