using UnityEngine;

public class Juice : MonoBehaviour
{
    [SerializeField]
    private KeyCode keyActivation = KeyCode.K;

    private static bool _isActive = true;

    public static bool IsActive(){return _isActive;}

    private void Update()
    {
        if (Input.GetKeyDown(keyActivation))
        {
            _isActive = !_isActive;
        }
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 100, 100), _isActive ? "Juice Active" : "Juice Inactive");
    }
#endif
}
