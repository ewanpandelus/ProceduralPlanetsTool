using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsManager : MonoBehaviour
{
    [SerializeField]
    private GameObject directionalLight;
    public Vector3 GetDirectionToSun()
    {
        return -directionalLight.transform.forward;
    }
}
