using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VR_Prototyping.Scripts.Icon_Scripts
{
    public class LaunchProject : SelectableObject
    {
        private Scene mainScene;
        private IconScenes scenes;
        private ViewpointManager viewpointManager;

        private IntroScene introScene;
        protected override void InitialisePostSetup()
        {
            viewpointManager = GetComponentInParent<ViewpointManager>();
            scenes = player.GetComponent<IconScenes>();
            selectEnd.AddListener(ManageScenes);
            //selectEnd.AddListener(LoadScene);
        }

        private void LoadScene()
        {
            locomotion.sceneWipeTrigger.AddListener(ManageScenes);
            player.GetComponent<Locomotion>().SceneWipe();
        }

        private void ManageScenes()
        {
            IconScenes.LoadScene(scenes.dioramaScene);
            IconScenes.UnloadScene(scenes.introScene);
        }
    }
}
