using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour {

    public Faction faction;
    public List<Base> connectingBase = new List<Base>();
    public Stat health;

    private void Start()
    {
        health.Initialize();
    }
    public void TakeDamage(int damage)
    {
        if (damage < health.CurrentVal)
        {
            health.CurrentVal -= damage;
        }
        else
            health.CurrentVal = 0;
    }
}
public enum Faction {Player,Enemy,None}
