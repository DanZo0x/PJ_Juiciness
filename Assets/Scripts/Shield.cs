using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private int shieldHealth = 3;

    [SerializeField] private string collideWithTag = "Untagged";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag(collideWithTag))
        {
            return;
        }

        //try to cast the object to Bullet
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();
        bullet.DecreaseBulletHealth();

        shieldHealth--;

        if (shieldHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
