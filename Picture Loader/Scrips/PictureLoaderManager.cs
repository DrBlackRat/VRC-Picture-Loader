﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Image;
using VRC.Udon;
using TMPro;
using UnityEngine.UI;
using System;

namespace DrBlackRat
{
    public class PictureLoaderManager : UdonSharpBehaviour
    {
        [Header("Settings")]
        [Tooltip("Load Pictures when you enter the World")]
        public bool loadOnStart = true;

        [Header("Internals")]
        public TextMeshProUGUI status;
        public TextMeshProUGUI indicator;
        public Button reoladButton;

        [HideInInspector]
        public PictureDownloader[] downloaders;

        private int picturesToLoad;
        private int picturesLoaded;
        private int errors;
        
        
        private void Start()
        {   
            // Inital set of Variables
            picturesLoaded = 0;
            picturesToLoad = downloaders.Length;
            indicator.text = $"{picturesLoaded} / {picturesToLoad}";

            SendCustomEventDelayedSeconds(nameof(PictureStart), 0.5f);
        }
        public void PictureStart()
        {
            if (picturesToLoad == 0)
            {
                status.text = "Status: Error, no Pictures found";
            }
            else if (loadOnStart == true)
            {
                Loadpictures();
            }
            else
            {
                Wait();
            }
        }

        public void _ButtonLoad()
        {
            Loadpictures();
        }

        // Different States the Loader can be in
        // What happens in each state
        private void Wait()
        {
            status.text = "Status: Waiting";
        }
        private void Loadpictures()
        {
            status.text = "Status: Loading";
            picturesLoaded = 0;
            indicator.text = $"{picturesLoaded} / {picturesToLoad}";
            reoladButton.interactable = false;
            foreach (PictureDownloader downloader in downloaders)
            {
                downloader.DownloadPicture();
            }
        }
        private void FinishedLoading()
        {
            status.text = "Status: Finished";
            reoladButton.interactable = true;
        }
        private void FinishedLoadingError() 
        {
            if (errors == 1)
            {
                status.text = $"Status: Finished with an Error";
            }
            else
            {
                status.text = $"Status: Finished with Errors";
            }
            reoladButton.interactable = true;
        }
        
        // Callbacks from the PictureDonwloaders
        public void PictureLoaded()
        {
            picturesLoaded++;
            if (errors == 0)
            {
                indicator.text = $"{picturesLoaded} / {picturesToLoad}";
            }
            else if (errors == 1)
            {
                indicator.text = $"{picturesLoaded} / {picturesToLoad} | Errors: {errors}";
            }
            else 
            {
                indicator.text = $"{picturesLoaded} / {picturesToLoad} | Errors: {errors}s";
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
            if (errors == 1)
            {
                indicator.text = $"{picturesLoaded} / {picturesToLoad} | Errors: {errors}";
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
    }
}