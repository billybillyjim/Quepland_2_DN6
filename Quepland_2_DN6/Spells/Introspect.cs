namespace Quepland_2_DN6.Spells
{
    public class Introspect : ISpell
    {
        public string Name { get; set; } = "Introspect";
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
        public Introspect() { }


        public void Cast(Inventory inventory, GameItem item)
        {
            if (CooldownRemaining > 0)
            {
                MessageManager.AddMessage($"You aren't quite ready to cast that spell again. ({Math.Round(CooldownRemaining / 5f, 2)})");
                return;
            }
            if (inventory.HasItem(item))
            {
                if(item.AlchemyInfo != null)
                {
                    if(item.AlchemyInfo.QueplarValue != 0 && item.AlchemyInfo.QueplarMultiplier == 0)
                    {
                        MessageManager.AddMessage($"The {item.Name} quivers. A blast of knowledge echoes throughout your mind. You see it clearly. A number forms... {item.AlchemyInfo.QueplarValue}.");
                    }
                    else if (item.AlchemyInfo.QueplarValue == 0 && item.AlchemyInfo.QueplarMultiplier != 0)
                    {
                        MessageManager.AddMessage($"The {item.Name} quivers. A blast of knowledge echoes throughout your mind. You see it clearly. A number forms... {item.AlchemyInfo.QueplarMultiplier}.");
                    }
                    else if (item.AlchemyInfo.QueplarValue != 0 && item.AlchemyInfo.QueplarMultiplier != 0)
                    {
                        MessageManager.AddMessage($"The {item.Name} quivers. A blast of knowledge echoes throughout your mind. You see them clearly. Two numbers form... {item.AlchemyInfo.QueplarValue} and {item.AlchemyInfo.QueplarMultiplier}.");
                    }
                    Player.Instance.GainExperience("Magic", 120);
                }
                else
                {
                    MessageManager.AddMessage("Nothing seems to happen with this item...");
                }
                CooldownRemaining = Cooldown;
            }
            
        }
        public ISpell Copy()
        {
            return new Introspect() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
