using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityIsDown : MonoBehaviour
{
	void Update () {
        float angle = Mathf.Atan2(Physics2D.gravity.y, Physics2D.gravity.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle + 90f);
        transform.localRotation = rotation;
    }
}
