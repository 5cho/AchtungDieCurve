using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    // On collision with the player call the function that kills that player
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Player>(out Player player))
        {
            player.PlayerHit();
        }
    }
}
