using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invader : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab = null;
    [SerializeField] private Transform shootAt = null;
    [SerializeField] private string collideWithTag = "Player";
    [SerializeField] private ParticleSystem particles;

    internal Action<Invader> onDestroy;

    public Vector2Int GridIndex { get; private set; }

    public void Initialize(Vector2Int gridIndex)
    {
        this.GridIndex = gridIndex;
    }

    public void OnDestroy()
    {
        onDestroy?.Invoke(this);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag != collideWithTag) { return; }

        //try to cast the object to Bullet
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();

        bullet.DecreaseBulletHealth();
        StartCoroutine(DestroyInvaderCoroutine());
    }

    public void Shoot()
    {
        Bullet bullet = Instantiate(bulletPrefab, shootAt.position, Quaternion.identity);
        bullet.SetVelocity(-2f, false);
    }

    private IEnumerator DestroyInvaderCoroutine()
    {
        CameraManager.Instance.ShakeCamera();
        particles.Play();
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject);
    }
}
