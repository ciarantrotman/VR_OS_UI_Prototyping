using Sirenix.OdinInspector;
using UnityEngine;

namespace VR_Prototyping.Scripts.UI_Blocks
{
    public class BaseDirectBlock : MonoBehaviour
    {
        [BoxGroup("Script Setup")] [Required] public ControllerTransforms c;
        protected Rigidbody rb;
        
    }
}
