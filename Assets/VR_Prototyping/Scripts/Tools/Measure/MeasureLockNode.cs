using UnityEngine;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools.Measure
{
    public class MeasureLockNode : ToolMenuButton
    {
        private MeasureTool measureTool;
        private MeshRenderer meshRenderer;       
        
        private void Start()
        {
            measureTool = transform.parent.transform.GetComponentInParent<MeasureTool>();
            controller = measureTool.Controller;
            measureTool.MeasureLockNode = this;
            activate.AddListener(measureTool.LockNode);
            InitialiseSelectableObject();
        }
    }
}