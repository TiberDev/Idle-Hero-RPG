using System;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float movingToBossSpeed;
    [SerializeField] private float movingDownTime;
    [SerializeField] private int yMovingPos;

    private Transform cachedTfm, tfmHero, tfmBoss;

    private Vector3 downPos;
    private bool movingDown;

    private void Awake()
    {
        cachedTfm = transform;
    }

    private void LateUpdate()
    {
        if (movingDown) // move down to reload turn and round
        {
            cachedTfm.position = Vector3.MoveTowards(cachedTfm.position, downPos, movingDownTime * Time.deltaTime);
            return;
        }

        if (tfmBoss != null) // follow boss
        {
            //cachedTfm.position = Vector3.MoveTowards(cachedTfm.position, tfmBoss.position + offset, movingToBossSpeed * Time.deltaTime);
            return;
        }

        if (tfmHero != null) // follow hero
        {
            Vector3 pos = tfmHero.position + offset;
            cachedTfm.position = pos;
        }
    }

    private IEnumerator IEMoveToBossView()
    {
        float curTime = 0;
        Vector3 fromPos = cachedTfm.position;
        while (tfmBoss != null)
        {
            if (curTime < movingToBossSpeed)
            {
                curTime += Time.deltaTime;
                cachedTfm.position = Vector3.Lerp(fromPos, tfmBoss.position + offset, curTime / movingToBossSpeed);
            }
            else
                cachedTfm.position = tfmBoss.position + offset;
            yield return null;
        }
    }

    public void SetTfmHero(Transform tfm)
    {
        tfmHero = tfm;
    }

    public void SetTfmBoss(Transform tfm)
    {
        tfmBoss = tfm;
        if (tfmBoss == null)
        {
            StopAllCoroutines();
        }
        else
        {
            StartCoroutine(IEMoveToBossView());
        }
    }

    public void MoveDown(bool moving)
    {
        movingDown = moving;
        downPos = cachedTfm.position + Vector3.down * yMovingPos;
    }
}
