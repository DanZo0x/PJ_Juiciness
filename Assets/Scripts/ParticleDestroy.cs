using UnityEngine;

public class ParticleDestroy : MonoBehaviour
{
    [SerializeField]
    private float particleDestroyTime = 2.0f;

    private float timer = 0.0f;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= particleDestroyTime)
        {
            Destroy(gameObject);
        }
    }
}
