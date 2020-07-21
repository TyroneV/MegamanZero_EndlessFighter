using System.Collections;
using UnityEngine;

public abstract class Enemy : Reploid
{
    [SerializeField] private readonly float despawnTime = 10f;
    [SerializeField] private float maxFollowDistance = 30f;
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;  // How much to smooth out the movement

    private Vector3 velocity = Vector3.zero;
    private float currentDespawnTime = 10;

    public bool canWalk = true;
    public bool attacking = false;
    public float minimumRange = 0.3f;
    public float distanceFromPlayer;
    public bool facingLeft;

    public Transform MainTarget { get; set; }

    protected override void Start()
    {
        base.Start();
        if (MainTarget == null)
        {
            if (Zero.current != null)
            {
                MainTarget = Zero.current.gameObject.transform;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        CheckHealth();
        LookForTarget();
        AnimationHandler();
        Despawn();
    }

    void LookForTarget()
    {
        if (Alive)
        {
            if (MainTarget != null)
            {
                distanceFromPlayer = Vector3.Distance(MainTarget.position, transform.position);
                if (distanceFromPlayer <= maxFollowDistance) FollowTarget();
            }
        }
    }

    void FollowTarget()
    {
        CheckFront();
        if (distanceFromPlayer > minimumRange && Mathf.Abs(rigidbody2.velocity.y) < 1 && canWalk)
        {
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(RunSpeed * MoveDirection(), rigidbody2.velocity.y);
            // And then smoothing it out and applying it to the character
            rigidbody2.velocity = Vector3.SmoothDamp(rigidbody2.velocity, targetVelocity, ref velocity, movementSmoothing);
        }
    }

    public virtual void CheckFront()
    {
        RaycastHit2D hit;
        if (!facingLeft)
        {
            hit = Physics2D.Raycast(attackPosition.position, Vector2.right);
        }
        else
        {
            hit = Physics2D.Raycast(attackPosition.position, Vector2.left); 
        }

        if (hit.collider != null)
        {
            if (hit.distance <= minimumRange &&( hit.collider.tag.Equals("Enemy") || hit.collider.tag.Equals("Player")))
            {
                if (hit.collider.GetComponent<Reploid>().Alive) 
                {
                    canWalk = false;
                    if (hit.collider.tag.Equals("Player"))
                        {
                            AttackPlayer(2);
                        }
                }
                else
                {
                    canWalk = true;
                }
            }
            else
            {
                canWalk = true;
            }
        }
        else
        {
            canWalk = true;
        }
    }

    public virtual float MoveDirection()
    {
        if (!attacking)
        {
            if (MainTarget.position.x > transform.position.x)
            {
                if (facingLeft)
                {
                    Flip();
                }
                return 0.4f;
            }
            else
            {
                if (!facingLeft)
                {
                    Flip();
                }
                return -0.4f;
            }
        }
        else
        {
            return 0;
        }

    }

    public virtual void AnimationHandler()
    {
        animator.SetFloat("Speed", Mathf.Abs(rigidbody2.velocity.x));
    }
    public void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingLeft = !facingLeft;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void Despawn()
    {
        
        if(distanceFromPlayer < 30)
        {
            currentDespawnTime = despawnTime;
        }
        else
        {
            currentDespawnTime -= Time.deltaTime;
        }

        if(currentDespawnTime <= 0)
        {
            currentDespawnTime = despawnTime;
            MainTarget = null;
            CurrentHealth = 0;
        }
    }
    public void AttackPlayer(float attackCooldown)
    {
        if (!attacking && Alive)
        {
            StartCoroutine(Attack(attackCooldown));
        }
    }

    public virtual IEnumerator Attack(float attackCooldown)
    {
        attacking = true;
        yield return new WaitForSeconds(0.3f);
        if (Alive) animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.4f);
        if (Alive) MeleeDamageTarget(attackPosition.position,AttackRange,1);
        yield return new WaitForSeconds(attackCooldown);
        attacking = false;
    }

    public abstract void Initialize(float runSpeed, float maxHealth, int points, float damage);
    public abstract void Initialize();
}
