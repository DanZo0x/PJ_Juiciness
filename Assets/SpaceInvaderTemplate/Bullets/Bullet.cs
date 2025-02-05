using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour
{
    [SerializeField] Vector3 startVelocity;
    [SerializeField] Vector2 startScale = new Vector2(0.2f, 0.2f); // Initial scale
    [SerializeField] float scaleDuration = 0.2f; // Duration of the scaling effect

    private Rigidbody2D rb;
    private int bulletHealth = 1;
    public int BulletHealth { get => bulletHealth; set => bulletHealth = value; }

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(DestroyBulletCoroutine());

        if (Juice.IsActive())
        {
            return;
        }

        TrailRenderer trailRenderer = GetComponent<TrailRenderer>();
        if (trailRenderer == null)
        {
            return;
        }

        trailRenderer.emitting = false;
    }

    public void SetVelocity(float velocity, bool isAtMaxVelocity)
    {
        if (!Juice.IsActive())
        {
            velocity = Mathf.Clamp(velocity, -9.0f, 4.0f);
            isAtMaxVelocity = false;
        }
        
        startVelocity = new Vector3(0, velocity, 0);
        rb.velocity = startVelocity;
        
        if (isAtMaxVelocity)
        {
            bulletHealth = 2; 
        }

        // Adjust scale based on velocity
        float scaleX = Mathf.Lerp(0.2f, 0.1f, velocity / 5f); // Thinner with higher velocity
        float scaleY = Mathf.Lerp(0.2f, 0.5f, velocity / 5f); // Longer with higher velocity
        transform.localScale = startScale;
        transform.DOScale(new Vector2(scaleX, scaleY), scaleDuration).SetEase(Ease.InOutBack);
    }

    private IEnumerator DestroyBulletCoroutine()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    public void DecreaseBulletHealth()
    {
        bulletHealth--;

        if (bulletHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
