using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReelItem {

    public Sprite itemImage;
    public Sprite itemBlurImage;
    public SpriteRenderer spriteRenderer;
    public string itemName;
    public float itemOdds;
    public float itemIntegerOdds;
    public int buffer = 0; // number of next spawns that must not contain this item

	// Use this for initialization
	public ReelItem()
    {
        setName("NULL");
    }

    public void setOdds(float odds)
    {
        itemOdds = odds;
        itemIntegerOdds = odds * 100;
    }

    public float getOdds()
    {
        return itemOdds;
    }

    public float getIntegerOdds()
    {
        return itemIntegerOdds;
    }

    public void setName(string name)
    {
        itemName = name;
    }

    public string getName()
    {
        return itemName;
    }

    public void setImage(Sprite icon)
    {
        itemImage = icon;
    }

    public Sprite getImage()
    {
        return itemImage;
    }

    public void setBlurImage(Sprite icon)
    {
        itemBlurImage = icon;
    }

    public Sprite getBlurImage()
    {
        return itemBlurImage;
    }

    public void setBuffer(int buff)
    {
        buffer = buff;
    }

    public int getBuffer()
    {
        return buffer;
    }

    
}
