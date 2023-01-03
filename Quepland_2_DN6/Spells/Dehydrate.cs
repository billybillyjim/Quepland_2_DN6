namespace Quepland_2_DN6.Spells
{
    public class Dehydrate : ISpell
    {
        public string Name { get; set; } = "Dehydrate";
        public string Description { get; set; }
        public int Power { get; set; }
        public string Message { get; set; } = "You draw all the water out from the ";
        public int Duration { get; set; }
        public string Target { get; set; } = "Item";
        public int TimeRemaining { get; set; } 
		public int Cooldown { get; set; } 
		public int CooldownRemaining { get; set; }
        public string Data { get; set; }
        public bool Unlocked { get; set; } = false;
        public List<Ingredient> Cost { get; set; }
        public Dehydrate() { }
        

        public void Cast(Inventory inventory, GameItem item)
        {
            if (CooldownRemaining > 0)
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
            if (inventory.HasItem(item))
            {
                if(item.TanningInfo != null)
                {
                    if(inventory.RemoveItems(item, 1) == 1)
                    {
                        inventory.AddItem(item.TanningInfo.TansInto);
                        CooldownRemaining = Cooldown;
                        MessageManager.AddMessage(Message + item.Name);
                        Player.Instance.GainExperience("Magic", item.Value / 10);
                        return;
                    }
                }
                else if(item.Name == "Bottle of Water")
                {
                    if (inventory.RemoveItems(item, 1) == 1)
                    {
                        inventory.AddItem(ItemManager.Instance.GetItemByUniqueID("Empty Bottle0"));
                        CooldownRemaining = Cooldown;
                        MessageManager.AddMessage(Message + item.Name);
                        Player.Instance.GainExperience("Magic", item.Value / 10);
                        return;
                    }
                }
                else if(item.Name == "Bucket of Water")
                {
                    if (inventory.RemoveItems(item, 1) == 1)
                    {
                        inventory.AddItem(ItemManager.Instance.GetItemByUniqueID("Empty Bucket0"));
                        CooldownRemaining = Cooldown;
                        MessageManager.AddMessage(Message + item.Name);
                        Player.Instance.GainExperience("Magic", item.Value / 10);
                        return;
                    }
                }
            }
            
            MessageManager.AddMessage("You draw all the water out of the " + item.Name + ". Nothing interesting happens.");

        }
        public ISpell Copy()
        {
            return new Dehydrate() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
