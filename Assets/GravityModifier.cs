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
    public float terminalVelocity = 30f;
    public float fallMultiplier = 2f;

    private bool grounded = false;
    private Vector2 groundNormal;
    private Rigidbody2D rb2d;
    private Vector2 direction;
    private Vector2 targetDirection;
    private Vector2 fallVelocity = Vector2.zero;
    private Vector2 moveVelocity = Vector2.zero;
    private bool jumping = true;
    private float gravityMultiplier = 2f;

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
        Vector2 gTan = new Vector2(-gDir.y, gDir.x);
        rb2d.velocity = gDir * Vector2.Dot(gDir, rb2d.velocity);

        float fallT = fallVelocity.magnitude / terminalVelocity;
        fallT = fallT * fallT * fallT * fallT;
        Vector2 fallDelta = Vector2.Lerp(gDir * 9.8f * gravityMultiplier * Time.deltaTime, Vector2.zero, fallT);

        if (Input.GetButtonDown("Jump"))
        {
            if (grounded)
            {
                gravityMultiplier = 1;
                jumping = true;
                fallVelocity = -gDir * jumpForce;
                grounded = false;
            }
        }
        else if (jumping && Input.GetButtonUp("Jump"))
        {
            jumping = false;
        }

        if (jumping == false || Vector2.Dot(gDir, fallVelocity) > 0)
        {
            jumping = false;
            gravityMultiplier = fallMultiplier;
        }

        fallVelocity += fallDelta;

        float speed = Input.GetAxis("Horizontal") * maxSpeed;
        if (grounded)
        {
            moveVelocity = new Vector2(groundNormal.y * speed, -groundNormal.x * speed);
        }
        else
        {
            Vector2 airVelocity = gTan * speed;
            moveVelocity = Vector2.Lerp(moveVelocity, airVelocity, airControl);
        }

        rb2d.velocity = fallVelocity + moveVelocity;
	}
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.contactCount == 0)
            grounded = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        grounded = false;
        Vector3 gravDir = Physics2D.gravity.normalized;
        Vector2 bestGroundNormal = Vector2.zero;
        float maxGroundSlope = -1;
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.contacts[i].normal;
            Vector2 nrmTan = new Vector2(normal.y, -normal.x);
            float slope = -Vector2.Dot(normal, gravDir);
            if (slope > minSlope)
            {
                if (slope > maxGroundSlope)
                {
                    maxGroundSlope = slope;
                    bestGroundNormal = normal;
                }
                grounded = true;
            }
            Debug.DrawRay(transform.position, fallVelocity, Color.red);
            fallVelocity = HandleCollision(fallVelocity, normal);
            Debug.DrawRay(transform.position, fallVelocity, Color.magenta);

            Debug.DrawRay(transform.position, moveVelocity, Color.blue);
            moveVelocity = HandleCollision(moveVelocity, normal);
            Debug.DrawRay(transform.position, moveVelocity, Color.cyan);
        }
        if (grounded)
        {
            groundNormal = bestGroundNormal * maxGroundSlope;
        }
    }

    private Vector2 HandleCollision(Vector2 velocity, Vector2 normal)
    {
        if (Vector2.Dot(velocity, normal) < 0)
        {
            return velocity * Mathf.Sqrt(1f - Mathf.Pow(Vector2.Dot(velocity.normalized, normal), 2));
        }
        return velocity;
    }

//#if UNITY_EDITOR
//    private void OnDrawGizmos()
//    {
//        if (Application.isPlaying)
//        {
//            Vector3 mouse = Input.mousePosition;
//            Vector3 mwrld = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, target.position.z - Camera.main.transform.position.z));
//            Gizmos.color = Color.green;
//            Gizmos.DrawRay(target.position, -Physics2D.gravity / 9.8f);
//            Vector2 hrz = new Vector2(-Physics2D.gravity.y, Physics2D.gravity.x) / 9.8f;
//            Gizmos.color = Color.red;
//            Gizmos.DrawRay(target.position, hrz);

//            Vector2 p1 = new Vector2(0, 0);
//            Vector2 p2 = Physics.gravity;
//            Vector2 p3 = rb2d.velocity;
//            Vector2 p4 = new Vector2(-p2.y, p2.x);
//            float q1 = (p1.x * p2.y - p1.y * p2.x);
//            float q2 = (p3.x * p4.y - p3.y * p4.x);
//            float d = (p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x);
//            Vector2 p = new Vector2(
//                (q1*(p3.x-p4.x)-(p1.x-p2.x)*q2)/d,
//                (q1*(p3.y-p4.y)-(p1.y-p2.y)*q2)/d);

//            p = Physics2D.gravity.normalized * Vector2.Dot(Physics2D.gravity.normalized, rb2d.velocity);

//            Gizmos.color = new Color(0, 0, 1, 0.5f);
//            Gizmos.DrawRay(target.position, p);
//            Gizmos.color = Color.white;
//            Gizmos.DrawRay(target.position, rb2d.velocity);
//            Gizmos.color = Color.white;
//            Gizmos.DrawLine((Vector2)target.position + p, (Vector2)target.position + rb2d.velocity);

//            if (grounded)
//            {
//                Gizmos.color = Color.yellow;
//                Gizmos.DrawRay(target.position, new Vector2(groundNormal.y, -groundNormal.x));
//            }
//        }
//    }
//#endif
}
