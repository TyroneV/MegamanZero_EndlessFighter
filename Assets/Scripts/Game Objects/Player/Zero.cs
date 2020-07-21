using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Zero : Reploid
{
    public static Zero current;

	[SerializeField] private float defaultHealth = 200f;
	[SerializeField] private float defaultInvulnerableTime = 2f;
	[SerializeField] private float defaultDamage = 15f;

	private bool canAttack = true;
	private bool canAttackAir = true;
	private float comboReset;
	private bool spinSlashDealingDamage = false;

	public bool spinSlash;
	public int comboCounter = 0;

    private void Awake()
    {
		current = this;
    }
    protected override void Start()
    {
        base.Start();
		MaxHealth = defaultHealth;
		CurrentHealth = defaultHealth;
		InvulnerableTime = defaultInvulnerableTime;
		Damage = defaultDamage;
	}
    // Update is called once per frame
    void Update()
    {
        CheckHealth();
		if (comboReset > 0)
		{
			comboReset -= Time.deltaTime;
		}
		else
		{
			comboCounter = 0;
			comboReset = 0;
		}
	}

	//Attack Controller
	public void AttackHandler(bool grounded,bool controllable,AudioSource audioSource,AudioClip spinSlashSound)
	{
		if (Input.GetButtonDown("Saber") && controllable && rigidbody2.velocity.y != 0 && canAttackAir)
		{
			animator.SetTrigger("Start Spin");
			StartCoroutine(SpinSlashDealDamage());
			audioSource.PlayOneShot(spinSlashSound);
			canAttackAir = false;
			spinSlash = true;
		}
		else if (controllable && grounded)
		{
			canAttackAir = true;
			spinSlash = false;
		}
		if (Input.GetButton("Saber") && controllable && rigidbody2.velocity.y == 0 && canAttack)
		{
			StartCoroutine(SlashCombo(audioSource));
		}

	}
	IEnumerator SpinSlashDealDamage()
    {
		if (!spinSlashDealingDamage)
		{
			spinSlashDealingDamage = true;
			while (rigidbody2.velocity.y != 0)
			{
				MeleeDamageTarget(transform.position, AttackRange * 1.4f, Mathf.Abs(rigidbody2.velocity.y) * 0.35f);
				yield return new WaitForSeconds(0.05f);
			}
			yield return new WaitForSeconds(0.05f);
			spinSlashDealingDamage = false;
		}
	}
	//Handles the 3 hit combo system
	public IEnumerator SlashCombo(AudioSource audioSource)
	{
		if (canAttack)
		{
			float delay = 0.3f;
			var holder = RunSpeed;
			RunSpeed = 0;
			comboReset = 0.7f;
			comboCounter++;
			canAttack = false;
			animator.SetTrigger($"Attack {comboCounter}");
			var comboSound = Resources.Load<AudioClip>($"Sounds/Sound Effects/Zero/Omega Zero - sabre slash {comboCounter}");
			audioSource.PlayOneShot(comboSound);
			yield return new WaitForSeconds(delay / 2);
			MeleeDamageTarget(attackPosition.position,AttackRange,comboCounter);
			yield return new WaitForSeconds(delay / 2);
			var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
			var length = clipInfo[0].clip.length;
			yield return new WaitForSeconds(length - delay);
			canAttack = true;
			RunSpeed = holder;

			if (comboCounter >= 3)
			{
				comboCounter = 0;
				comboReset = 0;
			}
		}
	}
}
