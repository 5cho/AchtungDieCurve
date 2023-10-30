using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupInvulnerable : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.InvulnerablePowerupPickedUp();

            Destroy(gameObject);
        }
    }
}
