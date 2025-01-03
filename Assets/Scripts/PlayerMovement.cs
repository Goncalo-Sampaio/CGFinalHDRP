using UnityEngine;
using Unity.Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float maxForwardSpeed;
    [SerializeField]
    private float maxBackwardSpeed;
    [SerializeField]
    private float maxStrafeSpeed;
    [SerializeField]
    private float walkMultiplier;

    [Header("Camera")]
    [SerializeField]
    private float maxLookUpAngle;
    [SerializeField]
    private float maxLookDownAngle;



    private CharacterController controller;
    private Transform head;
    private Vector3 headRotation;
    private Vector3 velocity;
    private Vector3 motion;

    private void Start()
    {
        //Getting the necessary components
        controller = GetComponent<CharacterController>();
        head = GetComponentInChildren<CinemachineCamera>().transform;

        HideCursor();
    }

    private void FixedUpdate()
    {
        UpdateVelocity();
        UpdatePosition();
    }

    private void Update()
    {
        UpdateRotation();
        UpdateHead();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void HideCursor()
    {
        //Locks the cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Gets the mouse's horizontal movement and rotates the player accordingly
    /// </summary>
    private void UpdateRotation()
    {
        float rotation = Input.GetAxis("Mouse X");

        transform.Rotate(0, rotation, 0);
    }

    /// <summary>
    /// Handles the camera's rotation around the x axis
    /// </summary>
    private void UpdateHead()
    {
        // Gets the current rotation of the head (camera) around the x axis
        headRotation = head.localEulerAngles;

        // Gets the mouse's vertical movement and subtracts it to the head's rotation
        headRotation.x -= Input.GetAxis("Mouse Y");

        // Gives an upper and lower limit to the rotation, setting the rotation to the correct values
        if (headRotation.x > 180f)
            headRotation.x = Mathf.Max(maxLookUpAngle, headRotation.x);
        else
            headRotation.x = Mathf.Min(maxLookDownAngle, headRotation.x);

        head.localEulerAngles = headRotation;
    }

    /// <summary>
    /// Handles the player's velocity when moving in different directions
    /// </summary>
    private void UpdateVelocity()
    {
        //Getting the player's movement input
        float forwardAxis = Input.GetAxis("Vertical");
        float strafeAxis = Input.GetAxis("Horizontal");

        //Changing forward/backward velocity depending on the player's input
        if (forwardAxis >= 0)
        {
            velocity.z = forwardAxis * maxForwardSpeed;
        }
        else
        {
            velocity.z = forwardAxis * maxBackwardSpeed;
        }

        //Changing strafe velocity depending on the player's input
        velocity.x = strafeAxis * maxStrafeSpeed;

        WalkVelocity();

        //Making sure that the player doesn't move a bigger distance when moving diagonaly
        if (velocity.magnitude > maxForwardSpeed)
        {
            velocity = velocity.normalized * (forwardAxis > 0 ? maxForwardSpeed : maxBackwardSpeed);
        }
    }

    /// <summary>
    /// Multiplies the previously calculated velocity by a defined multiplier
    /// </summary>
    private void WalkVelocity()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            velocity.z *= walkMultiplier;
            velocity.x *= walkMultiplier;
        }
    }

    /// <summary>
    /// Moves the player using the calculated velocity
    /// </summary>
    private void UpdatePosition()
    {
        motion = transform.TransformVector(velocity * Time.fixedDeltaTime);

        controller.Move(motion);
    }
}
