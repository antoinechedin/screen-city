using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FlyingController : MonoBehaviour
{
    public float moveSpeed;
    public float rotationSpeed;

    private float yaw, pitch;

    private CharacterController characterController;
    
    private void Awake() {
        characterController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        Vector3 forward = new Vector3(transform.forward.x, 0f,transform.forward.z).normalized * Input.GetAxis("Vertical");
        Vector3 side = new Vector3(transform.right.x, 0f,transform.right.z).normalized * Input.GetAxis("Horizontal");
        Vector3 up = transform.up * Input.GetAxis("Up"); 
        Vector3 move = (forward + side + up).normalized;

        characterController.Move(move * Time.deltaTime * moveSpeed);   

        // Orientation
        yaw += Input.GetAxis("Mouse X");
        
        pitch += Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        Vector3 orientation = new Vector3(pitch, yaw, 0f) * rotationSpeed;

        transform.eulerAngles = orientation; 
    }
}
