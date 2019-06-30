using UnityEngine;

namespace VR_Prototyping.Scripts.Icon_Scripts
{
    public class ReturnToDiorama : SelectableObject
    {
        private IconScenes iconScenes;
        protected override void InitialisePostSetup()
        {
            iconScenes = player.GetComponent<IconScenes>();
			selectEnd.AddListener(Return);
        }

        private void Return()
        {
            IconScenes.UnloadScene(iconScenes.fullScene);
            IconScenes.LoadScene(iconScenes.dioramaScene);
            RenderSettings.skybox = controllerTransforms.voidSkyBox;
        }
    }
}
