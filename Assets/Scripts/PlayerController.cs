using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerController : MonoBehaviour
{
    CustomJoystick joystick;
    public float walkSpeed;
    public GameObject focusedItem;
    public GameObject skillcheck;
    public GameObject toiletBar;
    public float startSkillTime;
    public float endSkillTime;
    public float curPoopValue;
    Vector3 dir;
    Animator animator;
    [HideInInspector]
    public float lastInteractTime;
    [HideInInspector]
    public bool blockLeftMovement;
    [HideInInspector]
    public bool blockRightMovement;
    [HideInInspector]
    public bool freeze;
    public PauseMenu ui;
    float startPoopValue;
    float lastMouseTime;
    int tapId;
    // Start is called before the first frame update
    void Start()
    {
        joystick = FindObjectOfType<CustomJoystick>();
        StartCoroutine(SpawnSkillchecks());
        lastInteractTime = -0.75f;
        animator = GetComponent<Animator>();
        startPoopValue = curPoopValue;
        ClearItem();
    }
    IEnumerator SpawnSkillchecks()
    {
        bool flag = true;
        while (true)
        {
            flag = true;
            yield return new WaitForSeconds(Random.Range(startSkillTime, endSkillTime));
            if (!animator.GetBool("poop"))
                AudioSystem.instance.PlaySound(0);
            else
                flag = false;
            yield return new WaitForSeconds(1);
            yield return new WaitForSeconds(0.75f - (Time.time - lastInteractTime) > 0 ? 0.75f - (Time.time - lastInteractTime) : 0);
            if (flag && !animator.GetBool("poop"))
            {
                lastInteractTime = 0;
                Instantiate(skillcheck, skillcheck.transform.position, skillcheck.transform.rotation);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        RefreshGut();
        Interact();

        var input = freeze ? 0 : Input.GetAxis("Horizontal") + joystick.Axis.x;
        //StepControl(input);
        if (input < 0 && blockLeftMovement)
        {
            input = 0;
            animator.SetTrigger("turnLeft");
        }
        else if (input > 0 && blockRightMovement)
        {
            input = 0;
            animator.SetTrigger("turnRight");
        }
        else
        {
            animator.ResetTrigger("turnLeft");
            animator.ResetTrigger("turnRight");
        }

        if (!freeze && GetComponent<NPCController>().lookAt == null)
            animator.SetFloat("speed", input);
        dir.x = input * Time.deltaTime * walkSpeed * transform.lossyScale.y;
        transform.position += dir;       
    }
    public void BlockMovement(bool b)
    {
        blockLeftMovement = b;
        blockRightMovement = b;
    }
    public void Cycle(float time)
    {
        Debug.Log(Time.time);
        GameManager.Instance.Invoke(Cycle,time,time);
    }
    public void Freeze(float time)
    {
        freeze = true;
        if (time > 0)
            Invoke("Unfreeze", time);
    }
    public void Unfreeze()
    {
        freeze = false;
    }

    public void GameOver()
    {
        animator.SetBool("gameOver", true);
    }

    public void PreGameOver()
    {
        lastInteractTime = Time.time + 10;
        AudioSystem.instance.PlaySound(3);
        animator.SetBool("gameOver", true);
        freeze = true;
        StopAllCoroutines();
        Invoke("SendGameManager", 1.5f);
    }
    void SendGameManager()
    {
        GameManager.Instance.GameOver();
    }
    public void ClearItem()
    {
        transform.Find("Тело").Find("Бутылка").GetComponent<SpriteRenderer>().sprite = null;
        transform.Find("Рука").Find("Бутылка (1)").GetComponent<SpriteRenderer>().sprite = null;
    }
    public static bool ExistTouch(TouchPhase targerPhase, out Touch touch)
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            touch = Input.GetTouch(i);
            if (touch.phase == targerPhase)
                return true;
        }
        touch = new Touch();
        return false;
    }
    bool IsPlayerTryToInteract()
    {
        if (ExistTouch(TouchPhase.Began, out var touch) && Time.timeScale > 0)
        {
            lastMouseTime = Time.unscaledTime;
            tapId = touch.fingerId;
        }
        if (ExistTouch(TouchPhase.Ended, out touch) && tapId == touch.fingerId && Time.timeScale > 0 && Time.unscaledTime - lastMouseTime < 0.2f)
        {
            return true;
        }

        return Input.GetButtonDown("Jump");
    }
    void Interact()
    {
        if (IsPlayerTryToInteract() && focusedItem != null && (Time.time - lastInteractTime) > 0.75f)
        {
            if (focusedItem.GetComponent<InteractObject>().type == InteractObject.TypeOfInteract.Other && !focusedItem.GetComponent<InteractObject>().block)
            {
                if (focusedItem.GetComponent<InteractObject>().interactSound == null)
                    AudioSystem.instance.PlaySound(1);
                lastInteractTime = Time.time;
                Freeze(0.75f);
                animator.SetTrigger("action");
                focusedItem.GetComponent<InteractObject>().Action(0.375f);
            }
            else
            {
                if (focusedItem.GetComponent<InteractObject>().block)
                    AudioSystem.instance.PlaySound(2);
                focusedItem.GetComponent<InteractObject>().Action();
            }
        }
    }

    void RefreshGut()
    {
        ui.SetGut(curPoopValue / startPoopValue);

        if (curPoopValue <= 0 && !animator.GetBool("poop"))
        {
            animator.SetBool("victory", true);            
            freeze = true;
            StopAllCoroutines();
            GetComponent<Timer>().freeze = true;
            GameManager.Instance.data.curTime = GetComponent<Timer>().startTime - GetComponent<Timer>().timeLeft;
            Invoke("SendGameComplete", 1);
            curPoopValue = 0.001f;
        }
    }

    void SendGameComplete()
    {
        GameManager.Instance.Complete();
    }

    void StepControl(float input)
    {
        var stepper = GetComponent<Stepper>();
        if (!stepper)
            return;

        if (input != 0)
            stepper.timeToStep -= Time.deltaTime;
    }

    public void DropItem(GameObject itemOnFloor)
    {
        var curSprite = transform.Find("Тело").Find("Бутылка").GetComponent<SpriteRenderer>().sprite;
        if (itemOnFloor.GetComponent<SpriteRenderer>().sprite == curSprite)
        {
            itemOnFloor.SetActive(true);
            itemOnFloor.GetComponent<BoxCollider>().enabled = true;
        }
    }

    public void ItemActive(bool b)
    {
        transform.Find("Тело").Find("Бутылка").GetComponent<SpriteRenderer>().enabled = b;
        transform.Find("Рука").Find("Бутылка (1)").GetComponent<SpriteRenderer>().enabled = b;
    }

    public void AnimatorSetSpeed(float f)
    {
        animator.SetFloat("speed", f);
    }

    public void AddListener(string name)
    {
        foreach (var i in GameObject.FindObjectsOfType<AudioListener>())
            i.enabled = false;

        var obj = GameObject.Find(name);

        obj.GetComponent<AudioListener>().enabled = true;

        foreach (var i in GameObject.FindObjectsOfType<ControlSpatial>())
            i.SetTarget(obj.transform);
    }

    public void StopSkillchecks()
    {
        StopAllCoroutines();
    }
    public void StartSkillchecks()
    {
        StartCoroutine(SpawnSkillchecks());
    }
}
