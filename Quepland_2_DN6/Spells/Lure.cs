namespace Quepland_2_DN6.Spells
{
    public class Lure : ISpell
    {
        public string Name { get; set; } = "Lure";
        public string Description { get; set; }
        public int Power { get; set; }
        public string Message { get; set; } = "";
        public int Duration { get; set; }
        public string Target { get; set; } = "None";
        public int TimeRemaining { get; set; } 
		public int Cooldown { get; set; } 
		public int CooldownRemaining { get; set; }
        public string Data { get; set; } 
		public bool Unlocked { get; set; } = false;
        public List<Ingredient> Cost { get; set; }
        public Lure() { }
        

        public void Cast()
        {
            if (CooldownRemaining > 0)
            {
                MessageManager.AddMessage($"You aren't quite ready to cast that spell again. ({Math.Round(CooldownRemaining / 5f, 2)})");
                return;
            }

            if (!Player.Instance.HasToolRequirement("Fishing"))
            {
                MessageManager.AddMessage("You'll need some kind of fishing gear in your inventory to activate this spell.");
                return;
            }
            ISpell spell = this;
            if (!spell.PayCost())
            {
                MessageManager.AddMessage($"You don't have the seeds or MP to cast this spell.");
                return;
            }
            GameState.AddActiveSpell(this, Duration);
            CooldownRemaining = Cooldown;
            MessageManager.AddMessage(Message);
            Player.Instance.GainExperience("Magic", 50);
        }
        public void Cast(Inventory inventory, GameItem item)
        {
            if (item.Category == "Fishing")
            {
                inventory.AddItem(item);
                MessageManager.AddMessage($"An extra {item.Name} appears in your inventory.");
                Data = item.UniqueID;
            }
        }

        public ISpell Copy()
        {
            return new Lure() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
