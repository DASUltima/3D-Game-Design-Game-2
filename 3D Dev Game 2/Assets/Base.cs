using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour {

    public Faction faction;
    public List<Base> connectingBase = new List<Base>();
}
public enum Faction {Player,Enemy,None}
