using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject go;
    public int bigNumber;
    public int curNumber;
    public void Start()
    {

        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();
        //BigInteger number = BigInteger.Parse(bigNumber);
        //UnityEngine.Debug.Log(Marshal.SizeOf(number));
        //UnityEngine.Debug.Log(FillData.Instance.FormatNumber(number));
        //stopwatch.Stop();
        //float time = stopwatch.ElapsedMilliseconds / 1000f;
        //UnityEngine.Debug.Log(time);

    }
    private void Update()
    {
        if (curNumber == bigNumber)
            return;

        curNumber++;
        Instantiate(go);

    }
}



