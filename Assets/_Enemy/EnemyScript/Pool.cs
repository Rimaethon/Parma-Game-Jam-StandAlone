using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Pool : MonoBehaviour
{
    public GameObject pooledObj;
    public List<GameObject> objs = new List<GameObject>();
    public int pooledAmount = 20;
    public Vector3 poolDist;
    public List<Transform> pooledobjTrans = new List<Transform>();

    private void Start()
    {
        CreatePool();
    }

    public void CreatePool()
    {
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject temp = Instantiate(pooledObj, pooledobjTrans[0].position, Quaternion.identity);
            objs.Add(temp);
            pooledobjTrans.Add(temp.transform);


        }
    }
    public void ReCreate(GameObject enemy)
    {
        if(!enemy.activeInHierarchy)
        {
            enemy.transform.position = pooledobjTrans[Random.Range(0, pooledobjTrans.Count)].position;
            enemy.SetActive(true);
        }

       




        //for (int i = 0; i < objs.Count; i++)
        //{
        //    if (!objs[i].activeInHierarchy)
        //    {
        //        objs[i].transform.position = transform.position + poolDist;
        //        objs[i].SetActive(true);

        //        break;
        //    }
        //}
    }

    public void Die(GameObject enemy)
    {
        int rand = Random.Range(0, objs.Count);
        objs[rand].SetActive(false);
        ReCreate(enemy);
    }
}

