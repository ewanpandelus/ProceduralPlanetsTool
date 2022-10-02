using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float rotSpeed = 10;
    public void Update()
    {
        transform.Rotate((Vector3.up) * Time.deltaTime*rotSpeed);
    }
}
