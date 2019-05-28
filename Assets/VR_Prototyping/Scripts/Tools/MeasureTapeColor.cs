using UnityEngine;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools
{
    public class MeasureTapeColor : DirectDial
    {
        private MeasureTool _tapeTool;
        
        private void Start()
        {
            _tapeTool = transform.parent.transform.GetComponentInParent<MeasureTool>();
            c = _tapeTool.controller;
            
            SetupDial();
        }

        private void LateUpdate()
        {
            SetColor(dialValue);
        }

        private void SetColor(float colorValue)
        {
            var color = Color.HSVToRGB(colorValue, 1, 1, true);
            _tapeTool.SetColor(color);
        }
    }
}