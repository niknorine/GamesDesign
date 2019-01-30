using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySenses : MonoBehaviour {

    [SerializeField] float FOVAngle = 110.0f; // This is the enemy's vision field of view 

    public bool isPlayerOneInSight; // If true, the enemy can see the player with no obstruction in between
    public bool isPlayerTwoInSight; // If both players are in sight, enemies should go towards the closest player

    public Vector3 playerOneLastKnownLocation; // This is unique to each enemy. If the player is heard but not spotted, the global location will not be effected
    public Vector3 playerTwoLastKnownLocation; // TODO: Maybe create an array instead? 

    private NavMeshAgent navMeshAgent;
    private SphereCollider sphereCollider;
    private Animator animator; // This is the enemies animator
    private PlayersLastLocation playersLastLocation;

    private Vector3 playerOnePreviousSighting; // The previous sighting will be compared to the lastKnownLocation to check whether the players location has changed
    private Vector3 playerTwoPreviousSighting; 

    private GameObject playerOne; // Since there will be two players in the world, we will need to store them inside an array
    private GameObject playerTwo;

    private Animator playerOneAnimator; // This is the players animator
    private Animator playerTwoAnimator;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        sphereCollider = GetComponent<SphereCollider>();
        animator = GetComponent<Animator>();
        playersLastLocation = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayersLastLocation>();

        playerOne = GameObject.FindGameObjectWithTag("PlayerOne");
        playerTwo = GameObject.FindGameObjectWithTag("PlayerTwo");

        playerOneAnimator = playerOne.GetComponent<Animator>();
        playerTwoAnimator = playerTwo.GetComponent<Animator>();

        playerOneLastKnownLocation = playersLastLocation.resetPosition;
        playerOnePreviousSighting = playersLastLocation.resetPosition;

        playerTwoLastKnownLocation = playersLastLocation.resetPosition;
        playerTwoPreviousSighting = playersLastLocation.resetPosition;
    }

    private void Update()
    {

        if (playersLastLocation.playerOnePosition != playerOnePreviousSighting)
        {
            playerOneLastKnownLocation = playersLastLocation.playerOnePosition;
        }

        if (playersLastLocation.playerTwoPosition != playerTwoPreviousSighting)
        {
            playerTwoLastKnownLocation = playersLastLocation.playerTwoPosition;
        }

        playerOnePreviousSighting = playersLastLocation.playerOnePosition;
        playerTwoPreviousSighting = playersLastLocation.playerTwoPosition;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject == playerOne)
        {
            isPlayerOneInSight = false;

            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);

            if(angle < FOVAngle * 0.5f)
            {
                RaycastHit hit;

                if(Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, sphereCollider.radius))
                {
                    if(hit.collider.gameObject == playerOne)
                    {
                        isPlayerOneInSight = true;
                        playersLastLocation.playerOnePosition = playerOne.transform.position;
                    }
                }
            }
            //sound here
            if(CalculatePathLength(playerOne.transform.position) <= sphereCollider.radius)
            {
                playerOneLastKnownLocation = playerOne.transform.position;
            }

        }

        if (other.gameObject == playerTwo)
        {
            isPlayerTwoInSight = false;

            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);

            if (angle < FOVAngle * 0.5f)
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, sphereCollider.radius))
                {
                    if (hit.collider.gameObject == playerTwo)
                    {
                        isPlayerTwoInSight = true;
                        playersLastLocation.playerTwoPosition = playerTwo.transform.position;
                    }
                }
            }
            //sound here
            if (CalculatePathLength(playerTwo.transform.position) <= sphereCollider.radius)
            {
                playerTwoLastKnownLocation = playerTwo.transform.position;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == playerOne)
        {
            isPlayerOneInSight = false;
        }

        if(other.gameObject == playerTwo)
        {
            isPlayerTwoInSight = false;
        }
    }

    float CalculatePathLength(Vector3 targetPosition)
    {
        NavMeshPath navMeshPath = new NavMeshPath();

        if (navMeshAgent.enabled)
        {
            navMeshAgent.CalculatePath(targetPosition, navMeshPath);
        }

        Vector3[] allWayPoints = new Vector3[navMeshPath.corners.Length + 2];

        allWayPoints[0] = transform.position;
        allWayPoints[allWayPoints.Length - 1] = targetPosition;

        for(int i = 0; i < navMeshPath.corners.Length; i++)
        {
            allWayPoints[i + 1] = navMeshPath.corners[i];
        }

        float pathLength = 0.0f;

        for(int i = 0; i < allWayPoints.Length-1; i++)
        {
            pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);
        }

        return pathLength;
    }
}
