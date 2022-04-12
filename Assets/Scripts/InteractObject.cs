using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractObject : MonoBehaviour
{
    const float ICON_SIZE = 2;
    public enum TypeOfInteract { Door, Dialog, Other, Toilet, Skillcheck };
    public TypeOfInteract type;
    public Transform iconPosition;
    public bool block = false;
    public UnityEvent[] Events;
    public UnityEvent OnBlock;
    int curEvent;
    public SpriteRenderer[] outline;
    public AudioClip interactSound;
    static GameObject[] icons = new GameObject[4];
    GameObject player;

    GameObject iconInstance;
    // Start is called before the first frame update
    void Start()
    {
        curEvent = 0;
        if (icons[0] == null)
            icons[0] = GameObject.Find("Иконка_дверь");
        if (icons[1] == null)
            icons[1] = GameObject.Find("Иконка_диалоги");
        if (icons[2] == null)
            icons[2] = GameObject.Find("Иконка_рука");
        if (icons[3] == null)
            icons[3] = GameObject.Find("Иконка_туалет");
        player = GameObject.Find("Player");
    }
    public void ReBlock()
    {
        block = !block;
    }
    public void Action(float latency = 0)
    {
        if (!block)
        {
            if (latency == 0)
                ActionLatency();
            else
                Invoke("ActionLatency", latency);
            RemoveItem();
        }
        else
        {
            Color error = new Color(1, 0, 0);
            foreach (var i in outline)
                i.color = error;

            RemoveItem(0.75f);
            OutlineOn();
            Invoke("OnBlockLatency", latency);
            Invoke("ResetOutlineColor", 0.75f);
        }
    }
    void ResetOutlineColor()
    {
        Color error = new Color(1, 1, 1);
        foreach (var i in outline)
            i.color = error;
        OutlineOff();
    }
    void OnBlockLatency()
    {
        OnBlock.Invoke();
    }
    void ActionLatency()
    {
        if (interactSound)
        { 
            AudioSystem.instance.PlaySound(interactSound, new Vector3 (transform.position.x, player.transform.position.y, player.transform.position.z));
        }
        curEvent++;
        Events[(curEvent - 1)%Events.Length].Invoke();
    }
    void OutlineOn()
    {
        foreach (var i in outline)
            i.enabled = true;
    }
    void OutlineOff()
    {
        foreach (var i in outline)
            i.enabled = false;
    }
    public void BlockOn()
    {
        block = true;
    }
    public void BlockOff()
    {
        block = false;
    }
    public void SetCurEvent(int i)
    {
        if (i >= 0 && i < Events.Length)
            curEvent = i;
    }
    public void ReloadCollider()
    {
        if (GetComponent<BoxCollider>() && (GetComponent<BoxCollider>().enabled == true))
        {
            GetComponent<BoxCollider>().enabled = false;
            Debug.Log("Start enable collider");
            Invoke("EnableCollider", 0.1f);
        }
    }
    void EnableCollider()
    {
        GetComponent<BoxCollider>().enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (this.enabled == false)
            return;
        if (other.gameObject != player)
            return;
        switch (type)
        {
            case TypeOfInteract.Skillcheck:
                var previousItemForSkillcheck = player.GetComponent<PlayerController>().focusedItem;
                if (previousItemForSkillcheck != null)
                {
                    previousItemForSkillcheck.GetComponent<InteractObject>().RemoveItem(0.1f);
                }
                player.GetComponent<PlayerController>().focusedItem = gameObject;
                break;
            default:
                if (Time.time - player.GetComponent<PlayerController>().lastInteractTime > 0.75f)
                {
                    StartCoroutine(SetItem());
                }
                else
                    Invoke("ReloadCollider", 0.75f - Time.time + player.GetComponent<PlayerController>().lastInteractTime);
                break;
        }
    }

    IEnumerator SetItem()
    {
        while (player.GetComponent<PlayerController>().freeze || player.GetComponent<PlayerController>().focusedItem)
        {
            yield return null;
            //Debug.Log(gameObject.name + "trying to set item");
        }
        OutlineOn();
        player.GetComponent<PlayerController>().focusedItem = gameObject;
        iconInstance = Instantiate(icons[(int)type], iconPosition.position, icons[(int)type].transform.rotation);
        iconInstance.transform.localScale = new Vector3 (Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y), Mathf.Abs(transform.lossyScale.z));
        iconInstance.transform.localScale *= iconPosition.localScale.y * ICON_SIZE;       

    }

    public void RemoveItem(float time = 0)
    {
        StopAllCoroutines();
        if (player.GetComponent<PlayerController>() && player.GetComponent<PlayerController>().focusedItem == gameObject)
        {
            OutlineOff();
            player.GetComponent<PlayerController>().focusedItem = null;
            Destroy(iconInstance);
            
        }
        if (GetComponent<BoxCollider>())
        {
            GetComponent<BoxCollider>().enabled = false;
            if (time > 0)
            {
                StartCoroutine(EnableCollider(time));
            }
        }
    }

    IEnumerator EnableCollider(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponent<BoxCollider>().enabled = true;
    }
    

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
            RemoveItem(0.01f);
    }
    public void DestoyComponent()
    {
        RemoveItem();
        Destroy(this);
    }
    public void DestroyObject()
    {
        RemoveItem();
        gameObject.SetActive(false);
    }
    public void SetColliderPosition(Transform pos)
    {
        GetComponent<BoxCollider>().center = pos.position - transform.position;
    }
    public void SetIconPosition(Transform pos)
    {
        iconPosition = pos;
    }

    public void ApplicationQuit()
    {
        Application.Quit();
    }

    public void UnlockAchievement(string name)
    {
        GameManager.UnlockAchievement(name);
    }
}
