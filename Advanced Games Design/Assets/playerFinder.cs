using UnityEngine;

namespace PlayerFinder
{
    public class playerFinder : MonoBehaviour
    {
        public GameObject Player1, Player2;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (PhotonNetwork.playerList.Length == 2 && (Player1 == null || Player2 == null))
            {
                Player1 = GameObject.FindGameObjectWithTag("PlayerOne");
                Player2 = GameObject.FindGameObjectWithTag("PlayerTwo");
            }
        }


    }
}
