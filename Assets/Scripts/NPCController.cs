using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPCController : MonoBehaviour
{
    public float walkSpeed;
    public bool isRepeat;
    public bool isLeftOnStart;
    public bool randomDrink = false;
    public bool randomLaugh = false;
    public bool randomPlay = false;
    public bool followOnEnable = false;
    public Transform[] points;
    public UnityEvent[] Actions;
    public Transform lookAt;
    public Vector3 offset;
    PlayerController playerController;
    IEnumerator curCoroutine;
    GameObject scaleController;
    bool freeze;
    List<Transform> exclusivePoints = new List<Transform>();
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().SetFloat("randomIdleStart", Random.Range(0.0f, 1.0f));
        playerController = GetComponent<PlayerController>();
        if (playerController)
        {
            walkSpeed = playerController.walkSpeed;
        }
        if (isLeftOnStart)
            GetComponent<Animator>().SetTrigger("turnLeft");
    }
    private void OnEnable()
    {
        StartCoroutine("RandomDrink");
        StartCoroutine("RandomLaugh");
        StartCoroutine("RandomPlay");

        if (followOnEnable)
            FollowPath(0);
    }
    IEnumerator RandomDrink()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3, 15));
            if (randomDrink)
                GetComponent<Animator>().SetTrigger("drink");
        }
    }
    IEnumerator RandomPlay()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3, 15));
            if (randomPlay)
                GetComponent<Animator>().SetTrigger("play");
        }
    }
    IEnumerator RandomLaugh()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3, 15));
            if (randomLaugh)
                GetComponent<Animator>().SetTrigger("laugh");
        }
    }
    public void SetItemSprite(Sprite sourse)
    {
        transform.Find("Тело").Find("Бутылка").GetComponent<SpriteRenderer>().sprite = sourse;
        transform.Find("Рука").Find("Бутылка (1)").GetComponent<SpriteRenderer>().sprite = sourse;
    }

    public void ItemActive(bool b)
    {
        transform.Find("Тело").Find("Бутылка").GetComponent<SpriteRenderer>().enabled = b;
        transform.Find("Рука").Find("Бутылка (1)").GetComponent<SpriteRenderer>().enabled = b;
    }

    // Update is called once per frame
    void Update()
    {
        if (exclusivePoints.Count != 0)
            FollowExclusive();
        TurnAtTarget();
    }

    void FollowExclusive()
    {
        if (curCoroutine == null)
        {
            curCoroutine = MoveTo(0, false, exclusivePoints[0]);
            exclusivePoints.RemoveAt(0);
            StartCoroutine(curCoroutine);
        }
    }
    public void AddExclusive(Transform dest)
    {
        exclusivePoints.Add(dest);
    }

    void TurnAtTarget()
    {
        var anim = GetComponent<Animator>();

        if (lookAt)
            if (lookAt.position.x - transform.position.x > 0)
                if (Mathf.Abs(anim.GetFloat("speed")) > 0.5f)
                    anim.SetFloat("speed", 1f);
                else
                    anim.SetFloat("speed", 0.01f);
            else
                if (Mathf.Abs(anim.GetFloat("speed")) > 0.5f)
                anim.SetFloat("speed", -1f);
            else
                anim.SetFloat("speed", -0.01f);
    }

    public void SetWalkSpeed(float f)
    {
        if (f < 0)
            walkSpeed *= -f;
        else
            walkSpeed = f;
    }
    public void Run(bool b)
    {
        GetComponent<Animator>().SetBool("run", b);
    }
    public void Squat(bool b)
    {
        GetComponent<Animator>().SetBool("squat", b);
    }
    public void Wash(bool b)
    {
        GetComponent<Animator>().SetBool("wash", b);
    }
    public void Prey(bool b)
    {
        GetComponent<Animator>().SetBool("prey", b);
    }
    public void Sit(bool b)
    {
        GetComponent<Animator>().SetBool("sit", b);
        if (b)
            transform.localPosition += new Vector3(0, 0.52f, 0);
        else
            transform.localPosition -= new Vector3(0, 0.52f, 0);
    }
    public void Ladder(bool b)
    {
        GetComponent<Animator>().SetBool("ladder", b);

        if(GetComponent<Stepper>())
            GetComponent<Stepper>().enabled = !b;
    }
    public void RandomDrink(bool b)
    {
        randomDrink = b;
    }
    public void SwitchRun(float time)
    {
        var anim = GetComponent<Animator>();
        var b = anim.GetBool("run");
        anim.SetBool("run", !b);
        GameManager.Instance.Invoke(Run, b, time);
    }
    public void IdleItem(bool b)
    {
        GetComponent<Animator>().SetBool("idleItem", b);
    }
    public void SetTarget(Transform target = null)
    {
        if (target == transform)
            target = null;
        lookAt = target;

        TurnAtTarget();
    }

    public void Freeze(float time)
    {
        freeze = true;
        Invoke("Unfreeze", time);
    }
    void Unfreeze()
    {
        freeze = false;
    }
    public void ReversePath()
    {
        Transform buf;
        for (int i = 0; i < points.Length / 2; i++)
        {
            buf = points[i];
            points[i] = points[points.Length - i - 1];
            points[points.Length - i - 1] = buf;
        }
    }
    public void FollowPath(int n = 1)
    {
        if (curCoroutine == null)
        {
            curCoroutine = MoveTo(n);
            StartCoroutine(curCoroutine);
        }
    }
    public void StopFollowPath()
    {
        if (curCoroutine != null)
            StopCoroutine(curCoroutine);
        curCoroutine = null;
        GetComponent<Animator>().SetFloat("speed", 0);
        if (playerController)
            playerController.freeze = false;
    }
    public void FollowPoint(int n)
    {
        if (curCoroutine == null)
        {
            curCoroutine = MoveTo(n, false);
            StartCoroutine(curCoroutine);
        }
    }

    public void LoadPath(Transform[] newPoints)
    {
        points = newPoints;
    }
    public void Teleport(Transform destination)
    {
        transform.position = destination.position + offset;
        SetScale(destination.lossyScale.y);
    }

    IEnumerator MoveTo(int point, bool isPath = true, Transform destination = null)
    {
        if (playerController)
            playerController.freeze = true;

        while (freeze)
            yield return null;          
        
        Vector3 sourcePos = transform.position;

        if (!destination)
            destination = points[point];
        else
        {
            point = Actions.Length;
            isPath = false;
        }
        
        float curTime = 0;

        if ((destination.position + offset * transform.lossyScale.y).x - sourcePos.x > 0)
            GetComponent<Animator>().SetFloat("speed", 1);
        else if ((destination.position + offset * transform.lossyScale.y).x - sourcePos.x < 0)
            GetComponent<Animator>().SetFloat("speed", -1);

        float curLocalScale;
        float startLossyScale;
        if (scaleController)
        {
            curLocalScale = scaleController.transform.localScale.y;
            startLossyScale = scaleController.transform.lossyScale.y;
        }
        else
        {
            curLocalScale = 1;
            startLossyScale = transform.parent ? transform.parent.lossyScale.y : 1;
        }

        float endLossyScale = destination.lossyScale.y;
        //Debug.Log("StartLossyScale is " + startLossyScale + "; EndLossyScale is " + endLossyScale);

        float difference = endLossyScale / startLossyScale;
        var timeForArrive = Magnitude2D(destination.position + offset * transform.lossyScale.y - sourcePos) / walkSpeed;

        float averageScale = endLossyScale + startLossyScale;
        averageScale /= 2;

        timeForArrive /= averageScale;

        while (curTime < timeForArrive)
        {
            transform.position = Vector3.Lerp(sourcePos, destination.position + offset * transform.lossyScale.y, curTime / timeForArrive);
            if (!Mathf.Approximately(difference,1))
                SetScale(Mathf.Lerp(curLocalScale, curLocalScale*difference, curTime / timeForArrive));
            yield return null;
            curTime += Time.deltaTime;
        }
        transform.position = destination.position + offset * transform.lossyScale.y;
        SetScale(curLocalScale * difference);

        curCoroutine = null;

        if (point < Actions.Length && Actions[point].GetPersistentEventCount() != 0)
            isPath = false;

        point++;        

        if (isPath)
        {
            if (point < points.Length)
                FollowPath(point);
            else if (isRepeat)
                FollowPath(0);
            else
            {
                GetComponent<Animator>().SetFloat("speed", 0);
                if (playerController)
                    playerController.freeze = false;
            }
        }
        else
        {
            GetComponent<Animator>().SetFloat("speed", 0);

            if (exclusivePoints.Count != 0)
                FollowExclusive();
            else if (playerController)
                playerController.freeze = false;

            if (point - 1 < Actions.Length)
                Actions[point - 1].Invoke();
        }
    }
    float Magnitude2D(Vector3 v)
    {
        return v.magnitude;
    }

    public void GameOver(Transform player)
    {
        StopFollowPath();
        if (transform.position.x - player.position.x > 0)
            GetComponent<Animator>().SetTrigger("turnLeft");
        else
            GetComponent<Animator>().SetTrigger("turnRight");
        GetComponent<Animator>().SetBool("gameOver", true);
    }
    public void SetParent(GameObject parent)
    {
        if (gameObject.name != "Player" && parent == gameObject && !transform.parent.GetComponent<RoomManager>().enable)
            transform.parent.GetComponent<RoomManager>().EnableRoom(transform);

        if (gameObject.name != "Player" && parent != gameObject && !parent.GetComponent<RoomManager>().enable)
            parent.GetComponent<RoomManager>().DisableRoom(transform);

        Transform parentTransform;
        if (parent != gameObject && parent != null)
            parentTransform = parent.transform;
        else
            if (scaleController)
            parentTransform = scaleController.transform.parent ? scaleController.transform.parent.parent : null;
        else
            parentTransform = transform.parent ? transform.parent.parent : null;

        float curLossy = scaleController ? scaleController.transform.lossyScale.y : transform.lossyScale.y;
        float parentLossy = parentTransform ? parentTransform.lossyScale.y : 1;

        //Debug.Log("curLossye is " + curLossy + "; parentLossy is " + parentLossy);
        SetScale(1);

        transform.parent = parentTransform;
        SetScale(curLossy / parentLossy);
    }
    public void SetScale(float s)
    {
        if (scaleController)
        {
            transform.parent = transform.parent.parent;
            Destroy(scaleController);
            scaleController = null;
        }
        if (s == 1)
            return;
        scaleController = new GameObject();
        scaleController.transform.position = transform.position;
        scaleController.transform.parent = transform.parent;
        transform.parent = scaleController.transform;
        scaleController.name = (gameObject.name + " -scale-control");
        scaleController.transform.localScale = new Vector3(s, s, s);
    }
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
    public void SetSilenceHead(Sprite head)
    {
        transform.Find("Голова_беспокоится_1").Find("Голова_беспокоится_2").GetComponent<SpriteRenderer>().sprite = head;
        transform.Find("Голова_беспокоится_1").Find("Голова_беспокоится_1").GetComponent<SpriteRenderer>().sprite = head;
        transform.Find("Голова_беспокоится_1").GetComponent<SpriteRenderer>().sprite = head;
    }
    public void SetTalkHead(Sprite head)
    {
        transform.Find("Голова_беспокоится_1").Find("Голова_говорит").GetComponent<SpriteRenderer>().sprite = head;
    }
    public void SetHorrorHead(Sprite head)
    {
        transform.Find("Голова_беспокоится_1").Find("Голова_ужас").GetComponent<SpriteRenderer>().sprite = head;
    }

    private void OnDisable()
    {
        StopFollowPath();
        StopAllCoroutines();
        transform.localScale = new Vector3(1, 1, 1);
    }

    public void CleanerSmartDisable(Transform target)
    {
        for (int i = 0; i < target.childCount; i++)
        {
            foreach (var j in target.GetChild(i).GetComponents<SimpleTrigger>())
                if (j.targets[0].name == "Player")
                    j.enabled = false;
        }
    }

    public void CleanerSmartEnable(Transform target)
    {
        for (int i = 0; i < target.childCount; i++)
        {
            foreach (var j in target.GetChild(i).GetComponents<SimpleTrigger>())
                if (j.targets[0].name == "Player")
                    j.enabled = true;
        }
    }


    public void DashZAxe(float f)
    {
        transform.position += new Vector3(0,0,f);
    }

    public void SwitchInteractObjectComponent()
    {
        foreach (var i in GetComponents<InteractObject>())
        {
            if (i.enabled)
                Destroy(i);
            else
                i.enabled = true;
        }
    }

    public void AnimatorSpeed(float f)
    {
        GetComponent<Animator>().SetFloat("speed", f);
    }
}
