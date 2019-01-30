using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerNetwork : MonoBehaviour {

    public static playerNetwork Instance;
    public string PlayerName { get; private set; }
    private PhotonView photonView;
    public GameObject player;
    private int PlayersInGame = 0;
    private void Awake()
    {
        Instance = this;
        photonView = GetComponent<PhotonView>();
        PlayerName = "PLAYER#" + Random.Range(1000, 9999);
        PhotonNetwork.sendRate = 60;
        PhotonNetwork.sendRateOnSerialize = 30;
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene")
        {
            if (PhotonNetwork.isMasterClient)
                MasterLoadedGame();

            else NonMasterLoadedGame();
        }
    }

    private void MasterLoadedGame()
    {
        //PlayersInGame = 1;
        Debug.Log("MasterLoadedGame");
        photonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player);
        photonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others);
    }

    private void NonMasterLoadedGame()
    {
        Debug.Log("Non Master Loaded Game");
        photonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player);
    }

    [PunRPC]
    private void RPC_LoadGameOthers()
    {
        PhotonNetwork.LoadLevel(2);
    }

    [PunRPC]
    private void RPC_LoadedGameScene(PhotonPlayer photonPlayer)
    {

        PlayersInGame++;
        print(PlayersInGame);
        if (PlayersInGame == PhotonNetwork.playerList.Length)
        {
            print("All players are in the game");

            photonView.RPC("RPC_CreatePlayer", PhotonTargets.All);
        }
    }
    [PunRPC]
    private void RPC_CreatePlayer()
    {
        float randomValue = Random.Range(40f, 45f);
        PhotonNetwork.Instantiate(player.name, new Vector3(randomValue, 2f, 230), Quaternion.identity, 0);

    }

}
