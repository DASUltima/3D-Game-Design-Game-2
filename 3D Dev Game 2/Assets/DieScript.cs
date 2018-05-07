using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieScript : MonoBehaviour {

    public void Destroy()
    {
        Destroy(transform.root.gameObject);
    }
}
