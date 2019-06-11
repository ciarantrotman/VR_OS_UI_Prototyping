using Sirenix.OdinInspector;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class ToolButton : SelectableObject
    {
        [BoxGroup("Tool Settings")] [SerializeField] private GameObject tool;
        [BoxGroup("Tool Settings")] [SerializeField] private GameObject mask;
        [BoxGroup("Tool Settings")] [SerializeField] private GameObject back;

        private static readonly int StencilReference = Shader.PropertyToID("_StencilReferenceID");

        public int Index { private get; set; }
        
        protected override void Initialise()
        {
            tool = Instantiate(tool, transform, true);
            tool.transform.localPosition = new Vector3(0,0,.1f);
            tool.transform.localRotation = Quaternion.identity;
            SetupStencilIndex();
        }

        private void SetupStencilIndex()
        {
            tool.GetComponent<Renderer>().material.SetFloat(StencilReference, Index);
            mask.GetComponent<Renderer>().material.SetFloat(StencilReference, Index);
            back.GetComponent<Renderer>().material.SetFloat(StencilReference, Index);
        }
    }
}
