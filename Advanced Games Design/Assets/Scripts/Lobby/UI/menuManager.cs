using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class menuManager : MonoBehaviour {
    public GameObject MainMenu, MainPanel, PlayButton, JoinButton, CreateButton, LobbyMenu, TopPannel, CreateGame, CreateGameText, MatchMakerSubPanel, CreateGameButton, BackToMenu, FindAGameBack;
    public Text txt;

	// Use this for initialization
	public void Play()
    {
        JoinButton.SetActive(true);
        CreateButton.SetActive(true);
        PlayButton.SetActive(false);
        
    }

    public void Create()
    {
        MainPanel.SetActive(true);
        LobbyMenu.SetActive(true);
        TopPannel.SetActive(true);
        CreateGame.SetActive(true);
        CreateGameButton.SetActive(true);
        CreateGameText.SetActive(true);
        MatchMakerSubPanel.SetActive(true);
        txt.text = "Create Game";
        CreateButton.SetActive(false);
        JoinButton.SetActive(false);
        BackToMenu.SetActive(true);

    }

    public void BackToMain()
    {
        
        TopPannel.SetActive(false);
        CreateGame.SetActive(false);
        CreateGameButton.SetActive(false);
        CreateGameText.SetActive(false);
        MatchMakerSubPanel.SetActive(false);
        txt.text = "Create Game";
        CreateButton.SetActive(false);
        JoinButton.SetActive(false);
        BackToMenu.SetActive(false);
        FindAGameBack.SetActive(false);
        PlayButton.SetActive(true);
    }

    public void ListServers()
    {

        CreateButton.SetActive(false);
        JoinButton.SetActive(false);
        BackToMenu.SetActive(true);

    }
    public void InMatching()
    {
        BackToMenu.SetActive(false);
    }
    public void BackToCreate()
    {
        BackToMenu.SetActive(true);
    }
}
