using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    public float bottomOffset;
    public float frontOffset;
    public float collisionRadius;
    public LayerMask GroundLayer;
    public PlayerMovement player;
    public bool CheckGround(Vector3 Direction)
    {
        Vector3 Pos = transform.position + (Direction * bottomOffset);
        Collider[] hitColliders = Physics.OverlapSphere(Pos, collisionRadius, GroundLayer);
        if (hitColliders.Length > 0)
        {
            //we are on the ground
            return true;
        }

        return false;
    }

    public void OnCollisionEnter(Collision col)
    {
        player.SetGrounded(true);
    }
    public void OnCollisionExit(Collision col)
    {
        player.SetGrounded(false);
    }

}