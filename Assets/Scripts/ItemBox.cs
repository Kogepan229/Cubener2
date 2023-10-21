using UnityEngine;

public class ItemBox : MonoBehaviour
{
    public int BlockID = 0;
    private GameObject BlockIconObj;

    void Start()
    {
        SetItem(BlockID);
    }

    public void SetItem(int blockID)
    {
        BlockID = blockID;

        if (BlockIconObj != null)
        {
            Destroy(BlockIconObj);
            BlockIconObj = null;
        }

        if (blockID == 0)
        {
            return;
        }

        BlockIconObj = BlockIcon.CreateBlockIcon(blockID);
        BlockIconObj.transform.SetParent(transform);
        BlockIconObj.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }
}
