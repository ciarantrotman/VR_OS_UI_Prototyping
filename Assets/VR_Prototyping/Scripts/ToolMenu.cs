using System;
using Leap.Unity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VR_Prototyping.Scripts
{
    public class ToolMenu : MonoBehaviour
    {
        [HideInInspector] public bool active;
        
        public bool cMenu;
        private bool pMenu;
        
        private bool trigger;
        
        private const float T = .01f;
        
        [BoxGroup("Script Setup")] [Required] [SerializeField] private ControllerTransforms player;
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
            menuPrefab.SetActive(active);
        }

        private void Update()
        {
            switch (dominantHand)
            {
                case Handedness.Right:
                    //cMenu = player.RightMenu();
                    CheckState(cMenu, pMenu, player.RightControllerTransform());
                    pMenu = cMenu;
                    if(!active) break;
                    RubberBanded(player.RightControllerTransform());
                    break;
                case Handedness.Left:
                    //cMenu = player.LeftMenu();
                    CheckState(cMenu, pMenu, player.LeftControllerTransform());
                    pMenu = cMenu;
                    if(!active) break;
                    RubberBanded(player.LeftControllerTransform());
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

        private void SetState(bool state, Transform controller)
        {
            menuPrefab.SetActive(state);
            
            if(!state) return;
            menuPrefab.transform.position = controller.position;
            menuPrefab.transform.LookAwayFrom(controller.transform, Vector3.up);
        }

        private void RubberBanded(Transform target)
        {
            var d = Vector3.Distance(menuPrefab.transform.position, target.position);
            var a = Set.Divergence(menuPrefab.transform, target);
            
            if (a >= angleThreshold && !trigger || d >= distanceThreshold && !trigger)
            {
                trigger = true;
            }
            if (trigger && a > T && d < T)
            {
                menuPrefab.transform.rotation = Quaternion.Lerp(menuPrefab.transform.rotation, target.rotation, rotateSpeed);
                menuPrefab.transform.position = Vector3.Lerp(menuPrefab.transform.position, target.position, moveSpeed);
            }
            else
            {
                trigger = false;
            }
        }
    }
}
