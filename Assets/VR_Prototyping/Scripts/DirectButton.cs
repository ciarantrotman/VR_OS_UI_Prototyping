using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.UIElements;

namespace VR_Prototyping.Scripts
{
    public class DirectButton : MonoBehaviour
    {
        private LineRenderer targetLr;

        private MeshFilter buttonSurface;
        private MeshRenderer buttonVisual;
        private MeshCollider buttonCollider;
        
        private const float DirectDistance = .05f;

        private GameObject parent;
        private GameObject target;
        private GameObject button;
        private GameObject visual;

        private GameObject restTarget;
        private GameObject hoverTarget;
        private GameObject toggleTarget;
        
        private Rigidbody rb;

        public enum ButtonState
        {
            Inactive,
            Hover,
            Touch,
            Active,
            Over
        }
        
        public ButtonState buttonState;

        [BoxGroup("Script Setup")] [SerializeField] [Required] private ControllerTransforms c;

        [TabGroup("Button Settings")] [Header("Button Setup")] [SerializeField] private bool toggle;
        [TabGroup("Button Settings")] [ShowIf("toggle")] [SerializeField] [Indent] private bool startsActive;
        [TabGroup("Button Settings")] [ShowIf("toggle")] [Indent] [Range(.01f, .05f)] public float toggleDepth;
        [TabGroup("Button Settings")] [Space(5)] [Range(0f, .05f)] public float restDepth;
        [TabGroup("Button Settings")] [Range(0f, .1f)] [ValidateInput("ValidateDepth", "Hover Depth should be bigger than Rest Depth!", InfoMessageType.Warning)] public float hoverDepth;
        [TabGroup("Button Settings")] [Range(.01f, .05f)] public float buttonRadius;
        [TabGroup("Button Settings")] [Range(.01f, .05f)] [ValidateInput("ValidateRadius", "Target Radius should be bigger than Button Radius!", InfoMessageType.Warning)] public float targetRadius;
        [TabGroup("Button Settings")] [Space(5)] public bool ignoreLeftHand;
        [TabGroup("Button Settings")] public bool ignoreRightHand;
        private bool ValidateRadius(float value)
        {
            return value > buttonRadius;
        }
        private bool ValidateDepth(float value)
        {
            return value > restDepth;
        }
        [TabGroup("Aesthetics Settings")] [SerializeField] [Range(.001f, .005f)] private float targetLineRenderWidth;
        [TabGroup("Aesthetics Settings")] [SerializeField] [Indent] [Range(6, 360)] private int circleQuality;
        [TabGroup("Aesthetics Settings")] [SerializeField] [Required] [Space(10)] private Material buttonMaterial;
        [TabGroup("Aesthetics Settings")] [SerializeField] [Required] [Space(10)] private Material targetMaterial;
        
        [BoxGroup("Button Events")] [SerializeField] private UnityEvent active;
        [BoxGroup("Button Events")] [SerializeField] private UnityEvent touch;
        [BoxGroup("Button Events")] [SerializeField] private UnityEvent over;
        [BoxGroup("Button Events")] [SerializeField] private UnityEvent hover;
        private void Start()
        {
            InitialiseSelectableObject();
        }

        private void InitialiseSelectableObject()
        {
            SetupButton();
        }

        private void SetupButton()
        {
            parent = gameObject;
            parent.name = "Button/Parent";
            target = new GameObject("Button/Target");
            visual = new GameObject("Button/Visual") {layer = c.layerIndex};
            button = new GameObject("Button/Button") {layer = c.layerIndex};

            restTarget = new GameObject("Button/RestTarget");
            hoverTarget = new GameObject("Button/HoverTarget");
            toggleTarget = new GameObject("Button/ToggleTarget");
            
            target.transform.SetParent(parent.transform);
            button.transform.SetParent(parent.transform);
            visual.transform.SetParent(button.transform);

            restTarget.transform.SetParent(parent.transform);
            hoverTarget .transform.SetParent(parent.transform);
            toggleTarget.transform.SetParent(parent.transform);
            
            targetLr = LineRender(target.transform, targetLineRenderWidth);
            Draw.CircleLineRenderer(targetLr, targetRadius, Draw.Orientation.Forward, circleQuality);
            
            rb = Setup.AddOrGetRigidbody(button.transform);
            Set.RigidBody(rb, .1f, 4.5f, true, false);
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            
            buttonSurface = visual.AddComponent<MeshFilter>();
            buttonVisual = visual.AddComponent<MeshRenderer>();
            buttonVisual.material = buttonMaterial;
            buttonSurface.mesh = Draw.GenerateCircleMesh(buttonRadius, Draw.Orientation.Forward);
            buttonCollider = visual.AddComponent<MeshCollider>();
            buttonCollider.convex = true;
            
            Set.LocalTransformZero(target.transform);
            Set.LocalTransformZero(button.transform);
            Set.LocalTransformZero(visual.transform);

            restTarget.transform.localPosition = Offset(target.transform, restDepth);
            hoverTarget.transform.localPosition = Offset(target.transform, hoverDepth);
            toggleTarget.transform.localPosition = Offset(target.transform, -toggleDepth);
        }
   
        private LineRenderer LineRender(Component a, float width)
        {
            var lr = a.gameObject.AddComponent<LineRenderer>();
            Setup.LineRender(lr, targetMaterial, width, true);
            return lr;
        }

        private void FixedUpdate()
        {
            var buttonPos = button.transform.position;
            
            switch (buttonState)
            {
                case ButtonState.Inactive:
                    rb.AddForce(Force(restTarget, buttonPos), ForceMode.Force);
                    break;
                case ButtonState.Hover:
                    rb.AddForce(Force(hoverTarget, buttonPos), ForceMode.Force);
                    break;
                case ButtonState.Active:
                    rb.AddForce(Force(target, buttonPos), ForceMode.Force);
                    break;
                case ButtonState.Touch:
                    break;
                case ButtonState.Over:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!c.debugActive) return;
            Debug.DrawRay(buttonPos, Force(hoverTarget, buttonPos), Color.yellow);
            Debug.DrawRay(buttonPos, Force(restTarget, buttonPos), Color.red);
            Debug.DrawRay(buttonPos, Force(target, buttonPos), Color.green);
        }

        private static Vector3 Offset(Transform local, float offset)
        {
            var pos = local.localPosition;
            return new Vector3(pos.x, pos.y, pos.z + offset);
        }

        private static Vector3 Force(GameObject a, Vector3 b)
        {
            return a.transform.position - b;
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(DirectButton)), CanEditMultipleObjects]
    public sealed class DirectButtonSetup : Sirenix.OdinInspector.Editor.OdinEditor
    {
        private void OnSceneGUI()
        {
            var button = (DirectButton)target;
            var transform = button.transform;
            var up = transform.up;
            var forward = transform.forward;
            var right = transform.right;
            var position = transform.position;
            var rest = new Vector3(position.x, position.y, position.z + button.restDepth);
            var hover = new Vector3(position.x, position.y, position.z + button.hoverDepth);
            var size = HandleUtility.GetHandleSize(rest) * .1f;
            const float arc = 360f;
            
            Handles.DrawWireArc(position, forward, right, arc, button.buttonRadius);
            Handles.DrawWireArc(position, forward, right, arc, button.targetRadius);
            Handles.DrawLine(position, rest);
            Handles.DrawLine(position, hover);
            Handles.Slider(rest, forward, size, Handles.ConeHandleCap, .1f);
            Handles.Slider(hover, forward, size, Handles.ConeHandleCap, .1f);
        }
    }
#endif
}
