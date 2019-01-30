using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {

    [SerializeField] float patrolSpeed = 1.0f;
    [SerializeField] float patrolWaitTime = 1.0f;
    [SerializeField] float chaseSpeed = 1.5f;
    [SerializeField] float chaseWaitTime = 5.0f;
    [SerializeField] Transform[] patrolWayPoints;

    private EnemySenses enemySenses;
    private NavMeshAgent navMeshAgent;
    private Transform playerOne;
    private Transform playerTwo;
    private PlayersLastLocation playersLastLocation;

    private float patrolTimer;
    private float chaseTimer;
    private int currentWaypointIndex;

    private void Awake()
    {
        enemySenses = GetComponent<EnemySenses>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        //playerOne = GameObject.FindGameObjectWithTag("PlayerOne").transform;
      //  playerTwo = GameObject.FindGameObjectWithTag("PlayerTwo").transform;
        playersLastLocation = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayersLastLocation>();
    }

    private void Update()
    {
        if(enemySenses.isPlayerOneInSight || enemySenses.isPlayerTwoInSight)
        {
            AnalysePlayer();
        }
        else if(enemySenses.playerOneLastKnownLocation != playersLastLocation.resetPosition || enemySenses.playerTwoLastKnownLocation != playersLastLocation.resetPosition)
        {
            ChasePlayer();
        }
        else
        {
            PatrolWaypoints();
        }
            
    }

    void AnalysePlayer()
    {

    }

    void ChasePlayer()
    {
        Debug.Log("Chasing player");

        Vector3 distanceToPlayerOne = enemySenses.playerOneLastKnownLocation - transform.position;
        Vector3 distanceToPlayerTwo = enemySenses.playerTwoLastKnownLocation - transform.position;

        if (distanceToPlayerOne.sqrMagnitude > 4.0f)
        {
            Debug.Log("Chasing player one");
            navMeshAgent.destination = enemySenses.playerOneLastKnownLocation;
        }
        else if (distanceToPlayerTwo.sqrMagnitude > 4.0f)
        {
            navMeshAgent.destination = enemySenses.playerTwoLastKnownLocation;
        }

        navMeshAgent.speed = chaseSpeed;

        if(navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            chaseTimer += Time.deltaTime;

            if(chaseTimer > chaseWaitTime)
            {
                playersLastLocation.playerOnePosition = playersLastLocation.resetPosition;
                playersLastLocation.playerTwoPosition = playersLastLocation.resetPosition;
                enemySenses.playerOneLastKnownLocation = playersLastLocation.resetPosition;
                enemySenses.playerTwoLastKnownLocation = playersLastLocation.resetPosition;
                chaseTimer = 0.0f;
            }
        }
        else
        {
            chaseTimer = 0.0f;
        }

    }

    void PatrolWaypoints()
    {
        navMeshAgent.speed = patrolSpeed;

        if(navMeshAgent.destination == playersLastLocation.resetPosition || navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            patrolTimer += Time.deltaTime;

            if(patrolTimer >= patrolWaitTime)
            {
                if (currentWaypointIndex == patrolWayPoints.Length - 1)
                    currentWaypointIndex = 0;
                else
                    currentWaypointIndex++;

                patrolTimer = 0.0f;
            }
        }
        else
            patrolTimer = 0.0f;

        navMeshAgent.destination = patrolWayPoints[currentWaypointIndex].position;
    }

}
