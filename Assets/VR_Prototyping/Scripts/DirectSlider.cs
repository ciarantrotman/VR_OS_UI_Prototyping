using System;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Valve.VR.InteractionSystem.Sample;

namespace VR_Prototyping.Scripts
{
    public class DirectSlider : MonoBehaviour
    {
        private LineRenderer activeLr;
        private LineRenderer inactiveLr;

        private float sliderValue;
        
        private const float DirectDistance = .05f;

        private GameObject slider;
        private GameObject min;
        private GameObject max;
        private GameObject handle;
        private GameObject handleNormalised;
        
        [HideInInspector] public Vector3 sliderMaxPos;
        [HideInInspector] public Vector3 sliderMinPos;
        [ShowIf("manualSliderSetup")] [Button("Manual Setup")]
        private void ManualSetup()
        {
            var p = transform.localPosition;
            sliderMaxPos = new Vector3(p.x + .25f, p.y, p.z);
            sliderMinPos = new Vector3(p.x - .25f, p.y, p.z);
        }

        private Rigidbody rb;

        [BoxGroup("Script Setup")] [SerializeField] [Required]
        private ControllerTransforms c;

        [TabGroup("Slider Settings")] [Range(.01f, .5f)] [SerializeField] private float directGrabDistance;
        [TabGroup("Slider Settings")] [Header("Slider Values")] [Space(5)] [SerializeField] [Range(0f, 1f)] private float startingValue;
        [TabGroup("Slider Settings")] [HideIf("manualSliderSetup")] [Indent] [Range(.01f, 2f)] public float sliderMax;
        [TabGroup("Slider Settings")] [HideIf("manualSliderSetup")] [Indent] [Range(.01f, 2f)] public float sliderMin;
        [TabGroup("Slider Settings")] [Space(10)] public bool manualSliderSetup;
        
        [TabGroup("Aesthetics Settings")] [SerializeField] [Range(.001f, .005f)] private float activeWidth;
        [TabGroup("Aesthetics Settings")] [SerializeField] [Range(.001f, .005f)] private float inactiveWidth;
        [TabGroup("Aesthetics Settings")] [SerializeField] [Required] [Space(10)] private Material sliderMaterial;
        [TabGroup("Aesthetics Settings")] [SerializeField] [Required] [Space(5)] private GameObject sliderCap;
        [TabGroup("Aesthetics Settings")] [SerializeField] [Required] private GameObject sliderHandle;

        private void Start()
        {
            InitialiseSelectableObject();
        }

        private void InitialiseSelectableObject()
        {
            SetupSlider();
        }

        private void SetupSlider()
        {
            var o = gameObject;
            slider = o;
            o.name = "Slider/Slider";
            min = Instantiate(sliderCap, slider.transform); // new GameObject("Slider/Min");
            max = Instantiate(sliderCap, slider.transform); //new GameObject("Slider/Max");
            handle = Instantiate(sliderHandle, slider.transform); //new GameObject("Slider/Handle");
            handleNormalised = new GameObject("Slider/Handle/Follow");

            min.transform.SetParent(slider.transform);
            max.transform.SetParent(slider.transform);
            handle.transform.SetParent(slider.transform);
            handleNormalised.transform.SetParent(slider.transform);

            activeLr = LineRender(min.transform, activeWidth);
            inactiveLr = LineRender(max.transform, inactiveWidth);

            rb = Setup.AddOrGetRigidbody(handle.transform);
            Set.RigidBody(rb, .1f, 4.5f, true, false);
            
            switch (manualSliderSetup)
            {
                case true:
                    min.transform.position =  sliderMinPos;
                    max.transform.position = sliderMaxPos;
                    sliderMin = transform.InverseTransformPoint(min.transform.localPosition).x;
                    if (sliderMin < 0) sliderMin = -sliderMin;
                    sliderMax = transform.InverseTransformPoint(max.transform.localPosition).x;
                    handle.transform.position = Vector3.Lerp(sliderMinPos, sliderMaxPos, startingValue);
                    handleNormalised.transform.localPosition = handle.transform.localPosition;
                    break;
                case false:
                    min.transform.localPosition = new Vector3(-sliderMin, 0, 0);
                    max.transform.localPosition = new Vector3(sliderMax, 0, 0);
                    handle.transform.localPosition = new Vector3(Mathf.Lerp(-sliderMin, sliderMax, startingValue), 0, 0);
                    handleNormalised.transform.localPosition = handle.transform.localPosition;
                    break;
                default:
                    throw new ArgumentException();
            }
        }
        
        private LineRenderer LineRender(Component a, float width)
        {
            var lr = a.gameObject.AddComponent<LineRenderer>();
            Setup.LineRender(lr, sliderMaterial, width, true);
            return lr;
        }

        private void FixedUpdate()
        {
            DrawLineRender(activeLr, min.transform, handle.transform);
            DrawLineRender(inactiveLr, max.transform, handle.transform);
            
            DirectSliderCheck(c.RightControllerTransform(), c.RightGrab());
            DirectSliderCheck(c.LeftControllerTransform(), c.LeftGrab());
            
            //sliderValue = SliderValue(sliderMax, sliderMin, HandleFollow());
        }

        private void DirectSliderCheck(Transform controller, bool grab)
        {          
            if (Vector3.Distance(handleNormalised.transform.position, controller.position) < directGrabDistance && !grab)
            {
                Set.TransformLerpPosition(handle.transform, controller, .05f);
            }
            if (Vector3.Distance(handle.transform.position, controller.position) < DirectDistance && grab)
            {
                sliderValue = SliderValue(sliderMax, sliderMin, HandleFollow());
                Set.TransformLerpPosition(handle.transform, controller, .5f);
                return;
            }
            Set.TransformLerpPosition(handle.transform, handleNormalised.transform, .05f);
        }
        
        private static float SliderValue(float max, float min, float current)
        {
            return Mathf.InverseLerp(-min, max, current);
        }

        private float HandleFollow()
        {
            var value = handle.transform.localPosition;
            var target = new Vector3(value.x, 0, 0);
            if (value.x <= -sliderMin) target = new Vector3(-sliderMin, 0, 0);
            if (value.x >= sliderMax) target = new Vector3(sliderMax, 0, 0);
            Set.VectorLerpLocalPosition(handleNormalised.transform, target, .2f);
            return handleNormalised.transform.localPosition.x;
        }

        private static void DrawLineRender(LineRenderer lr, Transform start, Transform end)
        {
            lr.SetPosition(0, start.position);
            lr.SetPosition(1, end.position);
        }
    }
    
    [CustomEditor(typeof(DirectSlider)), CanEditMultipleObjects]
    public sealed class PositionHandleEditor : Sirenix.OdinInspector.Editor.OdinEditor
    {
        private void OnSceneGUI()
        {
            var slider = (DirectSlider)target;
            var transform = slider.transform;
            var right = transform.right;
            var position = transform.position;
            var size = HandleUtility.GetHandleSize(slider.sliderMaxPos) * .25f;
            const float snap = .1f;
            
            EditorGUI.BeginChangeCheck();

            if (!slider.manualSliderSetup) return; 
            
            var max = Handles.Slider(slider.sliderMaxPos, right, size, Handles.ConeHandleCap, snap);
            var min = Handles.Slider(slider.sliderMinPos, -right, size, Handles.ConeHandleCap, snap);

            Handles.DrawLine(position, slider.sliderMaxPos);
            Handles.DrawLine(position, slider.sliderMinPos);

            if (!EditorGUI.EndChangeCheck()) return;
            slider.sliderMaxPos = max;
            slider.sliderMinPos = min;
        }
    }
}