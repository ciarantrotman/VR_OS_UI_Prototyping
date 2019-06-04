using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace VR_Prototyping.Scripts.UI_Blocks
{
    [DisallowMultipleComponent]
    public class DirectDial : BaseDirectBlock
    {
        private LineRenderer activeCircleLr;
        private LineRenderer inactiveCircleLr;
        private LineRenderer spokeLr;
        
        private const float DirectDistance = .05f;

        private GameObject dial;
        private GameObject center;
        private GameObject anchor;
        private GameObject handle;
        private GameObject handleNormalised;

        [HideInInspector] public float dialValue;

        [TabGroup("Dial Settings")] [Range(.01f, .5f)] [SerializeField] private float directGrabDistance;
        [TabGroup("Dial Settings")] [Header("Dial Values")] [Space(5)] [SerializeField] [Range(0f, 1f)] protected float startingValue;
        [TabGroup("Dial Settings")] [Range(.01f, .25f)] public float dialRadius;
        [TabGroup("Dial Settings")] [Space(5)] public bool ignoreLeftHand;
        [TabGroup("Dial Settings")] public bool ignoreRightHand;
        
        [TabGroup("Aesthetics Settings")] [SerializeField] [Range(.001f, .005f)] private float activeCircleLineRendererWidth;
        [TabGroup("Aesthetics Settings")] [SerializeField] [Range(.001f, .005f)] private float inactiveCircleLineRendererWidth;
        [TabGroup("Aesthetics Settings")] [SerializeField] [Range(.001f, .005f)] private float spokeLineRendererWidth;
        [TabGroup("Aesthetics Settings")] [SerializeField] [Indent] [Range(6, 360)] private int circleQuality;
        [TabGroup("Aesthetics Settings")] [SerializeField] [Required] [Space(10)] protected Material dialMaterial;
        [TabGroup("Aesthetics Settings")] [Required] [Space(5)] [SerializeField] protected GameObject dialCap;
        [TabGroup("Aesthetics Settings")] [Required] [SerializeField] protected GameObject dialHandle;

        private void Start()
        {
            InitialiseSelectableObject();
        }

        private void InitialiseSelectableObject()
        {
            SetupDial();
        }

        public void SetupDial()
        {
            var o = gameObject;
            dial = o;
            o.name = "Dial/Dial";
            center = new GameObject("Dial/Center");
            handle = Instantiate(dialHandle, dial.transform);
            handle.name = "Dial/Handle";
            handleNormalised = new GameObject("Dial/Handle/Follow");
            anchor = new GameObject("Dial/Anchor");
            var vis = Instantiate(dialCap, anchor.transform);
            
            vis.transform.SetParent(anchor.transform);
            center.transform.SetParent(dial.transform);
            anchor.transform.SetParent(center.transform);
            handle.transform.SetParent(dial.transform);
            handleNormalised.transform.SetParent(dial.transform);

            activeCircleLr = LineRender(dial.transform, activeCircleLineRendererWidth);
            inactiveCircleLr = LineRender(center.transform, inactiveCircleLineRendererWidth);
            spokeLr = LineRender(handle.transform, spokeLineRendererWidth);

            Draw.CircleLineRenderer(inactiveCircleLr, dialRadius, Draw.Orientation.Right, circleQuality);
            
            rb = Setup.AddOrGetRigidbody(handle.transform);
            Set.RigidBody(rb, .1f, 4.5f, true, false);
            
            center.transform.localPosition = Vector3.zero;
            center.transform.localEulerAngles = new Vector3(0, Mathf.Lerp(0,360, startingValue), 0);
            vis.transform.localPosition = Vector3.zero;
            anchor.transform.localPosition = new Vector3(0, 0, dialRadius);
            handle.transform.position = anchor.transform.position;
            handleNormalised.transform.localPosition = handle.transform.localPosition;

            dialValue = DialValue(0, 360, center.transform.localEulerAngles.y);
        }
   
        private LineRenderer LineRender(Component a, float width)
        {
            var lr = a.gameObject.AddComponent<LineRenderer>();
            Setup.SetupLineRender(lr, dialMaterial, width, true);
            return lr;
        }
        
        private void FixedUpdate()
        {
            Draw.LineRender(spokeLr, anchor.transform, handle.transform);
            Draw.ArcLineRenderer(activeCircleLr, dialRadius, 0, center.transform.localEulerAngles.y, Draw.Orientation.Right, circleQuality);

            if (!ignoreRightHand)
            {
                DirectDialCheck(c.RightTransform(), c.RightGrab());
            }

            if (!ignoreLeftHand)
            {
                DirectDialCheck(c.LeftTransform(), c.LeftGrab());
            }
        }

        private void DirectDialCheck(Transform controller, bool grab)
        {          
            if (Vector3.Distance(anchor.transform.position, controller.position) < directGrabDistance && !grab)
            {
                Set.TransformLerpPosition(handle.transform, controller, .05f);
            }
            if (Vector3.Distance(handle.transform.position, controller.position) < DirectDistance && grab)
            {
                dialValue = DialValue(360, 0, HandleFollow());
                Set.TransformLerpPosition(handle.transform, controller, .5f);
                return;
            }
            Set.TransformLerpPosition(handle.transform, anchor.transform, .05f);
        }
        
        private static float DialValue(float max, float min, float current)
        {
            return Mathf.InverseLerp(-min, max, current);
        }

        private float HandleFollow()
        {
            var value = handle.transform.localPosition;
            var target = new Vector3(value.x, 0, value.z);
            Set.VectorLerpLocalPosition(handleNormalised.transform, target, .2f);
            center.transform.LookAt(handleNormalised.transform);
            var localEulerAngles = center.transform.localEulerAngles;
            localEulerAngles = new Vector3(0, localEulerAngles.y, 0);
            center.transform.localEulerAngles = localEulerAngles;
            return localEulerAngles.y;
        }
    }
    #if UNITY_EDITOR
    [CustomEditor(typeof(DirectDial)), CanEditMultipleObjects]
    public sealed class DirectDialSetup : Sirenix.OdinInspector.Editor.OdinEditor
    {
        private void OnSceneGUI()
        {
            var dial = (DirectDial)target;
            var transform = dial.transform;
            var up = transform.up;
            var position = transform.position;
            const float arc = 360f;

            Handles.DrawWireArc(position, up, transform.forward, arc, dial.dialRadius);
        }
    }
    #endif
}