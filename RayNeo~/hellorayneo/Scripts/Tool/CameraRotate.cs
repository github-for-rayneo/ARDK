using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(new Vector3(0, 0.1f, 0));

    }
}
