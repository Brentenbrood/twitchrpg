using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int StartHealth = 100;
    public Healthbar Healthbar;

    public void Damage(int dmg)
    {
        Healthbar.Health -= dmg;
    }

    public void Heal(int amt)
    {
        Healthbar.Health += amt;
    }

    protected virtual void Start()
    {
        if (Healthbar)
        {
            Healthbar.Health = StartHealth;
        }
    }

    protected virtual void Update()
    {
        
    }
}
