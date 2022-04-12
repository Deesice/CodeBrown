using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    public Transform leftStair;
    public Transform rightStair;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            var target = other.transform;

            if (target.position.x >= leftStair.position.x && target.position.x <= rightStair.position.x)
            {
                var coef = (target.position.x - leftStair.position.x) / (rightStair.position.x - leftStair.position.x);
                other.transform.position = Vector3.Lerp(leftStair.position, rightStair.position, coef);
            }
            else if (target.position.x < leftStair.position.x)
                other.transform.position = new Vector3(target.position.x, leftStair.position.y, target.position.z);
            else
                other.transform.position = new Vector3(target.position.x, rightStair.position.y, target.position.z);
        }
    }

    public void SetLeftStair(Transform t)
    {
        leftStair = t;
    }
    public void SetRightStair(Transform t)
    {
        rightStair = t;
    }
}
