using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class CustomTrailRenderer : MonoBehaviour
{
    [SerializeField] private int clonesPerSecond = 100;
    [SerializeField] private float fadeSpeed = 4f;
    [SerializeField] private Material shader;
    [SerializeField] private Color colorPerSecond;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D targetRigidbody;
    private List<SpriteRenderer> clones;

    // Start is called before the first frame update
    void Start()
    {
        targetRigidbody = GetComponent<Rigidbody2D>();
        clones = new List<SpriteRenderer>();
        StartCoroutine(EnableTrail());
    }

    void Update()
    {
        for (int i = 0; i < clones.Count; i++)
        {
            clones[i].color -= colorPerSecond * Time.deltaTime * fadeSpeed;
            if (clones[i].color.a <= 0f)
            {
                Destroy(clones[i].gameObject);
                clones.RemoveAt(i);
                i--;
            }
        }
    }

    public IEnumerator EnableTrail()
    {
        while(true)
        {
            if (targetRigidbody.velocity != Vector2.zero && !CharacterController2D.current.canDash)
            {
                var clone = new GameObject("trailClone");
                clone.transform.position = transform.position;
                clone.transform.localScale = transform.localScale;
                var cloneRend = clone.AddComponent<SpriteRenderer>();
                cloneRend.color = colorPerSecond;
                cloneRend.material = shader;
                cloneRend.sprite = spriteRenderer.sprite;
                cloneRend.sortingOrder = spriteRenderer.sortingOrder - 1;
                clones.Add(cloneRend);
            }
            yield return new WaitForSeconds(1f / clonesPerSecond);
        }
    }
}