using System;
using Sirenix.OdinInspector;
using UnityEngine;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools
{
    [Serializable]
    public abstract class BaseTool : MonoBehaviour
    {
        public ControllerTransforms controller { get; set; }
        public ToolController toolController { private get; set; }
        public ToolMenu toolMenu { get; set; }
        public SelectableObject toolButton { get; private set; }
        public ToolMenu.Handedness handedness { private get; set; }
        private bool active { get; set; }
        private bool cTrigger { get; set; }
        private bool pTrigger { get; set; }

        private BaseDirectBlock[] directInterfaceBlocks;
        
        [FoldoutGroup("Generic Tool Prefabs")] [Required] public GameObject buttonPrefab;
        [FoldoutGroup("Generic Tool Prefabs")] [Required] public GameObject nonDominant;
        [FoldoutGroup("Generic Tool Prefabs")] [Required] public GameObject dominant;
        
        [BoxGroup("Generic Tool Settings")]  [SerializeField] [Range(.01f, 1f)] protected float dominantSpeed = 1;
        [BoxGroup("Generic Tool Settings")]  [SerializeField] [Range(.01f, 1f)] protected float nonDominantSpeed = 1;
        
        private void Awake()
        {
            SetupMenuItems();
            Initialise();
        }
        
        protected virtual void Initialise()
        {
            
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

        protected virtual void ToolStart()
        {
            
        }

        protected virtual void ToolStay()
        {
            
        }

        protected virtual void ToolEnd()
        {
            
        }

        protected virtual void ToolInactive()
        {
            
        }

        private void SelectTool()
        {
            handedness = toolMenu.dominantHand;
            toolController.ToggleTool(this);
        }

        private void Update()
        {
            ToolUpdate();
            
            switch (handedness)
            {
                case ToolMenu.Handedness.Right:
                    cTrigger = controller.RightSelect();
                    Set.LerpTransforms(dominant.transform, controller.RightTransform(), dominantSpeed);
                    Set.LerpTransforms(nonDominant.transform, controller.LeftTransform(), nonDominantSpeed);
                    break;
                case ToolMenu.Handedness.Left:
                    cTrigger = controller.LeftSelect();
                    Set.LerpTransforms(dominant.transform, controller.LeftTransform(), dominantSpeed);
                    Set.LerpTransforms(nonDominant.transform, controller.RightTransform(), nonDominantSpeed);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            if(!active) return;
            
            if (cTrigger && !pTrigger)
            {
                ToolStart();
            }

            if (cTrigger && pTrigger)
            {
                ToolStay();
            }

            if (!cTrigger && pTrigger)
            {
                ToolEnd();
            }
            
            if (!cTrigger && !pTrigger)
            {
                ToolInactive();
            }

            pTrigger = cTrigger;
        }

        protected virtual void ToolUpdate()
        {
            
        }
    }
}
