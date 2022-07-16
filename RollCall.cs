using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RollCall : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Text text;
    
    void Start()
    {
        Button button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            Popup popup = UIController.Instance.CreatePopup();
            string info = "Roll Call:";
            popup.Init(UIController.Instance.MainCanvas,info);
        });
        
    }
    /*
    public void click()
    {
        Popup popup = UIController.Instance.CreatePopup();
        popup.Init(UIController.Instance.MainCanvas,
            "The pop up window is working");
    }
    */
}
