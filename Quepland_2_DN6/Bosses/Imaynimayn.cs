using Quepland_2_DN6.Bosses.ImaynimaynElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quepland_2_DN6.Bosses
{
    public class Imaynimayn : IBoss
    {
        public void OnDie(Monster monster)
        {
            Drop d = ItemManager.Instance.GetMinigameDropTable("Sotu Nran Waterfall").DropTable.GetDrop();
            MessageManager.AddMessage("The creature screams in rage and disappears under the water. The waterfall opens up to reveal a pile of treasure. You grab " + d.Amount + " " + d.Item + " quickly and escape before the creature returns.");
            Player.Instance.Inventory.AddDrop(d, out _);
        }
        public void OnAttack() 
        {
            currentTick++;
            foreach(Lilypad pad in Lilypads)
            {
                pad.Tick();
                if(PlayerPosition == pad.Position && pad.HasFallen)
                {
                    if(Player.Instance.Inventory.HasItem("Potaki's Blessing0", false))
                    {
                        MessageManager.AddMessage("Potaki's Blessing glows a brilliant blue light! The water feels like its boiling but it does not hurt you!");
                    }
                    else if (Player.Instance.Inventory.HasItem("Potaki's Tear0", false))
                    {
                        MessageManager.AddMessage("Potaki's Tear glows a shining blue light! The water feels like its draining your life force slightly!");
                        Player.Instance.CurrentHP -= (Player.Instance.MaxHP / 25);
                        Monsters[0].CurrentHP += Player.Instance.MaxHP / 20;
                    }
                    else
                    {
                        MessageManager.AddMessage("The water feels like its draining your life force!", "red");
                        Player.Instance.CurrentHP -= (Player.Instance.MaxHP / 5);
                        Monsters[0].CurrentHP += Player.Instance.MaxHP / 4;
                    }
                    
                }
            }
            if(currentTick % attackRatio == 0)
            {
                BattleManager.Instance.BeAttacked(BattleManager.Instance.GetMonsterByName("Imaynimayn"));
            }
        }
        public void OnSpecialAttack()
        {
            if (Player.Instance.Inventory.HasItem("Potaki's Blessing0", false))
            {
                MessageManager.AddMessage("Potaki's Blessing glows a brilliant blue light! The creature roars to no effect!");
            }
            else if (Player.Instance.Inventory.HasItem("Potaki's Tear0", false))
            {
                MessageManager.AddMessage("Potaki's Tear glows a shining blue light! The creature roars and draws some of your life away!");
                Player.Instance.CurrentHP -= (Player.Instance.MaxHP / 35);
                Monsters[0].CurrentHP += Player.Instance.MaxHP / 30;
            }
            else
            {
                Player.Instance.CurrentHP -= (Player.Instance.MaxHP / 7);
                Monsters[0].CurrentHP += Player.Instance.MaxHP / 7;
                MessageManager.AddMessage("The creature roars and draws your life away.");
            }

            if(NextLilypadTarget != null)
            {
                if(Monsters[0].CurrentHP % 2 == 0)
                {
                    NextLilypadTarget.Fall = true;
                    NextLilypadTarget.CurrentTick = NextLilypadTarget.TicksToFall;
                    MessageManager.AddMessage("You feel the lilypad beneath your feet begin to tremble.", "red");
                }
            }
            TicksToNextSpecialAttack = SpecialAttackSpeed;
            
        }
        public void OnBeAttacked(Monster monster)
        {

        }
        public int SpecialAttackSpeed { get; set; } = 25;
        public int TicksToNextSpecialAttack { get; set; } = 25;
        private int attackRatio = 13;
        private int currentTick = 0;
        public List<Monster> Monsters { get; set; }
        public Lilypad NextLilypadTarget;
        public List<Lilypad> Lilypads = new List<Lilypad>();
        public string PlayerPosition { get; set; } = "Top";
        public bool CustomAttacks { get; set; } = true;
    }
}
