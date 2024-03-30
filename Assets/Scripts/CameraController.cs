using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float movingToBossSpeed;

    private Transform cachedTfm, tfmHero, tfmBoss;

    private float yAxis;

    private void Awake()
    {
        cachedTfm = transform;
    }

    private void Update()
    {
        if (tfmBoss != null)
        {
            cachedTfm.position = Vector3.MoveTowards(cachedTfm.position, tfmBoss.position + offset, movingToBossSpeed * Time.deltaTime);
            return;
        }

        if (tfmHero != null)
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
}
