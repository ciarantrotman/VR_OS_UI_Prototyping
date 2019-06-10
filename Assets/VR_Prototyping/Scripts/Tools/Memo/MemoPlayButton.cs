using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools.Memo
{
    public class MemoPlayButton : DirectButton
    {
        private MemoNode memoNode;

        private void Start()
        {
            memoNode = transform.parent.transform.GetComponentInParent<MemoNode>();
            controller = memoNode.Controller;
            activate.AddListener(memoNode.PlayMemo);
            SetupButton();
        }
    }
}
