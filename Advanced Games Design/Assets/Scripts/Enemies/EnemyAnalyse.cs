using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnalyse : MonoBehaviour {

    [SerializeField] float analyseTimer = 5.0f;
    [SerializeField] float warningDuration = 3.5f;
    [SerializeField] float followSpeedWhileAnalysing = 1.0f;
    //[SerializeField] AudioClip analyseSound;
    [SerializeField] Light spotlight;
    [SerializeField] float viewDistance, viewAngle;
    [SerializeField] LayerMask layerMask;

    private PlayersLastLocation playersLastLocation;

    Color startingSpotlightColour;
    NavMeshAgent navMeshAgent;
    private Animator anim;
    private BoxCollider boxCollider;
    private Transform playerOne, playerTwo;
    private bool analysingPlayer;
    private float playersWarningTimer;

    GameObject spotlightPosition;
    public Vector3 spotlightOffset;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerOne = GameObject.FindGameObjectWithTag("PlayerOne").transform;
        playerTwo = GameObject.FindGameObjectWithTag("PlayerTwo").transform;
        startingSpotlightColour = spotlight.color;
        playersLastLocation = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayersLastLocation>();
        //viewAngle = spotlight.spotAngle;

        //spotlightPosition = GameObject.Find("SpotlightPosition");
        //spotlightOffset = spotlightPosition.transform.position - transform.position;
    }

    private void Update()
    {

        if (PlayerInRange())
        {
            playersWarningTimer += Time.deltaTime;
            AnalysePlayer();
        }
        else
        {
            playersWarningTimer -= Time.deltaTime;

        }

        playersWarningTimer = Mathf.Clamp(playersWarningTimer, 0, warningDuration);
        spotlight.color = Color.Lerp(Color.yellow, Color.red, playersWarningTimer / warningDuration);

        if (playersWarningTimer == 0)
        {
            spotlight.color = startingSpotlightColour;
        }

        Debug.Log(spotlightOffset);
        Debug.Log(spotlightPosition);
    }

    bool PlayerInRange()
    {
        if (Vector3.Distance(transform.position, playerOne.position) < viewDistance)
        {
            Vector3 directionToPlayer = (playerOne.position - transform.position).normalized;
            float angleBetweenBotAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleBetweenBotAndPlayer < viewAngle / 2.0f)
            {
                if (!Physics.Linecast(transform.position, playerOne.position, layerMask))
                {
                    return true;
                }
            }
        }
        return false;

    }

    void AnalysePlayer()
    {
        playersLastLocation.playerOnePosition = playerOne.transform.position;

        navMeshAgent.destination = playerOne.transform.position;
        navMeshAgent.stoppingDistance = 7.0f;
        navMeshAgent.speed = followSpeedWhileAnalysing;
    }
}
