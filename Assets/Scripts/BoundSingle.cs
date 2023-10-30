using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundSingle : MonoBehaviour
{
    // On collision with the player call the function that kills the player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.PlayerHit();
        }
    }
}
