using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.localRotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.m_State != GameManager.State.Playing)
            return;

        RotateVertical();
    }

    void RotateVertical()
    {
        float verticalScope = 0.2f;
        Vector3 verticalRotation = Vector3.left * Input.GetAxis("Mouse Y");

        if (transform.localRotation.x > verticalScope && verticalRotation.x > 0)
            verticalRotation.x = 0;
        else if (transform.localRotation.x < -verticalScope && verticalRotation.x < 0)
            verticalRotation.x = 0;

        transform.Rotate(verticalRotation * Player.Instance.m_Stat.SpeedRotate);
    }
}
