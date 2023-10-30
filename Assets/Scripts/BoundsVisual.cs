using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsVisual : MonoBehaviour
{
    [Header("Array of bounds sprite renderers")]
    [SerializeField] private SpriteRenderer[] boundsSpriteRendererArray;

    private float alpha;

    private void Update()
    {
        // Set the alpha value to go between 0 and 1 back and forth
        alpha = Mathf.PingPong(Time.time, 1f);

        // If the PhaseBounds powerup is active set the alpha on all the bounds sprite renderers to the new alpha value (between 0 and 1)
        if (PowerupManager.Instance.GetIsPhaseBoundsPowerupActive())
        {
            foreach(SpriteRenderer spriteRender in boundsSpriteRendererArray)
            {
                spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, alpha);
            }
        }
        // If the PhaseBounds powerup is INactive set the alpha on all bounds sprite renderers to 1
        else
        {
            foreach (SpriteRenderer spriteRender in boundsSpriteRendererArray)
            {
                spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, 1);
            }
        }
    }
}
