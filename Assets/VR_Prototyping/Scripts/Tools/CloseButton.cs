using Sirenix.OdinInspector;
using UnityEngine;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools
{
    public class CloseButton : DirectButton
    {
        private BaseTool tool;
        [BoxGroup("Close Button Settings")] [SerializeField] private Color buttonColor = new Color(0,0,0, 255);

        private void Start()
        {
            tool = transform.parent.transform.GetComponentInParent<BaseTool>();
            c = tool.controller;
            activate.AddListener(DeactivateTool);
            
            SetupButton();
            
            buttonVisual.material.color = buttonColor;
        }

        private void DeactivateTool()
        {
            tool.SetToolState(false);
        }
    }
}
