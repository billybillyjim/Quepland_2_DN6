using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quepland_2_DN6.Bosses
{
    public class Stormwalker : IBoss
    {
        public void OnDie(Monster monster) 
        {
            
        }
        public void OnAttack() { }
        public void OnSpecialAttack() 
        {
            //Player.Instance.TicksToNextAttack = Player.Instance.GetWeaponAttackSpeed();
            //MessageManager.AddMessage("The Gashadokuro slams the bridge, you lose your balance and need to recover before attacking!", "red");
            //TicksToNextSpecialAttack = SpecialAttackSpeed;
        }
        public void OnBeAttacked(Monster monster) 
        {
           // if(monster.Name == "Gashadokuro Spine")
           // {
            //    if(monster.CurrentArmor == 140)
            //    {
           //         MessageManager.AddMessage("You try to attack the spine, but the ribcage blocks most of the damage!", "red");
           //     }               
           // }
            

        }
        public int SpecialAttackSpeed { get; set; } = 150;
        public int TicksToNextSpecialAttack { get; set; } = 150;
        public List<Monster> Monsters {get;set; } = new List<Monster>();
        public bool CustomAttacks { get; set; } = false;
    }
}
