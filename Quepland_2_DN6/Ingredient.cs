using System;

public class Ingredient
{
    private GameItem item;
    public GameItem Item 
    { 
        get 
        { 
            if (item == null) 
            { 
                item = ItemManager.Instance.GetItemByName(ItemName); 
            } 
            return item; 
        } 
    }
    public string ItemName { get; set; }
    public int Amount { get; set; } = 1;
    public bool DestroyOnUse { get; set; } = true;

    public override string ToString()
    {
        if(Amount == 1)
        {
            return Amount + " " + Item.Name;
        }
        return Amount + " " + GameState.Pluralizer.Pluralize(Item.Name);
    }
}
