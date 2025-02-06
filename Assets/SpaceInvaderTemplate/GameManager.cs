using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public enum DIRECTION { Right = 0, Up = 1, Left = 2, Down = 3 }

    public static GameManager Instance = null;

    [SerializeField] private Vector2 bounds;
    private Bounds Bounds => new Bounds(transform.position, new Vector3(bounds.x, bounds.y, 1000f));

    [SerializeField] private float gameOverHeight;
    [SerializeField] private float bpm = 120f;

    private DissolveImage gameOverImage;

    private Player player;
    public UnityEvent OnEnemyKilled;
    public TMPro.TextMeshProUGUI scoreText;
    private int score = 0;
    private bool canAnimateText = true;

    private List<GameObject> bounceObjects = new List<GameObject>();

    private bool bGameOver = false;

    public bool IsGameOver() {return bGameOver;} 

    void Awake()
    {
        Instance = this;
        gameOverImage = FindObjectOfType<DissolveImage>();
        player = FindObjectOfType<Player>();

        gameOverImage.onFinishDissolve.AddListener(player.DieLatent);
    }

    private void Start()
    {
        bounceObjects = FindObjectsOfType<Invader>().Select(invader => invader.gameObject).ToList();
        // add player to bounce objects
        bounceObjects.Add(player.gameObject);
    }

    public Vector3 KeepInBounds(Vector3 position)
    {
        return Bounds.ClosestPoint(position);
    }

    public float KeepInBounds(float position, DIRECTION side)
    {
        switch (side)
        {
            case DIRECTION.Right: return Mathf.Min(position, Bounds.max.x);
            case DIRECTION.Up: return Mathf.Min(position, Bounds.max.y);
            case DIRECTION.Left: return Mathf.Max(position, Bounds.min.x);
            case DIRECTION.Down: return Mathf.Max(position, Bounds.min.y);
            default: return position;
        }
    }

    public bool IsInBounds(Vector3 position)
    {
        return Bounds.Contains(position);
    }

    public bool IsInBounds(Vector3 position, DIRECTION side)
    {
        switch (side)
        {
            case DIRECTION.Right: case DIRECTION.Left: return IsInBounds(position.x, side);
            case DIRECTION.Up: case DIRECTION.Down: return IsInBounds(position.y, side);
            default: return false;
        }
    }

    public bool IsInBounds(float position, DIRECTION side)
    {
        switch (side)
        {
            case DIRECTION.Right: return position <= Bounds.max.x;
            case DIRECTION.Up: return position <= Bounds.max.y;
            case DIRECTION.Left: return position >= Bounds.min.x;
            case DIRECTION.Down: return position >= Bounds.min.y;
            default: return false;
        }
    }

    public bool IsBelowGameOver(float position)
    {
        return position < transform.position.y + (gameOverHeight - bounds.y * 0.5f);
    }

    public void PlayGameOver()
    {
        bGameOver = true;

        if (!Juice.IsActive())
        {
            player.DieLatent();
            return;
        }

        float targetZ = gameOverImage.transform.position.z;
        Vector3 targetLocation = player.transform.position;
        targetLocation.z = targetZ;

        gameOverImage.transform.position = targetLocation;
        gameOverImage.TriggerFX();
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(transform.position, new Vector3(bounds.x, bounds.y, 0f));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            transform.position + Vector3.up * (gameOverHeight - bounds.y * 0.5f) - Vector3.right * bounds.x * 0.5f,
            transform.position + Vector3.up * (gameOverHeight - bounds.y * 0.5f) + Vector3.right * bounds.x * 0.5f);
    }

    public void EnemyKilled(GameObject destroyedObject)
    {
        bounceObjects.Remove(destroyedObject);
        score += 10;
        scoreText.text = score.ToString();
        //OnEnemyKilled.Invoke();

        //use dotween to make the text shake in rotation and in scale
        if (canAnimateText)
        {
            canAnimateText = false;
            scoreText.transform.DOPunchRotation(new Vector3(0, 0, 70), 0.5f);
            scoreText.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), 0.7f).OnComplete(() => ResetTextScaleAndRotationValues());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        //call the BounceObjects method on a 120 bpm rythm
        if (Time.time % (60f / bpm) < 0.1f)
        {
            BounceObjects();
        }
    }

    private void ResetTextScaleAndRotationValues()
    {
        scoreText.transform.rotation = Quaternion.identity;
        scoreText.transform.localScale = Vector3.one;
        canAnimateText = true;
    }

    private void BounceObjects()
    {
        bounceObjects.ForEach(obj =>
        {
            obj.transform.DOScaleY(0.15f, 0.1f).SetLoops(2, LoopType.Yoyo).OnComplete(() => obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f));
            obj.transform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360).SetLoops(2, LoopType.Yoyo);

        });
    }
}
