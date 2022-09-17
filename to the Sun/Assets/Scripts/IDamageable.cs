using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    int currentHealth { get; set; }
    void Damage(int damageAmount);
}
