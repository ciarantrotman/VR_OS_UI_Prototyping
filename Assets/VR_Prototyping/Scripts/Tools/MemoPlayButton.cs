using UnityEngine;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools
{
    public class MemoPlayButton : DirectButton
    {
        private MemoNode _memoNode;

        private void Start()
        {
            _memoNode = transform.parent.transform.GetComponentInParent<MemoNode>();
            c = _memoNode.Controller;
            activate.AddListener(_memoNode.PlayMemo);
            SetupButton();
        }
    }
}
