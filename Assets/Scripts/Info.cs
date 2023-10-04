using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Info : MonoBehaviour
{
    static public Dictionary<string, string> InfoData = new Dictionary<string, string>();

    private List<GameObject> infoObjs = new List<GameObject>();

    [SerializeField] private RectTransform rectTransform;
    private Font font;

    // Start is called before the first frame update
    void Start()
    {
        font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        rectTransform.sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;

        Info.InfoData["X, Y, Z"] = "None";
        Info.InfoData["Rotation"] = "None";
        Info.InfoData["Look At"] = "None";
        Info.InfoData["Direction"] = "None";
        Info.InfoData["Front Look At"] = "None";
        Info.InfoData["Chunk"] = "None";
    }

    // Update is called once per frame
    void Update()
    {
        if (InfoData.Count > infoObjs.Count)
        {
            for (int i = infoObjs.Count; i < InfoData.Count; i++)
            {
                GameObject obj = new GameObject();
                obj.name = "info: " + i.ToString();
                Text text = obj.AddComponent<Text>();
                text.font = font;
                text.color = Color.white;
                text.horizontalOverflow = HorizontalWrapMode.Overflow;
                text.text = i.ToString();
                obj.transform.SetParent(transform);
                RectTransform objRect = obj.GetComponent<RectTransform>();
                objRect.anchorMin = Vector2.up;
                objRect.anchorMax = Vector2.up;
                objRect.pivot = Vector2.up;
                objRect.anchoredPosition = new Vector2(2, -16 * i - 2);
                infoObjs.Add(obj);
            }
        }
        else if (InfoData.Count < infoObjs.Count)
        {
            for (int i = InfoData.Count; i < infoObjs.Count; i++)
            {
                Destroy(infoObjs[i]);
                infoObjs.RemoveAt(i);
            }
        }

        int count = 0;
        foreach (KeyValuePair<string, string> item in InfoData)
        {
            infoObjs[count].GetComponent<Text>().text = item.Key + ": " + item.Value;
            count++;
        }
    }
}
