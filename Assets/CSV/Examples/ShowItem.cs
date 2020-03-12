using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowItem : MonoBehaviour
{
    public Image image;
    public Image background;
    public Text itemName;
    public Text category;
    public Text price;

    public void SetFrom(ShopItem item)
    {
        Button button = GetComponent<Button>();
        AudioSource click = GetComponent<AudioSource>();
        background.material = item.material;
        image.sprite = item.image;
        image.color = item.colour;
        itemName.text = item.name;
        category.text = item.category.ToString();
        price.text = item.price.ToString();
        button.interactable = item.available;
        click.clip = item.sound;
    }
}
