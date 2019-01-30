using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class delayedSpawn : MonoBehaviour
{


    public GameObject drone1, drone2;
    // Start is called before the first frame update 


    


    IEnumerator Start()
    {

        yield return new WaitForSeconds(6.5f);
        drone1.SetActive(true);
        drone2.SetActive(true);

    }
}