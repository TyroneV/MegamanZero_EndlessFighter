using System.Collections;
using UnityEngine;

public class DashEffects : MonoBehaviour
{
    [SerializeField] private GameObject dashParticles;
    [SerializeField] private GameObject smokeEffect;

    private Animator smokeEffectAnimator;
    private ParticleSystem thisParticleSystem;

    // Start is called before the first frame update
    void Start()
    {
        thisParticleSystem = dashParticles.GetComponent<ParticleSystem>();
        smokeEffectAnimator = smokeEffect.GetComponent<Animator>();
    }
    public void PlayEffects(Transform target,bool facingRight)
    {
        Vector3 targetPosition = target.transform.position;
        smokeEffect.transform.position = targetPosition;
        SpriteRenderer spriteRenderer = smokeEffect.GetComponent<SpriteRenderer>();

        if (facingRight)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }

        smokeEffectAnimator.SetTrigger("Play Dash");
        thisParticleSystem.Play();
        StartCoroutine(DisableSmokeTimer(0.1f));
    }
    public void DisableSmoke(bool grounded)
    {
        if (!grounded)
        {
            thisParticleSystem.Stop();
        }
    }
    IEnumerator DisableSmokeTimer(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        thisParticleSystem.Stop();
    }
}
