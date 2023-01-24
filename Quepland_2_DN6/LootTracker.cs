using System;

public class LootTracker
{
	private static readonly LootTracker instance = new LootTracker();
	private LootTracker() { }
	static LootTracker() { }
	public static LootTracker Instance
    {
        get
        {
            return instance;
        }
    }
    public Dictionary<string, Inventory> DataDict = new Dictionary<string, Inventory>();
    public Dictionary<string, int> KCDict = new Dictionary<string, int>();
    public List<string> HiddenItems = new List<string>();
    public Inventory Inventory { get; set; } = new Inventory(int.MaxValue);
    public bool TrackLoot { get; set; } = true;

    public void AddKC(Monster m, int amount)
    {
        if (!KCDict.ContainsKey(m.Name))
        {
            KCDict.Add(m.Name, amount);
        }
        else
        {
            KCDict[m.Name] += amount;
        }
    }
    public void AddDrop(Monster m, Drop d)
    {
        if (!DataDict.ContainsKey(m.Name))
        {
            DataDict.Add(m.Name, new Inventory(int.MaxValue, true));

        }
        DataDict[m.Name].AddDrop(d, out _);
    }
}
