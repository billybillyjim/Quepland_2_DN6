
public class SpellData
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Power { get; set; }
    public string Message { get; set; }
    public int Duration { get; set; }
    public string Target { get;set; }
    public string Data { get; set; }
    public int Cooldown { get; set; }

    public List<Ingredient> Cost { get; set; }

}

