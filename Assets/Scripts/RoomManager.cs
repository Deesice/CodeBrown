using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public bool enable { get; private set; }
    public float appearanceTime = 0.5f;
    [SerializeField] bool hideOnStart = true;
    Dictionary<Transform, IEnumerator> coroutines = new Dictionary<Transform, IEnumerator>();
    void Awake()
    {
        foreach (var npc in GetComponentsInChildren<NPCController>(true))
        {
            foreach (var sprite in npc.GetComponentsInChildren<SpriteRenderer>(true))
            {
                if (!sprite.enabled)
                {
                    sprite.gameObject.tag = "Outline";
                }
            }
        }
    }
    void Start()
    {
        var buf = appearanceTime;
        appearanceTime = 0;
        if (hideOnStart)
            DisableRoom(transform);
        appearanceTime = buf;
    }
    public void DisableRoom()
    {
        DisableRoom(null);
    }
    public void DisableRoom(Transform parent)
    {
        if (parent == null || parent == transform)
        {
            parent = transform;
            enable = false;
        }
        
        var speed = appearanceTime > 0 ? 1 / appearanceTime : 1000000;
        var sprites = parent.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var i in sprites)
            if (coroutines.TryGetValue(i.transform, out var value1))
            {
                coroutines.Remove(i.transform);
                StopCoroutine(value1);
            }
        var coroutine = Fading(sprites, 0, speed, parent);
        if (coroutines.TryGetValue(parent, out var value))
        {
            coroutines.Remove(parent);
            StopCoroutine(value);
        }
        coroutines.Add(parent, coroutine);
        StartCoroutine(coroutine);
    }
    public void EnableRoom()
    {
        EnableRoom(null);
    }
    public void EnableRoom(Transform parent)
    {
        if (parent == null || parent == transform)
        {
            parent = transform;
            enable = true;
        }

        var speed = appearanceTime > 0 ? 1 / appearanceTime : 1000000;
        var sprites = parent.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var i in sprites)
            if (coroutines.TryGetValue(i.transform, out var value1))
            {
                coroutines.Remove(i.transform);
                StopCoroutine(value1);
            }
        var coroutine = Fading(sprites, 1, speed, parent);
        if (coroutines.TryGetValue(parent, out var value))
        {
            coroutines.Remove(parent);
            StopCoroutine(value);
        }
        coroutines.Add(parent, coroutine);
        StartCoroutine(coroutine);
    }
    IEnumerator Fading(SpriteRenderer[] sprites, float targetAlpha, float fadeSpeed, Transform coroutineKey)
    {
        if (targetAlpha > 0)
            foreach (var i in sprites)
            {
                if (i.CompareTag("Outline"))
                    continue;
                i.enabled = true;
            }

        bool flag = true;
        while (flag)
        {
            yield return null;
            flag = false;
            foreach (var sprite in sprites)
            {
                if (sprite.CompareTag("Outline"))
                    continue;

                var prevColor = sprite.color;
                if (prevColor.a != targetAlpha)
                {
                    prevColor.a = Mathf.Lerp(prevColor.a, targetAlpha, Time.deltaTime * fadeSpeed / Mathf.Abs(prevColor.a - targetAlpha));
                    sprite.color = prevColor;
                    flag = true;
                }
            }
        }
        if (targetAlpha == 0)
            foreach (var i in sprites)
            {
                if (i.CompareTag("Outline"))
                    continue;
                i.enabled = false;
            }

        coroutines.Remove(coroutineKey);
    }
}
