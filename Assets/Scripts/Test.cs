using Stopwatch = System.Diagnostics.Stopwatch;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField] private Image[] imageList;
    [SerializeField] private Sprite[] spriteList;
    [SerializeField] private SpriteAtlas spriteAtlas;

    public GameObject tfm;
    private Vector3 vector3;
    public int number;

    //private void Start()
    //{
    //    for (int i = 0; i < imageList.Length; i++)
    //    {
    //        imageList[i].sprite = spriteList[i];
    //        //imageList[i].sprite = spriteAtlas.GetSprite(spriteList[i].name);
    //    }
    //}
    private void Update()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        for (int i = 0; i < number; i++)
        {
            bool d = tfm.activeInHierarchy;
        }
        stopwatch.Stop();
        Debug.Log((float)stopwatch.ElapsedMilliseconds / 1000f);
    }
}



