using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damagedCharacter = collision.GetComponent<IDamageable>();

        if(damagedCharacter != null)
        {
            damagedCharacter.Damage(90);
        }
    }
}
