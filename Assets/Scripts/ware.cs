using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ware", menuName = "Ware")]
public class ware : ScriptableObject
{
    public Sprite icon;
    public recipe recipe;

    public string name;
    public string description;
    public int price;
    public int id = 0;

    public ware ()
    {
        this.name = "ware";
        this.description = "A ware that was created by the default constructor.";
        this.price = 10;
    }
    public ware(string name_, int price_, string description_)
    {
        this.name = name_;
        this.description = description_;
        this.price = price_;
    }
    public ware(string name_, int price_, string description_, int _id)
    {
        this.name = name_;
        this.description = description_;
        this.price = price_;
        this.id = _id;
    }

    public void changePrice (int price_)
    {
        this.price += price_;
    }
}
