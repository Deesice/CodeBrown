using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo : MonoBehaviour
{
    public enum Direction {Up, Left, Down, Right };
    public Direction direction = Direction.Left;
    public float speed = 3000;
    public bool isLoadingScreen = false;
    Canvas ui;
    // Start is called before the first frame update
    void Start()
    {
        if (!isLoadingScreen)
        {
            GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            AudioSystem.instance.PlaySound(13);
        }
        Invoke("Destroy", 2.0f);

        speed *= transform.GetComponentInParent<Canvas>().pixelRect.height/400;
    }

    // Update is called once per frame
    void Update()
    {
        switch ((int)direction)
        {
            case 0:
                transform.position += Vector3.up * Time.deltaTime * speed;
                break;
            case 1:
                transform.position += Vector3.left * Time.deltaTime * speed;
                break;
            case 2:
                transform.position += Vector3.down * Time.deltaTime * speed;
                break;
            case 3:
                transform.position += Vector3.right * Time.deltaTime * speed;
                break;
            default:
                break;
        }

        if (isLoadingScreen)
            if (transform.localPosition.x < 0)
                transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
    }
    private void Destroy()
    {
        Destroy(gameObject);
    }
}
