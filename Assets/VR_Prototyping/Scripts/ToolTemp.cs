using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VR_Prototyping.Scripts
{
    [Serializable]
    public class ToolTemp : MonoBehaviour
    {
        [BoxGroup("Script Setup")] [Required] [SerializeField] public SelectableObject button;
        
        [BoxGroup("Tool Prefabs")] [Required] [SerializeField] public GameObject nonDominant;
        [BoxGroup("Tool Prefabs")] [Required] [SerializeField] public GameObject dominant;
    }
}
