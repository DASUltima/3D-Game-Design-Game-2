using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public static CameraScript Instance;

    public float zLimit;
    public float zNegLimit;
    public float xLimit;
    public float xNegLimit;
    public float cameraMoveSpeed;
    public float panBorderThickness;

    public Vector3 pivot;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        pivot = transform.position;
    }
    private void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (pivot.x <= xLimit && pivot.x >= xNegLimit && pivot.z <= zLimit && pivot.z >= zNegLimit)
        {
            pivot += new Vector3(horizontal * cameraMoveSpeed, 0, vertical * cameraMoveSpeed);
            if (pivot.x > xLimit)
                pivot.x = xLimit;
            if (pivot.x < xNegLimit)
                pivot.x = xNegLimit;
            if (pivot.z > zLimit)
                pivot.z = zLimit;
            if (pivot.z < zNegLimit)
                pivot.z = zNegLimit;
        }

        transform.position = pivot;
    }
}
