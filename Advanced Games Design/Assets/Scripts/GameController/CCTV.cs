using System.Collections;
using UnityEngine;

public class CCTV : MonoBehaviour {

    private float timer1, timer2;
    private GameObject playerOne;
    private GameObject playerTwo;
    private PlayersLastLocation playersLastLocation;

    private void Awake()
    {
        timer1 = timer2 = 0.0f;
        playerOne = GameObject.FindGameObjectWithTag("PlayerOne");
        playerTwo = GameObject.FindGameObjectWithTag("PlayerTwo");
        playersLastLocation = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayersLastLocation>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == playerOne)
        {
            StopCoroutine("PlayerOneTimerBeforeAlarmStops");
            timer1 = 0.0f;
        }

        if (other.gameObject == playerTwo)
        {
            StopCoroutine("PlayerTwoTimerBeforeAlarmStops");
            timer2 = 0.0f;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject == playerOne)
        {
            Vector3 playerPosition = playerOne.transform.position - transform.position;
            RaycastHit hit;

            if(Physics.Raycast(transform.position, playerPosition, out hit)) //This section ensures that the player is not behind a wall
            {

                Debug.Log(hit.collider.ToString());
                if(hit.collider.gameObject == playerOne)
                {
                    playersLastLocation.playerOnePosition = playerOne.transform.position;
                }
            }
        }

        if (other.gameObject == playerTwo)
        {
            Vector3 playerPosition = playerTwo.transform.position - transform.position;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, playerPosition, out hit)) //This section ensures that the player is not behind a wall
            {
                if (hit.collider.gameObject == playerTwo)
                {
                    playersLastLocation.playerTwoPosition = playerTwo.transform.position;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == playerOne)
        {
            StartCoroutine("PlayerOneTimerBeforeAlarmStops");
        }

        if(other.gameObject == playerTwo)
        {
            StartCoroutine("PlayerTwoTimerBeforeAlarmStops");
        }
    }

    void ResetPlayerOnePosition()
    {
        playersLastLocation.playerOnePosition = playersLastLocation.resetPosition;
    }

    void ResetPlayerTwoPosition()
    {
        playersLastLocation.playerTwoPosition = playersLastLocation.resetPosition;
    }

    IEnumerator PlayerOneTimerBeforeAlarmStops()
    {

        while(timer1 < 7.5f)
        {
            timer1 += Time.deltaTime;
            Debug.Log(timer1);
            yield return null;
        }

        ResetPlayerOnePosition();
    }

    IEnumerator PlayerTwoTimerBeforeAlarmStops()
    {

        while (timer2 < 7.5f)
        {
            timer2 += Time.deltaTime;
            yield return null;
        }

        ResetPlayerTwoPosition();
    }
}
