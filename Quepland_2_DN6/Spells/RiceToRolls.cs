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
        public string Data { get; set; } 
		public bool Unlocked { get; set; } = false;
        public RiceToRolls() { }
        

        public void Cast(Inventory inventory)
        {
            var rice = ItemManager.Instance.GetItemByUniqueID("Rice0");
            var rolls = ItemManager.Instance.GetItemByUniqueID("Rice Roll0");
            if (inventory.HasItem(rice))
            {
                int removed = inventory.RemoveAllOfItem(rice);
                inventory.AddMultipleOfItem(rolls, removed);
                MessageManager.AddMessage(Message);
            }

            
        }
        public ISpell Copy()
        {
            return new RiceToRolls() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
