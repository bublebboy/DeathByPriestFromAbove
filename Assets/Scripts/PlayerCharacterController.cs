using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    public string JumpButton = "Jump";
    public string MovementAxis = "Horizontal";
    public string SetGravityButton = "Fire2";
    public CharacterController characterController;
    public GravityModifier gravityModifier;
    public int life = 3;
    public float invulnerabilityTime = 1.25f;

    private bool invulnerable = false;
    private Rigidbody2D rb2d;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update ()
    {
		if (Input.GetButtonDown(JumpButton))
        {
            characterController.StartJump();
        }
        else if (Input.GetButtonUp(JumpButton))
        {
            characterController.EndJump();
        }

        characterController.HorizontalMovement = Input.GetAxis(MovementAxis);

        if (Input.GetButtonDown(SetGravityButton))
        {
            gravityModifier.ChangeGravityDirection();
        }
	}

    private IEnumerator GotHit()
    {
        invulnerable = true;
        characterController.enabled = false;
        yield return new WaitForSeconds(invulnerabilityTime);
        characterController.enabled = true;
        invulnerable = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (invulnerable)
        {
            return;
        }
        if (collision.gameObject.tag == "Enemy")
        {
            float dir = Vector2.Dot(transform.right, collision.contacts[0].point - (Vector2)transform.position) > 0 ? 1 : -1;
            dir *= characterController.maxSpeed;
            rb2d.velocity = transform.up * characterController.jumpForce + transform.right * dir;
            StartCoroutine(GotHit());
        }
    }
}
