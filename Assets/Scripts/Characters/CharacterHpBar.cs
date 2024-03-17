using System.Collections;
using BigInteger = System.Numerics.BigInteger;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterHpBar : MonoBehaviour
{
    [SerializeField] private Image imgHpBar;
    [SerializeField] private float time;
    [SerializeField] private TMP_Text txtInfo;

    private Coroutine corouHpEffect;
    private float curHpBar;

    private void LateUpdate()
    {
        if (txtInfo == null)
            transform.eulerAngles = Vector3.up * 135;
    }

    public void SetHpUI(BigInteger curHp, BigInteger maxHp, bool isEffect)
    {
        int exponential_1 = 0, exponential_2 = 0;
        while (curHp > (BigInteger)float.MaxValue)
        {
            exponential_1++;
            curHp /= 1000;
        }
        while (maxHp > (BigInteger)float.MaxValue)
        {
            exponential_2++;
            maxHp /= 1000;
        }
        int exponential = exponential_2 - exponential_1;
        BigInteger result = BigInteger.Pow(1000, exponential);
        float newMaxHp = (float)maxHp;
        float newCurHp = (float)curHp;
        if (result > (BigInteger)float.MaxValue)
        {
            newCurHp = 0;
        }
        else
        {
            newCurHp /= Mathf.Pow(1000, exponential);
        }

        if (!isEffect)
        {
            curHpBar = newCurHp;
            imgHpBar.fillAmount = newCurHp / newMaxHp;
            if (corouHpEffect != null)
                StopCoroutine(corouHpEffect);
            return;
        }
        if (corouHpEffect != null)
            StopCoroutine(corouHpEffect);
        corouHpEffect = StartCoroutine(IEHpEffect(newCurHp, newMaxHp));
    }

    /// <summary>
    /// Show text round, turn 
    /// </summary>
    /// <param name="text"></param>
    public void SetTextInfo(int round, int turn)
    {
        txtInfo.text = $"{round} - {turn}";
    }

    /// <summary>
    /// In boss turn
    /// </summary>
    public void SetTextInfo()
    {
        txtInfo.text = "BOSS";
    }

    public void SetHpBarColor(Color color)
    {
        imgHpBar.color = color;
    }

    private IEnumerator IEHpEffect(float desireHp, float maxHp)
    {
        float curTime = time;
        float tempHp = curHpBar;
        while (curTime > 0)
        {
            curTime -= Time.deltaTime;
            curHpBar = Mathf.Lerp(desireHp, tempHp, curTime / time);
            imgHpBar.fillAmount = curHpBar / maxHp;
            yield return null;
        }
    }
}
