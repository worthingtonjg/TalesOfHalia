using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovementRigidBody : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float damageForce = 1f;
    public LayerMask groundLayer;
    public LayerMask trampolineLayer;
    public LayerMask platformLayer;
    public GameObject DamageIndicator;
    public GameObject SpeedBoostOn;
    public GameObject SpeedBoostOff;
    public GameObject JumpBoostOn;
    public GameObject JumpBoostOff;
    public GameObject AttackOn;
    public GameObject AttackOff;

    public GameObject RestartLevelPanel;

    public GameObject bullet;
    public GameObject spawnPoint;



    public List<RuntimeAnimatorController> animatorControllers;
    public List<Collider2D> colliders;

    public int playerLevel = 1;
    
    private Rigidbody2D rb;
    private bool isGrounded;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D playerCollider;
    private bool isAlive = true;
    private float damageCooldown = 1.0f;
    private float lastDamageTime = 0.0f;

    private float bulletCooldown = .5f;
    private float lastBulletTime = 0.0f;

    private float groundDistance = 0.2f;
    private float originalMoveSpeed;
    private float origingalJumpForce;
    private bool canAttack = false;
    private float virtualInput = 0f;
    private bool jumpPressed;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();    
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();

        originalMoveSpeed = moveSpeed;
        origingalJumpForce = jumpForce;

        ToggleBoosts(false, false, false);
    }

    private void Update()
    {
        if(!isAlive) return;

        isGrounded = IsGrounded();

        Move();
        Jump();
        Attack();

        var platform = OnPlatform();
        if(platform != null)
        {
            transform.parent = platform.transform;
        }
        else
        {
            transform.parent = null;
        }

    }

    public void SetMoveInput(float value)
    {
        virtualInput = value;
    }

    public void JumpInput()
    {
        jumpPressed = true;
    }

    private void Move()
    {
        float moveInput = Input.GetAxis("Horizontal") + virtualInput;
        Vector2 moveVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = moveVelocity;

        // Flip sprite based on movement direction
        if (moveInput < 0)
            transform.rotation = Quaternion.Euler(0, 180, 0);
        else if (moveInput > 0)
            transform.rotation = Quaternion.Euler(0, 0, 0);

        // Only set idle or walk animation if the character is on the ground
        if (isGrounded || OnTrampoline() != null)
        {
            if (moveInput == 0)
            {
                animator.SetInteger("AnimationState", 0); // Idle animation
            }
            else
            {
                animator.SetInteger("AnimationState", 1); // Walk animation
            }
        }
    }

    private void Jump()
    {
        // If the player is not grounded and is moving vertically, play jump animation
        if (isGrounded)
        {
            if (Input.GetButtonDown("Jump") || jumpPressed)
            {
                jumpPressed = false;
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                animator.SetTrigger("Jump");
            }
        }

        var trampoline = OnTrampoline();
        if(trampoline != null)
        {
            if(Input.GetButtonDown("Jump") || jumpPressed)
            {
                jumpPressed = false;
                StartCoroutine(trampoline.gameObject.GetComponent<Trampoline>().Bounce());
            }
        }
    }

    private void Attack()
    {
        if(Input.GetButton("Fire1") || Input.GetButton("Fire2"))
        {
            if(!isAlive) return;
            if(!canAttack) return;
            if (lastBulletTime > 0 && Time.time - lastBulletTime < bulletCooldown) return;
            lastBulletTime = Time.time;
            StartCoroutine(FireBullet());
        }
    }

    private IEnumerator FireBullet()
    {
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(.3f);
        Instantiate(bullet, spawnPoint.transform.position, transform.rotation);
    }

    private void Die()
    {
        RestartLevelPanel.SetActive(true);
        animator.SetInteger("AnimationState", -1);
        animator.SetTrigger("Die");
        isAlive = false;
    }

    private bool IsGrounded()
    {
        // Cast a short ray downward to check if the character is on the ground
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundDistance, groundLayer);
        Debug.DrawRay(transform.position, Vector2.down * groundDistance, Color.red);
        return hit.collider != null || OnPlatform() != null;
    }

    private Collider2D OnTrampoline()
    {
        // Cast a short ray downward to check if the character is on the ground
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundDistance, trampolineLayer);
        Debug.DrawRay(transform.position, Vector2.down * groundDistance, Color.red);
        return hit.collider;
    }

    private Collider2D OnPlatform()
    {
        // Cast a short ray downward to check if the character is on the ground
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundDistance, platformLayer);
        Debug.DrawRay(transform.position, Vector2.down * groundDistance, Color.red);
        return hit.collider;
    }

    public void TakeDamage(int damage)
    {
        if(!isAlive) return;

        if (lastDamageTime > 0 && Time.time - lastDamageTime < damageCooldown) return;
        lastDamageTime = Time.time;

        print($"damage: {damage}");
        if(isGrounded)
        {
            rb.AddForce((Vector2.up + Vector2.left).normalized * damageForce, ForceMode2D.Impulse);
        }
        StartCoroutine(FlashDamageIndicator());

        --playerLevel;
        if(playerLevel <= 0) Die();
        else GrowShrinkPlayer();

        moveSpeed = originalMoveSpeed;
        jumpForce = origingalJumpForce;

        ToggleBoosts(false, false, false);

    }

    public void Pickup(PickupEnum pickupType)
    {
        if(!isAlive) return;

        print($"pickup: {pickupType}");
        switch(pickupType)
        {
            case PickupEnum.growth:
                playerLevel++;
                GrowShrinkPlayer();
                break;
            case PickupEnum.speed:
                ToggleBoosts(speed: true);
                break;
            case PickupEnum.jump:
                ToggleBoosts(jump: true);
                break;
            case PickupEnum.weapon:
                ToggleBoosts(attack: true);
                break;
        }
    }

    public void GrowShrinkPlayer()
    {
        if(!isAlive) return;

        groundDistance = 0.2f;
        if(playerLevel > 1)
        {
            groundDistance *= 2;
        }
        
        playerLevel = Mathf.Clamp(playerLevel, 0, 3);
        animator.runtimeAnimatorController = animatorControllers[playerLevel - 1];

        for (int i = 0; i < colliders.Count; i++)
        {
            colliders[i].enabled = (i == playerLevel - 1);
        }
    }

    private IEnumerator FlashDamageIndicator()
    {
        DamageIndicator.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        DamageIndicator.SetActive(false);
    }

    private void ToggleBoosts(bool? speed = null, bool? jump = null, bool? attack = null)
    {
        if(speed != null)
        {
            SpeedBoostOff.SetActive(!speed.Value);
            SpeedBoostOn.SetActive(speed.Value);
            if(speed.Value) {
                moveSpeed = originalMoveSpeed * 2;
            }
            else
            {
                moveSpeed = originalMoveSpeed;
            }
        }

        if(jump != null)
        {
            JumpBoostOff.SetActive(!jump.Value);
            JumpBoostOn.SetActive(jump.Value);

            if(jump.Value)
            {
                jumpForce = origingalJumpForce * 1.3f;
            }
            else
            {
                jumpForce = origingalJumpForce;
            }
        }

        if(attack != null)
        {
            AttackOff.SetActive(!attack.Value);
            AttackOn.SetActive(attack.Value);

            canAttack = attack.Value;
        }
    }
}
