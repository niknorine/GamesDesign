using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {

    [SerializeField] float stoppingDistance = 5.0f;

    private Transform playerOne;
    private Transform playerTwo;

    private EnemySenses enemySense;
    private NavMeshAgent navMeshAgent;
    private Animator anim;
    private EnemyMovementSetup enemyMovementSetup;

    private void Awake()
    {
        playerOne = GameObject.FindGameObjectWithTag("PlayerOne").transform;
        playerTwo = GameObject.FindGameObjectWithTag("PlayerTwo").transform;
        enemySense = GetComponent<EnemySenses>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        
        enemyMovementSetup = new EnemyMovementSetup(anim);

        stoppingDistance *= Mathf.Deg2Rad;
    }

    private void Update()
    {

        NavMeshAnimatorSetup();
    }
    
    void NavMeshAnimatorSetup()
    {
        float enemySpeed;
        float enemyAngle;

        if(enemySense.isPlayerOneInSight)
        {
            enemySpeed = 0.0f;

            // fromVector = enemies forward vector, toVector = from enemy to the player, upVector = enemies up Vector
            enemyAngle = FindAngleToPlayers(transform.forward, playerOne.position - transform.position, transform.up);
        }
        else
        {
            enemySpeed = Vector3.Project(navMeshAgent.desiredVelocity, transform.forward).magnitude;
            enemyAngle = FindAngleToPlayers(transform.forward, navMeshAgent.desiredVelocity, transform.up);

            if(Mathf.Abs(enemyAngle) < stoppingDistance)
            {
                transform.LookAt(transform.position + navMeshAgent.desiredVelocity);
                enemyAngle = 0.0f;
            }
        }
        
        if (enemySense.isPlayerTwoInSight)
        {
            enemySpeed = 0.0f;

            // fromVector = enemies forward vector, toVector = from enemy to the player, upVector = enemies up Vector
            enemyAngle = FindAngleToPlayers(transform.forward, playerTwo.position - transform.position, transform.up);
        }
        else
        {
            enemySpeed = Vector3.Project(navMeshAgent.desiredVelocity, transform.forward).magnitude;
            enemyAngle = FindAngleToPlayers(transform.forward, navMeshAgent.desiredVelocity, transform.up);

            if (Mathf.Abs(enemyAngle) < stoppingDistance)
            {
                transform.LookAt(transform.position + navMeshAgent.desiredVelocity);
                enemyAngle = 0.0f;
            }
        }

        enemyMovementSetup.Setup(enemySpeed, enemyAngle);
    }

    float FindAngleToPlayers(Vector3 fromVector, Vector3 toVector, Vector3 upVector)
    {
        if(toVector == Vector3.zero)
            return 0.0f;

        float angleToPlayers = Vector3.Angle(fromVector, toVector);
        Vector3 normal = Vector3.Cross(fromVector, toVector);
        angleToPlayers *= Mathf.Sign(Vector3.Dot(normal, upVector));
        angleToPlayers *= Mathf.Deg2Rad;

        return angleToPlayers;
    }
}
