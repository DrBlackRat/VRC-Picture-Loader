using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace DrBlackRat.VRC.PictureLoader 
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TabletMenu : UdonSharpBehaviour
    {
        [Header("Settings")]
        [Tooltip("Default state of the Tablet UI, disable to hide it by default.")]
        [SerializeField] private bool menuShown = true;
        [Space(10)]
        [Tooltip("URL Input the Menu is connected to, used to hide the Menu once loading has finished.")]
        [SerializeField] private PictureLoaderURLInput urlInput;
        [Tooltip("Hides the Menu once the Picture has finished loading.")]
        [SerializeField] private bool hideUiOnFinish = true;
        [Space(10)]
        [SerializeField] private RectTransform inputTransform;
        [SerializeField] private RectTransform bgTransform;
        [Header("Animation")]
        [SerializeField] private Vector2 normalBgSize;
        [SerializeField] private Vector2 hiddenBgSize;
        [SerializeField] private Vector2 normalBgPos;
        [SerializeField] private Vector2 hiddenBgPos;
        [Space(10)] 
        [SerializeField] private Vector2 normalPos;
        [SerializeField] private Vector2 hiddenPos;
        [Space(10)] 
        [SerializeField] private AnimationCurve smoothingCurve;
        [SerializeField] private float movementDuration;
        
        private bool animate;
        private float elapsedTime;

        private Vector2 newBgSize;
        private Vector2 oldBgSize;
        private Vector2 newBgPos;
        private Vector2 oldBgPos;
        private Vector2 newPos;
        private Vector2 oldPos;

        private void Start()
        {
            urlInput.tabletMenu = this;
            ToggleUI();
        }

        private void Update()
        {
            if (animate) AnimateUI();
        }
        
        public void _MenuPressed()
        {
            menuShown = !menuShown;
            ToggleUI();
        }

        public void _HiddeUI()
        {
            if (!hideUiOnFinish) return;
            if (!menuShown) return;
            menuShown = false;
            ToggleUI();
        }
        
        private void ToggleUI()
        {
            animate = true;
            elapsedTime = 0f;
            oldBgSize = bgTransform.sizeDelta;
            oldBgPos = bgTransform.anchoredPosition;
            oldPos = inputTransform.anchoredPosition;
            
            if (menuShown)
            {
                newBgSize = normalBgSize;
                newBgPos = normalBgPos;
                newPos = normalPos;
            }
            else
            {
                newBgSize = hiddenBgSize;
                newBgPos = hiddenBgPos;
                newPos = hiddenPos;
            } 
        }
        
        private void AnimateUI()
        {
            elapsedTime += Time.deltaTime;
            var percentageComplete = elapsedTime / movementDuration;
            UpdateUI(smoothingCurve.Evaluate(percentageComplete));
            if (percentageComplete >= 1f)
            {
                elapsedTime = 0f;
                animate = false;
            }
        }

        private void UpdateUI(float transition)
        {
            bgTransform.sizeDelta = Vector2.LerpUnclamped(oldBgSize, newBgSize, transition);
            bgTransform.anchoredPosition = Vector2.LerpUnclamped(oldBgPos, newBgPos, transition);
            
            inputTransform.anchoredPosition = Vector2.LerpUnclamped(oldPos, newPos, transition);
        }
    }
}

