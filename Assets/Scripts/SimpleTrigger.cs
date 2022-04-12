using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleTrigger : MonoBehaviour
{
    public delegate void ColliderEnterHandler(string message);
    [HideInInspector]
    public event ColliderEnterHandler Notify;
    public enum TriggerType { reaction, toiletZone}
    public GameObject[] targets;
    public TriggerType type;
    public UnityEvent OnEnter;
    public UnityEvent OnExit;
    public bool randomOnExit = false;
    public bool ignoreDisable = false;
    public GameObject bonusCloud;

    [HideInInspector]
    public float timeFromToilet = 0;
    int targetsInZone = 0;
    // Start is called before the first frame update
    void Start()
    {
        timeFromToilet = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (this.enabled == false)
            return;

        switch (type)
        {
            case TriggerType.reaction:
                if (FindTarget(other.gameObject))
                {
                    targetsInZone++;
                    if (targetsInZone == 1)
                    {
                        //Debug.Log(gameObject.name + " triggered");
                        if (Notify != null)
                            Notify(other.gameObject.name);
                        if (OnEnter != null)
                            OnEnter.Invoke();
                    }
                }
                break;
            case TriggerType.toiletZone:
                if (other.CompareTag("enemy"))
                {
                    if (timeFromToilet == 0)
                    {
                        Camera.main.GetComponent<Follow>().player = other.transform;
                        Debug.Log(Time.time - GameObject.FindObjectOfType<ToiletBar>().timeFromEnable);
                        GameManager.Instance.GameOver();
                    }
                    else
                    {
                        var bonusTime = Time.time - timeFromToilet;
                        GameManager.Instance.data.preToiletSummaryTime += bonusTime;
                        Color startColor = Color.Lerp(Color.green, Color.red, bonusTime/7.5f);
                        var obj = Instantiate(bonusCloud);
                        obj.transform.position = other.transform.position + new Vector3(0, 0, -5f);
                        int a = (int)((7.5f - bonusTime) * 1000 / 7.5f);
                        obj.GetComponent<BonusCloud>().Disappear("x" + a / 10 + "." + a % 10,startColor);
                        Destroy(gameObject);
                    }
                }
                break;
            default:
                break;
        }
                   
    }
    private void OnDisable()
    {
        targetsInZone = 0;
        if (!ignoreDisable)
            if (!randomOnExit)
            {
                if (OnExit != null)
                    OnExit.Invoke();
            }
            else
                RandomOnExit();
    }
    private void OnTriggerExit(Collider other)
    {
        if (this.enabled == false)
            return;
        if (FindTarget(other.gameObject))
        {
            targetsInZone--;
            if (targetsInZone == 0)
                if (!randomOnExit)
                { 
                    if (OnExit != null)
                        OnExit.Invoke();
                }
                else
                    RandomOnExit();
        }
    }

    bool FindTarget(GameObject obj)
    {
        for (int i = 0; i < targets.Length; i++)
            if (targets[i] == obj)
                return true;
        return false;
    }

    public void RandomOnExit()
    {
        if (OnExit == null)
            return;
        var index = Random.Range(0, OnExit.GetPersistentEventCount());
        var obj = OnExit.GetPersistentTarget(index);
        var str = OnExit.GetPersistentMethodName(index);

        if (str == "SetActive")
            (obj as GameObject).SetActive(true);

        if (str == "set_enabled")
            (obj as BoxCollider).enabled = true;
        Debug.Log(obj);
        Debug.Log(str);
    }

    public void SetColliderPosition(Transform pos)
    {
        GetComponent<BoxCollider>().center = pos.position;
    }

    private void OnEnable()
    {
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<BoxCollider>().enabled = true;
    }

    public void DashZAxeAllTargets(float f)
    {
        foreach (var i in targets)
            if (i.GetComponent<NPCController>())
                i.GetComponent<NPCController>().DashZAxe(f);
    }

    public void AddGrain(float f)
    {
        Camera.main.GetComponent<PostProcessingAnimation>().AddGrain(f);
    }

    public void AddTargetCounter(int i)
    {
        foreach (var item in targets)
            foreach (var item2 in item.GetComponents<Counter>())
                item2.AddValue(i);
    }

    public void UnlockAchievement(string name)
    {
        GameManager.UnlockAchievement(name);
    }
}
