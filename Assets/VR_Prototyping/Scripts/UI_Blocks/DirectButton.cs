using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace VR_Prototyping.Scripts.UI_Blocks
{
    public class DirectButton : BaseDirectBlock
    {
        private LineRenderer targetLr;

        private MeshFilter buttonSurface;
        private MeshRenderer buttonVisual;
        private MeshCollider buttonCollider;

        private GameObject parent;
        private GameObject target;
        private GameObject button;
        private GameObject visual;

        private GameObject restTarget;
        private GameObject hoverTarget;
        private GameObject toggleTarget;

        private const float Tolerance = .005f;

        public enum ButtonState
        {
            Inactive,
            Hover,
            Active
        }
        
        [HideInInspector] public ButtonState buttonState;

        [TabGroup("Button Settings")] [Header("Button Function")] public bool toggle;
        [TabGroup("Button Settings")] [ShowIf("toggle")] [SerializeField] [Indent] private bool startsActive;
        [TabGroup("Button Settings")] [ShowIf("toggle")] [Indent] [Range(.01f, .05f)] public float toggleDepth;
        [TabGroup("Button Settings")] [Header("Button Parameters")] [Space(5)] [Range(0f, 10f)] public float springiness;
        [TabGroup("Button Settings")] [Space(5)] [Range(.01f, .5f)] [SerializeField] private float hoverDistance = .05f;
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
        
        [BoxGroup("Button Events")] [SerializeField] private UnityEvent activate;
        [BoxGroup("Button Events")] [ShowIf("toggle")] [SerializeField] private UnityEvent deactivate;
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
            Set.RigidBody(rb, .1f, 10f, true, false);
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            
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

            buttonState = toggle && startsActive ? ButtonState.Active : ButtonState.Inactive;

            if (toggle && startsActive)
            {
                activate.Invoke();
            }
            else if (toggle && !startsActive)
            {
                deactivate.Invoke();
            }
        }
   
        private LineRenderer LineRender(Component a, float width)
        {
            var lr = a.gameObject.AddComponent<LineRenderer>();
            Setup.LineRender(lr, targetMaterial, width, true);
            return lr;
        }

        private void FixedUpdate()
        {
            SetState();  
            ButtonAlignment();
            
            var buttonPos = button.transform.position;
            switch (buttonState)
            {
                case ButtonState.Inactive:
                    rb.AddForce(Force(restTarget, buttonPos, springiness), ForceMode.Force);
                    break;
                case ButtonState.Hover:
                    rb.AddForce(Force(hoverTarget, buttonPos, springiness), ForceMode.Force);
                    break;
                case ButtonState.Active:
                    rb.AddForce(Force(target, buttonPos, springiness), ForceMode.Force);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!c.debugActive) return;
            Debug.DrawRay(buttonPos, Force(hoverTarget, buttonPos, springiness), Color.yellow);
            Debug.DrawRay(buttonPos, Force(restTarget, buttonPos, springiness), Color.red);
            Debug.DrawRay(buttonPos, Force(target, buttonPos, springiness), Color.green);
        }

        private void ButtonAlignment()
        {
            var local = button.transform.localPosition;
            button.transform.localPosition = new Vector3(0, 0, local.z);
        }

        private void SetState()
        {
            if (DirectCheck() && buttonState != ButtonState.Active)
            {
                buttonState = ButtonState.Hover;
                hover.Invoke();
            }
            else if (!DirectCheck() && buttonState != ButtonState.Active)
            {
                buttonState = ButtonState.Inactive;
                return;
            }
            
            if (buttonState == ButtonState.Hover && ActiveDistance())
            {
                buttonState = toggle? ButtonState.Active : ButtonState.Inactive;
                activate.Invoke();
            }

            if (buttonState == ButtonState.Active && ToggleDistance() && toggle)
            {
                buttonState = ButtonState.Inactive;
                deactivate.Invoke();
            }
        }

        private bool ActiveDistance()
        {
            return Vector3.Distance(button.transform.position, target.transform.position) <= Tolerance;
        }
        
        private bool ToggleDistance()
        {
            return Vector3.Distance(button.transform.position, toggleTarget.transform.position) <= Tolerance;
        }

        private bool DirectCheck()
        {
            var buttonPos = button.transform.position;
            
            if (ignoreLeftHand)
            {
                return Vector3.Distance(buttonPos, c.RightControllerTransform().position) <= hoverDistance;
            }

            if (ignoreRightHand)
            {
                return Vector3.Distance(buttonPos, c.LeftControllerTransform().position) <= hoverDistance;
            }
            
            return Vector3.Distance(buttonPos, c.LeftControllerTransform().position) <= hoverDistance ||
                   Vector3.Distance(buttonPos, c.RightControllerTransform().position) <= hoverDistance;
        }

        private static Vector3 Offset(Transform local, float offset)
        {
            var pos = local.localPosition;
            return new Vector3(pos.x, pos.y, pos.z + offset);
        }

        private static Vector3 Force(GameObject a, Vector3 b, float force)
        {
            return (a.transform.position - b) * force;
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
            var forward = transform.forward;
            var right = transform.right;
            var position = transform.position;
            var rest = new Vector3(position.x, position.y, position.z + button.restDepth);
            var hover = new Vector3(position.x, position.y, position.z + button.hoverDepth);
            var toggle = new Vector3(position.x, position.y, position.z - button.toggleDepth);
            var size = HandleUtility.GetHandleSize(rest) * .1f;
            const float arc = 360f;
            
            Handles.DrawWireArc(position, forward, right, arc, button.buttonRadius);
            Handles.DrawWireArc(position, forward, right, arc, button.targetRadius);
            Handles.DrawLine(position, rest);
            Handles.DrawLine(position, hover);
            if(button.toggle)
            {
                Handles.DrawLine(position, toggle);
                Handles.Slider(toggle, -forward, size, Handles.ConeHandleCap, .1f);
            }
            Handles.Slider(rest, forward, size, Handles.ConeHandleCap, .1f);
            Handles.Slider(hover, forward, size, Handles.ConeHandleCap, .1f);
        }
    }
#endif
}
