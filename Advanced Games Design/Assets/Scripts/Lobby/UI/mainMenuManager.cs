using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainMenuManager : MonoBehaviour {

    public GameObject createGame, joinGame, lobbyView2, mainMenu;

	public void OnCreate()
    {
        createGame.SetActive(true);
        joinGame.transform.SetAsFirstSibling();

        // mainMenu.SetActive(false);
    }

    public void OnJoin()
    {
        createGame.SetActive(false);

        joinGame.transform.SetAsLastSibling();
    }

    public void OnBackFromCreate()
    {
        createGame.SetActive(false);
        lobbyView2.SetActive(true);
        joinGame.transform.SetAsFirstSibling();
    }

    public void OnBackFromJoin()
    {
        lobbyView2.SetActive(true);
        joinGame.transform.SetAsFirstSibling();
    }

    public void OnClickHome()
    {
        joinGame.transform.SetAsFirstSibling();
        createGame.SetActive(false);
        
        
    }

}
