using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum WorldState
    {
        Grounded, //on ground
        InAir, //in the air
    }

    [HideInInspector]
    public WorldState States;
    private Transform Cam;
    private Transform CamY;
    [SerializeField] private Animator animator;
    private bool running = false;
    //public ControlsPivot AxisPivot;
    private CameraFollow CamFol;
   
    Vector3 prevMovDirection;
    private DetectCollision Colli;
    [HideInInspector]
    public Rigidbody Rigid;

    float delta;
    public bool grounded = true;
    [Header("Physics")]
    public Transform[] GroundChecks;
    public float DownwardPush; //what is applied to the player when on a surface to stick to it
    public float GravityAmt;    //how much we are pulled downwards when we are on a wall
    public float GravityBuildSpeed; //how quickly we build our gravity speed
    public float InAirSpeed;

    private float inAirAcceleration = 0;
    
    public LayerMask GroundLayers; //what layers the ground can be
    public float GravityRotationSpeed = 10f; //how fast we rotate to a new gravity direction

    [Header("Stats")]
    public float Speed = 15f; //max speed for basic movement
    public float Acceleration = 4f; //how quickly we build speed
    public float turnSpeed = 2f;
    private Vector3 MovDirection, targetDir, GroundDir; //where to move to

    [Header("Jumps")]
    public float JumpAmt;
    private bool jumping;

    // Start is called before the first frame update
    void Awake()
    {
        Rigid = GetComponentInChildren<Rigidbody>();
        Colli = GetComponent<DetectCollision>();
        GroundDir = transform.up;
        SetGrounded();
        
        Cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        CamY = Cam.transform.parent.parent.transform;
        CamFol = Cam.GetComponentInParent<CameraFollow>();

        //detatch rigidbody so it can move freely 
        Rigid.transform.parent = null;
    }

    private void Update()   //inputs
    {
        transform.position = Rigid.position;
        if (Input.GetKey(KeyCode.Space) &&!jumping)
        {
            StartCoroutine(JumpUp(JumpAmt));
        }
    }

    // Update is called once per frame
    void FixedUpdate()  //world movement
    {
        delta = Time.deltaTime;
        if (!grounded)
        {
            FallingCtrl(delta);
        }
        else
        {
            float Spd = Speed;

           if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
            {
                //we are not moving, lerp to a walk speed
                Spd = 0f;
            }

        
            MoveSelf(delta, Spd, Acceleration);
       
       

        }
   
 
    }

    //transition to ground
    public void SetGrounded()
    {
    

        States = WorldState.Grounded;
    }
    //transition to air
    void SetInAir()
    {
        States = WorldState.InAir;
    }
    //jump up
    IEnumerator JumpUp(float UpwardsAmt)
    {
        jumping = true;
        animator.SetBool("Jumping", jumping);
        yield return new WaitForSecondsRealtime(0.3f);

        Rigid.velocity = Vector3.zero;

        SetInAir();

        if (UpwardsAmt != 0)
            Rigid.velocity = Rigid.velocity+=((transform.up * UpwardsAmt));


        

    
    }     
    //check the angle of the floor we are stood on
    Vector3 FloorAngleCheck()
    {
        RaycastHit HitFront;
        RaycastHit HitCentre;
        RaycastHit HitBack;

        Physics.Raycast(GroundChecks[0].position, -GroundChecks[0].transform.up, out HitFront, 10f, GroundLayers);
        Physics.Raycast(GroundChecks[1].position, -GroundChecks[1].transform.up, out HitCentre, 10f, GroundLayers);
        Physics.Raycast(GroundChecks[2].position, -GroundChecks[2].transform.up, out HitBack, 10f, GroundLayers);

        Vector3 HitDir = transform.up;

        if (HitCentre.transform != null)
        {
            HitDir += HitCentre.normal;
        }
     


        return HitDir.normalized;
    }
    
    //move our character
    void MoveSelf(float deltaTime, float movementSpeed, float acceleration)
    {
      
        Vector3 moveDirection = EvaluateMovementInput();
        running = moveDirection.magnitude != 0;
        animator.SetBool("Running", running);
        if (!running)
        {
            targetDir = transform.forward;
        }
        else
        {
            targetDir = moveDirection;
        }



        Vector3 SetGroundDir = FloorAngleCheck();
        GroundDir = Vector3.Lerp(GroundDir, SetGroundDir, deltaTime * GravityRotationSpeed);

        //lerp mesh slower when not on ground
        RotateSelf(SetGroundDir, deltaTime, GravityRotationSpeed);
        RotateMesh(deltaTime, targetDir, turnSpeed);

        //move character
  
        Vector3 curVelocity = Rigid.velocity;

        if (!running) //if we are not pressing a move input we move towards velocity //or are crouching
        {
            movementSpeed *= 0.8f; //less speed is applied to our character
            MovDirection = Vector3.Lerp(transform.forward, Rigid.velocity.normalized, 12f * deltaTime);
        }
        else
        {
            MovDirection = transform.forward;
        }

        Vector3 targetVelocity = MovDirection * movementSpeed;

        //push downwards in downward direction of mesh
        targetVelocity -= SetGroundDir * DownwardPush;

        Vector3 dir = Vector3.Lerp(curVelocity, targetVelocity, deltaTime * acceleration);
        Rigid.velocity = dir;
    }

    Vector3 EvaluateMovementInput()
    {
        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");

        Vector3 screenMovementForward = CamY.transform.forward;
        Vector3 screenMovementRight = CamY.transform.right;

        Vector3 h = screenMovementRight * _xMov;
        Vector3 v = screenMovementForward * _zMov;

        return (v + h).normalized;
    }
    void FallingCtrl(float deltaTime)
    {
        Vector3 moveDirection = EvaluateMovementInput();

        if (moveDirection.magnitude!=0)
        {
            inAirAcceleration = Mathf.Clamp(inAirAcceleration + 0.04f, 0.5f, 1);
            targetDir = moveDirection;
            Rigid.position += targetDir * (InAirSpeed);

        }
        else
        {
            targetDir = transform.forward;

        }


        Vector3 SetGroundDir = FloorAngleCheck();
        GroundDir = Vector3.Lerp(GroundDir, SetGroundDir, deltaTime * GravityRotationSpeed);
        float lerpSpeed = 1f;
        RotateSelf(SetGroundDir, deltaTime, GravityRotationSpeed);
        RotateMesh(deltaTime, targetDir, turnSpeed*lerpSpeed);

        Rigid.AddForce(GroundDir*GravityAmt);
 



  
  
     
    }
    //rotate the direction we face down
    void RotateSelf(Vector3 Direction, float d, float GravitySpd)
    {
        Vector3 LerpDir = Vector3.Lerp(transform.up, Direction, d * GravitySpd);
        transform.rotation = Quaternion.FromToRotation(transform.up, LerpDir) * transform.rotation;
    }
    //rotate the direction we face forwards
    void RotateMesh(float d, Vector3 LookDir, float spd)
    {
        Quaternion SlerpRot = Quaternion.LookRotation(LookDir, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, SlerpRot, spd * d);
    }
    public void SetGrounded(bool grounded) 
    {
        this.grounded = grounded;
        if (grounded)
        {
            animator.SetBool("Jumping", false);
            jumping = false;
        }
    }


}
