using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : Photon.MonoBehaviour
{
    public bool devTesting = false;
    private PhotonView PhotonView;
    public GameObject plCam;
    private Vector3 selfPos;
    private Quaternion realRotation;
    private GameObject myInvCanvas;

    private Animator anim;

    [SerializeField] float walkSpeed = 2.0f;
    [SerializeField] float runSpeed = 6.0f;
    [SerializeField] float gravity = -12.0f;
    [SerializeField] float rotationSmoothTime = 0.2f;
    [SerializeField] float speedSmoothTime = 0.05f;

    private float playerSpeed, animSpeedPercent, turnSmoothVelocity, speedSmoothVelocity, currentSpeed, velocityY;

    Animator animator;
    CharacterController characterController;
    Transform cameraTransform;

    void Start()
    {
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
        characterController = GetComponent<CharacterController>();
        myInvCanvas = GetComponentInChildren<inventory>().transform.GetChild(0).gameObject;
    }

    private void Awake()
    {
        if (devTesting)
        {
            plCam.SetActive(true);
        }
        PhotonView = GetComponent<PhotonView>();
        if (!devTesting && PhotonView.isMine)
        {
            plCam.SetActive(true);
            if(GetComponent<PhotonView>().viewID.ToString().Contains("1"))
            {
                gameObject.transform.tag = "PlayerOne";
            }
            if (GetComponent<PhotonView>().viewID.ToString().Contains("2"))
            {
                gameObject.transform.tag = "PlayerTwo";
            }

        }
       
         
    }

    private void Update()
    {
        if (!devTesting)
        {
            if (photonView.isMine)
            {
                CheckInput();
            }
            else SmoothNetMovement();
        }
        else CheckInput();

    }

    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.P)){
            SceneManager.LoadScene(0);
        }
        // player inputs
        Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDirection = playerInput.normalized;
        bool running = Input.GetKey(KeyCode.LeftShift);
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            myInvCanvas.GetComponent<Canvas>().enabled = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if(Input.GetKeyUp(KeyCode.Tab))
        {
            myInvCanvas.GetComponent<Canvas>().enabled = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        PlayerMovementAndRotation(inputDirection, running);

        // animations
        animSpeedPercent = ((running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * 0.5f);
        animator.SetFloat("speedPercent", animSpeedPercent, speedSmoothTime, Time.deltaTime);
    }


    void PlayerMovementAndRotation(Vector2 inputDirection, bool running)
    {
        if (inputDirection != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, rotationSmoothTime);
        }
        
        playerSpeed = ((running) ? runSpeed : walkSpeed) * inputDirection.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, playerSpeed, ref speedSmoothVelocity, speedSmoothTime);

        velocityY += Time.deltaTime * gravity;
        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

        characterController.Move(velocity * Time.deltaTime);
        currentSpeed = new Vector2(characterController.velocity.x, characterController.velocity.z).magnitude;

        if(characterController.isGrounded)
        {
            velocityY = 0;
        }
    }

    private void SmoothNetMovement()
    {
        transform.position = Vector3.Lerp(transform.position, selfPos, Time.deltaTime * 8);
        transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, Time.deltaTime * 8);
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //THIS IS OUR PLAYER
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(animator.GetFloat("speedPercent"));

        }
        else
        {
            //THIS IS OTHER COOP PLAYER
            selfPos = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();

            animator.SetFloat("speedPercent", (float)stream.ReceiveNext());
        }
    }
}
