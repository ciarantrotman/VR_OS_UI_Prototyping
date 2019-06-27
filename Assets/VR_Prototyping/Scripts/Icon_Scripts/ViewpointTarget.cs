using UnityEngine;
using UnityEngine.SceneManagement;

namespace VR_Prototyping.Scripts.Icon_Scripts
{
    public class ViewpointTarget : SelectableObject
    {
        private new Camera camera;
        private RenderTexture renderTexture;
        public ViewpointManager ViewpointManager { get; set; }
        protected override void InitialisePostSetup()
        {
            camera = GetComponentInChildren<Camera>();
            renderTexture = new RenderTexture(245, 245, 16, RenderTextureFormat.R16);
            camera.targetTexture = renderTexture;
            selectEnd.AddListener(EnterModel);
        }

        private void EnterModel()
        {
            ViewpointManager.TriggerEnterModel(transform);
        }
    }
}
