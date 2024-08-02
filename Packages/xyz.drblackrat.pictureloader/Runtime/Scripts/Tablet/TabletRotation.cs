using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace DrBlackRat.VRC.PictureLoader 
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TabletRotation : UdonSharpBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private RectTransform canvasTransform;
        [SerializeField] private Transform tabletTransform;
        [Space(10)] 
        [SerializeField] private float tolerance;
        
        [Header("Animation")]
        [SerializeField] private Vector2 horizontalSize;
        [SerializeField] private Vector2 verticalSize;
        [Space(10)] 
        [SerializeField] private Vector3 horizontalUpRotation;
        [SerializeField] private Vector3 horizontalDownRotation;
        [SerializeField] private Vector3 verticalUpRotation;
        [SerializeField] private Vector3 verticalDownRotation;
        [Space(10)]
        [SerializeField] private AnimationCurve smoothingCurve;
        [SerializeField] private float movementDuration;
        
        private bool animate;
        private float elapsedTime;

        private Vector2 newSize;
        private Vector2 oldSize;
        private Vector3 newRotation;
        private Vector3 oldRotation;

        private TabletRotationState newRotationState = TabletRotationState.HorizontalUp;
        private TabletRotationState oldRotationState = TabletRotationState.HorizontalUp;

        private void Start()
        {
            _CheckRotation();
        }

        private void Update()
        {
            if (animate) Animate();
        }

        public void _CheckRotation()
        {
            Vector3 tabletUp = tabletTransform.up;
            Vector3 tabletRight = tabletTransform.right;
            
            if (Vector3.Angle(tabletUp, Vector3.up) <= tolerance)
            {
                oldRotationState = newRotationState;
                newRotationState = TabletRotationState.HorizontalUp;
            }
            else if (Vector3.Angle(tabletUp, Vector3.down) <= tolerance)
            {
                oldRotationState = newRotationState;
                newRotationState = TabletRotationState.HorizontalDown;
            }
            else if (Vector3.Angle(tabletRight, Vector3.down) <= tolerance)
            {
                oldRotationState = newRotationState;
                newRotationState = TabletRotationState.VerticalUp;
            }
            else if (Vector3.Angle(tabletRight, Vector3.up) <= tolerance)
            {
                oldRotationState = newRotationState;
                newRotationState = TabletRotationState.VerticalDown;
            }
            // Slow Update Loop
            SendCustomEventDelayedSeconds("_CheckRotation", 0.5f);
            // Change UI Rotation
            if (newRotationState == oldRotationState) return;
            animate = true;
            elapsedTime = 0f;
            oldSize = canvasTransform.sizeDelta;
            oldRotation = canvasTransform.localEulerAngles;
            switch (newRotationState)
            {
                case TabletRotationState.HorizontalUp:
                    newSize = horizontalSize;
                    newRotation = horizontalUpRotation;
                    break;
                case TabletRotationState.HorizontalDown:
                    newSize = horizontalSize;
                    newRotation = horizontalDownRotation;
                    break;
                case TabletRotationState.VerticalUp:
                    newSize = verticalSize;
                    newRotation = verticalUpRotation;
                    break;
                case TabletRotationState.VerticalDown:
                    newSize = verticalSize;
                    newRotation = verticalDownRotation;
                    break;
            }
        }
        
        private void Animate()
        {
            elapsedTime += Time.deltaTime;
            var percentageComplete = elapsedTime / movementDuration;
            RotateUI(smoothingCurve.Evaluate(percentageComplete));
            if (percentageComplete >= 1f)
            {
                elapsedTime = 0f;
                animate = false;
            }
        }

        private void RotateUI(float transition)
        {
            canvasTransform.sizeDelta = Vector3.Lerp(oldSize, newSize, transition);
            canvasTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(oldRotation), Quaternion.Euler(newRotation), transition);
        }
    }
}
