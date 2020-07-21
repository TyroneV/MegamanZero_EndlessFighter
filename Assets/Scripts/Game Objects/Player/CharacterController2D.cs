using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	public static CharacterController2D current;

	const float groundedRadius = .2f; // Radius of the overlap circle to determine if grounded

	[SerializeField] private float jumpForce = 400f;                          // Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool airControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask whatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform groundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private float dashSpeed = 2;
	[SerializeField] private float dashDuration;

	private Rigidbody2D thisRigidbody2D;
	private Vector3 velocity = Vector3.zero;
	private float dashMultiplier = 1;

	public bool grounded;            // Whether or not the player is grounded.
	public bool facingRight = true;  // For determining which way the player is currently facing.
	public bool canDash = true;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }


	private void Awake()
	{
		current = this;
		thisRigidbody2D = GetComponent<Rigidbody2D>();
		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = grounded;
		grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}


	public void Move(float move, bool jump,ref bool dash)
	{
		//only control the player if grounded or airControl is turned on
		if (grounded || airControl)
		{
			float momentum;
			//Forces the player to move in a direction during a dash.
			if (facingRight && !canDash)
			{
				momentum = 0.4f;
			}
			else if (!facingRight && !canDash)
			{
				momentum = -0.4f;
			}
			else
			{
				momentum = move;
			}

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !facingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && facingRight)
            {
                // ... flip the player.
                Flip();
            }
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(momentum * 10f * dashMultiplier, thisRigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			thisRigidbody2D.velocity = Vector3.SmoothDamp(thisRigidbody2D.velocity, targetVelocity, ref velocity, movementSmoothing);
		}
		// If the player should jump...
		if (grounded && jump)
		{
			// Add a vertical force to the player.
			grounded = false;
			thisRigidbody2D.AddForce(new Vector2(0f, jumpForce));
		}
		// If the player should dash...
        if (grounded && canDash && dash)
        {
			StartCoroutine(Dash(dashDuration));
			dash = false;
        }
	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	IEnumerator Dash(float duration)
    {
		canDash = false;
		float holder = dashMultiplier;
		dashMultiplier = dashSpeed;
		yield return new WaitForSeconds(duration);
		while (!grounded)
		{
			yield return 0;
		}
		dashMultiplier = holder;
		canDash = true;
    }
}