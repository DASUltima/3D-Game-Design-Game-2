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
        pivot += new Vector3(horizontal * cameraMoveSpeed, 0, vertical * cameraMoveSpeed);

        transform.position = pivot;
    }
}
