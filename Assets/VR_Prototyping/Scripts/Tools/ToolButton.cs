using Sirenix.OdinInspector;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class ToolButton : SelectableObject
    {
        [BoxGroup("Tool Settings")] [SerializeField] private GameObject mask;
        [BoxGroup("Tool Settings")] [SerializeField] private GameObject back;

        public GameObject ToolModel { private get; set; }
        
        private static readonly int StencilReference = Shader.PropertyToID("_StencilReferenceID");

        public int Index { private get; set; }
        
        protected override void Initialise()
        {
            ToolModel = Instantiate(ToolModel, transform, true);
            ToolModel.transform.localPosition = new Vector3(0,0,.1f);
            ToolModel.transform.localRotation = Quaternion.identity;
            SetupStencilIndex();
        }

        private void SetupStencilIndex()
        {
            ToolModel.GetComponent<Renderer>().material.SetFloat(StencilReference, Index);
            mask.GetComponent<Renderer>().material.SetFloat(StencilReference, Index);
            back.GetComponent<Renderer>().material.SetFloat(StencilReference, Index);
        }
    }
}
