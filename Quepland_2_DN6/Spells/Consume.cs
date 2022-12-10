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
        public string Data { get; set; } 
		public bool Unlocked { get; set; } = false;
        public Consume() { }
        

        public void Cast(Inventory inventory, GameItem item)
        {
            if(item.FoodInfo != null)
            {
                if (inventory.HasItem(item))
                {
                    int amt = inventory.GetNumberOfUnlockedItem(item);
                    GameState.Eat(item, amt, amt * item.FoodInfo.HealDuration);
                }
            }

            MessageManager.AddMessage(Message);
        }
        public ISpell Copy()
        {
            return new Consume() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
