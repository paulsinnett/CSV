using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopItem
{
    public string name;
    public int price;
    public Sprite image;
    public Color colour;
    public bool available;
    public Category category;
    public Material material;
    public AudioClip sound;
    public enum Category
    {
        Unknown,
        Baked,
        Drink,
        Meat,
        Fruit,
        Dairy
     }
}
