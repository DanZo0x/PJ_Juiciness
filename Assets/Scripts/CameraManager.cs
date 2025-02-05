using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] private float duration = 0.5f; // Duration of the shake
    [SerializeField] private float strength = 1f; // Strength of the shake
    [SerializeField] private int vibrato = 10; // Vibrato (frequency) of the shake
    [SerializeField] private float randomness = 20f; // Randomness of the shake

    private Vector3 originalPos;

    private void Awake()
    {
        Instance = this;
        originalPos = Camera.main.transform.position;
    }

    public void ShakeCamera()
    {
        Camera.main.transform.DOShakePosition(duration, strength, vibrato, randomness).OnComplete(ResetPos);
    }

    private void ResetPos()
    {
        Camera.main.transform.position = originalPos;
    }
}