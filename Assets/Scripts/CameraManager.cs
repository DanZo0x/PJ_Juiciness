using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] private float duration = 0.5f; // Duration of the shake
    [SerializeField] private float strength = 1f; // Strength of the shake
    [SerializeField] private int vibrato = 10; // Vibrato (frequency) of the shake
    [SerializeField] private float randomness = 90f; // Randomness of the shake

    private void Awake()
    {
        Instance = this;
    }

    public void ShakeCamera()
    {
        Camera.main.transform.DOShakePosition(duration, strength, vibrato, randomness);
    }
}