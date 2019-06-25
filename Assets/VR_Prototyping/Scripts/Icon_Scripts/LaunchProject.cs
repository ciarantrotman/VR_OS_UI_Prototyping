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
        private void Awake()
        {
            mainScene = SceneManager.GetActiveScene();
            foreach (GameObject rootGameObject in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (rootGameObject.name != "[VR Player]") continue;
                player = rootGameObject;
                scenes = rootGameObject.GetComponent<IconScenes>();
            }
        }

        private IntroScene introScene;
        protected override void Initialise()
        {
            selectEnd.AddListener(LoadScene);
        }

        private void LoadScene()
        {
            player.GetComponent<Locomotion>().SceneWipe();
            controllerTransforms.SceneWipeTrigger.AddListener(ManageScenes);
        }

        private void ManageScenes()
        {
            SceneManager.UnloadScene(scenes.introScene);
            SceneManager.LoadScene(scenes.dioramaScene, LoadSceneMode.Additive);
        }
    }
}
