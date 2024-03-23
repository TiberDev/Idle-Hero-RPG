using Stopwatch = System.Diagnostics.Stopwatch;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField] private Image[] imageList;
    [SerializeField] private Sprite[] spriteList;
    [SerializeField] private SpriteAtlas spriteAtlas;
    [SerializeField] private UITransformController transformController;

    public RectTransform tfm;
    public float number;

    private void Start()
    {
        Vector2 end;
        end = tfm.sizeDelta;
        end.y = 800;
        StartCoroutine(transformController.IEScalingRect(tfm, Vector2.zero, Vector2.one, number, LerpType.EaseInBack));
    }

    //private void Update()
    //{
    //    Stopwatch stopwatch = new Stopwatch();
    //    stopwatch.Start();
    //    for (int i = 0; i < number; i++)
    //    {
    //        Transform d = tfm;
    //    }
    //    stopwatch.Stop();
    //    Debug.Log((float)stopwatch.ElapsedMilliseconds / 1000f);
    //}
}



