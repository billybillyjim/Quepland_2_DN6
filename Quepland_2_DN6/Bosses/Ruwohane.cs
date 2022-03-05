using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quepland_2_DN6.Bosses
{
    public class Ruwohane : IBoss
    {
        public DrainEffect Drain = new DrainEffect(new StatusEffectData
        {
            CustomData = "",
            Duration = 60,
            Power = 5,
            Speed = 3,
            Message = "The Ruwohane begins to sing. The corners of your vision begin to darken. You feel weakened.",
            Name = "Drain",
            ProcOdds = 1,
            RemainingTime = 60,
            SelfInflicted = false
        });
        private List<string> BirdTypes = new List<string>() { "Egg", "Hatching", "" };
        public void OnDie(Monster monster) 
        {
            if(monster.Name == "Ruwohane")
            {
                int count = 0;
                
                foreach(Monster m in BattleManager.Instance.CurrentOpponents)
                {
                    count++;
                    m.CurrentArmor = (int)(m.CurrentArmor * 0.55);
                }
                if(count > 1)
                {
                    MessageManager.AddMessage("The Ruwohane screeches as it dies, striking fear into the remaining Aur.");
                }
               
            }
        }
        public void OnAttack() 
        {
            int roll = GameState.Random.Next(600);
            if (roll < 33)
            {
                if(CheckIfMonsterIsDead("Screeching Aur"))
                {
                    foreach(string b in BirdTypes)
                    {
                        BattleManager.Instance.RemoveOpponentMidBattle(BattleManager.Instance.GetMonsterByName("Screeching Aur " + b));
                    }
                }
                if (CheckIfMonsterExists("Screeching Aur") == false)
                {
                    BattleManager.Instance.SpawnOpponentMidBattle(BattleManager.Instance.GetMonsterByName("Screeching Aur Egg"));
                    MessageManager.AddMessage("The Ruwohane laid an egg!");
                }
            }
            else if (roll < 66)
            {
                if (CheckIfMonsterIsDead("Learing Aur"))
                {
                    foreach (string b in BirdTypes)
                    {
                        BattleManager.Instance.RemoveOpponentMidBattle(BattleManager.Instance.GetMonsterByName("Learing Aur " + b));
                    }
                }
                if (CheckIfMonsterExists("Learing Aur") == false)
                {
                    BattleManager.Instance.SpawnOpponentMidBattle(BattleManager.Instance.GetMonsterByName("Learing Aur Egg"));
                    MessageManager.AddMessage("The Ruwohane laid an egg!");
                }
            }
            else if(roll < 100)
            {
                if (CheckIfMonsterIsDead("Flammulated Aur"))
                {
                    foreach (string b in BirdTypes)
                    {
                        BattleManager.Instance.RemoveOpponentMidBattle(BattleManager.Instance.GetMonsterByName("Flammulated Aur " + b));
                    }
                }
                if (CheckIfMonsterExists("Flammulated Aur") == false)
                {
                    BattleManager.Instance.SpawnOpponentMidBattle(BattleManager.Instance.GetMonsterByName("Flammulated Aur Egg"));
                    MessageManager.AddMessage("The Ruwohane laid an egg!");
                }
            }
        }
        public void TraverseBranches()
        {

        }
        public void OnSpecialAttack() 
        {
            
            
        }
        public bool CheckIfMonsterExists(string name)
        {
            foreach(Monster opponent in BattleManager.Instance.CurrentOpponents)
            {
                if (opponent.Name.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }
        public bool CheckIfMonsterIsDead(string name)
        {
            foreach (Monster opponent in BattleManager.Instance.CurrentOpponents)
            {
                if (opponent.Name.Contains(name) && opponent.IsDefeated)
                {
                    return true;
                }
            }
            return false;
        }
        public void OnBeAttacked(Monster monster) 
        {
            if(Player.Instance.GetWeapon() == null || Player.Instance.GetWeapon().GetSkillForWeaponExp() != "Archery" || (Player.Instance.GetWeapon().GetSkillForWeaponExp() == "Archery" && Player.Instance.Inventory.HasArrows() == false))
            {
                bool activate = true;
                foreach (GameItem item in Player.Instance.GetEquippedItems())
                {
                    if (item.EnabledActions.Contains("Drain Protection"))
                    {
                        activate = false;
                    }
                }
                if (activate)
                {
                    MessageManager.AddMessage("The Ruwohane is dangerous at such a close range!", "red");
                    Player.Instance.AddStatusEffect(Drain);
                    MessageManager.AddMessage(Drain.Message);
                }
                
            }
            
            

        }
        public int SpecialAttackSpeed { get; set; } = 150;
        public int TicksToNextSpecialAttack { get; set; } = 150;
        public List<Monster> Monsters {get;set;}
        public bool CustomAttacks { get; set; } = false;

        public int Distance = 10;
        public int BranchPos = 15;
    }
}
