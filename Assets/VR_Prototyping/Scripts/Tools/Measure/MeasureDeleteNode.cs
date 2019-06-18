using UnityEngine;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools.Measure
{
    public class MeasureDeleteNode : ToolMenuButton
    {
        private MeasureTool measureTool;
        private MeshRenderer meshRenderer;       
        
        private void Start()
        {
            measureTool = transform.parent.transform.GetComponentInParent<MeasureTool>();
            controller = measureTool.Controller;
            activate.AddListener(measureTool.DeleteNode);
            InitialiseSelectableObject();
        }
    }
}