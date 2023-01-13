
public interface ISpell
{
    /// <summary>
    /// The name of the spell.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// What the spell does. Appears on the spell screen.
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Used by Cast to determine how strong the spell is at what it does. Allows for different power levels of a similar effect.
    /// </summary>
    public int Power { get; set; }
    /// <summary>
    /// The message sent when the effect is first applied.
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// How long the spell should last, in game ticks.
    /// </summary>
    public int Duration { get; set; }
    /// <summary>
    /// Time until spell expires, in seconds.
    /// </summary>
    public int TimeRemaining { get; set; }
    public int Cooldown { get; set; } 
    public int CooldownRemaining { get; set; }
    public string Target { get; set; }

    public string Data { get; set; } 
	public bool Unlocked { get; set; } 

    public void Cast() { }
    public void Cast(Monster m) { }
    public void Cast(Player player) { }
    public void Cast(Inventory inventory, GameItem item) { }
    public void Cast(Inventory inventory) { }
    public void Cast(Recipe recipe) { }

    public void Tick() { }
    public void Tick(Monster m) { }
    public void Tick(Player player) { }
    public void Tick(Inventory inventory, GameItem item) { }
    public void Tick(Inventory inventory) { }
    public void Tick(Recipe recipe) { }

    public List<Ingredient> Cost { get; set; }
    public ISpell Copy();

    public ISpell LoadData(SpellData data)
    {
        Description = data.Description;
        Power = data.Power;
        Message = data.Message;
        Duration = data.Duration;
        Target = data.Target;
        Data = data.Data;
        Cooldown = data.Cooldown;
        Cost = data.Cost;
        CooldownRemaining = -1;
        return this;
    }
    public string GetCostText()
    {
        string t = GetMPCost() + " MP\nor\n";
        foreach(Ingredient i in Cost)
        {
            t += i.ToString() + "\n";
        }

        return t;
    }
    public int GetMPCost()
    {
        int cost = 0;
        foreach(Ingredient i in Cost)
        {
            cost += i.Item.Value;
        }
        return Math.Max(1, cost / 75);
    }
    public bool CanPayCost()
    {
        var cost = GetMPCost();
        if (Player.Instance.CurrentMP >= cost)
        {
            Player.Instance.CurrentMP -= cost;
            return true;
        }
        foreach (Ingredient i in Cost)
        {
            if (Player.Instance.Inventory.GetNumberOfUnlockedItem(i.Item) < i.Amount)
            {
                return false;
            }
        }
        return true;
    }
    public bool PayCost()
    {
        if (CanPayCost())
        {
            foreach (Ingredient i in Cost)
            {
                if (i.Amount != Player.Instance.Inventory.RemoveItems(i.Item, i.Amount))
                {
                    return false;
                }

            }

        }
        
        return true;
    }
}

