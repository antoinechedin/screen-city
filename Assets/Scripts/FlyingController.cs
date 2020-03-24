using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FlyingController : MonoBehaviour
{
    public float moveSpeed;
    public float rotationSpeed;

    private float yaw = 0, pitch = 0;
    private Transform cameraTransform;
    private CharacterController characterController;
    
    private void Awake() {
        cameraTransform = transform.GetChild(0); // Child camera need to be the first child
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
               
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        Vector3 forward = transform.forward * Input.GetAxis("Vertical");
        Vector3 side = transform.right * Input.GetAxis("Horizontal");
        Vector3 up = Vector3.up * Input.GetAxis("Up"); 
        Vector3 move = (forward + side + up).normalized;

        float currentMoveSpeed = Input.GetKey(KeyCode.LeftShift) ? moveSpeed * 3f : moveSpeed;

        characterController.Move(move * Time.deltaTime * currentMoveSpeed);   

        // Orientation
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch += Input.GetAxis("Mouse Y") * rotationSpeed;
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        Vector3 charcacterOrientation = new Vector3(0f, yaw, 0f) ;
        Vector3 cameraOrientation = new Vector3(pitch, 0f, 0f) ;

        transform.eulerAngles = charcacterOrientation; 
        cameraTransform.transform.localEulerAngles = cameraOrientation; 
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
