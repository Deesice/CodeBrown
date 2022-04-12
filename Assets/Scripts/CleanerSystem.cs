using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CleanerSystem : MonoBehaviour
{
    public MoveTo.Direction scanDirection;
    public Transform targetFolder;
    public UnityEvent Actions;

    // Update is called once per frame
    void Update()
    {
        if (Scan() == 0)
        {
            Actions?.Invoke();
            enabled = false;
        }
    }

    int Scan()
    {
        int count = 0;
        Transform t;
        for (int i = 0; i < targetFolder.childCount; i++)
        {
            t = targetFolder.GetChild(i);
            if (t.GetComponent<SpriteRenderer>().enabled == false)
                continue;
            switch (scanDirection)
            {
                case MoveTo.Direction.Down:
                    if (transform.position.y - t.position.y > 0)
                        count++;
                    break;
                case MoveTo.Direction.Left:
                    if (transform.position.x - t.position.x > 0)
                        count++;
                    break;
                case MoveTo.Direction.Right:
                    if (transform.position.x - t.position.x < 0)
                        count++;
                    break;
                case MoveTo.Direction.Up:
                    if (transform.position.y - t.position.y < 0)
                        count++;
                    break;
            }
        }
        return count;
    }
}
