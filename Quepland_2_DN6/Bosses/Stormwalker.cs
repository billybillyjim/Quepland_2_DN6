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
        private int attackedCount = 0;
        private List<string> weaknessRotation = new List<string>()
        {
            "Hammermanship",
            "Axemanship",
            "Swordsmanship",
            "Knifesmanship"
        };
        private List<string> strengthRotation = new List<string>()
        {
            "Knifesmanship Swordsmanship Axemanship Archery",
            "Knifesmanship Swordsmanship Hammermanship Archery",
            "Knifesmanship Axemanship Hammermanship Archery",
            "Swordsmanship Axemanship Hammermanship Archery"
        };
        private List<string> weaknessRotationMessages = new List<string>()
        {
            "The Stormwalker grows a thick, brittle shell of bark!",
            "The Stormwalker grows an armored layer of trees!",
            "The Stormwalker covers itself in vines!",
            "The Stormwalker brings forth protective thistles!"
        };
        private List<int> weaknessRotationArmors = new List<int>()
        {
            600,
            425,
            320,
            140
        };
        private int currentWeaknessRotation = 0;
        public void OnBeAttacked(Monster monster) 
        {            
            attackedCount++;
            if(attackedCount > 4)
            {
                attackedCount = 0;
                currentWeaknessRotation++;
                if(currentWeaknessRotation >= weaknessRotation.Count)
                {
                    currentWeaknessRotation = 0;
                }
                monster.Strengths = strengthRotation[currentWeaknessRotation];
                monster.Weaknesses = weaknessRotation[currentWeaknessRotation];
                monster.TicksToNextAttack = 4 - currentWeaknessRotation * 10;
                MessageManager.AddMessage(weaknessRotationMessages[currentWeaknessRotation], "red");

            }


        }
        public int SpecialAttackSpeed { get; set; } = 150;
        public int TicksToNextSpecialAttack { get; set; } = 150;
        public List<Monster> Monsters {get;set; } = new List<Monster>();
        public bool CustomAttacks { get; set; } = false;
    }
}
