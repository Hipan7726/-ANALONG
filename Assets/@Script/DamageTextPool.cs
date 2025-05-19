using UnityEngine;
using System.Collections.Generic;

public class DamageTextPool : MonoBehaviour
{
    public static DamageTextPool Instance;

    public GameObject damageTextPrefab;
    public int poolSize = 20;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(damageTextPrefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetTextObject()
    {
        GameObject obj = pool.Dequeue();
        obj.SetActive(true);
        pool.Enqueue(obj);
        return obj;
    }
}
