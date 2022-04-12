using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterView : MonoBehaviour
{
    public Sprite normalHead;
    public Sprite thrilledHead;
    public Sprite talkingHead;
    public Sprite body;
    public Sprite rightHand;
    public Sprite leftHand;
    public Sprite legs;
    public Sprite step1;
    public Sprite step2;
    public Sprite step3;
    public Sprite step4;
    // Start is called before the first frame update
    void Start()
    {
        transform.Find("Голова_беспокоится_1").GetComponent<SpriteRenderer>().sprite = normalHead;
        transform.Find("Голова_беспокоится_1").transform.Find("Голова_беспокоится_2").GetComponent<SpriteRenderer>().sprite = thrilledHead;
        transform.Find("Голова_беспокоится_1").transform.Find("Голова_говорит").GetComponent<SpriteRenderer>().sprite = talkingHead;
        transform.Find("Тело").GetComponent<SpriteRenderer>().sprite = body;
        transform.Find("Тело").transform.Find("Правая рука").GetComponent<SpriteRenderer>().sprite = rightHand;
        transform.Find("Тело").transform.Find("Левая рука").GetComponent<SpriteRenderer>().sprite = leftHand;
        transform.Find("Ноги_стоит").GetComponent<SpriteRenderer>().sprite = legs;
        transform.Find("Ноги_шаг_1").GetComponent<SpriteRenderer>().sprite = step1;
        transform.Find("Ноги_шаг_2").GetComponent<SpriteRenderer>().sprite = step2;
        transform.Find("Ноги_шаг_3 (спереди тела)").GetComponent<SpriteRenderer>().sprite = step3;
        transform.Find("Ноги_шаг_4").GetComponent<SpriteRenderer>().sprite = step4;
    }
}
