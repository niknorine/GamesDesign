using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class photonButtons : MonoBehaviour {
    public photonHandler pHandler;
    public InputField createRoomInput, joinRoomInput;


    public void onClickCreateRoom()
    {
        pHandler.CreateNewRoom();
        
    }

}
