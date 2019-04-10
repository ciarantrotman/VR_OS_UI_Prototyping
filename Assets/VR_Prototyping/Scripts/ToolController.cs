using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace VR_Prototyping.Scripts
{
    public class ToolController : SerializedMonoBehaviour
    {
        [OdinSerialize] public Dictionary<int, ToolTemp> tools;
    }
}
