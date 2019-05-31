using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using VR_Prototyping.Scripts.Keyboard;

namespace VR_Prototyping.Scripts.Tools
{
    public class ToolController : SerializedMonoBehaviour
    {
        [BoxGroup("Tool Controls")] [OdinSerialize] public Dictionary<string, BaseTool> tools;
        [BoxGroup("Tool Controls")] [Range(0, 5)] public int gridSize = 3;
        [BoxGroup("Tool Controls")] [Range(0f, 1f)] public float spacing = .1f;
        public void Initialise(GameObject player, bool startsActive, ControllerTransforms controller, ToolMenu.Handedness handedness, ToolMenu toolMenu)
        {
            var x = 0f;
            var y = 0f;
            var toolNumber = 0;
            
            foreach (var item in tools)
            {
                toolNumber++;
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
                tool.buttonPrefab.transform.localPosition = new Vector3(x, y, .2f);
                tool.dominant.transform.SetParent(transform);
                tool.nonDominant.transform.SetParent(transform);
                x += spacing;

                continue;

                Debug.Log(toolNumber);
                               
                Debug.Log(toolNumber % gridSize == 0);
                
                continue;
                
                if (toolNumber % gridSize != 0) continue;
                x = 0f;
                y -= spacing;
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
        
        public void SetAllToolState(bool state)
        {
            foreach (var item in tools)
            {
                var tool = item.Value;
                tool.SetToolState(state);
                SetButtonVisualState(tool, state);
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
