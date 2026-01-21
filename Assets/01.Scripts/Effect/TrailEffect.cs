using UnityEngine;
using System;

public class TrailEffect : Effect
{
    public float speed = 200f;
    private Vector3 targetPos;
    private Action mOnComplete;

    public void Init(Vector3 startPos, Vector3 target, Action onComplete = null)
    {
        transform.position = startPos;
        targetPos = target;
        mOnComplete = onComplete;
        gameObject.SetActive(true);
    }
    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            Invoke(nameof(ReturnToPool), 0.3f);
        }
    }
    private void ReturnToPool()
    {
        mOnComplete?.Invoke();
        gameObject.SetActive(false);
    }
}
