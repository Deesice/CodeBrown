using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{
    GameObject cur;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            cur = other.gameObject;
            if (other.transform.position.x - transform.position.x > 0)
                other.gameObject.GetComponent<PlayerController>().blockLeftMovement = true;
            else
                other.gameObject.GetComponent<PlayerController>().blockRightMovement = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == cur)
        {
            cur = null;
            other.gameObject.GetComponent<PlayerController>().blockLeftMovement = false;
            other.gameObject.GetComponent<PlayerController>().blockRightMovement = false;
        }
    }

    public void Deactivate()
    {
        if (cur)
        {
            cur.GetComponent<PlayerController>().blockLeftMovement = false;
            cur.gameObject.GetComponent<PlayerController>().blockRightMovement = false;
        }
        GetComponent<BoxCollider>().enabled = false;
    }
}
