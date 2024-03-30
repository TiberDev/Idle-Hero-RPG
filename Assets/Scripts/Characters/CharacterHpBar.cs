using System.Collections;
using BigInteger = System.Numerics.BigInteger;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterHpBar : MonoBehaviour
{
    [SerializeField] private Image imgHpBar;
    [SerializeField] private TMP_Text txtInfo;
    [SerializeField] protected RectTransform rectTfm;
    [SerializeField] private Color colorTurn, colorBossHp, colorTextTurn, colorTextBossHp;

    [SerializeField] private float time;

    private Coroutine corouHpEffect;

    private float curHpBar;
    private void LateUpdate()
    {
        if (txtInfo == null)
            transform.eulerAngles = Vector3.up * 134;
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
    /// In boss stage
    /// </summary>
    public void SetTextInfo()
    {
        txtInfo.text = "BOSS";
    }

    public void SetHpBarColor(Color color)
    {
        imgHpBar.color = color;
    }

    public void SetTurnBarColor(bool bossStage)
    {
        if (bossStage)
        {
            imgHpBar.color = colorBossHp;
        }
        else
        {
            imgHpBar.color = colorTurn;
        }
    }

    public void SetTextTurnBar(bool bossStage)
    {
        if (bossStage)
        {
            txtInfo.color = colorTextBossHp;
        }
        else
        {
            txtInfo.color = colorTextTurn;
        }
    }

    public void SetSize(bool bossStage)
    {
        if (bossStage)
        {
            rectTfm.sizeDelta = new Vector2(900, 75);
        }
        else
        {
            rectTfm.sizeDelta = new Vector2(465, 65);
        }
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
