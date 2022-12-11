namespace Quepland_2_DN6.Spells
{
    public class Consume : ISpell
    {
        public string Name { get; set; } = "Consume";
        public string Description { get; set; }
        public int Power { get; set; }
        public string Message { get; set; } = "";
        public int Duration { get; set; }
        public string Target { get; set; } = "Item";
        public int TimeRemaining { get; set; } 
		public int Cooldown { get; set; } 
		public int CooldownRemaining { get; set; }
        public string Data { get; set; } 
		public bool Unlocked { get; set; } = false;
        public Consume() { }
        

        public void Cast(Inventory inventory, GameItem item)
        {
            if (CooldownRemaining > 0)
            {
                MessageManager.AddMessage($"You aren't quite ready to cast that spell again. ({Math.Round(CooldownRemaining / 5f, 2)})");
                return;
            }
            if (item.FoodInfo != null)
            {
                if (inventory.HasItem(item))
                {
                    int amt = inventory.GetNumberOfUnlockedItem(item);
                    GameState.Eat(item, amt, amt * item.FoodInfo.HealDuration);
                    CooldownRemaining = Cooldown;
                    MessageManager.AddMessage(Message);
                }
            }
            
        }
        public ISpell Copy()
        {
            return new Consume() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
