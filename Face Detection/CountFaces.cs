using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CountFaces : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Text text;
    public static long totalFaces;
    
    void Start()
    {
        Button button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            Popup popup = UIController.Instance.CreatePopup();
            string holder = text.text;
            long val = 0;
            for(int i = 0; i < holder.Length; i++)
            {
                if (Char.IsDigit(holder[i]))
                {
                    long b = holder.Length - i;
                    val += ((long)(Math.Pow(10.0, (double)(b - 1))) * (holder[i] - '0'));
                }
            }
            totalFaces += val; 
            string info = "Please pan the camera \n until the frame contains \n a new set of faces, then \n press the 'Count Faces' button. \n \n Faces # " + totalFaces.ToString(); 
            popup.Init(UIController.Instance.MainCanvas,
                info);
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