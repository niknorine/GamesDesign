using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBots : MonoBehaviour {

    [SerializeField] float botMovementSpeed = 3.0f;
    [SerializeField] float botWaitTime = 0.5f;
    [SerializeField] float botTurnSpeed = 90.0f;
    [SerializeField] float botLookRotation = 18.0f;
    [SerializeField] float warningDuration = 3.5f;
    [SerializeField] float viewDistance;
    [SerializeField] LayerMask layerMask;

    [SerializeField] Light spotlight;
    [SerializeField] Transform pathWaypoints;

    private float viewAngle, playersWarningTimer;
    Color startingSpotlightColour, currentColour;
    Transform player;

	// Use this for initialization
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotlight.spotAngle;
        startingSpotlightColour = spotlight.color;

        Vector3[] waypoints = new Vector3[pathWaypoints.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathWaypoints.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        StartCoroutine(FollowPath(waypoints));
	}

    // Update is called once per frame
    void Update()
    {
        
        if (PlayerInRange())
        {
            playersWarningTimer += Time.deltaTime;
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

        
    }

    bool PlayerInRange()
    {
        if(Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleBetweenBotAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if(angleBetweenBotAndPlayer < viewAngle / 2.0f)
            {
                if(!Physics.Linecast(transform.position, player.position, layerMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];
        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];

        transform.LookAt(targetWaypoint);
        transform.rotation = Quaternion.Euler(botLookRotation, transform.rotation.y, transform.rotation.z);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, botMovementSpeed * Time.deltaTime);
            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(botWaitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }

            yield return null;
        }
    }
	
    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 directionToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(directionToLookTarget.z, directionToLookTarget.x) * Mathf.Rad2Deg;

        while(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, botTurnSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(botLookRotation, angle, 0);
            yield return null;
        }
    }

    // visualise the waypoints and the link between the waypoints
    void OnDrawGizmos()
    {
        Vector3 firstWaypoint = pathWaypoints.GetChild(0).position;
        Vector3 previousPosition = firstWaypoint;

        foreach(Transform waypoint in pathWaypoints)
        {
            Gizmos.DrawSphere(waypoint.position, 0.3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }

        Gizmos.DrawLine(previousPosition, firstWaypoint);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}
