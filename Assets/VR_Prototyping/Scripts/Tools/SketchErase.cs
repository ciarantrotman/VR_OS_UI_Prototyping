using UnityEngine;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools
{
    public class SketchErase : DirectButton
    {
        private SketchTool sketchTool;

        private void Start()
        {
            sketchTool = transform.parent.transform.GetComponentInParent<SketchTool>();
            controller = sketchTool.controller;
            activate.AddListener(EraseDeactivate);
            deactivate.AddListener(EraseActivate);
            SetupButton();
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
