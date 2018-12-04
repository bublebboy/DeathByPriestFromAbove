using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private static int IS_WALKING = Animator.StringToHash("IsWalking");
    private static int IS_FALLING = Animator.StringToHash("IsFalling");

    public float jumpForce = 20f;
    public float minSlope = 0.65f;
    public float airControl = 0.3f;
    public float maxSpeed = 5f;
    public float acceleration = 3f;
    public float breakMultiplier = 5f;
    public float terminalVelocity = 30f;
    public float fallMultiplier = 2f;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private bool grounded = false;
    private Vector2 groundNormal;
    private Rigidbody2D rb2d;
    private Vector2 fallVelocity = Vector2.zero;
    private Vector2 moveVelocity = Vector2.zero;
    private bool jumping = true;
    private float gravityMultiplier = 2f;
    private float horizontalMovement = 0;

    public bool Grounded
    {
        get { return grounded; }
    }

    public Vector2 GroundNormal
    {
        get { return groundNormal; }
    }

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void StartJump()
    {
        if (grounded)
        {
            gravityMultiplier = 1;
            jumping = true;
            fallVelocity = -Physics2D.gravity.normalized * jumpForce;
            grounded = false;
        }
    }

    public float HorizontalMovement
    {
        get { return horizontalMovement; }
        set { horizontalMovement = Mathf.Clamp(value, -1.0f, 1.0f); }
    }

    public void EndJump()
    {
        jumping = false;
    }

    void FixedUpdate () {
        Vector2 gDir = Physics2D.gravity.normalized;
        Vector2 gTan = new Vector2(-gDir.y, gDir.x);

        float fallT = fallVelocity.magnitude / terminalVelocity;
        fallT = fallT * fallT * fallT * fallT;
        Vector2 fallDelta = Vector2.Lerp(gDir * 9.8f * gravityMultiplier * Time.fixedDeltaTime, Vector2.zero, fallT);

        if (jumping == false || Vector2.Dot(gDir, fallVelocity) > 0)
        {
            jumping = false;
            gravityMultiplier = fallMultiplier;
        }

        fallVelocity += fallDelta;

        float speed = horizontalMovement * maxSpeed;
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

        bool isWalking = grounded && Mathf.Abs(speed) > 0.1f;
        spriteRenderer.flipX = speed < 0;
        animator.SetBool(IS_WALKING, isWalking);
        animator.SetBool(IS_FALLING, !grounded);
	}
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.contactCount == 0)
            grounded = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "ground")
        {
            return;
        }
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
            fallVelocity = HandleCollision(fallVelocity, normal);

            moveVelocity = HandleCollision(moveVelocity, normal);
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
}
