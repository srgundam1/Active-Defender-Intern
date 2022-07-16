using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIController : MonoBehaviour
{

    /*
     * A class used to control the UI of the game view
     * This script should be attached to an empty game object
     * Set the main canvas in the editor to your main game canvas used
     * */

    public static UIController Instance;

    public Transform MainCanvas;   //The main canvas that should be set in the editor

    // Start is called before the first frame update
    //Change the instance of the UIController on start if the instance is initially null
    void Start()
    {
        if (Instance != null)
        {
            GameObject.Destroy(this.gameObject);
            return;
        }
        Instance = this;
        
    }

    /*
     * Creates a popup by creating a new game object using the popup object saved in the resources folder
     * return this created game object
     */

    public Popup CreatePopup()
    {
        GameObject popUpGo = Instantiate(Resources.Load("UI/Popup") as GameObject);
        return popUpGo.GetComponent<Popup>();
    }

   
}