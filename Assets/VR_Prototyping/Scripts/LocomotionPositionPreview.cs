using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VR_Prototyping.Scripts
{
    public class LocomotionPositionPreview : MonoBehaviour
    {
        [BoxGroup] [SerializeField] private Transform head;
        [BoxGroup] [SerializeField] private Transform lHand;
        [BoxGroup] [SerializeField] private Transform rHand;
        private bool _active;
        
        public ControllerTransforms ControllerTransforms { private get; set; }

        private void Update()
        {
            if (!_active) return;

            head.localPosition = new Vector3(0, ControllerTransforms.CameraLocalPosition().y, 0);
            head.forward = transform.parent.forward;
            lHand.localPosition = RelativeHandPosition(ControllerTransforms.LeftLocalPosition());
            rHand.localPosition = RelativeHandPosition(ControllerTransforms.RightLocalPosition());
        }

        private Vector3 RelativeHandPosition(Vector3 handPosition)
        {
            return handPosition - ControllerTransforms.CameraLocalPosition();
        }

        public void GhostToggle(Transform parent, bool state)
        {
            var self = transform;
            self.SetParent(parent);
            self.localPosition = Vector3.zero;
            head.gameObject.SetActive(state);
            lHand.gameObject.SetActive(state);
            rHand.gameObject.SetActive(state);
            _active = state;
        }
    }
}
