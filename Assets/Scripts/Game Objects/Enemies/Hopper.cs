using System.Collections;
using UnityEngine;

public class Hopper : Enemy
{
    private float jumpForce = 150;

    public override void CheckFront()
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
            if (hit.distance <= minimumRange && (hit.collider.tag.Equals("Player")))
            {
                if (hit.collider.GetComponent<Reploid>().Alive)
                {
                    canWalk = false;
                    AttackPlayer(2);
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

    public override float MoveDirection()
    {
        if (Mathf.Abs(rigidbody2.velocity.y) == 0)
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
            return 0f;
        }
    }

    public override void AnimationHandler()
    {
        base.AnimationHandler();
        animator.SetFloat("VelocityY", rigidbody2.velocity.y);
    }

    public override IEnumerator Attack(float attackCooldown)
    {
        attacking = true;
        if (facingLeft) rigidbody2.AddForce(new Vector2(-jumpForce, jumpForce));
        else rigidbody2.AddForce(new Vector2(jumpForce, jumpForce));
        yield return new WaitForSeconds(0.1f);
        if (Alive) animator.SetTrigger("Attack");
        while (Mathf.Abs(rigidbody2.velocity.y) > 0.5f)
        {
            if (Alive) MeleeDamageTarget(attackPosition.position, AttackRange, 1);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(attackCooldown);
        attacking = false;
    }
    public override void Initialize(float runSpeed,float maxHealth,int points,float damage)
    {
        RunSpeed = runSpeed;
        minimumRange = 1f;
        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
        Points = points;
        Damage = damage;
    }
    public override void Initialize()
    {
        RunSpeed = 3;
        minimumRange = 1f;
        MaxHealth = 30;
        CurrentHealth = MaxHealth;
        Points = 10;
        Damage = 15;
    }
}
