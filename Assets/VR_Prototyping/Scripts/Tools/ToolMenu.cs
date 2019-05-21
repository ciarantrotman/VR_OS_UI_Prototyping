using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    [RequireComponent(typeof(ControllerTransforms))]
    public class ToolMenu : MonoBehaviour
    {
        [HideInInspector] public bool active;
        [HideInInspector] public bool rubberBanded;
        private bool cMenu;
        private bool pMenu;
        
        private bool trigger;
        
        private ControllerTransforms c;
        private ToolController toolController;
        
        [BoxGroup("Script Setup")] [Required] [SerializeField] private GameObject menuPrefab;
        
        [BoxGroup("RubberBanding Settings")] [ShowIf("rubberBanded")] [SerializeField][Range(0,1)] private float moveSpeed = .5f;
        [BoxGroup("RubberBanding Settings")] [ShowIf("rubberBanded")] [Space(10)] [SerializeField] private float angleThreshold = 45f;
        [BoxGroup("RubberBanding Settings")] [ShowIf("rubberBanded")] [SerializeField] private float distanceThreshold = .1f;
        public enum Handedness
        {
            Right,
            Left
        }
        [BoxGroup("Tool Settings")] public Handedness dominantHand;

        private void Awake()
        {
            c = GetComponent<ControllerTransforms>();
            SetupMenu();
        }

        private void SetupMenu()
        {
            menuPrefab = Instantiate(menuPrefab);
            menuPrefab.name = "Tools/Menu";
            toolController = menuPrefab.GetComponent<ToolController>();
            toolController.Initialise(gameObject, active, c, dominantHand, this);
            toolController.ToggleButtonState(false);
        }

        private void Update()
        {
            switch (dominantHand)
            {
                case Handedness.Right:
                    cMenu = c.RightMenu();
                    CheckState(cMenu, pMenu, c.RightTransform());
                    RubberBanded(c.RightTransform());
                    pMenu = cMenu;
                    return;
                case Handedness.Left:
                    cMenu = c.LeftMenu();
                    CheckState(cMenu, pMenu, c.LeftTransform());
                    RubberBanded(c.LeftTransform());
                    pMenu = cMenu;
                    return;
                default:
                    return;
            }
        }

        private void CheckState(bool current, bool previous, Transform controller)
        {
            if (current || !previous) return;
            
            active = !active;
            SetState(active, controller);
        }

        public void SetState(bool state, Transform controller)
        {
            toolController.ToggleButtonState(state);
            active = state;
            rubberBanded = state;
            
            if(!state) return;
            menuPrefab.transform.position = controller.position;
            Set.SplitRotation(controller, menuPrefab.transform, false);
            toolController.SetAllToolState(false);
        }

        private void RubberBanded(Transform target)
        {
            if(!rubberBanded) return;
            
            var d = Vector3.Distance(menuPrefab.transform.position, target.position);
            var a = Set.Divergence(menuPrefab.transform, target);

            if (!(a >= angleThreshold) && !(d >= distanceThreshold)) return;
            
            var rotTarget = new Vector3(0, target.rotation.eulerAngles.y, 0);
            
            menuPrefab.transform.DORotate(rotTarget, moveSpeed);
            menuPrefab.transform.DOMove(target.position, moveSpeed);
        }
    }
}
