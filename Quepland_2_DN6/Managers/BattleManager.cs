using Quepland_2_DN6.Bosses;
using Quepland_2_DN6.Spells;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class BattleManager
{
    private static readonly BattleManager instance = new BattleManager();
    private BattleManager() { }
    static BattleManager() { }
    public static BattleManager Instance { get { return instance; } }
    public List<Monster> Monsters = new List<Monster>();
    public List<Monster> CurrentOpponents { get; set; } = new List<Monster>();
    public List<Monster> SpawnOpponents { get; set; } = new List<Monster>();
    public List<Monster> RemoveOpponents { get; set; } = new List<Monster>();
    public List<Monster> Allies { get; set; } = new List<Monster>();
    public IBoss CurrentBoss { get; set; }
    public Monster Target { get; set; }
    public Area CurrentArea { get; set; }
    public string ReturnLocation { get; set; }
    public Dojo CurrentDojo { get; set; }
    public bool BattleHasEnded = true;
    public bool WonLastBattle = false;
    public bool WaitedAutoBattleGameTick { get; set; }
    public bool AutoBattle { get; set; }
    public Monster SelectedOpponent { get; set; }
    private static readonly Random random = new Random();

    public static string autoBattleOpponent = "";

    public string GetKillCounts()
    {
        string kc = "";
        foreach(Monster m in Monsters)
        {
            kc += m.Name + "_" + m.KillCount + ",";
        }
        return kc.Trim(',');
    }
    public void LoadKillCounts(string kc)
    {
        string[] lines = kc.Split(',');
        foreach(string line in lines)
        {
            Monster m = Monsters.FirstOrDefault(x => x.Name == line.Split('_')[0]);
            if(m == null)
            {
                Console.WriteLine($"No monster with name:{line} found.");
                continue;
            }
            if (int.TryParse(line.Split('_')[1], out int k))
            {
                m.KillCount = k;
            }
            else
            {
                Console.WriteLine("Failed to parse:" + line);
            }
        }
    }
    public async Task LoadMonsters(HttpClient Http)
    {
        Monsters.AddRange(await Http.GetFromJsonAsync<Monster[]>("data/Monsters/DojoOpponents.json"));

        Monsters.AddRange(await Http.GetFromJsonAsync<Monster[]>("data/Monsters/Overworld.json"));
        Monsters.AddRange(await Http.GetFromJsonAsync<Monster[]>("data/Monsters/Underworld.json"));
        Monsters.AddRange(await Http.GetFromJsonAsync<Monster[]>("data/Monsters/Bosses.json"));
        Monsters.AddRange(await Http.GetFromJsonAsync<Monster[]>("data/Monsters/Lighthouse.json"));
        Monsters.AddRange(await Http.GetFromJsonAsync<Monster[]>("data/Monsters/EasternLands.json"));

        foreach(Monster m in Monsters)
        {
            m.LoadStatusEffects();
        }
    }
    public void StartBattle()
    {
        if (CurrentOpponents == null || CurrentOpponents.Count == 0)
        {
            Console.WriteLine("Opponents were null or nonexistent.");
            return;
        }
        else
        {
            foreach(Monster monster in CurrentOpponents)
            {
                ResetOpponent(monster);
            }
            BattleHasEnded = false;
        }
        Player.Instance.TicksToNextAttack = Player.Instance.GetWeaponAttackSpeed();
        WaitedAutoBattleGameTick = false;
        Target = GetNextTarget();
    }
    public void StartBattle(Area area)
    {
        CurrentArea = area;
        int r = random.Next(0, CurrentArea.Monsters.Count);
        CurrentOpponents.Clear();
        Allies.Clear();
        CurrentOpponents.Add(Monsters.FirstOrDefault(x => x.Name == CurrentArea.Monsters[r]));
        SelectedOpponent = null;
        if(CurrentOpponents[0] == null)
        {
            Console.WriteLine("No monsters found for area.");
        }

        StartBattle();
        
    }
    public void StartBattle(Monster opponent)
    {
        CurrentOpponents.Clear();
        Allies.Clear();
        CurrentOpponents.Add(opponent);
        if (AutoBattle)
        {
            SelectedOpponent = opponent;
        }
        StartBattle();
    }
    public void StartBattle(List<Monster> opponents)
    {
        CurrentOpponents.Clear();
        Allies.Clear();
        CurrentOpponents.AddRange(opponents);
        SelectedOpponent = null;
        StartBattle();
    }
    public void DoBattle()
    {
        if(BattleHasEnded == false)
        {
            if(SpawnOpponents.Count > 0)
            {
                foreach(Monster o in SpawnOpponents)
                {
                    ResetOpponent(o);
                    CurrentOpponents.Add(o);
                }
                SpawnOpponents.Clear();
            }
            if(RemoveOpponents.Count > 0)
            {
                foreach (Monster o in RemoveOpponents)
                {
                    CurrentOpponents.Remove(o);
                }
                RemoveOpponents.Clear();
            }
            foreach(Monster opponent in CurrentOpponents)
            {
                if(opponent.IsDefeated)
                {
                    opponent.TicksToNextAttack = opponent.AttackSpeed;
                }
                else
                {
                    opponent.TicksToNextAttack--;
                    opponent.TickStatusEffects();
                }
            }
            Player.Instance.TicksToNextAttack--;
            if (Player.Instance.TicksToNextAttack < 0)
            {
                Attack();
                if (CurrentBoss != null)
                {
                    CurrentBoss.OnBeAttacked(Target);
                }
                Player.Instance.TicksToNextAttack = Player.Instance.GetWeaponAttackSpeed();
            }
            foreach(Monster ally in Allies)
            {
                if (ally.IsDefeated == false)
                {
                    if (ally.CurrentHP <= 0)
                    {
                        MessageManager.AddMessage(ally.Name + " died!");
                        ally.CurrentHP = 0;
                        ally.IsDefeated = true;
                    }
                    else if(ally.TicksToNextAttack < 0)
                    {
                        DoAllyAttack(ally);
                        ally.TicksToNextAttack = ally.AttackSpeed;
                    }
                    ally.TicksToNextAttack--;
                }
                
            }
            foreach (Monster opponent in CurrentOpponents)
            {
                if(opponent.IsDefeated == false)
                {
                    if (opponent.CurrentHP <= 0)
                    {
                        opponent.CurrentHP = 0;
                        RollForDrops(opponent);
                        opponent.IsDefeated = true;
                        opponent.KillCount++;
                        if (CurrentBoss != null)
                        {
                            CurrentBoss.OnDie(opponent);
                        }
                    }
                    else if (opponent.TicksToNextAttack < 0)
                    {
                        if (CurrentBoss != null && CurrentBoss.CustomAttacks)
                        {
                            CurrentBoss.OnAttack();
                        }
                        else if (CurrentBoss != null)
                        {
                            CurrentBoss.OnAttack();
                            BeAttacked(opponent);
                        }
                        else
                        {
                            BeAttacked(opponent);

                        }
                        opponent.TicksToNextAttack = opponent.AttackSpeed;
                    }
                    else if (opponent.TicksToNextAttack == 1)
                    {
                        if (CheckForQuickStrike())
                        {
                            Attack();
                            if (CurrentBoss != null)
                            {
                                CurrentBoss.OnBeAttacked(Target);
                            }
                            Player.Instance.TicksToNextAttack = Player.Instance.GetWeaponAttackSpeed();
                            opponent.TicksToNextAttack = opponent.AttackSpeed;
                        }

                    }
                }
            }
            if (AllOpponentsDefeated())
            {
                if(CurrentDojo != null)
                {
                    CurrentDojo.CurrentOpponent++;
                    if(CurrentDojo.CurrentOpponent >= CurrentDojo.Opponents.Count)
                    {
                        CurrentDojo.LastWinTime = DateTime.Now;
                    }
                }
                WonLastBattle = true;
                EndBattle();
            }
            if (CurrentBoss != null)
            {
                CurrentBoss.TicksToNextSpecialAttack--;
                if(CurrentBoss.TicksToNextSpecialAttack <= 0)
                {
                    CurrentBoss.OnSpecialAttack();
                }
                
            }

            if (Player.Instance.CurrentHP <= 0)
            {
                if (GameState.PlayerIsUnderSpell("Endure"))
                {
                    Player.Instance.CurrentHP = 1;
                    GameState.RemoveActiveSpellByName("Endure");

                }
                else if(CurrentOpponents.Count > 0)
                {
                    string opponentList = String.Join(", ", CurrentOpponents.Select(x => x.Name).ToArray(), 0, CurrentOpponents.Count - 1) + ", and " + CurrentOpponents.LastOrDefault().Name;
                    Player.Instance.Die("Killed by " + opponentList);
                    WonLastBattle = false;
                }
                else
                {
                    Player.Instance.Die();
                    WonLastBattle = false;
                }
               
                
            }

        }

    }
    private void RollForDrops(Monster opponent)
    {
        if (opponent.DropTable != null && (opponent.DropTable.Drops.Count > 0 || opponent.DropTable.AlwaysDrops.Count > 0))
        {
            Drop drop = opponent.DropTable.GetDrop();
            if (drop != null && drop.Item != null)
            {
                if (drop.Item.Category == "QuestItems" && 
                    (Player.Instance.Inventory.HasItem(drop.Item) || 
                     Bank.Instance.Inventory.HasItem(drop.Item)))
                {

                }
                else
                {
                    if (LootTracker.Instance.TrackLoot)
                    {
                        LootTracker.Instance.AddDrop(opponent, drop);
                        LootTracker.Instance.AddKC(opponent, 1);
                    }
                    MessageManager.AddMessage("You defeated the " + opponent.Name + ". It dropped " + 
                        drop.Amount + " " + (drop.Amount > 1 ? drop.Item.GetPlural() : drop.ToString()) + ".", "white", "Loot");
                    AddDrop(drop);
                        
                }
                

            }
            foreach (Drop d in opponent.DropTable.AlwaysDrops)
            {
                if (LootTracker.Instance.TrackLoot)
                {
                    LootTracker.Instance.AddDrop(opponent, d);
                }
                MessageManager.AddMessage("You " + (drop != null ? "also" : "") + " got " + 
                                          d.Amount + " " + (d.Amount > 1 ? d.Item.GetPlural() : d.ToString()) + ".", "white", "Loot");
                AddDrop(d);
            }
        }
        else
        {
            MessageManager.AddMessage("You defeated the " + opponent.Name + ".");
        }
    }
    public void AddAlly(Monster m)
    {
        var ally = m.Copy();
        ResetOpponent(ally);
        Allies.Add(ally);
        MessageManager.AddMessage(ally.Name + " has joined your side!");
    }
    public void SpawnOpponentMidBattle(Monster m)
    {
        if(CurrentOpponents.Contains(m) == false && SpawnOpponents.Contains(m) == false)
        {
            SpawnOpponents.Add(m);
        }
        
    }
    public void RemoveOpponentMidBattle(Monster m)
    {
        if (CurrentOpponents.Contains(m))
        {
            RemoveOpponents.Add(m);
        }
    }
    public void AddDrop(Drop drop)
    {
        if (Player.Instance.CurrentFollower != null && Player.Instance.CurrentFollower.AutoCollectSkill == "Gathering Scout")
        {
            Player.Instance.CurrentFollower.Inventory.AddDrop(drop, out _);

            if (Player.Instance.CurrentFollower.Inventory.GetAvailableSpaces() == 0)
            {
                Player.Instance.CurrentFollower.BankItems();
            }
        }
        else
        {
            Player.Instance.Inventory.AddDrop(drop, out _);
        }
    }

    public int GetTotalPlayerDamage(Monster m)
    {
        double minHit = (((Player.Instance.GetWeaponAttackSpeed() / 5d) - 1d) / 12d);
 
        var baseDmg = (int)(Player.Instance.GetTotalDamage() * CalculateTypeBonus(m));
        int total = (int)Math.Max(minHit * baseDmg, baseDmg.ToRandomDamage() );
        if (m.CurrentStatusEffects.OfType<CleaveEffect>().Any())
        {
        }
        else
        {
            total = (int)Math.Max(total * Extensions.CalculateArmorDamageReduction(m), 1);
        }
        
        return (int)Math.Min(Target.CurrentHP, total); ;
    }
    public void Attack()
    {
        ConfirmTarget();
        RollForPlayerAttackEffects();

        int total = GetTotalPlayerDamage(Target);
        Target.CurrentHP -= total;

        GainCombatExperience(total);
        
        if (Target.IsDefeated)
        {
            Target = GetNextTarget();
        }
    }
    public void ConfirmTarget()
    {
        if (Target == null || Target.IsDefeated || Target.CurrentHP == 0)
        {
            if (CurrentOpponents == null || CurrentOpponents.Count == 0)
            {
                return;
            }
            Target = GetNextTarget();
        }
    }

    public void DoAllyAttack(Monster ally)
    {
        ConfirmTarget();
        int total = (int)Math.Max(1, (ally.Damage.ToRandomDamage()));
        Target.CurrentHP -= total;
        MessageManager.AddMessage(ally.Name + " attacks for " + total + " damage!");
        if (Target.IsDefeated)
        {
            Target = GetNextTarget();
        }
    }
    public void GainCombatExperience(int total)
    {
        if (Player.Instance.GetWeapon() == null)
        {
            Player.Instance.GainExperience("Strength", total);
            Player.Instance.GainExperience("Deftness", total / 3);
            MessageManager.AddMessage("You punch the " + Target.Name + " for " + total + " damage!");
            return;
        }

        if (Player.Instance.GetWeapon().EnabledActions == "Archery")
        {
            if (Player.Instance.GetWeapon().Name == "Spine Shooter")
            {
                if (Player.Instance.Inventory.HasItem("Cactus Spines0", true))
                {

                    if(Player.Instance.Inventory.RemoveItems(ItemManager.Instance.GetItemByUniqueID("Cactus Spines0"), 1) == 1)
                    {
                        Player.Instance.GainExperienceFromWeapon(Player.Instance.GetWeapon(), total);
                        MessageManager.AddMessage("You shoot the " + Target.Name + " for " + total + " damage!");
                    }
                }
                else if(Player.Instance.Inventory.HasItem("Lion's Mane Stingers0", true))
                {
                    if (Player.Instance.Inventory.RemoveItems(ItemManager.Instance.GetItemByUniqueID("Lion's Mane Stingers0"), 1) == 1)
                    {
                        Player.Instance.GainExperienceFromWeapon(Player.Instance.GetWeapon(), total);
                        MessageManager.AddMessage("You shoot the " + Target.Name + " for " + total + " damage!");
                    }
                }
                else
                {
                    Player.Instance.GainExperience("Strength", total);
                    MessageManager.AddMessage("You whack the " + Target.Name + " with your spine shooter for " + total + " damage!");
                }
            }
            else
            {
                if (Player.Instance.Inventory.HasArrows())
                {
                    
                    if(Player.Instance.Inventory.RemoveItems(Player.Instance.Inventory.GetStrongestArrow(), 1) == 1)
                    {
                        Player.Instance.GainExperienceFromWeapon(Player.Instance.GetWeapon(), total);
                        MessageManager.AddMessage("You hit the " + Target.Name + " for " + total + " damage!");
                    }
                    else
                    {
                        Player.Instance.GainExperience("Strength", total);
                        MessageManager.AddMessage("You whack the " + Target.Name + " with your bow for " + total + " damage!");
                    }
                }
                else
                {
                    Player.Instance.GainExperience("Strength", total);
                    MessageManager.AddMessage("You whack the " + Target.Name + " with your bow for " + total + " damage!");
                }
            }
            return;
        }

        Player.Instance.GainExperienceFromWeapon(Player.Instance.GetWeapon(), total);
        MessageManager.AddMessage("You hit the " + Target.Name + " for " + total + " damage!");
        


        

    }
    public void BeAttacked(Monster opponent)
    {
        RollForMonsterAttackEffects(opponent);
        int total = (int)Math.Max(1, (opponent.Damage.ToRandomDamage()));
        if (Player.Instance.CurrentStatusEffects.OfType<CleaveEffect>().Any())
        {
        }
        else
        {
            total = (int)Math.Max(total * Extensions.CalculateArmorDamageReduction(), 1);
        }       
        
        Player.Instance.CurrentHP -= total;
        Player.Instance.GainExperience("HP", total);
        if(CurrentDojo != null)
        {
            MessageManager.AddMessage(opponent.Name + " hit you for " + total + " damage!");
        }
        else
        {
            MessageManager.AddMessage("The " + opponent.Name + " hit you for " + total + " damage!");
        }
        
    }
    public double CalculateTypeBonus(Monster m)
    {
        double bonus = 1;
        if(Player.Instance.GetWeapon() != null)
        {
            foreach(string s in Player.Instance.GetWeapon().GetRequiredSkills())
            {
                if (m.Weaknesses.Contains(s))
                {
                    MessageManager.AddMessage("Your opponent seems weak to your weapon!");
                    bonus += 0.4;
                    break;
                }
                else if (m.Strengths.Contains(s))
                {
                    MessageManager.AddMessage("Your opponent seems to be resistant to your weapon...");
                    bonus -= 0.4;
                    break;
                }
            }
        }

        return Math.Min(Math.Max(bonus, 0.1), 10);
    }
    public bool AllOpponentsDefeated()
    {
        if(CurrentOpponents == null || CurrentOpponents.Count == 0)
        {
            return true;
        }
        foreach(Monster opponent in CurrentOpponents)
        {
            if(opponent.IsDefeated == false)
            {
                return false;
            }
        }
        return true;
    }
    public void ResetOpponent(Monster monster)
    {
        monster.CurrentHP = monster.HP;
        monster.CurrentArmor = monster.Armor;
        monster.TicksToNextAttack = monster.AttackSpeed;
        monster.IsDefeated = false;
        monster.CurrentStatusEffects.Clear();
    }
    public void SetBoss(Quepland_2_DN6.Bosses.IBoss boss)
    {
        CurrentBoss = boss;
        CurrentBoss.Monsters = CurrentOpponents;
    }
    public void EndBattle()
    {
        if (String.IsNullOrEmpty(ReturnLocation))
        {
            BattleHasEnded = true;
            CurrentBoss = null;
            if (Player.Instance.JustDied == false &&
                AutoBattle &&
                CurrentArea != null)
            {
                WaitedAutoBattleGameTick = true;
            }
        }
        else
        {
            GameState.GoTo(ReturnLocation);
            ReturnLocation = null;
        }
    }
    private Monster GetNextTarget()
    {
        if(CurrentOpponents == null || CurrentOpponents.Count == 0)
        {
            return null;
        }
        foreach(Monster m in CurrentOpponents)
        {
            if(m.IsDefeated == false)
            {
                return m;
            }
        }
        return null;
    }

    public Monster GetMonsterByName(string name)
    {
        return Monsters.FirstOrDefault(x => x.Name == name);
    }
    public void RollForMonsterAttackEffects(Monster m)
    {
        foreach(IStatusEffect e in m.StatusEffects)
        {
            double roll = random.NextDouble();
            if(roll <= e.ProcOdds)
            {
                if (e.SelfInflicted)
                {
                    m.AddStatusEffect(e);
                }
                else
                {
                    bool activate = true;
                    if(e.Name == "Drain")
                    {
                        foreach (GameItem item in Player.Instance.GetEquippedItems())
                        {
                            if (item.EnabledActions.Contains("Drain Protection"))
                            {
                                activate = false;
                            }
                        }
                    }
                    if (activate)
                    {
                        Player.Instance.AddStatusEffect(e);

                    }
                                    
                }
                MessageManager.AddMessage(e.Message);

            }
        }
    }
    public bool CheckForQuickStrike()
    {
        foreach (GameItem item in Player.Instance.GetEquippedItems())
        {
            double roll = random.NextDouble();

            if (item.WeaponInfo != null)
            {
                foreach (IStatusEffect e in item.WeaponInfo.StatusEffects)
                {
                    if (e.Name.Contains("Quick Strike"))
                    {
                        if (roll <= e.ProcOdds)
                        {
                            e.RemainingTime = 1;
                            e.DoEffect(Player.Instance);
                            if(Player.Instance.TicksToNextAttack == 0)
                            {
                                e.RemainingTime = -1;
                                return true;
                            }
                            
                        }
                    }
                }
            }
            if (item.ArmorInfo != null)
            {
                foreach (IStatusEffect e in item.ArmorInfo.StatusEffects)
                {
                    if (e.Name.Contains("Quick Strike"))
                    {
                        if (roll <= e.ProcOdds)
                        {
                            e.RemainingTime = 1;
                            e.DoEffect(Player.Instance);
                            if (Player.Instance.TicksToNextAttack == 0)
                            {
                                e.RemainingTime = -1;
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }
    public void RollForPlayerAttackEffects()
    {
        foreach (GameItem item in Player.Instance.GetEquippedItems())
        {
            double roll = random.NextDouble();
            
            if (item.WeaponInfo != null)
            {
                foreach(IStatusEffect e in item.WeaponInfo.StatusEffects)
                {
                    if(e.Name.Contains("Quick Strike"))
                    {
                        continue;
                    }
                    if (roll <= e.ProcOdds)
                    {
                        if (e.SelfInflicted)
                        {
                            Player.Instance.AddStatusEffect(e);
                        }
                        else
                        {
                            Target.AddStatusEffect(e);

                        }
                        MessageManager.AddMessage(e.Message);
                    }
                }
            }
            if(item.ArmorInfo != null)
            {
                foreach (IStatusEffect e in item.ArmorInfo.StatusEffects)
                {
                    if (e.Name.Contains("Quick Strike"))
                    {
                        continue;
                    }
                    if (roll <= e.ProcOdds)
                    {
                        if (e.SelfInflicted)
                        {
                            Player.Instance.AddStatusEffect(e);
                        }
                        else
                        {
                            Target.AddStatusEffect(e);
                        }                        
                        MessageManager.AddMessage(e.Message);
                    }
                }
            }           
        }
    }
    public IStatusEffect GenerateStatusEffect(StatusEffectData data)
    {
        if(data.Name == "Poison")
        {
            return new PoisonEffect(data);
        }
        else if (data.Name == "Burn")
        {
            return new BurnEffect(data);
        }
        else if(data.Name == "Chicken")
        {
            return new SummonChickenEffect(data);
        }
        else if(data.Name == "Stun")
        {
            return new StunEffect(data);
        }
        else if (data.Name == "Empty")
        {
            return new EmptyEffect(data);
        }
        else if(data.Name == "Cleave")
        {
            return new CleaveEffect(data);
        }
        else if(data.Name == "Freeze")
        {
            return new FreezeEffect(data);
        }
        else if(data.Name == "Hypnotize")
        {
            return new HypnotizeEffect(data);
        }
        else if(data.Name == "Self Heal")
        {
            return new SelfHealEffect(data);
        }
        else if(data.Name == "Drain")
        {
            return new DrainEffect(data);
        }
        else if (data.Name == "Undulate")
        {
            return new UndulateEffect(data);
        }
        else if (data.Name == "Quick Strike")
        {
            return new QuickStrikeEffect(data);
        }
        else if (data.Name == "Quick Strike Gold")
        {
            return new QuickStrikeGoldEffect(data);
        }
        else if (data.Name == "Hatch")
        {
            return new HatchEffect(data);
        }
        else if (data.Name == "Reflect")
        {
            return new ReflectEffect(data);
        }
        else if (data.Name == "Entangle")
        {
            return new EntangleEffect(data);
        }
        else if (data.Name == "Rally")
        {
            return new RallyEffect(data);
        }
        else
        {
            Console.WriteLine("Warning:" + data.Name + " not in list of status effects in Battle Manager.");

        }
        return null;
    }
    public List<Monster> GetMonstersWithDrop(GameItem item)
    {
        List<Monster> monsters = new List<Monster>();
        foreach (Monster m in Monsters)
        {
            if (m.DropTable.HasDrop(item))
            {
                monsters.Add(m);
            }
        }
        return monsters;
    }
}

