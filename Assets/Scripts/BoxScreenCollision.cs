using System.Collections.Generic;
using UnityEngine;
using ScreenDevice = UnityEngine.Device.Screen;

public class BoxScreenCollision : Singleton<BoxScreenCollision>, ICharacterCollisionHandler
{
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SkillTable skillTable;
    [SerializeField] private Transform tfmBottom;
    [SerializeField] private float scaleY;

    private List<Character> enemyInBoxList = new List<Character>();
    private Transform cachedTfm;
    private Vector3 top, bottom, right;

    private void Start()
    {
        cachedTfm = transform;
        FindPoints();
        SetBox();
        cachedTfm.SetParent(mainCamera.transform);
        gameObject.layer = 6; // layer = hero
    }

    private void FindPoints()
    {
        Vector3 position = Vector3.zero;
        position.x = ScreenDevice.width / 2;
        position.y = ScreenDevice.height;
        Ray ray = mainCamera.ScreenPointToRay(position);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        top = hit.point;

        position = tfmBottom.position;
        ray = mainCamera.ScreenPointToRay(position);
        Physics.Raycast(ray, out hit);
        bottom = hit.point;

        position.x *= 2;
        ray = mainCamera.ScreenPointToRay(position);
        Physics.Raycast(ray, out hit);
        right = hit.point;
    }

    private void SetBox()
    {
        // direction
        Vector3 dir = top - cachedTfm.position;
        float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        cachedTfm.eulerAngles = new Vector3(0, -angle + 90, 0);
        // position
        cachedTfm.position = (top + bottom) / 2;
        cachedTfm.position = new Vector3(cachedTfm.position.x, 0, cachedTfm.position.z);
        // size
        float z = Vector3.Distance(top, bottom);
        boxCollider.size = Vector3.forward * z;

        float x = Vector3.Distance(bottom, right) * 2;
        boxCollider.size = new Vector3(x, scaleY, z);
    }

    void ICharacterCollisionHandler.HandleCollision(Character character)
    {
        if (character.IsDead())
            return;

        enemyInBoxList.Add(character);
        skillTable.HandleAutomatic();
    }

    void ICharacterCollisionHandler.HandleEndCollision(Character character)
    {
        enemyInBoxList.Remove(character);
    }

    public void ClearEnemyInBox()
    {
        enemyInBoxList.Clear();
    }

    public bool IsEnenmiesEmpty()
    {
        return enemyInBoxList.Count <= 0;
    }
}
