using System.Collections;
using UnityEngine;

public class BossStage : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private BoxScreenCollision boxScreenCollision;
    [SerializeField] private SkillTable skillTable;
    [SerializeField] private CharacterHpBar turnBar;
    [SerializeField] private RectTransform rectTfmTurnBar;
    [SerializeField] private GameObject gObjTurnBar;

    [SerializeField] private float followingBossTime, movingTime;

    private Character boss;

    private IEnumerator IECountTimeFollowingBoss()
    {
        yield return new WaitForSeconds(followingBossTime);
        EndBossStage();
        yield return new WaitForSeconds(0.5f);
        MoveOutScreen();
    }

    private void SetInfoBoss()
    {
        turnBar.SetTextTurnBar(true);
        turnBar.SetSize(true);
        turnBar.SetTurnBarColor(true);
        boss.SetHpBar(turnBar);
        turnBar.SetTextInfo();
        MoveInScreen();
    }

    /// <summary>
    /// Hp bar move out screen when game in boss stage
    /// </summary>
    private void MoveOutScreen()
    {
        Vector2 endPos = rectTfmTurnBar.anchoredPosition;
        endPos.x = -1500;
        StartCoroutine(UITransformController.Instance.IEMovingRect(rectTfmTurnBar, rectTfmTurnBar.anchoredPosition, endPos, movingTime, LerpType.Liner, SetInfoBoss));
    }

    /// <summary>
    /// Hp bar move in screen when game in boss stage
    /// </summary>
    private void MoveInScreen()
    {
        Vector2 startPos = rectTfmTurnBar.anchoredPosition;
        startPos.x = 1500;
        Vector2 endPos = startPos;
        endPos.x = 0;
        StartCoroutine(UITransformController.Instance.IEMovingRect(rectTfmTurnBar, startPos, endPos, movingTime, LerpType.Liner));
    }

    private void EndBossStage()
    {
        cameraController.SetTfmBoss(null);
        boxScreenCollision.gameObject.SetActive(true);
        skillTable.ResetAllSkillTableItem();
    }

    public void StartBossStage(Character _boss)
    {
        // turn bar gobj is inactive because board is transaprent or opaque
        if (!gObjTurnBar.activeInHierarchy)
            return;

        boss = _boss;
        // changes camera view
        cameraController.SetTfmBoss(boss.GetTransform());
        boxScreenCollision.gameObject.SetActive(false);
        StartCoroutine(IECountTimeFollowingBoss());
    }

    public void TerminateExcecution()
    {
        StopAllCoroutines();
        cameraController.SetTfmBoss(null);
        boxScreenCollision.gameObject.SetActive(true);

        Vector2 pos = rectTfmTurnBar.anchoredPosition;
        pos.x = 0;
        rectTfmTurnBar.anchoredPosition = pos;
    }
}
