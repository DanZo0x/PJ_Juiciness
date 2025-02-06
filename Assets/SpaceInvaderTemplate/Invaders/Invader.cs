using DG.Tweening;
using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Invader : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab = null;
    [SerializeField] private Bullet notJuicyBulletPrefab = null;

    [SerializeField] private GameObject kawaiParticles = null;
    [SerializeField] private GameObject goreParticles = null;
    [SerializeField] private BloodSplash bloodSplash = null;
    
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float bloodSplashPercent = 0.35f;

    [SerializeField] private Transform shootAt = null;
    [SerializeField] private string collideWithTag = "Player";

    [SerializeField] private GameObject cryZone;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite crySprite;
    [SerializeField] private float cryTime = 1.5f;

    [SerializeField] private AudioClip shootSFX;
    [SerializeField] private List<AudioClip> killedSFX = new List<AudioClip>();
    [SerializeField] private List<AudioClip> crySFX = new List<AudioClip>();

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
        if (GameManager.Instance.IsGameOver())
        {
            return;
        }

        Bullet targetPrefab = Juice.IsActive() ? bulletPrefab : notJuicyBulletPrefab;
        Bullet bullet = Instantiate(targetPrefab, shootAt.position, Quaternion.identity);
        bullet.SetVelocity(-2f, false);
        
        AudioManager.Instance.PlaySFX(shootSFX);

        if (Juice.IsActive())
        {
            //ShootFeedback.PlayFeedbacks();
            transform.DOScaleY(0.15f, 0.1f).SetLoops(2, LoopType.Yoyo).OnComplete(() => transform.localScale = new Vector3(0.1f, 0.1f, 0.1f));
        }
    }

    private IEnumerator DestroyInvaderCoroutine()
    {
        if (Juice.IsActive())
        {
            CameraManager.Instance.ShakeCamera();

            //DestroyFeedback.PlayFeedbacks();
            ToggleCryZone(true);
            GameManager.Instance.EnemyKilled(this.gameObject);
        }

        int SFXIndex = Random.Range(0, killedSFX.Count - 1);
        AudioManager.Instance.PlaySFX(killedSFX[SFXIndex]);

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        if (Juice.IsActive())
        {
            Instantiate(kawaiParticles, transform.position, Quaternion.identity);
            Instantiate(goreParticles, transform.position, Quaternion.identity);

            float percent = Random.value;

            if(bloodSplashPercent >= percent)
            {
                Instantiate(bloodSplash, transform.position, Quaternion.identity);
            }
        }

        yield return new WaitForSeconds(0.5f);

        SFXIndex = Random.Range(0, crySFX.Count - 1);
        AudioManager.Instance.PlaySFX(crySFX[SFXIndex]);

        yield return new WaitForSeconds(2.0f);
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