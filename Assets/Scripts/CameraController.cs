using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 offset;

    public Transform tfmHero;

    private Transform cachedTfm;

    private void Awake()
    {
        cachedTfm = transform;
    }

    private void Update()
    {
        if(tfmHero != null)
        {
            cachedTfm.position = tfmHero.position + offset;
        }
    }

    public void SetTfmHero(Transform tfm)
    {
        tfmHero = tfm;
    }
}
