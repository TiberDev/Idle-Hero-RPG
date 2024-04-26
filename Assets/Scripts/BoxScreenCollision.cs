using System.Collections.Generic;
using UnityEngine;
using ScreenDevice = UnityEngine.Device.Screen;

public class BoxScreenCollision : Singleton<BoxScreenCollision>, ICharacterCollisionHandler
{
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SkillTable skillTable;
    [SerializeField] private ShrinkButton shrinkButton;
    [SerializeField] private float scaleY;

    private List<Character> enemyInBoxList = new List<Character>();
    private Transform cachedTfm;
    private Vector3 top, middle, bottom, right, distance;

    protected override void Awake()
    {
        base.Awake();
        cachedTfm = transform;
    }

    private void Start()
    {
        FindPoints();
        shrinkButton.ShrinkPanel(true);
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

        position = shrinkButton.GetSkillTblePos(false); // middle = top position of skilltable
        ray = mainCamera.ScreenPointToRay(position);
        Physics.Raycast(ray, out hit);
        middle = hit.point;

        position = shrinkButton.GetSkillTblePos(true); // bottom = bottom position of skilltable
        ray = mainCamera.ScreenPointToRay(position);
        Physics.Raycast(ray, out hit);
        bottom = hit.point;

        position.x = ScreenDevice.width;
        position.y = ScreenDevice.height;
        ray = mainCamera.ScreenPointToRay(position);
        Physics.Raycast(ray, out hit);
        right = hit.point;

        // direction
        Vector3 dir = top - cachedTfm.position;
        float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        cachedTfm.eulerAngles = Vector3.up * (-angle + 90);

        distance = (top + middle) / 2 - (top + bottom) / 2;
    }

    /// <summary>
    /// Change size and position of box collider 
    /// </summary>
    public void SetBox(bool isBottom, bool gameInit)
    {
        // position
        if (isBottom)
        {
            if (gameInit)
            {
                Vector3 position = (top + bottom) / 2;
                position.y = 0;
                cachedTfm.position = position;
            }
            else
                cachedTfm.position -= distance;
        }
        else
        {
            if (gameInit)
            {
                Vector3 position = (top + middle) / 2;
                position.y = 0;
                cachedTfm.position = position;
            }
            else
                cachedTfm.position += distance;
        }
        Vector3 bottomPos = isBottom ? bottom : middle;
        // size
        float x = Vector3.Distance(top, right) * 2;
        float z = Vector3.Distance(top, bottomPos);
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
