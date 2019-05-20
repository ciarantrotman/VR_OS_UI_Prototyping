using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class ToolController : SerializedMonoBehaviour
    {
        [OdinSerialize] public Dictionary<string, BaseTool> tools;
        private const float Spacing = .1f;
        
        public void Initialise(GameObject player, bool startsActive, ControllerTransforms controller, ToolMenu.Handedness handedness, ToolMenu toolMenu)
        {
            var x = 0f;
            foreach (var item in tools)
            {
                var n = item.Key;
                var tool = item.Value;
                tool.toolController = this;
                tool.toolButton.player = player;
                tool.toolButton.enabled = startsActive;
                tool.controller = controller;
                tool.toolMenu = toolMenu;
                tool.dominant.name = n + "/Dominant";
                tool.nonDominant.name = n + "/Non-Dominant";
                tool.toolButton.buttonText.SetText(item.Key + " Tool");
                tool.handedness = handedness;
                tool.buttonPrefab.name = n + "/Button";
                tool.buttonPrefab.transform.SetParent(transform);
                tool.buttonPrefab.transform.localPosition = new Vector3(x, 0, .2f);
                tool.dominant.transform.SetParent(transform);
                tool.nonDominant.transform.SetParent(transform);
                x += Spacing;
            }
        }

        public void ToggleTool(BaseTool activeTool)
        {
            foreach (var item in tools)
            {
                var tool = item.Value;
                tool.SetToolState(activeTool == tool);
                SetButtonVisualState(tool, activeTool == tool);
            }
        }

        public void ToggleButtonState(bool state)
        {
            foreach (var item in tools)
            {
                var tool = item.Value;
                tool.buttonPrefab.SetActive(state);
                tool.toolButton.enabled = state;
            }
        }

        private static void SetButtonVisualState(BaseTool tool, bool active)
        {
            tool.toolButton.SetState(active);
        }
    }
}
