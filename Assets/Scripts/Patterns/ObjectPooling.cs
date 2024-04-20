using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : Singleton<ObjectPooling>
{
    private Dictionary<PoolType, List<ObjPool>> objPValue = new Dictionary<PoolType, List<ObjPool>>();

    public GameObject SpawnG0InPool(GameObject prefab, Vector3 position, PoolType type)
    {
        GameObject localGO = null;
        if (objPValue.ContainsKey(type))
        {
            ObjPool objPool = objPValue[type].Find(obj => prefab.name == obj.nameObj);
            if (objPool != null) // Found obj
            {
                if (objPool.objList.Count > 0)
                {
                    localGO = objPool.objList[0];
                    objPool.objList.RemoveAt(0);
                    localGO.SetActive(true);
                    localGO.transform.position = position;
                }
                else
                {
                    localGO = Instantiate(prefab, position, Quaternion.identity);
                    localGO.name = prefab.name;
                }
            }
            else
            {
                objPool = new ObjPool()
                {
                    nameObj = prefab.name,
                    objList = new List<GameObject>()
                };
                objPValue[type].Add(objPool);
                localGO = Instantiate(prefab, position, Quaternion.identity);
                localGO.name = prefab.name;
            }
        }
        else
        {
            ObjPool objPool = new ObjPool()
            {
                nameObj = prefab.name,
                objList = new List<GameObject>()
            };
            objPValue.Add(type, new List<ObjPool> { objPool });
            localGO = Instantiate(prefab, position, Quaternion.identity);
            localGO.name = prefab.name;
        }
        //localGO.transform.position = position;
        return localGO;
    }

    public void RemoveGOInPool(GameObject go, PoolType type)
    {
        if (objPValue.ContainsKey(type))
        {
            go.SetActive(false);
            ObjPool objPool = objPValue[type].Find(obj => go.name == obj.nameObj);
            if (objPool == null)
            {
                Debug.LogError("Gameobject isn't in Pool. Fix bug now!!!");
                return;
            }
            objPool.objList.Add(go);
        }
        else
        {
            Debug.LogError("Can not find the key. Fix bug now!!!");
        }
    }

    public void ClearGOPool()
    {
        objPValue.Clear();
    }
}

public enum PoolType
{
    Hero,
    Enemy,
    Skill,
    Bullet,
    Effect,
}

public class ObjPool
{
    public string nameObj;
    public List<GameObject> objList;
}
