﻿using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace VR_Prototyping.Scripts
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ControllerTransforms))]
    public class Locomotion : MonoBehaviour
    {
        private GameObject parent;
        private GameObject cN;  // camera normalised
        
        private GameObject rCf; // follow
        private GameObject rCp; // proxy
        private GameObject rCn; // normalised
        private GameObject rMp; // midpoint
        private GameObject rTs; // target
        private GameObject rHp; // hit
        private GameObject rVo; // visual
        private GameObject rRt; // rotation
        
        private GameObject lCf; // follow
        private GameObject lCp; // proxy
        private GameObject lCn; // normalised
        private GameObject lMp; // midpoint
        private GameObject lTs; // target
        private GameObject lHp; // hit
        private GameObject lVo; // visual
        private GameObject lRt; // rotation
        
        private Vector3 _rLastValidPosition;
        private Vector3 _lLastValidPosition;

        private Vignette vignetteLayer;

        private LocomotionPositionPreview _ghost;
        
        [HideInInspector] public LineRenderer lLr;
        [HideInInspector] public LineRenderer rLr;
        
        private const float MaxAngle = 110f;
        private const float MinAngle = 80f;
        
        private const float Trigger = .7f;
        private const float Sensitivity = 10f;
        private const float Tolerance = .1f;
        
        private readonly List<Vector2> rJoystickValues = new List<Vector2>();
        private readonly List<Vector2> lJoystickValues = new List<Vector2>();
        
        private bool pTouchR;
        private bool pTouchL;

        private Vector3 rRotTarget;
        
        private bool active;

        private enum Method
        {
            Dash,
            Blink
        }
        private static bool TypeCheck(Method type)
        {
            return type != Method.Dash;
        }
        
        [BoxGroup("Distance Settings")] [Range(.1f, 1f)] [SerializeField] private float min = .5f;
        [BoxGroup("Distance Settings")] [Range(1f, 100f)] [SerializeField] private float max = 15f;
        
        [BoxGroup("References")] [Range(1, 15)] [SerializeField] private int layerIndex = 10;
        
        [ValidateInput("TypeCheck", "Dash is the recommended locomotion type, but should be disabled for motion sickness prone users.", InfoMessageType.Info)]
        [TabGroup("Locomotion Settings")] [Space(5)] [SerializeField] private Method locomotionMethod = Method.Dash;
        [TabGroup("Locomotion Settings")] [Tooltip("This controls the ability to control the direction you face when moving, it is recommended, but should be disabled for the Vive.")] [Space(5)] [SerializeField] private bool advancedLocomotion = true;
        [TabGroup("Locomotion Settings")] [Space(10)][SerializeField] private bool rotation = true;
        [TabGroup("Locomotion Settings")] [ShowIf("rotation")] [Indent] [Range(15f, 90f)] [SerializeField] private float angle = 45f;
        [TabGroup("Locomotion Settings")] [ShowIf("rotation")] [Indent] [Range(0f, 1f)] [SerializeField] private float rotateSpeed = .15f;
        [TabGroup("Locomotion Settings")] [Space(10)][SerializeField] private bool disableLeftHand;
        [TabGroup("Locomotion Settings")] [SerializeField] private bool disableRightHand;

        [TabGroup("Aesthetic Settings")] [Required] [SerializeField] private GameObject ghost;
        [TabGroup("Aesthetic Settings")] [Required] [SerializeField] private PostProcessVolume volume;
        [TabGroup("Aesthetic Settings")] [SerializeField] private bool motionSicknessVignette;
        [TabGroup("Aesthetic Settings")] [ShowIf("motionSicknessVignette")] [Indent] [Range(0f, 1f)] [SerializeField] private float vignetteStrength = .35f;
        [TabGroup("Aesthetic Settings")] [Range(0f, 1f)] [SerializeField] private float moveSpeed = .75f;
        [TabGroup("Aesthetic Settings")] [Space(5)] [SerializeField] [Required] private GameObject targetVisual;
        [TabGroup("Aesthetic Settings")] [SerializeField] [Required] private AnimationCurve locomotionEasing;
        [TabGroup("Aesthetic Settings")] [SerializeField] [Required] private Material lineRenderMat;
        [TabGroup("Aesthetic Settings")] [Range(3f, 50f)] [SerializeField] private int lineRenderQuality = 40;

        private ControllerTransforms c;
        
        private void Start()
        {
            c = GetComponent<ControllerTransforms>();
            SetupGameObjects();
        }

        private void SetupGameObjects()
        {
            volume.profile.TryGetSettings(out vignetteLayer);
            parent = new GameObject("Locomotion/Calculations");
            var p = parent.transform;
            p.SetParent(transform);
            
            cN = new GameObject("Locomotion/Temporary");
            
            rCf = new GameObject("Locomotion/Follow/Right");
            rCp = new GameObject("Locomotion/Proxy/Right");
            rCn = new GameObject("Locomotion/Normalised/Right");
            rMp = new GameObject("Locomotion/MidPoint/Right");
            rTs = new GameObject("Locomotion/Target/Right");
            rHp = new GameObject("Locomotion/HitPoint/Right");
            rRt = new GameObject("Locomotion/Rotation/Right");
            
            lCf = new GameObject("Locomotion/Follow/Left");
            lCp = new GameObject("Locomotion/Proxy/Left");
            lCn = new GameObject("Locomotion/Normalised/Left");
            lMp = new GameObject("Locomotion/MidPoint/Left");
            lTs = new GameObject("Locomotion/Target/Left");
            lHp = new GameObject("Locomotion/HitPoint/Left");
            lRt = new GameObject("Locomotion/Rotation/Left");
            
            rVo = Instantiate(targetVisual, rHp.transform);
            rVo.name = "Locomotion/Visual/Right";
            rVo.SetActive(false);
            
            lVo = Instantiate(targetVisual, lHp.transform);
            lVo.name = "Locomotion/Visual/Left";
            lVo.SetActive(false);
            
            ghost = Instantiate(ghost);
            ghost.name = "Locomotion/Ghost";
            _ghost = ghost.GetComponent<LocomotionPositionPreview>();
            _ghost.ControllerTransforms = c;
            _ghost.GhostToggle(null, false);;

            rCf.transform.SetParent(p);
            rCp.transform.SetParent(rCf.transform);
            rCn.transform.SetParent(rCf.transform);
            rMp.transform.SetParent(rCp.transform);
            rTs.transform.SetParent(rCn.transform);
            rHp.transform.SetParent(transform);
            rRt.transform.SetParent(rHp.transform);
            
            lCf.transform.SetParent(p);
            lCp.transform.SetParent(lCf.transform);
            lCn.transform.SetParent(lCf.transform);
            lMp.transform.SetParent(lCp.transform);
            lTs.transform.SetParent(lCn.transform);
            lHp.transform.SetParent(transform);
            lRt.transform.SetParent(lHp.transform);
            
            rLr = rCp.AddComponent<LineRenderer>();
            rLr.SetupLineRender(lineRenderMat, .005f, false);
            
            lLr = lCp.AddComponent<LineRenderer>();
            lLr.SetupLineRender(lineRenderMat, .005f, false);
        }

        private void Update()
        {
            rTs.transform.LocalDepth(rCf.ControllerAngle(rCp, rCn, c.RightTransform(), c.CameraTransform(), c.debugActive).CalculateDepth(MaxAngle, MinAngle, max, min, rCp.transform), false, .2f);
            lTs.transform.LocalDepth(lCf.ControllerAngle(lCp, lCn, c.LeftTransform(), c.CameraTransform(), c.debugActive).CalculateDepth(MaxAngle, MinAngle, max, min, lCp.transform), false, .2f);

            rTs.TargetLocation(rHp, _rLastValidPosition = rTs.LastValidPosition(_rLastValidPosition), layerIndex);
            lTs.TargetLocation(lHp, _lLastValidPosition = lTs.LastValidPosition(_lLastValidPosition), layerIndex);

            rMp.transform.LocalDepth(rCp.transform.Midpoint(rTs.transform), false, 0f);
            lMp.transform.LocalDepth(lCp.transform.Midpoint(lTs.transform), false, 0f);
            
            rVo.Target(rHp, rCn.transform, c.RightJoystick(), rRt, advancedLocomotion);
            lVo.Target(lHp, lCn.transform, c.LeftJoystick(), lRt, advancedLocomotion);
            
            rLr.BezierLineRenderer(c.RightTransform().position,rMp.transform.position,rHp.transform.position,lineRenderQuality);
            lLr.BezierLineRenderer(c.LeftTransform().position, lMp.transform.position, lHp.transform.position, lineRenderQuality);
        }

        private void LateUpdate()
        {
            rJoystickValues.JoystickTracking(c.RightJoystick(), Sensitivity);
            lJoystickValues.JoystickTracking(c.LeftJoystick(), Sensitivity);
            
            this.GestureDetection(c.RightJoystick(), rJoystickValues[0], angle, rotateSpeed, Trigger, Tolerance, rVo, rLr, c.RightJoystickPress(), pTouchR, disableRightHand, active);
            this.GestureDetection(c.LeftJoystick(), lJoystickValues[0], angle, rotateSpeed, Trigger, Tolerance, lVo, lLr, c.LeftJoystickPress(), pTouchL, disableLeftHand, active);
            
            pTouchR = c.RightJoystickPress();
            pTouchL = c.LeftJoystickPress();
        }

        private static Vector3 RotationAngle(Transform target, float a)
        {
            var t = target.eulerAngles;
            return new Vector3(t.x, t.y + a, t.z);
        }

        public void RotateUser(float a, float time)
        {
            if(transform.parent == cN.transform || !rotation) return;
            active = true;
            
            c.CameraTransform().SplitRotation(cN.transform, false);
            c.CameraTransform().SplitPosition(transform, cN.transform);
            
            transform.SetParent(cN.transform);
            cN.transform.DORotate(RotationAngle(cN.transform, a), time);
            StartCoroutine(Uncouple(transform, time));
        }

        public void LocomotionStart(GameObject visual, LineRenderer lr)
        {
            visual.SetActive(true);
            _ghost.GhostToggle(visual.transform, true);
            lr.enabled = true;
            active = true;
        }
        
        public void LocomotionEnd(GameObject visual, Vector3 posTarget, Vector3 rotTarget, LineRenderer lr)
        {
            if (transform.parent == cN.transform) return;
            
            c.CameraTransform().SplitRotation(cN.transform, false);
            c.CameraTransform().SplitPosition(transform, cN.transform);
            
            transform.SetParent(cN.transform);
            switch (locomotionMethod)
            {
                case Method.Dash:
                    cN.transform.DOMove(posTarget, moveSpeed);
                    cN.transform.DORotate(rotTarget, moveSpeed);
                    StartCoroutine(Uncouple(transform, moveSpeed));
                    break;
                case Method.Blink:
                    cN.transform.position = posTarget;
                    cN.transform.eulerAngles = rotTarget;
                    transform.SetParent(null);
                    active = false;
                    break;
                default:
                    throw new ArgumentException();
            }
            
            visual.SetActive(false);
            _ghost.GhostToggle(null, false);
            lr.enabled = false;
        }
        
        private IEnumerator Uncouple(Transform a, float time)
        {
            SetVignette(vignetteStrength);
            yield return new WaitForSeconds(time);
            a.SetParent(null);
            active = false;
            SetVignette(0);
            yield return null;
        }

        private void SetVignette(float intensity)
        {
            if (!motionSicknessVignette) return;
            vignetteLayer.intensity.value = intensity;
        }
    }
}
