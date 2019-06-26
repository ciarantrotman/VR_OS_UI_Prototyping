using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VR_Prototyping.Scripts.Icon_Scripts
{
    public class IconScenes : MonoBehaviour
    {
        [Header("Icon Scenes")] 
        [BoxGroup] [SerializeField] private bool launchIntroScene;
        [BoxGroup] public SceneAsset introScene;
        [BoxGroup] public SceneAsset dioramaScene;

        private void Awake()
        {
            if(!launchIntroScene) return;
            LoadScene(introScene);
        }

        public void LoadScene(SceneAsset scene)
        {
            SceneManager.LoadScene(scene.name, LoadSceneMode.Additive);
        }
        
        public void UnloadScene(SceneAsset scene)
        {
            SceneManager.UnloadScene(scene.name);
        }
    }
}
