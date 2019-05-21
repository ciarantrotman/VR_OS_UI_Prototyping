﻿using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class MeasureVisual : MonoBehaviour
    {
        private MeasureTool _measureTool;
        private MeshRenderer _meshRenderer;       
        
        private void Start()
        {
            _measureTool = transform.parent.transform.GetComponentInParent<MeasureTool>();
            _meshRenderer = transform.GetComponent<MeshRenderer>();
            _meshRenderer.material = _measureTool.tapeMaterial;
            _meshRenderer.material.color = _measureTool.tapeColor;
        }
    }
}
