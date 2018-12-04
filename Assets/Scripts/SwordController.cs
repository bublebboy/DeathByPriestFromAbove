using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    public CharacterController characterController;
    public Rigidbody2D playerRigidbody;
    public float damping = 0.5f;

    private float a = 0;

	void FixedUpdate () {
        float angle;
        Vector2 direction;
        if (characterController.Grounded)
        {
            angle = -characterController.HorizontalMovement * 15;
            direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        }
        else
        {
            direction = playerRigidbody.velocity;
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90 - characterController.transform.localEulerAngles.z;
        }

        Quaternion rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, angle), transform.localRotation, damping);
        transform.localRotation = rotation;
        a += Time.fixedDeltaTime;
    }

    private IEnumerator Kill(GameObject obj)
    {
        obj.GetComponent<Collider2D>().enabled = false;
        obj.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        yield return null;
        obj.GetComponent<Animator>().SetTrigger("Die");
        yield return new WaitForSeconds(0.75f);
        Destroy(obj.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(Kill(collision.gameObject));
    }
}
