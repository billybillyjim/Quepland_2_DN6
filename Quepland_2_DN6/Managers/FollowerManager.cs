using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class FollowerManager
{
    private static readonly FollowerManager instance = new FollowerManager();
    private FollowerManager() { }
    static FollowerManager() { }
    public static FollowerManager Instance
    {
        get
        {
            return instance;
        }
    }
    public List<Follower> Followers = new List<Follower>();
    public async Task LoadFollowers(HttpClient Http)
    {
        Followers.AddRange(await Http.GetFromJsonAsync<Follower[]>("data/Followers.json"));
    }
    public List<Follower> GetUnlockedFollowers()
    {
        return Followers.Where(x => x.IsUnlocked).ToList();
    }
    public Follower GetFollowerByName(string name)
    {
        return Followers.FirstOrDefault(x => x.Name == name);
    }
    public string GetNewSaveData()
    {
        string data = "[";
        foreach (Follower f in Followers)
        {
            data += JsonConvert.SerializeObject(f, Formatting.Indented, new InventoryJsonConverter(typeof(Follower))) + ",";
        }
        data += "]";
        Console.WriteLine(data);
        return data;
    }
    public string GetSaveData()
    {
        string data = "";
        foreach(Follower f in Followers)
        {
            data += f.Name + ":" + f.IsUnlocked + ":" + f.Banking.Experience + ":" + SaveManager.GetFollowerItemSave(f.Inventory) + ",";
        }

        return data;
    }
    public void LoadNewSaveData(string data)
    {
        try
        {
            Console.WriteLine("Loading new save data...");
            List<Follower> followers = JsonConvert.DeserializeObject<List<Follower>>(data);
            if (followers == null)
            {
                throw new NullReferenceException();
            }
            foreach(Follower f in followers)
            {
                Follower f2 = Followers.FirstOrDefault(x => x.Name == f.Name);
                if(f2 == null)
                {
                    Console.WriteLine("Failed to find follower with name:" + f.Name);
                }
                f2.IsUnlocked = f.IsUnlocked;
                f2.Banking = f.Banking;
                f2.Inventory.LoadData(SaveManager.GetFollowerItemSave(f.Inventory));
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
        }
        

    }
    public void LoadSaveData(string data)
    {
        string[] lines = data.Split(',');
        foreach(string line in lines)
        {
            if(line.Length < 2)
            {
                continue;
            }
            string[] d = line.Split(':');
            Follower f = Followers.FirstOrDefault(x => x.Name == d[0]);
            if (f == null)
            {
                Console.WriteLine("Failed to load save data for follower:" + line);
                continue;
            }
                     
            f.IsUnlocked = bool.Parse(d[1]);

            if (d.Length > 2)
            {
                long exp = long.Parse(d[2]);
                f.GainExperience(exp, false);
            }
            if(d.Length > 3)
            {
                Console.WriteLine(d[3]);
                f.Inventory.LoadData(d[3]);
            }
        }
    }
}

