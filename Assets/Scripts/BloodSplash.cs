using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BloodSplash : MonoBehaviour
{

    [SerializeField] private Transform parentFX;
    [SerializeField] private Transform childFX;

    [SerializeField]
    private float minScale = 1.0f;

    [SerializeField]
    private float maxScale = 5.0f;

    private void Awake()
    {
        float targetRotation = Random.Range(0.0f, 360.0f);
        transform.rotation = Quaternion.Euler(targetRotation, 90.0f, 90.0f);

        float targetScale = Random.Range(minScale, maxScale);
        parentFX.localScale = new Vector3(targetScale, targetScale, targetScale);
        childFX.localScale = new Vector3(targetScale, targetScale, targetScale);
    }
}
