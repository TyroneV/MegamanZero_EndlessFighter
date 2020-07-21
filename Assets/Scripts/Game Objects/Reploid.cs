using System.Collections;
using UnityEngine;

public abstract class Reploid : MonoBehaviour
{
    private bool deathAnimation = false;
    private float currentHealth = 100f;
    private float maxHealth = 100f;
    private bool addedScore = false;

    public Transform attackPosition;
    public LayerMask whatIsEnemies;
    public Animator animator;
    public Rigidbody2D rigidbody2;
    public float RunSpeed = 10f;

    public int Points { set; get; } = 0;
    public bool Alive { private set; get; } = true;
    public float AttackRange { set; get; } = 0.3f;
    public bool Hurt { set; get; } = false;
    public float InvulnerableTime { set; get; } = 0.2f;
    public bool Invulnerable { set; get; } = false;
    public float Damage { set; get; } = 10f;
    public float MaxHealth
    {
        get { return maxHealth; }
        set
        {
            if (currentHealth > value)
            {
                currentHealth = value;
                maxHealth = value;
            }
            else
            {
                maxHealth = value;
            }
        }
    }
    public float CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            if (value >= MaxHealth)
            {
                currentHealth = MaxHealth;
                Alive = true;
            }
            else if (value <= 0)
            {
                currentHealth = 0;
                Alive = false;
            }
            else
            {
                currentHealth = value;
            }
        }
    }

    protected virtual void Start()
    {
        if (GetComponent<Animator>())
        {
            animator = GetComponent<Animator>();
        }
        else if (GetComponentInChildren<Animator>())
        {
            animator = GetComponentInChildren<Animator>();
        }
        if (transform.childCount > 0)
        {
            for (int x = 0; x < transform.childCount; x++)
            {
                if (transform.GetChild(x).name.Equals("Attack Position"))
                {
                    attackPosition = transform.GetChild(x);
                }
            }
        }
        if (whatIsEnemies.value == 0)
        {
            whatIsEnemies = Zero.current.gameObject.layer;
        }
        if (rigidbody2 == null)
        {
            rigidbody2 = GetComponent<Rigidbody2D>();
        }
    }

    public void CheckHealth()
    {
        //Plays checks if the reploid died and prevents the animation to be played multiple times
        if (currentHealth == 0 && !deathAnimation)
        {
            deathAnimation = true;
            StartCoroutine(PlayDeath(animator));
        }
        animator.SetBool("Alive", Alive);
    }
    
    public void TakeDamage(float damage)
    {
        if (!Invulnerable)
        {
            StartCoroutine(SetInvulnerable(damage));
        }
    }
    public void MeleeDamageTarget(Vector2 attackPoint, float range ,float multiplier)
    {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPoint, range, whatIsEnemies);

        foreach (var enemy in enemiesToDamage)
        {
            if (enemy != null)
            {
                if (enemy.GetComponent<Reploid>())
                {
                    enemy.GetComponent<Reploid>().TakeDamage(Damage * multiplier);
                }
            }
        }
    }


    IEnumerator SetInvulnerable(float damage)
    {
        CurrentHealth -= damage;
        Hurt = true;
        if (!Invulnerable)
        {
            if (currentHealth <= 0 && !addedScore)
            {
                addedScore = true;
                EventHandler.current.score += Points;
            }
            Invulnerable = true;
            yield return new WaitForSeconds(InvulnerableTime);
            Invulnerable = false;
        }
        else
        {
            yield return 0;
        }
    }

    IEnumerator PlayDeath(Animator animator)
    {
        float delay = 0.3f;
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(delay);
        var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        var length = clipInfo[0].clip.length;
        yield return new WaitForSeconds(length - delay);
        gameObject.SetActive(false);
    }
}
