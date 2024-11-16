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
        [Tooltip("Turns Auto Rotation On / Off. Disable this in case you don't need it to rotate to save on performance.")]
        [SerializeField] private bool autoRotation = true;
        [Tooltip("Time between each rotation check in seconds. High values may result in auto rotation feeling unresponsive.")]
        [SerializeField] private float updateInterval = 0.5f;
        [Tooltip("The angle at which the Tablet will switch to a different screen rotation.")]
        [Range(0f, 45f)]
        [SerializeField] private float tolerance = 45f;
        [Header("Internals")] 
        [SerializeField] private RectTransform canvasTransform;
        [SerializeField] private Transform tabletTransform;
        
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
        private Quaternion newRotation;
        private Quaternion oldRotation;

        private TabletRotationState newRotationState = TabletRotationState.HorizontalUp;
        private TabletRotationState oldRotationState = TabletRotationState.HorizontalUp;

        private void Start()
        {
            if (!autoRotation) return;
            // Random Start Delay
            SendCustomEventDelayedSeconds("_CheckRotation", Random.Range(0f, updateInterval));
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
            if (!autoRotation) return;
            SendCustomEventDelayedSeconds("_CheckRotation", updateInterval);
            // Change UI Rotation
            if (newRotationState != oldRotationState) UpdateRotation();
        }

        private void UpdateRotation()
        {
            animate = true;
            elapsedTime = 0f;
            oldSize = canvasTransform.sizeDelta;
            oldRotation = canvasTransform.localRotation;
            switch (newRotationState)
            {
                case TabletRotationState.HorizontalUp:
                    newSize = horizontalSize;
                    newRotation = Quaternion.Euler(horizontalUpRotation);
                    break;
                case TabletRotationState.HorizontalDown:
                    newSize = horizontalSize;
                    newRotation = Quaternion.Euler(horizontalDownRotation);
                    break;
                case TabletRotationState.VerticalUp:
                    newSize = verticalSize;
                    newRotation = Quaternion.Euler(verticalUpRotation);
                    break;
                case TabletRotationState.VerticalDown:
                    newSize = verticalSize;
                    newRotation = Quaternion.Euler(verticalDownRotation);
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
            canvasTransform.localRotation = Quaternion.Lerp(oldRotation, newRotation, transition);
        }
    }
}
