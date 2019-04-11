using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    [Serializable] [RequireComponent(typeof(SelectableObject))]
    public class BaseTool : MonoBehaviour
    {
        public SelectableObject Button { get; private set; }
        public ControllerTransforms Controller { get; set; }
        public ToolController ToolController { get; set; }
        public ToolMenu ToolMenu { get; set; }

        public ToolMenu.Handedness Handedness { get; set; }
        public bool Active { get; private set; }

        [BoxGroup("Tool Prefabs")] [Required] [SerializeField] public GameObject nonDominant;
        [BoxGroup("Tool Prefabs")] [Required] [SerializeField] public GameObject dominant;

        private void Awake()
        {
            Button = GetComponent<SelectableObject>();
            Button.enabled = false;
            Button.selectEnd.AddListener(SelectTool);
            
            SetupMenuItems();
        }

        private void SetupMenuItems()
        {
            dominant = Instantiate(dominant);
            dominant.SetActive(false);
            
            nonDominant = Instantiate(nonDominant);
            nonDominant.SetActive(false);
        }

        public void SetToolState(bool state)
        {
            dominant.SetActive(state);
            nonDominant.SetActive(state);
            ToolController.SetState(state);
            Active = state;
            
            ToolMenu.SetState(state, transform);
        }

        private void SelectTool()
        {
            ToolController.ToggleTool(this);
        }

        private void Update()
        {
            if(!Active) return;
            switch (Handedness)
            {
                case ToolMenu.Handedness.Right:
                    Set.Transforms(dominant.transform, Controller.RightControllerTransform());
                    Set.Transforms(nonDominant.transform, Controller.LeftControllerTransform());
                    break;
                case ToolMenu.Handedness.Left:
                    Set.Transforms(dominant.transform, Controller.LeftControllerTransform());
                    Set.Transforms(nonDominant.transform, Controller.RightControllerTransform());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
