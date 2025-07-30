using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyProjectile : EnemyDamage
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float resetTime = 2f;

    private Animator anim;
    private BoxCollider2D coll;
    private bool hit;
    private float lifetime;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();

        if (coll == null)
            Debug.LogError("EnemyProjectile: BoxCollider2D not found!");

        if (anim == null)
            Debug.LogWarning("EnemyProjectile: Animator not found (this is optional).");
    }

    public void ActivateProjectile()
    {
        // Recheck in case Awake wasn't triggered before this method
        if (coll == null) coll = GetComponent<BoxCollider2D>();
        if (anim == null) anim = GetComponent<Animator>();

        hit = false;
        lifetime = 0f;
        gameObject.SetActive(true);
        coll.enabled = true;
    }

    private void Update()
    {
        if (hit) return;

        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(movementSpeed, 0f, 0f);

        lifetime += Time.deltaTime;
        if (lifetime > resetTime)
            Deactivate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        base.OnTriggerEnter2D(collision); // Execute logic from parent script first

        coll.enabled = false;

        if (anim != null)
            anim.SetTrigger("explode");
        else
            Deactivate(); // When there's no animation, deactivate immediately
    }

    // Called either when lifetime expires or after hit animation
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
