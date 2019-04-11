using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class ToolController : SerializedMonoBehaviour
    {
        [OdinSerialize] public Dictionary<string, BaseTool> tools;
        
        public void Initialise(GameObject player, bool startsActive, ControllerTransforms controller, ToolMenu.Handedness handedness, ToolMenu toolMenu)
        {
            foreach (var item in tools)
            {
                var n = item.Key;
                var tool = item.Value;
                tool.ToolController = this;
                tool.Button.player = player;
                tool.Button.enabled = startsActive;
                tool.Controller = controller;
                tool.ToolMenu = toolMenu;
                tool.dominant.name = n + "/Dominant";
                tool.nonDominant.name = n + "/Non-Dominant";
                tool.name = n + "/Button";
                tool.Button.buttonText.SetText(item.Key + " Tool");
                tool.Handedness = handedness;
            }
        }

        public void ToggleTool(BaseTool activeTool)
        {
            foreach (var item in tools)
            {
                var tool = item.Value;
                tool.SetToolState(activeTool == tool);
            }
        }
        
        public void SetState(bool state)
        {
            foreach (var item in tools)
            {
                item.Value.Button.SetState(state);
                item.Value.Button.enabled = state;
            }
        }
    }
}
