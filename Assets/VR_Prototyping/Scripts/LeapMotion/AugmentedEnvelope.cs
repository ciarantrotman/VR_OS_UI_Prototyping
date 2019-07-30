using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VR_Prototyping.Scripts.LeapMotion
{
    [RequireComponent(typeof(ControllerTransforms))]
    public class AugmentedEnvelope : MonoBehaviour
    {
        ControllerTransforms controllerTransforms;

        GameObject anchor; // common parent
        
        GameObject lHl; // left hand local
        GameObject lTl; // left thumb local
        GameObject lIl; // left index local
        GameObject lMl; // left middle local
        GameObject lRl; // left ring local
        GameObject lLl; // left little local
         
        GameObject rHl; // right hand local
        GameObject rTl; // right thumb local
        GameObject rIl; // right index local
        GameObject rMl; // right middle local
        GameObject rRl; // right ring local
        GameObject rLl; // right little local
         
        GameObject lHa; // left hand augmented
        GameObject lTa; // left thumb augmented
        GameObject lIa; // left index augmented
        GameObject lMa; // left middle augmented
        GameObject lRa; // left ring augmented
        GameObject lLa; // left little augmented
         
        GameObject rHa; // right hand augmented
        GameObject rTa; // right thumb augmented
        GameObject rIa; // right index augmented
        GameObject rMa; // right middle augmented
        GameObject rRa; // right ring augmented
        GameObject rLa; // right little augmented

        void Start()
        {
            SetupReferences();
            SetupLocalObjects();
        }

        private void SetupReferences()
        {
            controllerTransforms = GetComponent<ControllerTransforms>();
        }

        private void SetupLocalObjects()
        {
            anchor = Set.NewGameObject(gameObject, "[Augmented Rig]");
            
            lHl = Set.NewGameObject(anchor, "Augmented/Hand/Left");
            lTl = Set.NewGameObject(lHl, "Augmented/Thumb/Left");
            lIl = Set.NewGameObject(lHl, "Augmented/Index/Left");
            lMl = Set.NewGameObject(lHl, "Augmented/Middle/Left");
            lRl = Set.NewGameObject(lHl, "Augmented/Ring/Left");
            lLl = Set.NewGameObject(lHl, "Augmented/Little/Left");
            
            rHl = Set.NewGameObject(anchor, "Augmented/Hand/Right");
            rTl = Set.NewGameObject(lHl, "Augmented/Thumb/Right");
            rIl = Set.NewGameObject(lHl, "Augmented/Index/Right");
            rMl = Set.NewGameObject(lHl, "Augmented/Middle/Right");
            rRl = Set.NewGameObject(lHl, "Augmented/Ring/Right");
            rLl = Set.NewGameObject(lHl, "Augmented/Little/Right");
        }
    }
}
