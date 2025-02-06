using System;
using UnityEngine;
using System.Collections.Generic;

public class Shield : MonoBehaviour
{
    [SerializeField] private string collideWithTag = "Untagged";

    private SpriteRenderer spriteRenderer;

    [SerializeField] private List<Sprite> shieldSprites = new List<Sprite>();

    private int shieldIndex = 0;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = shieldSprites[shieldIndex];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag(collideWithTag))
        {
            return;
        }

        //try to cast the object to Bullet
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();
        bullet.DecreaseBulletHealth();

        shieldIndex++;

        if (shieldIndex >= shieldSprites.Count)
        {
            Destroy(gameObject);
        }
        else
        {
            spriteRenderer.sprite = shieldSprites[shieldIndex];
        }
    }
}
