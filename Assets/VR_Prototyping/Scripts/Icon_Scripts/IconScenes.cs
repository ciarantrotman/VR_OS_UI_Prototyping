using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VR_Prototyping.Scripts.Icon_Scripts
{
    [DisallowMultipleComponent]
    public class IconScenes : MonoBehaviour
    {
        [Header("Icon Scenes")] 
        [BoxGroup] [SerializeField] private bool launchIntroScene;
        [BoxGroup] public SceneAsset introScene;
        [BoxGroup] public SceneAsset dioramaScene;
        [BoxGroup] public SceneAsset fullScene;

        private void Awake()
        {
            if(!launchIntroScene) return;
            LoadScene(introScene);
        }

        public static void LoadScene(SceneAsset scene)
        {
            Debug.Log(">>> " + scene.name + " was loaded.");
            SceneManager.LoadScene(scene.name, LoadSceneMode.Additive);
        }
        
        public static void UnloadScene(SceneAsset scene)
        {
            Debug.Log("<<< " + scene.name + " was unloaded.");
            SceneManager.UnloadScene(scene.name);
        }
    }
}
