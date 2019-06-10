﻿using UnityEngine;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools.Measure
{
    public class MeasureLockNode : DirectButton
    {
        private MeasureTool measureTool;
        private MeshRenderer meshRenderer;       
        
        private void Start()
        {
            measureTool = transform.parent.transform.GetComponentInParent<MeasureTool>();
            controller = measureTool.Controller;
            activate.AddListener(measureTool.LockNode);
            SetupButton();
        }
    }
}