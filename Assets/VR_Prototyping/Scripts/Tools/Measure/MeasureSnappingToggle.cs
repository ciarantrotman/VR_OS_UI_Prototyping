using VR_Prototyping.Scripts.Tools.Sketch;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools.Measure
{
    public class MeasureSnappingToggle : ToolMenuButton
    {
        private MeasureTool measureTool;
        private void Start()
        {
            measureTool = transform.parent.transform.GetComponentInParent<MeasureTool>();
            controller = measureTool.Controller;
            activate.AddListener(ToggleSnapOn);
            deactivate.AddListener(ToggleSnapOff);
            InitialiseSelectableObject();
        }

        private void ToggleSnapOn()
        {
            measureTool.axisSnapping = true;
        }
        
        private void ToggleSnapOff()
        {
            measureTool.axisSnapping = false;
        }
    }
}
