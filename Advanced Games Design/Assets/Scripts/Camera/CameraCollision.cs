using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour {

    [SerializeField] float minDistanceFromPlayer = 1.0f;
    [SerializeField] float maxDistanceFromPlayer = 4.0f;
    [SerializeField] float smooth = 10.0f;

    [SerializeField] private float distance;

    Vector3 cameraDirection; 
    public Vector3 altDirection;

	// Use this for initialization
	void Start ()
    {
        cameraDirection = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 desiredCameraPosition = transform.parent.TransformPoint(cameraDirection * maxDistanceFromPlayer);
        RaycastHit hit;

        if(Physics.Linecast (transform.parent.position, desiredCameraPosition, out hit))
        {
            distance = Mathf.Clamp((hit.distance * 0.9f), minDistanceFromPlayer, maxDistanceFromPlayer);

        }
        else
        {
            distance = maxDistanceFromPlayer;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, cameraDirection * distance, Time.deltaTime * smooth);
	}
}
