namespace Quepland_2_DN6.Spells
{
    public class RiceToRolls : ISpell
    {
        public string Name { get; set; } = "Rice to Rolls";
        public string Description { get; set; }
        public int Power { get; set; }
        public string Message { get; set; } = "";
        public int Duration { get; set; }
        public string Target { get; set; } = "Inventory";
        public int TimeRemaining { get; set; } 
		public int Cooldown { get; set; } 
		public int CooldownRemaining { get; set; }
        public string Data { get; set; } 
		public bool Unlocked { get; set; } = false;
        public List<Ingredient> Cost { get; set; }
        public RiceToRolls() { }
        

        public void Cast(Inventory inventory)
        {
            if(CooldownRemaining > 0)
            {
                MessageManager.AddMessage($"You aren't quite ready to cast that spell again. ({Math.Round(CooldownRemaining / 5f, 2)})");
                return;
            }
            ISpell spell = this;
            if (!spell.PayCost())
            {
                MessageManager.AddMessage($"You don't have the seeds or MP to cast this spell.");
                return;
            }
            var rice = ItemManager.Instance.GetItemByUniqueID("Rice0");
            var rolls = ItemManager.Instance.GetItemByUniqueID("Rice Roll0");
            if (inventory.HasItem(rice))
            {
                int removed = inventory.RemoveAllOfItem(rice);
                inventory.AddMultipleOfItem(rolls, removed);
                CooldownRemaining = Cooldown;
                MessageManager.AddMessage(Message);
                Player.Instance.GainExperience("Magic", 145);
            }

            
        }

        public ISpell Copy()
        {
            return new RiceToRolls() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
