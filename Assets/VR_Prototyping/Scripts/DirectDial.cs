using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace VR_Prototyping.Scripts
{
    [DisallowMultipleComponent]
    public class DirectDial : MonoBehaviour
    {
        private LineRenderer circleLr;
        private LineRenderer spokeLr;
        
        private const float DirectDistance = .05f;

        private GameObject dial;
        private GameObject center;
        private GameObject anchor;
        private GameObject handle;
        private GameObject handleNormalised;

        private Rigidbody rb;
        
        [HideInInspector] public float dialValue;

        [BoxGroup("Script Setup")] [SerializeField] [Required] private ControllerTransforms c;

        [TabGroup("Slider Settings")] [Range(.01f, .5f)] [SerializeField] private float directGrabDistance;
        [TabGroup("Slider Settings")] [Header("Dial Values")] [Space(5)] [SerializeField] [Range(0f, 1f)] private float startingValue;
        [TabGroup("Slider Settings")] [Range(.01f, .25f)] public float dialRadius;
        
        [TabGroup("Aesthetics Settings")] [SerializeField] [Range(.001f, .005f)] private float circleLineRendererWidth;
        [TabGroup("Aesthetics Settings")] [SerializeField] [Range(.001f, .005f)] private float spokeLineRendererWidth;
        [TabGroup("Aesthetics Settings")] [SerializeField] [Indent] [Range(6, 360)] private int circleQuality;
        [TabGroup("Aesthetics Settings")] [SerializeField] [Required] [Space(10)] private Material dialMaterial;
        [TabGroup("Aesthetics Settings")] [SerializeField] [Required] [Space(5)] private GameObject dialCap;
        [TabGroup("Aesthetics Settings")] [SerializeField] [Required] private GameObject dialHandle;

        private void Start()
        {
            InitialiseSelectableObject();
        }

        private void InitialiseSelectableObject()
        {
            SetupDial();
        }

        private void SetupDial()
        {
            var o = gameObject;
            dial = o;
            o.name = "Dial/Dial";
            center = new GameObject("Dial/Center");
            var vis = Instantiate(dialCap, dial.transform);
            handle = Instantiate(dialHandle, dial.transform);
            handle.name = "Dial/Handle";
            handleNormalised = new GameObject("Dial/Handle/Follow");
            anchor = new GameObject("Dial/Anchor");

            vis.transform.SetParent(center.transform);
            center.transform.SetParent(dial.transform);
            anchor.transform.SetParent(center.transform);
            handle.transform.SetParent(dial.transform);
            handleNormalised.transform.SetParent(dial.transform);

            circleLr = LineRender(center.transform, circleLineRendererWidth);
            spokeLr = LineRender(handle.transform, spokeLineRendererWidth);

            Draw.CircleLineRenderer(circleLr, dialRadius, Draw.Orientation.Right, circleQuality);
            
            rb = Setup.AddOrGetRigidbody(handle.transform);
            Set.RigidBody(rb, .1f, 4.5f, true, false);
            
            center.transform.localPosition = Vector3.zero;
            center.transform.localEulerAngles = new Vector3(0, Mathf.Lerp(0,360, startingValue), 0);
            vis.transform.localPosition = Vector3.zero;
            anchor.transform.localPosition = new Vector3(0, 0, dialRadius);
            handle.transform.position = anchor.transform.position;
            handleNormalised.transform.localPosition = handle.transform.localPosition;
        }
   
        private LineRenderer LineRender(Component a, float width)
        {
            var lr = a.gameObject.AddComponent<LineRenderer>();
            Setup.LineRender(lr, dialMaterial, width, true);
            return lr;
        }
        
        private void FixedUpdate()
        {
            DrawLineRender(spokeLr, dial.transform, handle.transform);
            
            DirectSliderCheck(c.RightControllerTransform(), c.RightGrab());
            DirectSliderCheck(c.LeftControllerTransform(), c.LeftGrab());
        }

        private void DirectSliderCheck(Transform controller, bool grab)
        {          
            if (Vector3.Distance(anchor.transform.position, controller.position) < directGrabDistance && !grab)
            {
                Set.TransformLerpPosition(handle.transform, controller, .05f);
            }
            if (Vector3.Distance(handle.transform.position, controller.position) < DirectDistance && grab)
            {
                Debug.Log(dialValue);
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
            return center.transform.localEulerAngles.y;
        }
        
        private static void DrawLineRender(LineRenderer lr, Transform start, Transform end)
        {
            lr.SetPosition(0, start.position);
            lr.SetPosition(1, end.position);
        }
    }
    
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
}