using UnityEngine;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools
{
    public class MeasureLockNode : DirectButton
    {
        private MeasureTool _measureTool;
        private MeshRenderer _meshRenderer;       
        
        private void Start()
        {
            _measureTool = transform.parent.transform.GetComponentInParent<MeasureTool>();
            c = _measureTool.controller;
            activate.AddListener(_measureTool.LockNode);
            SetupButton();
        }
    }
}