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

    [SerializeField] private Vector3 rotationOffset;
    [SerializeField] private Color colorTurn, colorBossHp, colorTextTurn, colorTextBossHp;
    [SerializeField] private float time;
    [SerializeField] private float offsetY;

    private Coroutine corouHpEffect;
    private Transform cachedTfm;
    private Character character;

    private float curPoint;

    private void Awake()
    {
        cachedTfm = transform;
    }

    private void Start()
    {
        character = GetComponentInParent<Character>();
    }

    private void Update()
    {
        if (txtInfo == null)
        {
            Vector3 position = character.GetTransform().position;
            position.y += offsetY;
            cachedTfm.eulerAngles = rotationOffset;
            cachedTfm.position = position + cachedTfm.forward * -20; // place hp bar near camera
        }
    }

    private IEnumerator IEHpEffect(float desireHp, float maxHp)
    {
        float curTime = time;
        float tempHp = curPoint;
        while (curTime > 0)
        {
            curTime -= Time.deltaTime;
            curPoint = Mathf.Lerp(desireHp, tempHp, curTime / time);
            imgHpBar.fillAmount = curPoint / maxHp;
            yield return null;
        }
    }

    public void SetFillAmountUI(BigInteger curHp, BigInteger maxHp, bool isEffect)
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
            curPoint = newCurHp;
            imgHpBar.fillAmount = newCurHp / newMaxHp;
            if (corouHpEffect != null)
                StopCoroutine(corouHpEffect);
            return;
        }
        if (corouHpEffect != null)
            StopCoroutine(corouHpEffect);
        corouHpEffect = StartCoroutine(IEHpEffect(newCurHp, newMaxHp));
    }

    public void SetFillAmountUI(int curWave, int totalWave, bool isEffect)
    {
        if (!isEffect)
        {
            curPoint = curWave;
            imgHpBar.fillAmount = curPoint / totalWave;
            if (corouHpEffect != null)
                StopCoroutine(corouHpEffect);
            return;
        }
        if (corouHpEffect != null)
            StopCoroutine(corouHpEffect);
        corouHpEffect = StartCoroutine(IEHpEffect(curWave, totalWave));
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
}
