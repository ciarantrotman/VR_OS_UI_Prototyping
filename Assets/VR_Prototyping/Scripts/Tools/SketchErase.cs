using UnityEngine;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools
{
    public class SketchErase : DirectButton
    {
        private SketchTool _sketchTool;

        private void Start()
        {
            _sketchTool = transform.parent.transform.GetComponentInParent<SketchTool>();
            c = _sketchTool.controller;
            activate.AddListener(EraseActivate);
            deactivate.AddListener(EraseDeactivate);
            SetupButton();
        }

        private void EraseActivate()
        {
            _sketchTool.EraseToggle(true);
        }
        
        private void EraseDeactivate()
        {
            _sketchTool.EraseToggle(false);
        }
    }
}
