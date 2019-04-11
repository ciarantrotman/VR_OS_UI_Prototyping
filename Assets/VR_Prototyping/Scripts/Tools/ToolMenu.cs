using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    [RequireComponent(typeof(ControllerTransforms))]
    public class ToolMenu : MonoBehaviour
    {
        [HideInInspector] public bool active;
        private bool cMenu;
        private bool pMenu;
        
        private bool trigger;
        
        private const float A = 5f;
        private const float D = .1f;
        
        private ControllerTransforms c;
        private ToolController toolController;
        
        [BoxGroup("Script Setup")] [Required] [SerializeField] private GameObject menuPrefab;
        
        [BoxGroup("RubberBanding Settings")] [SerializeField] private float angleThreshold = 45f;
        [BoxGroup("RubberBanding Settings")] [SerializeField][Range(0,1)] private float rotateSpeed = .001f;
        [BoxGroup("RubberBanding Settings")] [Space(10)][SerializeField] private float distanceThreshold = .1f;
        [BoxGroup("RubberBanding Settings")] [SerializeField][Range(0,1)] private float moveSpeed = .001f;

        public enum Handedness
        {
            Right,
            Left
        }
        [BoxGroup("Tool Settings")] public Handedness dominantHand;

        private void Start()
        {
            c = GetComponent<ControllerTransforms>();
            SetupMenu();
        }

        private void SetupMenu()
        {
            menuPrefab = Instantiate(menuPrefab);
            menuPrefab.name = "VR Tool Menu";
            menuPrefab.SetActive(active);
            toolController = menuPrefab.GetComponent<ToolController>();
            toolController.Initialise(gameObject, active, c, dominantHand, this);
        }

        private void Update()
        {
            switch (dominantHand)
            {
                case Handedness.Right:
                    cMenu = c.RightMenu();
                    CheckState(cMenu, pMenu, c.RightControllerTransform());
                    pMenu = cMenu;
                    if(!active) break;
                    RubberBanded(c.RightControllerTransform());
                    break;
                case Handedness.Left:
                    cMenu = c.LeftMenu();
                    CheckState(cMenu, pMenu, c.LeftControllerTransform());
                    pMenu = cMenu;
                    if(!active) break;
                    RubberBanded(c.LeftControllerTransform());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CheckState(bool current, bool previous, Transform controller)
        {
            if (!current && previous)
            {
                active = !active;
                SetState(active, controller);
            }
        }

        public void SetState(bool state, Transform controller)
        {
            toolController.SetState(state);
            menuPrefab.SetActive(state);
            if(!state) return;
            menuPrefab.transform.position = controller.position;
            Set.SplitRotation(controller, menuPrefab.transform, false);
        }

        private void RubberBanded(Transform target)
        {
            var d = Vector3.Distance(menuPrefab.transform.position, target.position);
            var a = Set.Divergence(menuPrefab.transform, target);
            
            if (a >= angleThreshold && !trigger || d >= distanceThreshold && !trigger)
            {
                trigger = true;
            }
            if (trigger && (a > A || d > D))
            {
                var rotation = target.rotation;
                menuPrefab.transform.rotation = Quaternion.Lerp(menuPrefab.transform.rotation, new Quaternion(0, rotation.y, 0, rotation.w), rotateSpeed);
                menuPrefab.transform.position = Vector3.Lerp(menuPrefab.transform.position, target.position, moveSpeed);
            }
            else
            {
                trigger = false;
            }
        }
    }
}
