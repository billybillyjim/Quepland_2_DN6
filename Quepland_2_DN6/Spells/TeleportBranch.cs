namespace Quepland_2_DN6.Spells
{
    public class TeleportBranch : ISpell
    {
        public string Name { get; set; } = "Teleport Branch";
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
        public TeleportBranch() { }

        public void Cast(Inventory inventory)
        {
            if (CooldownRemaining > 0)
            {
                MessageManager.AddMessage($"You aren't quite ready to cast that spell again. ({Math.Round(CooldownRemaining / 5f, 2)})");
                return;
            }
            ISpell spell = this;
            if (!spell.CanPayCost())
            {
                MessageManager.AddMessage($"You don't have the seeds or MP to cast this spell.");
                return;
            }
            
            int amountToRemove = Math.Min(inventory.GetAvailableSpaces(), Player.Instance.GetLevel("Magic"));
            if(amountToRemove == 0)
            {
                MessageManager.AddMessage($"You don't have any inventory space to cast this spell.");
                return;
            }

            var branch = ItemManager.Instance.GetItemByUniqueID("Sticks0");
            if (inventory.HasItem(branch))
            {
                spell.PayCost();
                var newItem = ItemManager.Instance.GetItemByUniqueID("Transit Branch0");
                
                int amountRemoved = inventory.RemoveItems(branch, amountToRemove);

                inventory.AddMultipleOfItem(newItem, amountRemoved);
                CooldownRemaining = Cooldown;
                Player.Instance.GainExperience("Magic", 250);
                MessageManager.AddMessage($"You imbued the {branch.Name} with magic, getting {amountRemoved} {newItem.GetName(amountRemoved)}.");
                
            }
            else
            {
                MessageManager.AddMessage($"You don't have any sticks.");
            }
        }

        public ISpell Copy()
        {
            return new TeleportBranch() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
