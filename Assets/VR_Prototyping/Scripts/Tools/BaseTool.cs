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

        private bool initialised;

        private BaseDirectBlock[] directInterfaceBlocks;
        
        [FoldoutGroup("Generic Tool Prefabs")] [Required] public GameObject buttonPrefab;
        [FoldoutGroup("Generic Tool Prefabs")] [Required] public GameObject nonDominant;
        [FoldoutGroup("Generic Tool Prefabs")] [Required] public GameObject dominant;
        
        [FoldoutGroup("Generic Tool Settings")]  [SerializeField] [Range(.01f, 1f)] protected float dominantSpeed = 1;
        [FoldoutGroup("Generic Tool Settings")]  [SerializeField] [Range(.01f, 1f)] protected float nonDominantSpeed = 1;


        public void SetupMenuItems()
        {
            buttonPrefab = Instantiate(buttonPrefab);
            
            toolButton = buttonPrefab.GetComponent<SelectableObject>();
            toolButton.enabled = false;
            toolButton.selectEnd.AddListener(SelectTool);

            if (dominant != null)
            {
                dominant = Instantiate(dominant);
                dominant.SetActive(false);   
            }

            if (nonDominant != null)
            {
                nonDominant = Instantiate(nonDominant);
                nonDominant.SetActive(false);   
            }
            
            InitialiseDirectInterface();
            
            Initialise();

            initialised = true;
        }
        
        protected virtual void Initialise()
        {
            
        }

        private void Start()
        { 
            OnStart();   
        }
        
        protected virtual void OnStart()
        {
            
        }

        private void InitialiseDirectInterface()
        {
            if (nonDominant == null) return;
            
            directInterfaceBlocks = nonDominant.GetComponentsInChildren<BaseDirectBlock>();

            foreach (var block in directInterfaceBlocks)
            {
                block.c = controller;
            }
        }

        public void SetToolState(bool state)
        {
            active = state;

            if (dominant != null)
            {
                dominant.SetActive(state);
            }

            if (nonDominant != null)
            {
                nonDominant.SetActive(state);
            }

            if (controller.debugActive)
            {
                Debug.Log(dominant.name + " has been set to " + state);
            }

            switch (state)
            {
                case true:
                    ToolActivate();
                    break;
                case false:
                    ToolDeactivate();
                    break;
            }

            if(!state) return;
            toolMenu.SetState(false, transform);
        }

        protected virtual void ToolActivate()
        {
            
        }
        
        protected virtual void ToolDeactivate()
        {
            
        }
        
        protected virtual void ToolUpdate()
        {
            
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
            if (!initialised) return;
            
            ToolUpdate();
            
            switch (handedness)
            {
                case ToolMenu.Handedness.Right when dominant != null && nonDominant != null:
                    cTrigger = controller.RightSelect();
                    dominant.transform.LerpTransform(controller.RightTransform(), dominantSpeed);
                    nonDominant.transform.LerpTransform(controller.LeftTransform(), nonDominantSpeed);
                    break;
                case ToolMenu.Handedness.Left when dominant != null && nonDominant != null:
                    cTrigger = controller.LeftSelect();
                    dominant.transform.LerpTransform(controller.LeftTransform(), dominantSpeed);
                    nonDominant.transform.LerpTransform(controller.RightTransform(), nonDominantSpeed);
                    break;
                default:
                    return;
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
    }
}
