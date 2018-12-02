using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerCharacterController : MonoBehaviour
{
    public CharacterController characterController;

    private int dir = -1;

    private void FixedUpdate()
    {
        characterController.HorizontalMovement = dir;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right * dir, 0.5f);
        Debug.DrawRay(transform.position, transform.right * dir * 0.5f);
        if (hit.collider != null)
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
        }
    }
}
