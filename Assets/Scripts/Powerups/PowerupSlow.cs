using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSlow : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            PowerupManager.Instance.SlowAllOtherPlayers(player);

            Destroy(gameObject);
        }
    }
}
