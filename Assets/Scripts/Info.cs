using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Info : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset fontAsset;

    private static readonly Dictionary<string, GameObject> InfoObjects = new();
    static private int positionY = 0;

    void Awake()
    {
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;
    }

    void Start()
    {
        AddInfo("fps");
        AddInfo("Chunk Loaded");
        positionY -= 18;

        AddInfo("X, Y, Z");
        AddInfo("Rotation");
        AddInfo("Look At");
        AddInfo("Direction");
        AddInfo("Front Look At");
        AddInfo("Chunk At");
    }

    private void AddInfo(string key)
    {
        GameObject objInfo = new(key);
        objInfo.transform.SetParent(transform);

        RectTransform objRect = objInfo.AddComponent<RectTransform>();
        objRect.anchorMin = Vector2.up;
        objRect.anchorMax = Vector2.up;
        objRect.pivot = Vector2.up;
        objRect.anchoredPosition = new Vector2(2, positionY);
        positionY -= 18;

        Image image = objInfo.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.7f);

        GameObject objText = new();
        InfoObjects.Add(key, objText);

        RectTransform objTextRect = objText.AddComponent<RectTransform>();
        objText.transform.SetParent(objInfo.transform);
        objTextRect.anchorMin = Vector2.up;
        objTextRect.anchorMax = Vector2.up;
        objTextRect.pivot = Vector2.up;
        objTextRect.anchoredPosition = Vector2.zero;

        TextMeshProUGUI text = objText.AddComponent<TextMeshProUGUI>();
        text.font = fontAsset;
        text.fontSize = 12;
        text.color = Color.white;
        text.enableWordWrapping = false;
        text.text = key + ": " + "None";

        objRect.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
    }

    public static void UpdateInfo(string key, string text)
    {
        var textMesh = InfoObjects[key].GetComponent<TextMeshProUGUI>();
        textMesh.text = key + ": " + text;
        InfoObjects[key].transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(textMesh.preferredWidth, textMesh.preferredHeight);
    }
}
