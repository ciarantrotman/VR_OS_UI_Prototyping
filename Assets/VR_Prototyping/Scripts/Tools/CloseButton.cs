using Sirenix.OdinInspector;
using UnityEngine;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools
{
    public class CloseButton : ToolMenuButton
    {
        private BaseTool tool;

        private void Start()
        {
            tool = transform.parent.transform.GetComponentInParent<BaseTool>();
            controller = tool.Controller;
            activate.AddListener(DeactivateTool);
            InitialiseSelectableObject();
        }

        private void DeactivateTool()
        {
            tool.SetToolState(false);
        }
    }
}
