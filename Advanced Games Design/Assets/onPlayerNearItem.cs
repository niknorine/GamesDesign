using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerFinder;

public class onPlayerNearItem : MonoBehaviour

{
    public GameObject networkManager;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "PlayerOne")
        {

            col.gameObject.GetComponentInChildren<inventory>().itemInRange = this.gameObject;
            col.gameObject.GetComponentInChildren<inventory>().IsitemInRange = true;
        }
        if (col.tag == "PlayerTwo")
        {

            col.gameObject.GetComponentInChildren<inventory>().itemInRange = this.gameObject;
            col.gameObject.GetComponentInChildren<inventory>().IsitemInRange = true;
        }

    }

    public void OnPickUp(GameObject player)
    {

        foreach(GameObject gameobject in player.GetComponentInChildren<inventory>().invSlots)
        {
            if(gameobject.GetComponent<item>().Item.ID == 0)
            {

                gameobject.GetComponent<item>().Item = this.gameObject.GetComponent<item>().Item;

                player.GetComponentInChildren<inventory>().IsitemInRange = false;
                player.GetComponentInChildren<inventory>().itemInRange = null;
                PhotonNetwork.Destroy(this.gameObject);
                networkManager.GetComponentInChildren<DestoryItem>().DestroyItem(this.gameObject);

                return;
            }
        }
       


    }
}
