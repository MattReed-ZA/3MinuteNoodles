using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private TrailRenderer tr;
    
    private Animator anim;
    private bool isWalking;

    private float movementInputDirection;
    private float jumpTimer;
    private float turnTimer;
    private float wallJumpTimer;

    private Rigidbody2D rb;

    public float movementSpeed = 10f;
    public float jumpForce=16.0f;
    
    private bool isFacingRight = true;
    private bool isGrounded=true;
    private bool isTouchingWall;
    private bool canNormalJump;
    private bool canWallJump;
    private bool isWallSliding;
    private bool isAttemptingToJump;
    private bool checkJumpMultiplier;
    private bool canMove;
    private bool canFlip;
    private bool hasWallJumped;
    private bool isTouchingLedge;
    private bool canClimbLedge=false;
    private bool ledgeDetected;

    public int amountOfJumps=2;

    private int amountOfJumpsleft; 
    private int facingDirection=1;
    private int lastWallJumpDirection;

    public Transform groundCheck;
    public Transform wallCheck;
    public Transform ledgeCheck;

    public LayerMask whatIsGround;
    
    public float groundCheckRadius;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float movementForceInAir;
    public float airDragMultiplier=0.95f;
    public float varJumpHeightMultiplier=0.5f;
    public float wallHopForce;
    public float wallJumpForce;
    public float jumpTimerSet=0.15f;
    public float turnTimerSet=0.1f;
    public float wallJumpTimerSet=0.5f;
    public float ledgeClimbXOff1=0f;
    public float ledgeClimbYOff1=0f;
    public float ledgeClimbXOff2=0f;
    public float ledgeClimbYOff2=0f;

    private Vector2 ledgePosBottom;
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;


    //FOR DASHING///////////////////////
    private bool isDashing;
    public float dashTime;
    public float dashSpeed;
    public float dashCooldown;
    private float dashTimeLeft;
    private float lastDash=-100;

    public float distanceBetweenImages;
    private float lastImageXpos;

    ///////////////////////////////////

    //FOR PARTICLE EFFECT//////////////
    public ParticleSystem dust;

    void CreateDust()
    {
        dust.Play();
    }
    ///////////////////////////////////

    //FOR RESPAWNING///////////////////
    private Vector3 respawnPoint;
    public GameObject respawnDetector;
    ///////////////////////////////////

    //FOR PULLING & PUSHING//////////////////
    private bool isPulling = false;
    private bool isPushing = false;
    private bool canDrag = false;
    private bool dragging = false;
    private float dragSpeed = 5.0f;
    private Rigidbody2D dragObject = null;
    ///////////////////////////////////////

    void Start()
    {
        rb=GetComponent<Rigidbody2D>();
        amountOfJumpsleft=amountOfJumps;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
        anim=GetComponent<Animator>();
        respawnPoint=rb.position;
    }

    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSliding();
        CheckDash();
        CheckJump();
        CheckLedgeClimb();
        CheckDashJump();
        //TO MOVE RESPAWN POINT///
        respawnDetector.transform.position = new Vector2(rb.position.x,respawnDetector.transform.position.y);
        //////////////////////////

        if(dragging)
        {
            canNormalJump=false;
            canWallJump=false;
        }
        
    }

    //FOR CHECKPOINTS/////////////////////////////////////////
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Respawn")
        {
            rb.position=respawnPoint;
            dragging=false;
            isPushing=false;
            isPulling=false;
        }
        else if(collision.tag=="Checkpoint")
        {
            respawnPoint=rb.position;
        }
        else if(collision.tag == "Draggable")
        {
            canDrag = true;
        }
    }

    private void onTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Draggable")
        {
            canDrag = false;
        }
    }
    //////////////////////////////////////////////////////////

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking",isWalking);
        anim.SetBool("isGrounded",isGrounded);      
        anim.SetFloat("yVelocity",rb.velocity.y);
        anim.SetBool("isWallSliding",isWallSliding);
        anim.SetBool("isDashing",isDashing);
        anim.SetBool("isPulling", isPulling);
        anim.SetBool("isPushing", isPushing);
    }

    private void CheckIfWallSliding()
    {
        if(isTouchingWall && movementInputDirection==facingDirection  && rb.velocity.y<0 )
        {
            isWallSliding=true;
        }
        else
        {
            isWallSliding=false;
        }
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }

    private void CheckMovementDirection()
    {
        if(isFacingRight && movementInputDirection<0)
        {
            Flip();
        }
        else if(!isFacingRight && movementInputDirection>0)
        {
            Flip();
        }
    }

    private void Flip()
    {
        if(!isWallSliding && !dragging && canFlip)
        {
            facingDirection*=-1;
            isFacingRight=!isFacingRight;
            transform.Rotate(0.0f,180.0f,0.0f);
        }
    }
   

    private void CheckInput()
    {
        //Dragging Mechanic################################################################################################################################
        if(canDrag == true && dragObject != null && Input.GetButtonDown("Drag"))
        {
            if(dragging == false)
            {
                dragging = true;

                dragObject.constraints = RigidbodyConstraints2D.None;
                dragObject.constraints = RigidbodyConstraints2D.FreezeRotation;

                //dragObject.constraints = RigidbodyConstraints2D.FreezePositionY;
                //rb.constraints=RigidbodyConstraints2D.FreezePositionY;
                rb.constraints=RigidbodyConstraints2D.FreezeRotation;

                isWalking = false;
            }
            else if(dragging == true)
            {
                dragging = false;

                dragObject.constraints = RigidbodyConstraints2D.None;
                dragObject.constraints = RigidbodyConstraints2D.FreezeAll;

                isWalking = true;
                isPulling = false;
                isPushing = false;
            }
        }
        //Dragging Mechanic################################################################################################################################


        movementInputDirection=Input.GetAxisRaw("Horizontal");
        
        if(Input.GetButtonDown("Jump"))
        {
            if(isGrounded || (amountOfJumpsleft>0 && !isTouchingWall))
            {
                NormalJump();
                StartDJTimer=true;
                //Debug.Log("JUMP");
            }
            else
            {
                jumpTimer=jumpTimerSet;
                isAttemptingToJump=true;
            }
        }

        if(Input.GetButtonDown("Horizontal") && isTouchingWall)
        {
            if(!isGrounded && movementInputDirection != facingDirection)
            {
                canMove=false;
                canFlip=false;

                turnTimer=turnTimerSet;
            }
        }

        if(turnTimer>=0)
        {
            turnTimer-= Time.deltaTime;

            if(turnTimer<=0)
            {
                canMove=true;
                canFlip=true;
            }
        }

        if(checkJumpMultiplier && !Input.GetButton("Jump"))
        {
            checkJumpMultiplier=false;
            rb.velocity= new Vector2(rb.velocity.x,rb.velocity.y * varJumpHeightMultiplier);
        }

        if(Input.GetButtonDown("Dash"))
        {
            if(Time.time >= (lastDash+dashCooldown))
            {
                tr.emitting=true;
                Dash();
                SoundManagerScript.PlaySound("Dash");
                //Debug.Log("DASH");
            }
            
        }
    }

    private void Dash()
    {
        isDashing=true;
        dashTimeLeft=dashTime;
        lastDash=Time.time;
    }

    private void CheckDash()
    {
        if(isDashing)
        {  
            if(dashTimeLeft>0)
            {  
                canMove=false;
                canFlip=false;
                rb.velocity=new Vector2(dashSpeed*facingDirection,0);
                dashTimeLeft -= Time.deltaTime;
            }

            if(dashTimeLeft<=0 || isTouchingWall)
            {
                isDashing=false;
                tr.emitting=false;
                canFlip=true;
                canMove=true;
            }
            
        }
    }

    public void CheckJump()
    {
        if(jumpTimer>0)
        {
            //Wall Jump
            if(!isGrounded && isTouchingWall && movementInputDirection!=0 && movementInputDirection != facingDirection)
            {
                WallJump();
                //Debug.Log("WALL JUMP");
            }
            else if(isGrounded || amountOfJumpsleft!=0)
            {
                NormalJump();
            }
        }

        if(isAttemptingToJump)
        {
            jumpTimer -= Time.deltaTime;
        }

        if(wallJumpTimer>0)
        {
            if(hasWallJumped && movementInputDirection == -lastWallJumpDirection)
            {
                rb.velocity=new Vector2(rb.velocity.x,0);
                hasWallJumped=false;
            }
            else if(wallJumpTimer <= 0)
            {
                hasWallJumped=false;
            }
            else
            {
                wallJumpTimer-=Time.deltaTime;
            }
        }
    }

    private void NormalJump()
    {
        if(canNormalJump && (!isPulling||!isPushing))
        {
            CreateDust();
            SoundManagerScript.PlaySound("Jump");
            rb.velocity=new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsleft--;
            jumpTimer=0;
            isAttemptingToJump=false;
            checkJumpMultiplier=true;
        }
    }

    private void WallJump()
    {
        if(canWallJump)
        {
            rb.velocity=new Vector2(rb.velocity.x,0.0f);
            isWallSliding=false;
            amountOfJumpsleft=1;//FIXES INFINITE WALL JUMP EXPLOIT
            amountOfJumpsleft--;
            Vector2 forceToAdd=new Vector2(wallJumpForce*wallJumpDirection.x*movementInputDirection,wallJumpForce*wallJumpDirection.y);
            rb.AddForce(forceToAdd,ForceMode2D.Impulse);
            jumpTimer=0;
            isAttemptingToJump=false;
            checkJumpMultiplier=true;
            turnTimer=0;
            canMove=true;
            canFlip=true;
            hasWallJumped=true;
            wallJumpTimer=wallJumpTimerSet;
            lastWallJumpDirection=-facingDirection;
        }
    }

    private void CheckIfCanJump()
    {
        if((isGrounded && rb.velocity.y<=0.01f))
        {
            amountOfJumpsleft=amountOfJumps;
        }

        if(isTouchingWall)
        {
            canWallJump=true;
        }

        if(amountOfJumpsleft<=0)
        {
            canNormalJump=false;
        }
        else
        {
            canNormalJump=true;
        }
    }

    private void ApplyMovement()
    {
        if(!isGrounded && !isWallSliding && movementInputDirection==0)
        {
            rb.velocity=new Vector2(rb.velocity.x * airDragMultiplier,rb.velocity.y);
        }
        else if(canMove)
        {  
            //Debug.Log("In the Apply Movement Method, dragging is: " + dragging);
            if(dragging == true && dragObject != null)
            {
                if((isFacingRight && rb.velocity.x>0.5) || (!isFacingRight && rb.velocity.x<0.5))
                {
                    //Pushing
                    isPulling = false;
                    isPushing = true;                   
                }
                else if((isFacingRight && rb.velocity.x<0.5) || (!isFacingRight && rb.velocity.x>0.5))
                {
                    //Pulling
                    isPushing = false;
                    isPulling = true;
                }

                rb.velocity = new Vector2(dragSpeed*movementInputDirection, rb.velocity.y);
                dragObject.velocity = new Vector2(dragSpeed*movementInputDirection, rb.velocity.y);
            }
            else
            {
                if(rb.velocity.x>=0.5 || rb.velocity.x<=-0.5)
                {
                    isWalking=true;
                }
                else
                {
                    isWalking=false;
                }

                rb.velocity=new Vector2(movementSpeed*movementInputDirection, rb.velocity.y);
            }      
        }
       
        
        
        if(isWallSliding)
        {
            if(rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity=new Vector2(rb.velocity.x,-wallSlideSpeed);
            }
        }
    }

    private void CheckLedgeClimb()
    {
       
    }

    public void FinishLedgeClimb()
    {

    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall=Physics2D.Raycast(wallCheck.position,transform.right,wallCheckDistance,whatIsGround);
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position,new Vector3(wallCheck.position.x + wallCheckDistance,wallCheck.position.y,wallCheck.position.z));
    }

    public void setDragObject(Rigidbody2D obj)
    {
        if(obj != null)
        {
            dragObject = obj;
        }
        else
        {
            dragObject = null;
        }
    }


    //NEW STUFF AFTER D5///////////////////////////
    public float DashJumpTimer=1.5f;
    public bool StartDJTimer=false;
    public void CheckDashJump()
    {
        
        if(StartDJTimer)
        {
            DashJumpTimer-=Time.deltaTime;
            if(DashJumpTimer <= 0.0f)
            {
                StartDJTimer=false;
                DashJumpTimer=1.5f;
                jumpForce=16.0f;
            }
            else
            {
                if(Input.GetButtonDown("Dash"))
                {
                    jumpForce=25.0f;
                }
                else
                {
                
                }
            }
            
        }
        
    }
    ///////////////////////////////////////////////
    
                           
}
