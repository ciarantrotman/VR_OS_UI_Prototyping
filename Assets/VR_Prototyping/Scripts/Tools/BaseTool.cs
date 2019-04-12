using System;
using Sirenix.OdinInspector;
using UnityEngine;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools
{
    [Serializable]
    public class BaseTool : MonoBehaviour
    {
        public ControllerTransforms controller { private get; set; }
        public ToolController toolController { private get; set; }
        public ToolMenu toolMenu { private get; set; }
        public SelectableObject toolButton { get; private set; }
        public ToolMenu.Handedness handedness { private get; set; }
        protected bool active { get; private set; }
        protected bool cTrigger { get; private set; }
        protected bool pTrigger { get; set; }

        private BaseDirectBlock[] directInterfaceBlocks;
        
        [FoldoutGroup("Generic Tool Prefabs")] [Required] public GameObject buttonPrefab;
        [FoldoutGroup("Generic Tool Prefabs")] [Required] public GameObject nonDominant;
        [FoldoutGroup("Generic Tool Prefabs")] [Required] public GameObject dominant;
        
        [FoldoutGroup("Generic Tool References")] [SerializeField] private DirectButton closeButton;
        
        private void Awake()
        {
            SetupMenuItems();
        }

        private void SetupMenuItems()
        {
            buttonPrefab = Instantiate(buttonPrefab);
            
            toolButton = buttonPrefab.GetComponent<SelectableObject>();
            toolButton.enabled = false;
            toolButton.selectEnd.AddListener(SelectTool);
            
            dominant = Instantiate(dominant);
            dominant.SetActive(false);
            
            nonDominant = Instantiate(nonDominant);
            nonDominant.SetActive(false);
            InitialiseDirectInterface();
        }

        private void InitialiseDirectInterface()
        {
            directInterfaceBlocks = nonDominant.GetComponentsInChildren<BaseDirectBlock>();

            foreach (var block in directInterfaceBlocks)
            {
                block.c = controller;
            }
        }

        public void SetToolState(bool state)
        {
            active = state;
            dominant.SetActive(state);
            nonDominant.SetActive(state);
            
            if (controller.debugActive)
            {
                Debug.Log(dominant.name + " has been set to " + state);
            }
            
            if(!state) return;
            toolMenu.SetState(false, transform);
        }

        private void SelectTool()
        {
            handedness = toolMenu.dominantHand;
            toolController.ToggleTool(this);
        }

        private void Update()
        {
            switch (handedness)
            {
                case ToolMenu.Handedness.Right:
                    cTrigger = controller.RightSelect();
                    Set.Transforms(dominant.transform, controller.RightControllerTransform());
                    Set.Transforms(nonDominant.transform, controller.LeftControllerTransform());
                    break;
                case ToolMenu.Handedness.Left:
                    cTrigger = controller.LeftSelect();
                    Set.Transforms(dominant.transform, controller.LeftControllerTransform());
                    Set.Transforms(nonDominant.transform, controller.RightControllerTransform());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
