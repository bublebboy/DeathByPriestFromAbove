using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityModifier : MonoBehaviour {

    public Transform target;

    private Vector2 direction;
    private Vector2 targetDirection;
    private float gravityAcceleration;

    // Use this for initialization
    void Start ()
    {
        direction = Physics2D.gravity;
        targetDirection = direction;
        gravityAcceleration = direction.magnitude;
    }

    private void Update()
    {
        direction = Vector2.Lerp(direction, targetDirection, 0.05f);
        Physics2D.gravity = direction;
    }

    public void ChangeGravityDirection()
    {
        Vector3 mouse = Input.mousePosition;
        Vector3 mwrld = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, target.position.z - Camera.main.transform.position.z));
        Vector3 delta = mwrld - target.position;
        targetDirection = delta.normalized * gravityAcceleration;
    }
}
