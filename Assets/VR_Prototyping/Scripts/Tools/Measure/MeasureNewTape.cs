using UnityEngine;
using VR_Prototyping.Scripts.Tools.Measure;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools
{
    public class MeasureNewTape : DirectButton
    {
        private MeasureTool measureTool;
        private MeshRenderer meshRenderer;       
        
        private void Start()
        {
            measureTool = transform.parent.transform.GetComponentInParent<MeasureTool>();
            controller = measureTool.controller;
            activate.AddListener(measureTool.NewTape);    
            SetupButton();
        }
    }
}
