using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(DashEffects))]
[RequireComponent(typeof(Zero))]
public class PlayerController : MonoBehaviour
{
	public static PlayerController current;

	private Rigidbody2D thisRigidbody2D;
	private Animator animator;
	private DashEffects dashEffects;
	private AudioSource audioSource;
	private AudioClip spinSlashSound;
	private AudioClip dashSound;
	private AudioClip hurtSound;
	private float horizontalMove = 0f;

	private bool jump = false;// Handles player's jump
	private bool dash = false;// Handles player's dash
	private delegate AudioClip GetClip(string text);
	private bool controllable = false;// Prevents player from moving
    private void Awake()
    {
		current = this;
    }
    void Start()
	{
		try
		{
			SetupAudioClips();
		}
		catch (Exception e)
		{
			Debug.LogError(e);
		}
		audioSource = GetComponentInChildren<AudioSource>();
		thisRigidbody2D = GetComponent<Rigidbody2D>();
		animator = GetComponentInChildren<Animator>();
		dashEffects = GetComponent<DashEffects>();

		Zero.current.InvulnerableTime = 1;

		StartCoroutine(Spawn());
	}
	// Update is called once per frame
	void Update()
	{
		if (controllable && Zero.current.Alive)
		{
			horizontalMove = Input.GetAxisRaw("Horizontal") * Zero.current.RunSpeed;
			JumpHandler();
			Zero.current.AttackHandler(CharacterController2D.current.grounded,controllable,audioSource,spinSlashSound);
			DashHandler();
		}
		else if (!Zero.current.Alive && !audioSource.isPlaying) 
		{
			thisRigidbody2D.velocity = Vector2.zero;
			thisRigidbody2D.gravityScale = 0;
			audioSource.PlayOneShot(hurtSound);
			audioSource.volume -= 30 * Time.deltaTime;
		}
	}

	void FixedUpdate()
	{
		if (controllable && Zero.current.Alive) 
		{
			// Moves the character
			CharacterController2D.current.Move(horizontalMove * Time.fixedDeltaTime, jump, ref dash);
			jump = false;

			// Animates the character
			AnimationHandler(); 
		}

	}
	//Sets up most of the audio clips
	void SetupAudioClips()
    {
		GetClip getClip = Resources.Load<AudioClip>;
		string mainAudioPath = ("Sounds/Sound Effects/");
		dashSound = getClip($"{mainAudioPath}Zero/Omega Zero - dash");
		spinSlashSound = getClip($"{mainAudioPath}Zero/Omega Zero - charged z sabre,aerial spin attack,z sabre wave");
		hurtSound = getClip($"{mainAudioPath}Zero/Omega Zero - gethit");
	}
	//Handles most of the animations
	void AnimationHandler()
	{
		animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
		animator.SetFloat("Vertical Velocity", thisRigidbody2D.velocity.y);
		animator.SetBool("Grounded", CharacterController2D.current.grounded);
		animator.SetBool("Dashing", !CharacterController2D.current.canDash);
		animator.SetBool("Spin Slash", Zero.current.spinSlash);
		animator.SetBool("Invulnerable", Zero.current.Invulnerable);
		if (Zero.current.Hurt && Zero.current.Alive)
		{
			audioSource.PlayOneShot(hurtSound);
			animator.SetTrigger("Hurt");
			Zero.current.Hurt = false;
		}
	}
	//Jump Controller
	void JumpHandler()
	{
		if (Input.GetButtonDown("Jump") && controllable && CharacterController2D.current.grounded)
		{
			jump = true;
			animator.SetTrigger("Jump");
		}
	}
	//Dash Controller
	void DashHandler()
	{
		if (Input.GetButtonDown("Dash") && !dash
			&& CharacterController2D.current.grounded
			&& CharacterController2D.current.canDash && controllable)
		{
			Zero.current.comboCounter = 0;
			dash = true;
			animator.SetTrigger("Dash");
			audioSource.PlayOneShot(dashSound);
			dashEffects.PlayEffects(transform, CharacterController2D.current.facingRight);
		}
		dashEffects.DisableSmoke(CharacterController2D.current.grounded);
	}

	//Plays spawn animation at the beginning
	IEnumerator Spawn()
	{
		GetComponentInChildren<Animator>().SetTrigger("Spawn");
		yield return new WaitForSeconds(2f);
		controllable = true;
	}

}