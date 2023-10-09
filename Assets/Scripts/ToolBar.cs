using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBar : MonoBehaviour
{
    public static readonly int ITEM_BOX_NUMBER = 8;

    private int NumberInFocus = 1;

    private readonly ItemBox[] ItemBoxes = new ItemBox[ITEM_BOX_NUMBER];
    private GameObject FocusFrameObj;
    

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i <= ITEM_BOX_NUMBER; i++)
        {
            ItemBoxes[i-1] = transform.Find("ItemBox" + i).GetComponent<ItemBox>();
        }
        FocusFrameObj = transform.Find("FocusFrame").gameObject;
        if (FocusFrameObj == null )
        {
            Debug.Log("facus frame null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetNumberInFocus()
    {
        return NumberInFocus;
    }

    public void SetNumberInFocus(int number)
    {
        NumberInFocus = number;
        FocusFrameObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(40 * number - 20, 0);
    }

    public void ShiftNumberInFocus(float direction)
    {
        if (direction < 0)
        {
            if (NumberInFocus >= ITEM_BOX_NUMBER)
            {
                SetNumberInFocus(1);
            }
            else
            {
                SetNumberInFocus(NumberInFocus + 1);
            }
        }

        if (direction > 0)
        {
            if (NumberInFocus <= 1)
            {
                SetNumberInFocus(ITEM_BOX_NUMBER);
            }
            else
            {
                SetNumberInFocus(NumberInFocus - 1);
            }
        }
    }
}
