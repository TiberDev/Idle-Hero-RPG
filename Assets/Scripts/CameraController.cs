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
    private float yAxis;

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
            cachedTfm.position = Vector3.MoveTowards(cachedTfm.position, tfmBoss.position + offset, movingToBossSpeed * Time.deltaTime);
            return;
        }

        if (tfmHero != null) // follow hero
        {
            Vector3 pos = tfmHero.position + offset;
            cachedTfm.position = new Vector3(pos.x, yAxis, pos.z);
        }
    }

    public void SetTfmHero(Transform tfm)
    {
        tfmHero = tfm;
        yAxis = tfmHero.position.y + offset.y;
    }

    public void SetTfmBoss(Transform tfm)
    {
        tfmBoss = tfm;
    }

    public void MoveDown(bool moving)
    {
        movingDown = moving;
        downPos = cachedTfm.position + Vector3.down * yMovingPos;
    }
}
