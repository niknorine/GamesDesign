using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform target;


    private Vector3 _cameraOffest;

    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 0.5f;
    
    // Use this for initialization
    void Start () {
        _cameraOffest = transform.position - target.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        Vector3 newPos = target.position + _cameraOffest;

        transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);
	}
}
