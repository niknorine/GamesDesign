using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : Photon.MonoBehaviour
{

    public GameObject enemyAI;
    private bool active;
    
    
    void Start()
    {
        //enemyAI = GameObject.FindGameObjectWithTag("Enemy");
        StartCoroutine(WaitFor());
    }

    void Update()
    {
        active = enemyAI.activeInHierarchy;
        if(active == true)
        {
            Destroy(this);
        }
    }
   
	IEnumerator WaitFor()
    {
        yield return new WaitForSeconds(3);
        enemyAI.SetActive(true);
       
    }
}
