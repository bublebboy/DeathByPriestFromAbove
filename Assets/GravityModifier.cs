using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityModifier : MonoBehaviour
{
    public Transform target;
    public float jumpForce = 20f;
    public float minSlope = 0.65f;
    public float airControl = 0.3f;
    public float maxSpeed = 5f;
    public float acceleration = 3f;
    public float breakMultiplier = 5f;

    private bool grounded = false;
    private Vector2 groundNormal;
    private Rigidbody2D rb2d;
    private Vector2 direction;
    private Vector2 targetDirection;
    private float moveVelocity = 0;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        direction = Physics2D.gravity;
        targetDirection = direction;
    }

    void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouse = Input.mousePosition;
            Vector3 mwrld = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, target.position.z - Camera.main.transform.position.z));
            Vector3 delta = mwrld - target.position;
            targetDirection = delta.normalized * 9.8f;
        }

        direction = Vector2.Lerp(direction, targetDirection, 0.05f);
        Physics2D.gravity = direction;

        Vector2 gDir = Physics2D.gravity.normalized;
        rb2d.velocity = gDir * Vector2.Dot(gDir, rb2d.velocity);

        if (Input.GetButton("Jump") && grounded)
        {
            rb2d.velocity += groundNormal * jumpForce;
            grounded = false;
        }

        float speedDelta = Input.GetAxis("Horizontal") * acceleration;
        if (!grounded)
        {
            speedDelta *= airControl;
        }
        if (grounded && speedDelta * moveVelocity < 0)
        {
            speedDelta *= breakMultiplier;
        }
        speedDelta *= Time.deltaTime;
        moveVelocity = Mathf.Clamp(moveVelocity + speedDelta, -maxSpeed, maxSpeed);
        if (!Input.GetButton("Horizontal"))
        {
            moveVelocity = Mathf.Lerp(moveVelocity, 0, Mathf.Abs(Input.GetAxis("Horizontal")));
        }

        rb2d.velocity += new Vector2(-gDir.y * moveVelocity, gDir.x * moveVelocity);
	}

    private void OnCollisionStay2D(Collision2D collision)
    {
        grounded = false;
        Vector3 gravDir = Physics2D.gravity.normalized;
        Vector2 bestGroundNormal = Vector2.zero;
        float maxSlope = -1;
        for (int i = 0; i < collision.contactCount; i++)
        {
            float slope = -Vector2.Dot(collision.contacts[i].normal, gravDir);
            if (slope > minSlope)
            {
                if (slope > maxSlope)
                {
                    maxSlope = slope;
                    bestGroundNormal = collision.contacts[i].normal;
                }
                grounded = true;
            }
        }
        if (grounded)
        {
            groundNormal = bestGroundNormal * maxSlope;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Vector3 mouse = Input.mousePosition;
            Vector3 mwrld = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, target.position.z - Camera.main.transform.position.z));
            Gizmos.color = Color.green;
            Gizmos.DrawRay(target.position, -Physics2D.gravity / 9.8f);
            Vector2 hrz = new Vector2(-Physics2D.gravity.y, Physics2D.gravity.x) / 9.8f;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(target.position, hrz);

            Vector2 p1 = new Vector2(0, 0);
            Vector2 p2 = Physics.gravity;
            Vector2 p3 = rb2d.velocity;
            Vector2 p4 = new Vector2(-p2.y, p2.x);
            float q1 = (p1.x * p2.y - p1.y * p2.x);
            float q2 = (p3.x * p4.y - p3.y * p4.x);
            float d = (p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x);
            Vector2 p = new Vector2(
                (q1*(p3.x-p4.x)-(p1.x-p2.x)*q2)/d,
                (q1*(p3.y-p4.y)-(p1.y-p2.y)*q2)/d);

            p = Physics2D.gravity.normalized * Vector2.Dot(Physics2D.gravity.normalized, rb2d.velocity);

            Gizmos.color = new Color(0, 0, 1, 0.5f);
            Gizmos.DrawRay(target.position, p);
            Gizmos.color = Color.white;
            Gizmos.DrawRay(target.position, rb2d.velocity);
            Gizmos.color = Color.white;
            Gizmos.DrawLine((Vector2)target.position + p, (Vector2)target.position + rb2d.velocity);
        }
    }
#endif
}
