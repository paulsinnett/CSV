using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowStock : MonoBehaviour
{
    public Stock shop;
    public ShowItem prefab;

    void Start()
    {
        foreach (var item in shop.stock)
        {
            ShowItem line = Instantiate(prefab, transform);
            line.SetFrom(item);
        }
    }
}
