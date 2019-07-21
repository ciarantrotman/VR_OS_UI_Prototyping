using System;
using UnityEngine;

namespace VR_Prototyping.Scripts.LeapMotion
{
    [RequireComponent(typeof(ControllerTransforms))]
    public class HandMenu : MonoBehaviour
    {
        private ControllerTransforms controllerTransforms;
        private LineRenderer proximityLine;
        private void Awake()
        {
            SetupReferences();
            SetupLineRenderer();
        }

        private void SetupReferences()
        {
            controllerTransforms = transform.GetComponent<ControllerTransforms>();
        }
        private void SetupLineRenderer()
        {
            proximityLine = controllerTransforms.leftThumb.AddOrGetLineRenderer();
            proximityLine.SetupLineRender(controllerTransforms.dottedLineRenderMat, .001f, true);
            proximityLine.positionCount = 2;
        }

        private void Update()
        {
            proximityLine.StraightLineRender(controllerTransforms.leftThumb, controllerTransforms.leftIndex);
        }
    }
}
