using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class Follower
{
    [JsonProperty]
    public string Name { get; set; }
    [JsonProperty]
    public string AutoCollectMessage { get; set; }
    [JsonProperty]
    public string AutoCollectSkill { get; set; }
    [JsonProperty]
    public int AutoCollectLevel { get; set; }
    [JsonProperty]
    private int autoCollectSpeed;
    
    public int AutoCollectSpeed { get 
        {
            return autoCollectSpeed;
        }
        set
        {
            autoCollectSpeed = value;
        } 
    }
    [JsonProperty]
    public bool IsUnlocked { get; set; }
    public bool IsBanking { get; set; }
    [JsonProperty]
    public int InventorySize { get; set; }
    [JsonProperty]
    public Skill Banking { get; set; } = new Skill()
    {
        Name = "Banking",
        Level = 1,
        Description = "Banking is the ability of your follower to properly manage inventory space and movement to transport goods from you to the bank efficiently."
    };
    //Todo: make custom json converter for inventories
    
    private Inventory inv;
    [JsonProperty]
    public Inventory Inventory
    {
        get
        {
            if (inv == null)
            {
                inv = new Inventory(InventorySize);
            }
            return inv;
        }
    }
    public int TicksToNextAction { get; set; }
    [JsonProperty]
    public string Mode { get; set; } = "Normal";

    public void BankItems()
    {
        if(GameState.CurrentGameMode == GameState.GameType.Ultimate)
        {
            return;
        }
        Bank.Instance.Inventory.SkipIndexing = true;
        foreach(KeyValuePair<GameItem, int> itemPair in Inventory.GetItems())
        {
            Bank.Instance.Inventory.AddMultipleOfItem(itemPair.Key.Copy(), itemPair.Value);
            GainExperience(itemPair.Value);
        }
        Bank.Instance.Inventory.SkipIndexing = false;
        Bank.Instance.Inventory.FixItems = true;
        Bank.Instance.Inventory.UpdateItemCount();
        Inventory.Clear();
        IsBanking = false;

    }
    public void TakeItems()
    {
        List<KeyValuePair<GameItem, int>> takenItems = new List<KeyValuePair<GameItem, int>>();
        foreach (KeyValuePair<GameItem, int> itemPair in Inventory.GetItems())
        {
            if(Player.Instance.Inventory.AddMultipleOfItem(itemPair.Key.Copy(), itemPair.Value, out int taken))
            {
                takenItems.Add(new KeyValuePair<GameItem, int>(itemPair.Key, taken));
                GainExperience(itemPair.Value);
            }
            else
            {
                break;
            }
        }
        Console.WriteLine("Taken items:");
        foreach (KeyValuePair<GameItem, int> itemPair in takenItems)
        {
            Console.WriteLine(itemPair.Value + " "  + itemPair.Key.Name);
            Inventory.RemoveItems(itemPair.Key, itemPair.Value);
        }
            
        IsBanking = false;
    }

    public void GainExperience(long amount, bool writeToMessageLog = true)
    {

        if (Banking.GetSkillLevelUnboosted() >= AutoCollectLevel)
        {
            return;
        }
        Banking.GainExperience(amount);

        if (Banking.Experience >= (long)Skill.GetExperienceRequired(Banking.GetSkillLevelUnboosted()))
        {
            LevelUp(writeToMessageLog);
        }

    }
    public void LevelUp(bool writeToMessageLog = true)
    {
        if(Banking.GetSkillLevelUnboosted() >= AutoCollectLevel)
        {
            return;
        }
        if(Banking.GetSkillLevelUnboosted() % 2 == 0)
        {
            if (writeToMessageLog)
            {
                MessageManager.AddMessage(Name + " leveled up! Their banking level is now " + Banking.GetSkillLevelUnboosted() + ". It seems they can carry a little bit more now.");

            }
            InventorySize++;
            Inventory.IncreaseMaxSizeBy(1);
        }
        else
        {
            if (writeToMessageLog)
            {
                MessageManager.AddMessage(Name + " leveled up! Their banking level is now " + Banking.GetSkillLevelUnboosted() + ".");
            }
            
        }

        Banking.SetSkillLevel(Banking.GetSkillLevelUnboosted() + 1);
        
        if (Banking.Experience >= Skill.GetExperienceRequired(Banking.GetSkillLevelUnboosted()))
        {
            LevelUp(writeToMessageLog);

        }
    }

    public void SendToBank()
    {
        IsBanking = true;
        TicksToNextAction = AutoCollectSpeed;
    }

    public bool MeetsRequirements(GameItem item)
    {
        if(item == null)
        {
            return false;
        }
        if(item.Requirements == null || item.Requirements.Count == 0)
        {
            return true;
        }
        foreach(Requirement req in item.Requirements)
        {
            if(req.Skill != "None" && req.Skill != AutoCollectSkill)
            {
                return false;
            }
            if(req.Skill == AutoCollectSkill && req.SkillLevel > AutoCollectLevel + Banking.GetSkillLevel())
            {
                return false;
            }
        }
        return true;
    }
}
