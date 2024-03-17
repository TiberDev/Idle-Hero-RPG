using System.Collections.Generic;
using UnityEngine;
using ScreenDevice = UnityEngine.Device.Screen;

[ExecuteInEditMode]
public class Test : MonoBehaviour
{
    public GameObject go;
    public int bigNumber;
    public int curNumber;
    public ParticleSystem ps;

    public List<CharacterCollision> characters = new List<CharacterCollision>();
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

    public Transform tfmBottom;
    public Transform top, bottom, left, right, box;
    public RectTransform rect;
    public BoxCollider boxCollider;

    [ContextMenu("Test")]
    public void Testt()
    {
        Debug.Log(tfmBottom.position);
    }
    public void Set()
    {
        Vector3 position = new Vector3(0, 0, 0);
        position.x = ScreenDevice.width / 2;
        position.y = ScreenDevice.safeArea.height;
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        position.z = hit.distance;
        top.position = hit.point; 
        //Debug.Log(hit.distance);
        //top.position = Camera.main.ScreenToWorldPoint(position);

        position.y = ScreenDevice.height / 2;
        ray = Camera.main.ScreenPointToRay(position);
        Physics.Raycast(ray, out hit);
        position.z = hit.distance;
        bottom.position = hit.point;
        //Debug.Log(hit.distance);
        //bottom.position = Camera.main.ScreenToWorldPoint(position);
        //SetBox(top.position);

        position.x = ScreenDevice.width;
        ray = Camera.main.ScreenPointToRay(position);
        Physics.Raycast(ray, out hit);
        position.z = hit.distance;
        right.position = hit.point;
        //Debug.Log(hit.distance);
        //right.position = Camera.main.ScreenToWorldPoint(position);
        SetBox(top.position);
    }

    private void SetBox(Vector3 targetPos)
    {
        // direction
        Vector3 dir = targetPos - box.position;
        float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        box.eulerAngles = new Vector3(0, -angle + 90, 0);
        // position
        box.position = (top.position + bottom.position) / 2;
        box.position = new Vector3(box.position.x, 0, box.position.z);
        // size
        float z = Vector3.Distance(top.position, bottom.position);
        boxCollider.size = Vector3.forward * z;

        float x = Vector3.Distance(bottom.position,right.position) * 2;
        boxCollider.size = new Vector3(x, 0, z);
    }
}



