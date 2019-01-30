using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    [SerializeField] float mouseSensitivity = 3.0f;
    [SerializeField] float distanceFromTarget = 3.5f;
    [SerializeField] float rotationSmoothTime = 0.09f;
    [SerializeField] Transform cameraTarget;
    [SerializeField] Vector2 pitchMinMax = new Vector2(-40, 85);
    [SerializeField] bool lockCursor;

    float yaw, pitch;
    Vector3 rotationSmoothVelocity, currentRotation;

    private void Start()
    {
        if(lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate()
    {
        CameraRotation();
    }

    void CameraRotation()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);

        transform.eulerAngles = currentRotation;

        transform.position = cameraTarget.position - transform.forward * distanceFromTarget;
    }
}

//[SerializeField] float cameraMovementSpeed = 0.09f;
//[SerializeField] float xTilt = 10.0f;
//[SerializeField] Transform cameraTarget;
//[SerializeField] Vector3 cameraOffset;

//private float rotateSpeed = 0.0f;
//Vector3 newCamPosition = Vector3.zero;
//CharacterController charController;

//// Use this for initialization
//void Start()
//{
//    SetCameraTarget(cameraTarget);
//}

//void SetCameraTarget(Transform t)
//{
//    cameraTarget = t;

//    if (cameraTarget != null)
//    {
//        if (cameraTarget.GetComponent<CharacterController>())
//        {
//            charController = cameraTarget.GetComponent<CharacterController>();
//        }
//        else
//        {
//            Debug.LogError("The camera's target needs a character controller");
//        }
//    }
//    else
//    {
//        Debug.LogError("Your camera has no target.");
//    }
//}

//void LateUpdate()
//{
//    MoveToTarget();
//    LookAtTarget();
//}

//void MoveToTarget()
//{
//    newCamPosition = cameraTarget.rotation * cameraOffset;
//    newCamPosition += cameraTarget.position;
//    transform.position = newCamPosition;
//}

//void LookAtTarget()
//{
//    float eularYAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, cameraTarget.eulerAngles.y, ref rotateSpeed, cameraMovementSpeed);
//    transform.rotation = Quaternion.Euler(transform.eulerAngles.x, eularYAngle, 0.0f);
//}