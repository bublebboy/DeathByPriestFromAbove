using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerCharacterController : MonoBehaviour
{
    public CharacterController characterController;
    public LayerMask navMask;

    private Collider2D c2d;
    private RaycastHit2D[] hits = new RaycastHit2D[1];
    private int dir = 1;

    private void Start()
    {
        c2d = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (characterController.Grounded)
        {
            bool turn = false;
            Vector2 checkStart = transform.position + transform.right * dir * (c2d.bounds.extents.x + 0.05f);
            RaycastHit2D wallCheck = Physics2D.Raycast(checkStart, transform.right * dir, 0.1f, navMask);
            Debug.DrawRay(checkStart, transform.right * dir * 0.1f);
            if (wallCheck.collider != null)
            {
                turn = true;
            }

            RaycastHit2D ledgeCheck = Physics2D.Raycast(checkStart, -transform.up, c2d.bounds.extents.y + 0.5f, navMask);
            Debug.DrawRay(checkStart, -transform.up * c2d.bounds.extents.y * 1.5f);
            if (ledgeCheck.collider == null)
            {
                turn = true;
            }

            if (turn)
            {
                dir *= -1;
            }
        }

        characterController.HorizontalMovement = dir;
    }
}
