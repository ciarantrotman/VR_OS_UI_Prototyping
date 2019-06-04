using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace VR_Prototyping.Scripts.UI_Blocks
{
    public abstract class DirectButton : BaseDirectBlock
    {
        private LineRenderer targetLr;

        private MeshFilter buttonSurface;
        protected MeshRenderer buttonVisual;
        private MeshCollider buttonCollider;

        private GameObject parent;
        private GameObject target;
        private GameObject button;
        private GameObject visual;

        private GameObject restTarget;
        private GameObject hoverTarget;
        private GameObject toggleTarget;

        private bool state;
        private bool statePrevious;

        private const float Tolerance = .005f;

        public enum ButtonState
        {
            INACTIVE,
            HOVER,
            ACTIVE
        }

        public ButtonState buttonState { get; private set; }
        private ButtonState previousButtonState;

        [TabGroup("Button Settings")] [Header("Button Function")] public bool toggle;
        [TabGroup("Button Settings")] [ShowIf("toggle")] [SerializeField] [Indent] private bool startsActive;
        [TabGroup("Button Settings")] [ShowIf("toggle")] [Indent] [Range(.01f, .05f)] public float toggleDepth;
        [TabGroup("Button Settings")] [Header("Button Parameters")] [Space(5)] [Range(0f, 10f)] public float springiness = 10f;
        [TabGroup("Button Settings")] [Space(5)] [Range(.01f, .5f)] [SerializeField] private float hoverDistance = .01f;
        [TabGroup("Button Settings")] [Space(5)] [Range(0f, .05f)] public float restDepth = .005f;
        [TabGroup("Button Settings")] [Range(0f, .1f)] [ValidateInput("ValidateDepth", "Hover Depth should be bigger than Rest Depth!", InfoMessageType.Warning)] public float hoverDepth = .02f;
        [TabGroup("Button Settings")] [Range(.01f, .05f)] public float buttonRadius = .02f;
        [TabGroup("Button Settings")] [Range(.01f, .05f)] [ValidateInput("ValidateRadius", "Target Radius should be bigger than Button Radius!", InfoMessageType.Warning)] public float targetRadius = .025f;
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
        [TabGroup("Aesthetics Settings")] [SerializeField] [Range(.001f, .005f)] private float targetLineRenderWidth = .002f;
        [TabGroup("Aesthetics Settings")] [SerializeField] [Indent] [Range(6, 360)] private int circleQuality = 360;
        [TabGroup("Aesthetics Settings")] [SerializeField] [Required] [Space(10)] private Material buttonMaterial;
        [TabGroup("Aesthetics Settings")] [SerializeField] [Required] [Space(10)] private Material targetMaterial;
        
        [BoxGroup("Button Events")] public UnityEvent activate;
        [BoxGroup("Button Events")] [ShowIf("toggle")] public UnityEvent deactivate;
        [BoxGroup("Button Events")] public  UnityEvent hover;
        private void Start()
        {
            InitialiseSelectableObject();
        }

        private void InitialiseSelectableObject()
        {
            SetupButton();
        }

        public void SetupButton()
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
            targetLr.CircleLineRenderer(targetRadius, Draw.Orientation.Forward, circleQuality);
            
            rb = button.transform.AddOrGetRigidbody();
            rb.RigidBody(.1f, 10f, true, false);
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            
            buttonSurface = visual.AddComponent<MeshFilter>();
            buttonVisual = visual.AddComponent<MeshRenderer>();
            buttonVisual.material = buttonMaterial;
            buttonSurface.mesh = buttonRadius.GenerateCircleMesh(Draw.Orientation.Forward);
            buttonCollider = visual.AddComponent<MeshCollider>();
            buttonCollider.convex = true;
            
            Set.LocalTransformZero(target.transform);
            Set.LocalTransformZero(button.transform);
            Set.LocalTransformZero(visual.transform);

            restTarget.transform.localPosition = Offset(target.transform, restDepth);
            hoverTarget.transform.localPosition = Offset(target.transform, hoverDepth);
            toggleTarget.transform.localPosition = Offset(target.transform, -toggleDepth);

            buttonState = toggle && startsActive ? ButtonState.ACTIVE : ButtonState.INACTIVE;

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
            lr.SetupLineRender(targetMaterial, width, true);
            return lr;
        }

        private void FixedUpdate()
        {
            SetState();  
            ButtonAlignment();
            
            var buttonPos = button.transform.position;
            switch (buttonState)
            {
                case ButtonState.INACTIVE:
                    rb.AddForce(Force(restTarget, buttonPos, springiness), ForceMode.Force);
                    break;
                case ButtonState.HOVER:
                    rb.AddForce(Force(hoverTarget, buttonPos, springiness), ForceMode.Force);
                    break;
                case ButtonState.ACTIVE:
                    rb.AddForce(Force(target, buttonPos, springiness), ForceMode.Force);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            statePrevious = state;

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
            if (DirectCheck() && buttonState != ButtonState.ACTIVE)
            {
                buttonState = ButtonState.HOVER;
                hover.Invoke();
            }
            else if (!DirectCheck() && buttonState != ButtonState.ACTIVE)
            {
                buttonState = ButtonState.INACTIVE;
                state = false;
                return;
            }
            
            if (/*buttonState == ButtonState.Hover && */ActiveDistance())// && state != statePrevious)
            {
                buttonState = toggle ? ButtonState.ACTIVE : ButtonState.INACTIVE;
                activate.Invoke();
                state = true;
            }

            if (buttonState == ButtonState.ACTIVE && ToggleDistance() && toggle)
            {
                buttonState = ButtonState.INACTIVE;
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
                return Vector3.Distance(buttonPos, c.RightTransform().position) <= hoverDistance;
            }

            if (ignoreRightHand)
            {
                return Vector3.Distance(buttonPos, c.LeftTransform().position) <= hoverDistance;
            }
            
            return Vector3.Distance(buttonPos, c.LeftTransform().position) <= hoverDistance ||
                   Vector3.Distance(buttonPos, c.RightTransform().position) <= hoverDistance;
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
