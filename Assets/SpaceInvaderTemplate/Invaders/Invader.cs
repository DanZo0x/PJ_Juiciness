using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Invader : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab = null;
    [SerializeField] private Transform shootAt = null;
    [SerializeField] private string collideWithTag = "Player";

    [SerializeField] private GameObject cryZone;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite crySprite;
    [SerializeField] private float cryTime = 1.5f;

    [Header("Feedbacks")]
    public MMF_Player ShootFeedback;
    public MMF_Player DestroyFeedback;

    private SpriteRenderer _spriteRenderer;

    internal Action<Invader> onDestroy;

    public Vector2Int GridIndex { get; private set; }

    public void Initialize(Vector2Int gridIndex)
    {
        this.GridIndex = gridIndex;
    }

    public void OnDestroy()
    {
        onDestroy?.Invoke(this);
        StopAllCoroutines();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CryZone"))
        {
            if (!Juice.IsActive()) return;
            StartCoroutine(ToggleInvaderSpriteCoroutine());
        }

        if (!collision.gameObject.CompareTag(collideWithTag)) { return; }

        //try to cast the object to Bullet
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();

        bullet.DecreaseBulletHealth();
        StartCoroutine(DestroyInvaderCoroutine());
    }

    public void Shoot()
    {
        Bullet bullet = Instantiate(bulletPrefab, shootAt.position, Quaternion.identity);
        bullet.SetVelocity(-2f, false);
        ShootFeedback.PlayFeedbacks();
    }

    private IEnumerator DestroyInvaderCoroutine()
    {
        if (Juice.IsActive())
        {
            CameraManager.Instance.ShakeCamera();

            DestroyFeedback.PlayFeedbacks();
            ToggleCryZone(true);
        }

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(2.5f);
        ToggleCryZone(false);
        Destroy(gameObject);
    }

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = normalSprite;
        ToggleCryZone(false);
        
    }

    private void ToggleCryZone(bool isActive)
    {
        cryZone.SetActive(isActive);
    }

    private IEnumerator ToggleInvaderSpriteCoroutine()
    {
        _spriteRenderer.sprite = crySprite;
        
        yield return new WaitForSecondsRealtime(cryTime);
        
        _spriteRenderer.sprite = normalSprite;
    }
}