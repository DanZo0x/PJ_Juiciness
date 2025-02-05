using UnityEngine;

public class BackgroundActivation : MonoBehaviour
{
    private Material material;

    bool bJuiceActive = false;

    private void Start()
    {
        bJuiceActive = Juice.IsActive();
        material = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        if (Juice.IsActive() != bJuiceActive)
        {
            bJuiceActive = Juice.IsActive();
            material.SetInt("_JuiceActive", bJuiceActive ? 1 : 0);
        }
    }
}
