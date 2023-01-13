using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

public class Player
{
    private static readonly Player instance = new Player();
    private Player() { }
    static Player() { }
    public static Player Instance
    {
        get
        {
            return instance;
        }
    }
    public string Name { get; set; } = "";
    public Inventory Inventory = new Inventory(30);
    private List<GameItem> equippedItems = new List<GameItem>();
    public List<Skill> Skills = new List<Skill>();
    public List<AlchemicalFormula> KnownAlchemicalFormulae = new List<AlchemicalFormula>();
    private Follower currentFollower;
    public Follower CurrentFollower { get { return currentFollower; } }
    public int MaxHP = 50;
    public int CurrentHP;
    public int MaxMP = 1;
    public int CurrentMP;
    public int TicksToNextAttack { get; set; }
    public int Deaths { get; set; }
    public int ArtisanPoints { get; set; }
    public bool JustDied { get; set; }
    public int Air { get; set; }

    public Skill LastGainedExp { get; set; }
    public Skill ExpTrackerSkill { get; set; }
    public List<IStatusEffect> CurrentStatusEffects { get; set; } = new List<IStatusEffect>();
    
    public void SetFollower(Follower f)
    {
        BattleManager.Instance.AutoBattle = false;
        BattleManager.Instance.SelectedOpponent = null;
        BattleManager.autoBattleOpponent = "";
        currentFollower = f;

    }

    public async Task LoadSkills(HttpClient Http)
    {
        Skills.AddRange(await Http.GetFromJsonAsync<Skill[]>("data/Skills.json"));
    }
    public Skill GetSkill(string skill)
    {
        foreach(Skill s in Skills)
        {
            if(s.Name == skill)
            {
                return s;
            }
        }
        return null;
    }
    private void IncreaseMaxHPBy(int amount)
    {
        MaxHP += amount;
    }
    public void GainExperience(string skill, long amount)
    {
        foreach(Skill s in Skills)
        {
            if(s.Name == skill)
            {
                GainExperience(s, amount);
                return;
            }
        }
    }
    public void GainExperience(string skillAndExp)
    {
        if (string.IsNullOrEmpty(skillAndExp) || skillAndExp == "None")
        {
            return;
        }
        if (int.TryParse(skillAndExp.Split(':')[1], out int amount))
        {
            foreach (Skill s in Skills)
            {
                if (s.Name == skillAndExp.Split(':')[0])
                {
                    GainExperience(s, amount);
                    return;
                }
            }
        }
    }
    public void GainExperienceMultipleTimes(string skillAndExp, int times)
    {
        for(int i = 0; i < times; i++)
        {
            GainExperience(skillAndExp);
        }
    }
    public double GetGearMultiplier(GameItem item)
    {
        if(item.Requirements == null || item.Requirements.Count == 0)
        {
            return 1;
        }

        string skill = "None";
        foreach(Requirement req in item.Requirements)
        {
            if(req.Skill != "None")
            {
                skill = req.Skill;
                continue;
            }
        }
        
        if(skill == "None")
        {
            return 1;
        }
        double multi = 1;
        foreach(GameItem i in equippedItems)
        {
            if (i.EnabledActions.Contains(skill))
            {
                multi -= i.GatherSpeedBonus;
            }         
        }
        return Math.Max(multi, 0.01);
    }
    public double GetLevelMultiplier(GameItem item)
    {
        if (item.Requirements == null || item.Requirements.Count == 0)
        {
            return 1;
        }
        string skill = "None";
        foreach (Requirement req in item.Requirements)
        {
            if (req.Skill != "None")
            {
                skill = req.Skill;
                continue;
            }
        }

        Skill s = null;
        foreach(Skill sk in Skills)
        {
            if(sk.Name == skill)
            {
                s = sk;
            }
        }
        if(s == null)
        {
            return 1;
        }
        double multi = 1;
        if(s.GetSkillLevel() < 100)
        {
            multi = 1 - (s.GetSkillLevel() * 0.005);
        }
        else if(s.GetSkillLevel() < 200)
        {
            multi = 1 - (0.5 + ((s.GetSkillLevel() - 100) * 0.002));
        }
        else if(s.GetSkillLevel() < 300)
        {
            multi = 1 - (0.7 + ((s.GetSkillLevel() - 200) * 0.001));
        }
        else
        {
            multi = 1 - (0.8 + ((s.GetSkillLevel() - 300) * 0.0005));
        }
        return Math.Max(multi, 0.01);
    }
    public void Equip(GameItem item)
    {
        GameItem unequip = null;
        foreach (GameItem i in equippedItems)
        {
            if(i.EquipSlot == item.EquipSlot)
            {
                unequip = i;            
                break;
            }
        }
        if(unequip != null)
        {
            Unequip(unequip);
        }
        equippedItems.Add(item);
        if(item.WeaponInfo != null)
        {
            TicksToNextAttack = GetWeaponAttackSpeed();
        }
        item.Rerender = true;
        item.IsEquipped = true;
    }
    public void Equip(string itemName)
    {
        foreach (KeyValuePair<GameItem, int> pair in Inventory.GetItems())
        {
            if(pair.Key.Name == itemName)
            {
                Equip(pair.Key);
                return;
            }
        }
    }
    public void Unequip(GameItem item)
    {
        if (item != null)
        {
            if (item.WeaponInfo != null)
            {
                TicksToNextAttack = GetWeaponAttackSpeed();
            }
            item.IsEquipped = false;
            item.Rerender = true;
            equippedItems.Remove(item);
        }
    }
    public int GetTotalDamage()
    {
        int total = 0;
        total += GetSkill("Strength").GetSkillLevel() * 3;
        GameItem weapon = GetWeapon();
        foreach(GameItem item in equippedItems)
        {
            if(item.WeaponInfo != null)
            {
                total += item.WeaponInfo.Damage;
                if (weapon != null && weapon.EnabledActions == "Archery")
                {
                    if (Inventory.HasArrows())
                    {
                        total += item.WeaponInfo.RangedDamage;
                    }
                    else
                    {
                        total += GetLevel("Strength");
                    }
                }
                else
                {
                    string skill = item.GetSkillForWeaponExp();
                    if (skill == "")
                    {
                        total += GetLevel("Strength");
                    }
                    else
                    {
                        total += GetSkill(skill).GetSkillLevel() * 3;
                    }              
                }

            }
            if(item.ArmorInfo != null)
            {
                total += item.ArmorInfo.Damage;
                if (weapon != null &&
                    (weapon.EnabledActions == "Archery" && Inventory.HasArrows() ||
                    (weapon.Name == "Spine Shooter" && Inventory.HasItem("Cactus Spines0", true))))
                {
                    total += item.ArmorInfo.RangedDamage;
                }
            }
        }
        if(weapon != null)
        {
            if(weapon.Name == "Spine Shooter" && Inventory.HasItem("Cactus Spines0", true))
            {
                total += 10;
                
            }
            else if(weapon.Name == "Spine Shooter" && Inventory.HasItem("Lion's Mane Stingers0", true))
            {
                total += 100;
            }
            else if (weapon.EnabledActions == "Archery" && Inventory.HasArrows())
            {
                total += Inventory.GetStrongestArrow().WeaponInfo.RangedDamage;                        
            }

        }
        if (HasStatusEffect("Rally"))
        {
            total = (int)(total * (1 + (CurrentHP / (double)MaxHP)));
        }
        return Math.Max(1, total);
    }
    public void ClearBoosts()
    {
        foreach(Skill s in Skills)
        {
            s.Boost = 0;
        }
    }
    public int PayArtisanPoints(int amountToPay)
    {
        if(ArtisanPoints >= amountToPay)
        {
            ArtisanPoints -= amountToPay;
        }
        else
        {
            return 0;
        }
        return amountToPay;
    }
    public int GetTotalLevel()
    {
        return Skills.Select(x => x.Level).Sum();
    }
    public GameItem GetWeapon()
    {
        return equippedItems.Find(x => x.EquipSlot == "R Hand");
    }
    public int GetWeaponAttackSpeed()
    {
        GameItem weapon = GetWeapon();
        if (weapon != null && weapon.WeaponInfo != null)
        {
            return Math.Max(4, GetWeapon().WeaponInfo.AttackSpeed - (GetLevel("Deftness") / 25));
        }
        else
        {
            return Math.Max(8, 12 - (GetLevel("Deftness") / 25));
        }
    }
    public int GetLevel(string skillName)
    {
        foreach (Skill skill in Skills)
        {
            if (skill.Name == skillName)
            {
                return skill.GetSkillLevel();
            }
        }
        return 0;
    }
    public void GainExperience(Skill skill, long amount)
    {
        if (skill == null)
        {
            Console.WriteLine("Player gained " + amount + " experience in unfound skill.");
            return;
        }
        if (amount <= 0)
        {
            return;
        }
        LastGainedExp = skill;
        double multi = 1;
        foreach(GameItem i in equippedItems)
        {
            if(i.ExperienceBonusSkill == skill.Name)
            {
                multi += i.ExperienceGainBonus;
            }
        }
        amount = (long)(amount * multi);

        skill.GainExperience(amount);
        

        if (skill.Experience >= (long)Skill.GetExperienceRequired(skill.GetSkillLevelUnboosted()))
        {
            LevelUp(skill);
        }
    }
    public void GainExperienceFromWeapon(GameItem weapon, int damageDealt)
    {
        if (weapon.EnabledActions == null)
        {
            return;
        }
        if (weapon.EnabledActions.Contains("Knife"))
        {
            GainExperience("Deftness", (int)(damageDealt * 1.5));
            GainExperience("Knifesmanship", (int)(damageDealt));
        }
        else if (weapon.EnabledActions.Contains("Sword"))
        {
            GainExperience("Deftness", (int)(damageDealt * 0.5));
            GainExperience("Strength", damageDealt);
            GainExperience("Swordsmanship", (int)(damageDealt));
        }
        else if (weapon.EnabledActions.Contains("Axe"))
        {
            GainExperience("Deftness", (int)(damageDealt * 0.5));
            GainExperience("Strength", damageDealt);
            GainExperience("Axemanship", (int)(damageDealt));
        }
        else if (weapon.EnabledActions.Contains("Hammer"))
        {
            GainExperience("Strength", (int)(damageDealt * 1.5));
            GainExperience("Hammermanship", (int)(damageDealt));
        }
        else if (weapon.EnabledActions.Contains("Archery"))
        {

                GainExperience("Archery", (int)(damageDealt * 1.5));

        }
        else if (weapon.EnabledActions.Contains("Fishing"))
        {
            GainExperience("Fishing", (int)(damageDealt * 0.1));
        }
        else if (weapon.EnabledActions.Contains("Mining"))
        {
            GainExperience("Hammermanship", (int)(damageDealt * 0.1));
            GainExperience("Strength", (int)(damageDealt * 0.5));
            GainExperience("Mining", (int)(damageDealt * 0.08));
        }
        else
        {
            GainExperience("Strength", (int)(damageDealt * 0.5));
            GainExperience("Deftness", (int)(damageDealt * 0.5));
        }
    }
    public void LevelUp(Skill skill)
    {
        if(skill.Level == Skill.MaxLevel)
        {
            return;
        }
        skill.SetSkillLevel(skill.GetSkillLevelUnboosted() + 1);
        MessageManager.AddMessage("You leveled up! Your " + skill.Name + " level is now " + skill.GetSkillLevelUnboosted() + ".");
        if (skill.Name == "Strength")
        {
            Inventory.IncreaseMaxSizeBy(1);

            if (skill.GetSkillLevelUnboosted() % 10 == 0)
            {
                Inventory.IncreaseMaxSizeBy(1);
                MessageManager.AddMessage("You feel much stronger. You can now carry 2 more items in your inventory.");
            }
            else
            {
                MessageManager.AddMessage("You feel stronger. You can now carry 1 more item in your inventory.");
            }
        }
        else if (skill.Name == "HP")
        {
            IncreaseMaxHPBy(5);
            if (skill.GetSkillLevelUnboosted() % 5 == 0)
            {
                IncreaseMaxHPBy(10);
                MessageManager.AddMessage("You feel much healthier. Your maximum HP has increased by 15!");
            }
            else
            {
                MessageManager.AddMessage("You feel healthier. Your maximum HP has increased by 5.");
            }
        }
        else if (skill.Name == "Magic")
        {
            MaxMP += 1;
            if(skill.GetSkillLevelUnboosted() % 3 == 0 && skill.GetSkillLevelUnboosted() % 7 == 0)
            {
                MaxMP += 8;
                MessageManager.AddMessage("You feel a great deal better at magic. Your maximum MP has increased by 9!");
            }
            else if (skill.GetSkillLevelUnboosted() % 3 == 0)
            {
                MaxMP += 3;
                MessageManager.AddMessage("You feel much better at magic. Your maximum MP has increased by 4!");
            }
            else if (skill.GetSkillLevelUnboosted() % 7 == 0)
            {
                MaxMP += 5;
                MessageManager.AddMessage("You feel much better at magic. Your maximum MP has increased by 6!");
            }
            else
            {
                MessageManager.AddMessage("You feel better at magic. Your maximum MP has increased by 1.");
            }
        }

        if (skill.Experience >= Skill.GetExperienceRequired(skill.GetSkillLevelUnboosted()))
        {
            if(skill.Level < Skill.MaxLevel)
            {
                LevelUp(skill);
            }
            

        }
    }
    public void Die(string cause)
    {
        if (GameState.CurrentGameMode == GameState.GameType.Hardcore)
        {
            GameState.HCDeathInfo = new HCDeathInfo()
            {
                CauseOfDeath = cause,
                TotalPlaytime = GameState.CurrentTick
            };
            foreach (Skill s in Skills)
            {
                Skill copy = new Skill();
                copy.Name = s.Name;
                copy.GainExperience(s.Experience);
                copy.Level = s.Level;
                GameState.HCDeathInfo.FinalLevels.Add(copy);
            }

            JustDied = true;
            SaveManager.DeleteHCSave();
            GameState.ResetGame();
            MessageManager.Clear();
            GameState.GoTo("/");
            GameState.ShowStartMenu = true;
        }
        else
        {
            CurrentHP = MaxHP;
            JustDied = true;
            Deaths++;
            CurrentStatusEffects.Clear();
            if (Deaths == 1)
            {
                MessageManager.AddMessage("Whoops! Looks like you died. Don't worry, you don't lose anything but pride when you die in Quepland.");
            }
            else
            {
                MessageManager.AddMessage("Whoops! Looks like you died.");
            }
            if (BattleManager.Instance.CurrentDojo != null)
            {
                BattleManager.Instance.CurrentDojo.CurrentOpponent = 0;
                BattleManager.Instance.CurrentDojo.HasBegunChallenge = false;
                BattleManager.Instance.CurrentDojo = null;
            }
            BattleManager.Instance.EndBattle();
        }

    }
    public void Die()
    {
        Die("Unknown Reasons");
    }
    public bool FollowerGatherItem(GameItem item, GameItem? magicItem)
    {
        if (CurrentFollower != null && CurrentFollower.IsBanking == false)
        {
            if(CurrentFollower.InventorySize == 0)
            {
                return false;
            }
            if (CurrentFollower.Inventory.GetAvailableSpaces() <= 0)
            {
                CurrentFollower.SendToBank();
                MessageManager.AddMessage(CurrentFollower.AutoCollectMessage.Replace("$", item.Name));
                return false;
            }
            else if(CurrentFollower.MeetsRequirements(item))
            {
                if(magicItem != null)
                {
                    CurrentFollower.Inventory.AddItem(magicItem.Copy());
                    
                }
                else
                {
                    CurrentFollower.Inventory.AddItem(item.Copy());
                    MessageManager.AddMessage(item.GatherString);
                }

                GainExperience(item.ExperienceGained);
                
                return true;
            }
            else
            {
                if(MessageManager.GetMessages().Any(x => x.Text.Contains(CurrentFollower.Name + " is unable to carry ")) == false)
                {
                    MessageManager.AddMessage(CurrentFollower.Name + " is unable to carry " + item.Name + ".");
                }
                
                return false;
            }

        }
        return false;
    }
    public bool PlayerGatherItem(GameItem item, GameItem? magicGatherSwap)
    {
        if(item == null)
        {
            return false;
        }
        if (magicGatherSwap != null && Inventory.AddItem(magicGatherSwap.Copy()) == false)
        {
            if (CurrentFollower != null && CurrentFollower.IsBanking)
            {
                MessageManager.AddMessage("Your inventory is full. You wait for your follower to return from banking.");
            }
            else
            {
                MessageManager.AddMessage("Your inventory is full.");
            }
            return false;
        }
        else if (magicGatherSwap == null && Inventory.AddItem(item.Copy()) == false)
        {
            if (CurrentFollower != null && CurrentFollower.IsBanking)
            {
                MessageManager.AddMessage("Your inventory is full. You wait for your follower to return from banking.");
            }
            else
            {
                MessageManager.AddMessage("Your inventory is full.");
            }
            return false;
        }
        else
        {
            GainExperience(item.ExperienceGained);
            MessageManager.AddMessage(item.GatherString);

        }
        return true;
    }
    public List<GameItem> GetEquippedItems()
    {
        return equippedItems;
    }
    public GameItem GetItemInSlot(string slot)
    {
        return equippedItems.FirstOrDefault(x => x.EquipSlot == slot);
    }
    public bool HasSkillRequirement(string skill, int lvl)
    {
        if(skill == "None")
        {
            return true;
        }
        Skill s = Skills.FirstOrDefault(x => x.Name == skill);
        if (s == null)
        {
            Console.WriteLine("Failed to find skill:" + skill);
            return false;
        }
        return s.GetSkillLevel() >= lvl;
    }
    public bool HasToolRequirement(GameItem item)
    {
        return Inventory.HasToolRequirement(item);
    }
    /// <summary>
    /// Player's inventory contains an item that includes the action in its EnabledActions.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public bool HasToolRequirement(string action)
    {
        return Inventory.HasToolRequirement(action);
    }
    public bool HasStatusEffect(string name)
    {
        return CurrentStatusEffects.Any(x => x.Name == name);
    }
    public void AddStatusEffect(IStatusEffect effect)
    {
        if (effect.OnProc)
        {
            effect.DoEffect(this);
            return;
        }

        CurrentStatusEffects.Add(effect.Copy());
    }
    public void TickStatusEffects()
    {
        List<IStatusEffect> endedEffects = new List<IStatusEffect>();
        try
        {
            
            foreach (IStatusEffect effect in CurrentStatusEffects)
            {
                effect.RemainingTime--;
                if (effect.RemainingTime <= 0)
                {
                    endedEffects.Add(effect);
                }
                else
                {
                    effect.DoEffect(this);
                }
            }
            
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }
        CurrentStatusEffects.RemoveAll(x => endedEffects.Contains(x));
    }
    public void ResetStats()
    {
        foreach(Skill s in Skills)
        {
            s.SetSkillLevel(1);
            s.ResetExperience();
        }
        CalculateMaxHP();
        CalculateMaxMP();
        CalculateInventorySpaces();
    }
    public PlayerSaveData GetSaveData()
    {
        List<string> equipped = new List<string>();
        try
        {
            if(equippedItems.Count > 0) 
            {
                equipped = equippedItems.Select(x => x.Name).ToList();
            }
            
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            Console.WriteLine("Failed to set equipped items list.");
        }
        return new PlayerSaveData {
            ActiveFollowerName = CurrentFollower?.Name ?? "None",
            CurrentHP = CurrentHP,
            MaxHP = MaxHP,
            CurrentMP = CurrentMP,
            DeathCount = Deaths,
            ArtisanPoints = ArtisanPoints,
            InventorySize = Inventory.GetSize(),
            EquippedItems = equipped,
            KnownAlchemyFormulae = KnownAlchemicalFormulae,
            MagicUnlocks = MagicManager.Instance.GetMagicUnlocks()
        };
    }

    public void LoadSaveData(PlayerSaveData data, string version)
    {
        string error = "";
        try
        {
            if (data.ActiveFollowerName != "None")
            {
                error = "Failed to load active follower data for:" + data.ActiveFollowerName;
                SetFollower(FollowerManager.Instance.GetFollowerByName(data.ActiveFollowerName));
            }
            CurrentHP = data.CurrentHP;
            CalculateMaxHP();
            CalculateMaxMP();
            if (GameState.CheckVersion(version))
            {
                CurrentMP = data.CurrentMP;

            }
            
            Deaths = data.DeathCount;
            ArtisanPoints = data.ArtisanPoints;
            CalculateInventorySpaces();
            if(data.MagicUnlocks != null)
            {
                MagicManager.Instance.LoadSpellUnlocks(data.MagicUnlocks);
            }
            
            if(data.EquippedItems.Count == 0)
            {
                return;
            }
            foreach (string s in data.EquippedItems)
            {
                Console.WriteLine("Trying to equip:" + s);
                if (s != null && s.Length > 1)
                {
                    try
                    {
                        error = "Failed to equip item:" + s;
                        var g = Inventory.GetItems().FirstOrDefault(x => x.Key.Name == s).Key;
                        if(g != null)
                        {
                            Equip(g);
                        }
                        else
                        {
                            MessageManager.AddMessage(error, "red");
                        }
                        
                    }
                    catch (Exception e)
                    {

                        MessageManager.AddMessage(error, "red");
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                    }
                    
                }
            }
            if (GameState.CheckVersion("1.1.0", version))
            {
                foreach (AlchemicalFormula l in data.KnownAlchemyFormulae)
                {
                    KnownAlchemicalFormulae.Add(new AlchemicalFormula(l));
                }
            }
        }
        catch(Exception e)
        {
            MessageManager.AddMessage(error, "red");

            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);

        }

    }
    private void CalculateMaxHP()
    {
        int hp = 50;
        for(int i = 1; i <= GetLevel("HP"); i++)
        {
            if(i % 5 == 0)
            {
                hp += 10;
            }
            hp += 5;
        }
        MaxHP = hp;
    }
    private void CalculateMaxMP()
    {
        int mp = 1;
        for (int i = 1; i <= GetLevel("Magic"); i++)
        {
            if (i % 3 == 0)
            {
                mp += 3;
            }
            if(i % 7 == 0)
            {
                mp += 5;
            }
            mp += 1;
        }
        MaxMP = mp;
    }
    private void CalculateInventorySpaces()
    {
        int spaces = 30;
        for(int i = 1; i <= GetLevel("Strength"); i++)
        {
            if(i % 10 == 0)
            {
                spaces++;
            }
            spaces++;
        }
        Inventory.SetSize(spaces);
    }
    /// <summary>
    /// The rate in game ticks that magic is restored.
    /// </summary>
    /// <returns></returns>
    public int GetMagicRestoreRate()
    {
        return Math.Max(10, 50 - Player.Instance.GetLevel("Magic") / 4);
    }
}

