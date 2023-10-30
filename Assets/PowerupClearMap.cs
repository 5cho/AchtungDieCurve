using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupClearMap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {         
        if(collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            MapCleaner.Instance.ClearAllLines();
            Destroy(gameObject);  
        }
    }
}
