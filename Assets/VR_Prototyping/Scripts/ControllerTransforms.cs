using Sirenix.OdinInspector;
using UnityEngine;
using Valve.VR;

namespace VR_Prototyping.Scripts
{
    public class ControllerTransforms : MonoBehaviour
    {
        public enum SDK
        {
            SteamVR,
            VRTK,
            LeapMotion
        }
        
        [BoxGroup("Settings")] [SerializeField] public bool debugActive;
        [BoxGroup("Settings")] [SerializeField] public bool directInteraction;
        [BoxGroup("Settings")] [ShowIf("directInteraction")] [Indent] [Range(.01f, .05f)] [SerializeField] private float directDistance = .025f;
        [BoxGroup("Settings")] [ShowIf("directInteraction")] [Indent] [Range(0, 31)] public int layerIndex = 9;
        
        [BoxGroup("Transforms")] [SerializeField] [Required] private Transform leftController;
        [BoxGroup("Transforms")] [SerializeField] [Required] private Transform rightController;
        [BoxGroup("Transforms")] [SerializeField] [Required] private Transform hmdCamera;
        [BoxGroup("Transforms")] [SerializeField] [Required] private GameObject vrPlayer;

        [BoxGroup("Aesthetics")] [ SerializeField] [Required] public Material lineRenderMat;
        
        [FoldoutGroup("Button Events")] public SDK VR_SDK;
        [FoldoutGroup("Button Events")] public SteamVR_Action_Boolean grabGrip;
        [FoldoutGroup("Button Events")] public SteamVR_Action_Boolean triggerGrip;
        [FoldoutGroup("Button Events")] public SteamVR_Action_Boolean menu;
        [FoldoutGroup("Button Events")] public SteamVR_Action_Boolean joystickPress;
        [FoldoutGroup("Button Events")] public SteamVR_Action_Vector2 joystickDirection;
        [FoldoutGroup("Button Events")] public SteamVR_Action_Vibration haptic;

        private GameObject lHandDirect;
        private GameObject rHandDirect;
        
        public const string LTag = "Direct/Left";
        public const string RTag = "Direct/Right";
        private void Start()
        {
            lHandDirect = new GameObject(LTag);
            rHandDirect = new GameObject(RTag);
            lHandDirect.layer = layerIndex;
            rHandDirect.layer = layerIndex;
            var ls = lHandDirect.AddComponent<SphereCollider>();
            var rs = rHandDirect.AddComponent<SphereCollider>();
            ls.radius = directDistance;
            rs.radius = directDistance;
        }

        private void FixedUpdate()
        {
            Set.Transforms(lHandDirect.transform, LeftTransform());
            Set.Transforms(rHandDirect.transform, RightTransform());
        }

        public GameObject Player()
        {
            return vrPlayer;
        }
        public Vector3 LeftPosition()
        {
            return leftController.position;
        }
    
        public Vector3 RightPosition()
        {
            return rightController.position;
        }
        
        public Transform LeftTransform()
        {
            return leftController;
        }
    
        public Transform RightTransform()
        {
            return rightController;
        }

        public float ControllerDistance()
        {
            return Vector3.Distance(rightController.position, leftController.position);
        }

        public Transform CameraTransform()
        {
            return hmdCamera;
        }

        public Vector3 CameraPosition()
        {
            return hmdCamera.position;
        }

        public bool LeftGrab()
        {
            return grabGrip.GetState(SteamVR_Input_Sources.LeftHand);
        }
    
        public bool RightGrab()
        {
            return grabGrip.GetState(SteamVR_Input_Sources.RightHand);
        }
        
        public bool LeftMenu()
        {
            return menu.GetState(SteamVR_Input_Sources.LeftHand);
        }
    
        public bool RightMenu()
        {
            return menu.GetState(SteamVR_Input_Sources.RightHand);
        }

        public bool LeftSelect()
        {
            return triggerGrip.GetState(SteamVR_Input_Sources.LeftHand);
        }
        
        public bool RightSelect()
        {
            return triggerGrip.GetState(SteamVR_Input_Sources.RightHand);
        }
        
        public bool LeftJoystickPress()
        {
            return joystickPress.GetState(SteamVR_Input_Sources.LeftHand);
        }
        
        public bool RightJoystickPress()
        {
            return joystickPress.GetState(SteamVR_Input_Sources.RightHand);
        }

        public Vector2 LeftJoystick()
        {
            return joystickDirection.GetAxis(SteamVR_Input_Sources.LeftHand);
        }

        public Vector2 RightJoystick()
        {
            return joystickDirection.GetAxis(SteamVR_Input_Sources.RightHand);
        }
        
        public Vector3 LeftForwardVector()
        {
            return leftController.transform.TransformVector(Vector3.forward);
        }
    
        public Vector3 RightForwardVector()
        {
            return rightController.transform.TransformVector(Vector3.forward);
        }

        public Vector3 CameraForwardVector()
        {
            return hmdCamera.forward;
        }

        public SteamVR_Input_Sources LeftSource()
        {
            return SteamVR_Input_Sources.LeftHand;
        }
        
        public SteamVR_Input_Sources RightSource()
        {
            return SteamVR_Input_Sources.RightHand;
        }
    }
}

