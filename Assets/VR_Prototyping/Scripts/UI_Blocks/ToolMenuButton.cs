using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace VR_Prototyping.Scripts.UI_Blocks
{
    public class ToolMenuButton : DirectButton
    {
        private float visualDepth;
        private float blendShapeWeight;
        private MeshRenderer textRenderer;

        private const float BlendShapeActive = 0f;
        private const float BlendShapeInactive = 100f;
        
        [TabGroup("Button Settings")] [HideIf("placeholderButton")] [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
        [TabGroup("Button Settings")] [Indent] [HideIf("placeholderButton")] [Range(.01f, .1f)] [SerializeField] private  float hoverVisualDepth = .1f;
        [TabGroup("Button Settings")] [Indent] [HideIf("placeholderButton")] [Range(.01f, 1)] [SerializeField] private  float buttonAnimationDuration = .5f;
        [TabGroup("Button Settings")] [Space(10)] [HideIf("placeholderButton")] [SerializeField] private GameObject buttonText;
        [TabGroup("Button Settings")] [Indent] [HideIf("placeholderButton")] [SerializeField] private Color activeColor = new Color(255f,255f,255f, 255f);
        [TabGroup("Button Settings")] [Indent] [HideIf("placeholderButton")] [SerializeField] private Color inactiveColor = new Color(0f,0f,0f, 255f);

        protected override void ButtonSetup()
        {
            buttonText.transform.SetParent(customButton.transform);
            textRenderer = buttonText.GetComponent<MeshRenderer>();
            
            activate.AddListener(ButtonTextActive);
            deactivate.AddListener(ButtonTextInactive);

            if (!toggle) return;
            activate.AddListener(ButtonBlendShapeActive);
            deactivate.AddListener(ButtonBlendShapeInactive);
        }

        protected override void ButtonUpdate()
        {
            SetBlendShape();
        }
        
        private void SetBlendShape()
        {
            skinnedMeshRenderer.SetBlendShapeWeight(0, toggle ? blendShapeWeight : BlendShapeWeight());
        }

        private float BlendShapeWeight()
        {
            Vector3 buttonPosition = Button.transform.position;
            float distance = Vector3.Distance(buttonPosition, Target.transform.position);
            if (Active) return BlendShapeActive;
            return Mathf.InverseLerp(Target.transform.localPosition.z, restDepth, distance) * 100f;
        }
        
        private void ButtonBlendShapeActive()
        {
            blendShapeWeight = skinnedMeshRenderer.GetBlendShapeWeight(0);
            DOTween.To(()=> blendShapeWeight, x=> blendShapeWeight = x, BlendShapeActive, buttonAnimationDuration);
        }
        
        private void ButtonBlendShapeInactive()
        {
            blendShapeWeight = skinnedMeshRenderer.GetBlendShapeWeight(0);
            DOTween.To(()=> blendShapeWeight, x=> blendShapeWeight = x, BlendShapeInactive, buttonAnimationDuration);
        }

        private void ButtonTextActive()
        {
            textRenderer.material.color = activeColor;
        }
        
        private void ButtonTextInactive()
        {
            textRenderer.material.color = inactiveColor;
        }
    }
}
