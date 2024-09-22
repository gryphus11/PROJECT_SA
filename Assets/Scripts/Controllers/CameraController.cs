using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Target { get; set; }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Target == null)
            return;

        transform.position = new Vector3(Target.position.x, Target.position.y, -10);
    }
}
