using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    [Serializable] [RequireComponent(typeof(SelectableObject))]
    public class BaseTool : MonoBehaviour
    {
        public SelectableObject button { get; private set; }
        public ControllerTransforms controller { get; set; }
        public ToolController toolController { get; set; }
        public ToolMenu toolMenu { get; set; }

        [HideInInspector] public ToolMenu.Handedness handedness;
        public bool active { get; private set; }

        [BoxGroup("Tool Prefabs")] [Required] [SerializeField] public GameObject nonDominant;
        [BoxGroup("Tool Prefabs")] [Required] [SerializeField] public GameObject dominant;

        private void Awake()
        {
            button = GetComponent<SelectableObject>();
            button.enabled = false;
            button.selectEnd.AddListener(SelectTool);
            
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
            toolController.SetState(state);
            active = state;
            
            toolMenu.SetState(state, transform);
        }

        private void SelectTool()
        {
            toolController.ToggleTool(this);
        }

        private void Update()
        {
            if(!active) return;
            switch (handedness)
            {
                case ToolMenu.Handedness.Right:
                    Set.Transforms(dominant.transform, controller.RightControllerTransform());
                    Set.Transforms(nonDominant.transform, controller.LeftControllerTransform());
                    break;
                case ToolMenu.Handedness.Left:
                    Set.Transforms(dominant.transform, controller.LeftControllerTransform());
                    Set.Transforms(nonDominant.transform, controller.RightControllerTransform());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
