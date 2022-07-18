using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Popup : MonoBehaviour
{
    [SerializeField] Button popupButton;
    [SerializeField] Text message;

    public void Init(Transform canvas, string txt) 
    {
        message.text = txt;

        transform.SetParent(canvas);
        Vector3 scale = new Vector3(3.7f, 3.7f, 1);
        transform.localScale = scale;
        transform.localPosition = Vector3.zero;
        popupButton.onClick.AddListener(() =>
        {
            GameObject.Destroy(this.gameObject);
        });

    }
}
