using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityModifier : MonoBehaviour {

    public Transform target;

    private float gravityAcceleration;

    // Use this for initialization
    void Start ()
    {
        gravityAcceleration = Physics2D.gravity.magnitude;
    }

    public void ChangeGravityDirection()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeDir());
    }

    private IEnumerator ChangeDir()
    {
        Vector3 mouse = Input.mousePosition;
        Vector3 mwrld = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, target.position.z - Camera.main.transform.position.z));
        Vector3 delta = mwrld - target.position;
        Vector2 startDirection = Physics2D.gravity;
        Vector2 targetDirection = delta.normalized * gravityAcceleration;
        float dot = Vector2.Dot(startDirection.normalized, targetDirection.normalized) * 0.5f + 0.5f;
        float duration = (1f - dot);
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            Physics2D.gravity = Vector2.Lerp(startDirection.normalized, targetDirection.normalized, t / duration) * gravityAcceleration;
            Debug.DrawRay(transform.position, Physics2D.gravity);
            yield return null;
        }
        Physics2D.gravity = targetDirection;
    }

    //private IEnumerator ChangeDir()
    //{
    //    Vector3 mouse = Input.mousePosition;
    //    Vector3 mwrld = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, target.position.z - Camera.main.transform.position.z));
    //    Vector3 delta = mwrld - target.position;
    //    Vector2 startDirection = Physics2D.gravity;
    //    Vector2 endDirection = delta.normalized * gravityAcceleration;
    //    Debug.Log("Start Dir: " + startDirection + "; End Dir: " + endDirection);

    //    float startAngle = Mathf.Atan2(startDirection.y, startDirection.x);
    //    float endAngle = Mathf.Atan2(endDirection.y, endDirection.x);
    //    float angleDelta = Mathf.Abs(startAngle - endAngle);
    //    Debug.Log("Start Angle: " + startAngle + "; End Angle: " + endAngle + "; Delta: " + angleDelta);
    //    if (angleDelta > Mathf.PI)
    //    {
    //        angleDelta = 2 * Mathf.PI - angleDelta;
    //        if (startAngle > endAngle)
    //        {
    //            endAngle = startAngle + angleDelta;
    //        }
    //        else
    //        {
    //            endAngle = startAngle - angleDelta;
    //        }
    //        Debug.Log("Correction: " + angleDelta + "; new End Angle: " + endAngle);
    //    }

    //    float duration = Mathf.Abs(angleDelta / Mathf.PI) * 4f;
    //    Debug.Log("Duration: " + duration);
    //    float t = 0;
    //    while (t < duration)
    //    {
    //        t += Time.deltaTime;
    //        float angle = Mathf.Lerp(startAngle, endAngle, t);
    //        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * gravityAcceleration;
    //        Debug.Log("T: " + t + "; Angle: " + angle + "; Direction: " + direction);
    //        Physics2D.gravity = direction;
    //        Debug.DrawRay(transform.position, Physics2D.gravity);
    //        yield return null;
    //    }
    //    Physics2D.gravity = endDirection;
    //}
}
