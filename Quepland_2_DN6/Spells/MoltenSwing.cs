namespace Quepland_2_DN6.Spells
{
    public class MoltenSwing : ISpell
    {
        public string Name { get; set; } = "Molten Swing";
        public string Description { get; set; }
        public int Power { get; set; } = 3;
        public string Message { get; set; } = "The pickaxe in your hand begins to glow white hot.";
        public int Duration { get; set; } = 600;
        public string Target { get; set; } = "Gather";
        public int TimeRemaining { get; set; } 
		public int Cooldown { get; set; } 
		public int CooldownRemaining { get; set; }
        public string Data { get; set; } 
		public bool Unlocked { get; set; } = false;
        public List<Ingredient> Cost { get; set; }
        public MoltenSwing() { }

        public static Dictionary<string, string> replacements = new Dictionary<string, string>()
        {
            { "Copper Ore", "Molten Copper Bar0" },
            { "Tin Ore", "Molten Tin Bar0" },
            { "Calamine", "Molten Zinc Bar0" },
            { "Pentlandite", "Molten Nickel Bar0" },
            { "Iron Ore", "Molten Iron Bar0" },
            { "Coal", "Ashes0" },
            { "Silver Ore", "Molten Silver Bar0" },
            { "Galena", "Molten Lead Bar0" },
            { "Cinnabar", "" },
            { "Bauxite", "Molten Aluminum Bar0" },
            { "Chelonite", "Molten Iseroite Bar0" },
            { "Gold Ore", "Molten Gold Bar0" },
            { "Platinum Ore", "Molten Platinum Bar0" },
            { "Strongtium Ore", "Molten Strongtium Bar0" },
            { "Sahotite Ore", "" },
            { "Plastinium Ore", "Molten Plastinium Bar0" },
            { "Queplinium Ore", "Molten Queplinium Bar0" }
        };

        public void Cast()
        {
            if (CooldownRemaining > 0)
            {
                MessageManager.AddMessage($"You aren't quite ready to cast that spell again. ({Math.Round(CooldownRemaining / 5f, 2)})");
                return;
            }
            if (!Player.Instance.HasToolRequirement("Mining"))
            {
                MessageManager.AddMessage("You'll need some kind of pickaxe in your inventory to activate this spell.");
                GameState.CancelAutoCastSpells.Add(this);
                return;
            }
            ISpell spell = this;
            if (!spell.CanPayCost())
            {
                MessageManager.AddMessage($"You don't have the seeds or MP to cast this spell.");
                return;
            }
            spell.PayCost();
            GameState.AddActiveSpell(this, Duration);
            CooldownRemaining = Cooldown;
            Player.Instance.GainExperience("Magic", 145);
            MessageManager.AddMessage(Message);
        }

        public void Cast(Inventory inventory, GameItem item)
        {
            if (replacements.ContainsKey(item.Name))
            {
                if(item.Name == "Cinnabar")
                {
                    MessageManager.AddMessage("The cinnabar bursts into a thousand shards as you strike it with your pickaxe.");
                }
                else if(item.Name == "Sahotite Ore")
                {
                    MessageManager.AddMessage("The sahotite bursts into a cloud of vapor as you strike it with your pickaxe.");
                }
                else if (item.Name == "Coal")
                {
                    MessageManager.AddMessage("The coal quickly burns into ash as you strike it with your pickaxe.");
                }
                else
                {
                    Data = replacements[item.Name];
                }
                
            }
        }

        public ISpell Copy()
        {
            return new MoltenSwing() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
