using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class photonConnect : MonoBehaviour {

    public string versionName = "1.0";

    public GameObject lobbyView1, lobbyView2, lobbyView3;

    public void Awake()
    {
        
        
        Debug.Log("Connecting to photon...");
    }

    private void OnConnectedToMaster()
    {
        PhotonNetwork.automaticallySyncScene = false;
        PhotonNetwork.playerName = playerNetwork.Instance.PlayerName;
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    private void OnJoinedLobby()
    {
        lobbyView1.SetActive(false);
        lobbyView2.SetActive(true);
        Debug.Log("On join lobby");
    }

    private void OnDisconnectedFromPhoton()
    {
        if (lobbyView1.activeInHierarchy)
            lobbyView1.SetActive(false);

        if (lobbyView2.activeInHierarchy)
            lobbyView2.SetActive(false);

        lobbyView3.SetActive(true);

        Debug.Log("Disconectec from photon servers");
    }

    public void StartConnection()
    {
        PhotonNetwork.ConnectUsingSettings(versionName);
    }


}
