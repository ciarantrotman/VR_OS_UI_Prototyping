using Sirenix.OdinInspector;
using UnityEngine;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools
{
    public class CloseButton : DirectButton
    {
        private BaseTool _tool;

        private void Start()
        {
            _tool = transform.parent.transform.GetComponentInParent<BaseTool>();
            c = _tool.controller;
            activate.AddListener(DeactivateTool);
            
            SetupButton();
        }

        private void DeactivateTool()
        {
            _tool.SetToolState(false);
        }
    }
}
