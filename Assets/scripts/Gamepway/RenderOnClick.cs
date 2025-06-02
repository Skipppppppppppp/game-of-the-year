using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class RenderOnClick : MonoBehaviour
{
    private SpriteRenderer[] spriteRenderers;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.enabled = true;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.enabled = false;
            }
        }
     }
}
