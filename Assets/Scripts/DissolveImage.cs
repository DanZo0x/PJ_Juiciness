using System;
using System.Collections;
using UnityEngine;

public class DissolveImage : MonoBehaviour
{

    private Material material;

    [Header("Edit")]
    [SerializeField]
    private float dissolveSpeed = 1.0f;

    [SerializeField]
    private Texture2D dissolveTexture;

    [SerializeField]
    private bool invertFX = false;

    [Header("Debug")]
    [SerializeField]
    private float noise = 0.0f;

    [SerializeField]
    private bool bTriggerFX = false;

    private void Start()
    {
        material = GetComponent<Renderer>().material;
        material.SetTexture("_Dissolve", dissolveTexture);
        material.SetInt("_InverseDissolve", invertFX ? 1 : 0);

        noise = invertFX ? 1.0f : 0.0f;
        material.SetFloat("_Noise", noise);
    }

    private void Update()
    {
        if (!bTriggerFX)
        {
            return;
        }

        int factor = invertFX ? -1 : 1;
        noise += Time.deltaTime * dissolveSpeed * factor;
        material.SetFloat("_Noise", noise);

        if (noise >= 1.0f)
        {
            Destroy(gameObject);
        }
    }

    public void TriggerFX()
    {
        bTriggerFX = true;
    }
}
