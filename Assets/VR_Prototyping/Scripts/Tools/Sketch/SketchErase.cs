using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools.Sketch
{
    public class SketchErase : DirectButton
    {
        private SketchTool sketchTool;

        private void Start()
        {
            sketchTool = transform.parent.transform.GetComponentInParent<SketchTool>();
            controller = sketchTool.Controller;
            activate.AddListener(EraseDeactivate);
            deactivate.AddListener(EraseActivate);
            InitialiseSelectableObject();
        }

        private void EraseActivate()
        {
            sketchTool.EraseToggle(true);
        }
        
        private void EraseDeactivate()
        {
            sketchTool.EraseToggle(false);
        }
    }
}
