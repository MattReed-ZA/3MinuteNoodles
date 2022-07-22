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


    void Start()
    {
        rb=GetComponent<Rigidbody2D>();
        amountOfJumpsleft=amountOfJumps;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
        anim=GetComponent<Animator>();
        respawnPoint=rb.position;
        //Debug.Log(rb.position);
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

        //TO MOVE RESPAWN POINT///
        respawnDetector.transform.position = new Vector2(rb.position.x,respawnDetector.transform.position.y);
        //////////////////////////
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Respawn")
        {
            rb.position=respawnPoint;
        }
        else if(collision.tag=="Checkpoint")
        {
            respawnPoint=rb.position;
        }
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking",isWalking);
        anim.SetBool("isGrounded",isGrounded);      
        anim.SetFloat("yVelocity",rb.velocity.y);
        anim.SetBool("isWallSliding",isWallSliding);
        anim.SetBool("isDashing",isDashing);
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


        if(rb.velocity.x>=0.5 || rb.velocity.x<=-0.5)
        {
            isWalking=true;
        }
        else
        {
            isWalking=false;

        }

    }

    private void Flip()
    {
        if(!isWallSliding && canFlip)
        {
            facingDirection*=-1;
            isFacingRight=!isFacingRight;
            transform.Rotate(0.0f,180.0f,0.0f);
        }
    }
   

    private void CheckInput()
    {
        movementInputDirection=Input.GetAxisRaw("Horizontal");
        
        if(Input.GetButtonDown("Jump"))
        {
            if(isGrounded || (amountOfJumpsleft>0 && isTouchingWall))
            {
                SoundManagerScript.PlaySound("Jump");
                NormalJump();
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
        if(canNormalJump)
        {
            CreateDust();
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
            amountOfJumpsleft=amountOfJumps;
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
            rb.velocity=new Vector2(movementSpeed*movementInputDirection, rb.velocity.y);
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
        // if(ledgeDetected && !canClimbLedge)
        // {
        //     canClimbLedge=true;
        
        //     if(isFacingRight)
        //     {
        //         ledgePos1=new Vector2(Mathf.Floor(ledgePosBottom.x + wallCheckDistance)-ledgeClimbXOff1,Mathf.Floor(ledgePosBottom.y)+ledgeClimbYOff1);
        //         ledgePos2=new Vector2(Mathf.Floor(ledgePosBottom.x + wallCheckDistance)+ledgeClimbXOff2,Mathf.Floor(ledgePosBottom.y)+ledgeClimbYOff2);
        //     }
        //     else
        //     {
        //         ledgePos1=new Vector2(Mathf.Ceil(ledgePosBottom.x - wallCheckDistance)+ledgeClimbXOff1,Mathf.Floor(ledgePosBottom.y)+ledgeClimbYOff1);
        //         ledgePos2=new Vector2(Mathf.Ceil(ledgePosBottom.x - wallCheckDistance)-ledgeClimbXOff2,Mathf.Floor(ledgePosBottom.y)+ledgeClimbYOff2);
        //     }

        //     canMove=false;
        //     canFlip=false;
        //     anim.SetBool("canClimbLedge",canClimbLedge);
        // }
        // if(canClimbLedge)
        // {
        //     transform.position=ledgePos1;
        // }
        
    }

    public void FinishLedgeClimb()
    {
        // canClimbLedge=false;
        // transform.position=ledgePos2;
        // canMove=true;
        // canFlip=true;
        // ledgeDetected=false;
        // anim.SetBool("canClimbLedge",canClimbLedge);
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall=Physics2D.Raycast(wallCheck.position,transform.right,wallCheckDistance,whatIsGround);

        // isTouchingLedge=Physics2D.Raycast(ledgeCheck.position,transform.right,wallCheckDistance,whatIsGround);

        // if(isTouchingWall && !isTouchingLedge && !ledgeDetected)
        // {
        //     ledgeDetected=true;
        //     ledgePosBottom=wallCheck.position;
        // }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position,new Vector3(wallCheck.position.x + wallCheckDistance,wallCheck.position.y,wallCheck.position.z));
    }

    
    
                           
}
