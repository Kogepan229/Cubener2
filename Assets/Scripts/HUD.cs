using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private Transform followTargetTransform;

    void Start()
    {
        RectTransform rect = transform.Find("Canvas").GetComponent<RectTransform>();
        rect.anchoredPosition3D = new Vector3(rect.sizeDelta.x / -2, rect.sizeDelta.y / -2, rect.position.z);
    }
}
