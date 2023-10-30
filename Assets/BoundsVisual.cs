using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] boundsSpriteRendererArray;

    private float alpha;

    private void Update()
    {
        alpha = Mathf.PingPong(Time.time, 1f);
        if (PowerupManager.Instance.IsPhaseBoundsPowerupActive())
        {
            foreach(SpriteRenderer spriteRender in boundsSpriteRendererArray)
            {
                spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, alpha);
            }
        }
        else
        {
            foreach (SpriteRenderer spriteRender in boundsSpriteRendererArray)
            {
                spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, 1);
            }
        }
    }
}
