using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [SerializeField] private float deadzone = 0.3f;
    [SerializeField] private float speed = 1f;

    [SerializeField] private Bullet bulletPrefab = null;
    [SerializeField] private Bullet notJuicyBulletPrefab = null;

    [SerializeField] private Transform shootAt = null;
    [SerializeField] private float shootCooldown = 1f;
    [SerializeField] private string collideWithTag = "Untagged";

    [SerializeField] private GameObject kawaiParticles = null;
    [SerializeField] private GameObject goreParticles = null;

    [SerializeField] private AnimationCurve velocityCurve = AnimationCurve.Linear(0, 1, 1, 8); // Curve for bullet velocity

    private DissolveImage gameOverImage;

    private float lastShootTimestamp = Mathf.NegativeInfinity;
    private float shootButtonHoldTime = 0f;
    private float maxShootButtonHoldTime = 1f;
    private bool canPlayEffect = true;

    public MMF_Player ShootFeedback;

    public UnityEvent OnShoot;

    private bool bIsDead = false;

    private void Update()
    {
        if (bIsDead)
        {
            return;
        }

        UpdateMovement();
        UpdateActions();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            Die();
        }
#endif
    }

    private void UpdateMovement()
    {
        float move = Input.GetAxis("Horizontal");
        if (Mathf.Abs(move) < deadzone) { return; }

        move = Mathf.Sign(move);
        float delta = move * speed * Time.deltaTime;
        transform.position = GameManager.Instance.KeepInBounds(transform.position + Vector3.right * delta);
    }

    private void UpdateActions()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            shootButtonHoldTime += Time.deltaTime;
            if(canPlayEffect && Juice.IsActive()) 
            {
                ShootFeedback.PlayFeedbacks();
                canPlayEffect = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && Time.time > lastShootTimestamp + shootCooldown)
        {
            Shoot();
            shootButtonHoldTime = 0f;
            ShootFeedback.StopFeedbacks();
            canPlayEffect = true;
        }
    }

    private void Shoot()
    {
        float holdRatio = Mathf.Clamp01(shootButtonHoldTime / shootCooldown);
        float bulletVelocity = velocityCurve.Evaluate(holdRatio);

        Bullet targetPrefab = Juice.IsActive() ? bulletPrefab : notJuicyBulletPrefab;
        Bullet bullet = Instantiate(targetPrefab, shootAt.position, Quaternion.identity);

        bullet.SetVelocity(bulletVelocity, shootButtonHoldTime >= maxShootButtonHoldTime);
        lastShootTimestamp = Time.time;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != collideWithTag || bIsDead)
        {
            return;
        }

        Die();
    }

    public void Die()
    {
        GetComponent<Collider2D>().enabled = false;

        bIsDead = true;
        GameManager.Instance.PlayGameOver();
    }

    public void DieLatent()
    {
        if (Juice.IsActive())
        {
            Instantiate(kawaiParticles, transform.position, Quaternion.identity);
            Instantiate(goreParticles, transform.position, Quaternion.identity);
        }

        GetComponent<SpriteRenderer>().enabled = false;
    } 
}
