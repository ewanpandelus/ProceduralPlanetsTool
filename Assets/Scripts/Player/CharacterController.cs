using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] Animator animator;
    [SerializeField] private Rigidbody rb;
    bool running = false;
    private Vector3 moveDirection;
    private Vector3 savedForward;
    private Vector3 savedDirection;
    float  moveRotation;

    Vector3 prevPosition;
    void Start()
    {
        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        prevPosition = rb.position;
        savedDirection = moveDirection;

    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        running = moveDirection.magnitude > 0;
        animator.SetBool("Running", running);

    }
    private void FixedUpdate()
    {

     
       // rb.MovePosition(rb.position +  moveDirection * movementSpeed * Time.deltaTime);//

        if(moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            rb.MovePosition(rb.position + transform.forward * Time.deltaTime * movementSpeed);
            Debug.Log(Vector3.Dot(Vector3.up, transform.up));
        }
       
      



    }



    }
  


