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
            { "Copper Ore", "Molten Copper Bar" },
            { "Tin Ore", "Molten Tin Bar" },
            { "Calamine", "Molten Zinc Bar" },
            { "Pentlandite", "Molten Nickel Bar" },
            { "Iron Ore", "Molten Iron Bar" },
            { "Coal", "Ashes" },
            { "Silver Ore", "Molten Silver Bar" },
            { "Galena", "Molten Lead Bar" },
            { "Cinnabar", "" },
            { "Bauxite", "Molten Aluminum Bar" },
            { "Chelonite", "Molten Iseroite Bar" },
            { "Gold Ore", "Molten Gold Bar" },
            { "Platinum Ore", "Molten Platinum Bar" },
            { "Strongtium Ore", "Molten Strongtium Bar" },
            { "Sahotite Ore", "" },
            { "Plastinium Ore", "Molten Plastinium Bar" },
            { "Queplinium Ore", "Molten Queplinium Bar" }
        };

        public void Cast()
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
            if (Player.Instance.HasToolRequirement("Mining"))
            {
                GameState.AddActiveSpell(this, Duration);
                CooldownRemaining = Cooldown;
                Player.Instance.GainExperience("Magic", 145);
                MessageManager.AddMessage(Message);
            }
            else
            {
                MessageManager.AddMessage("You'll need some kind of pickaxe in your inventory to activate this spell.");
            }
        }

        public void Cast(Inventory inventory, GameItem item)
        {
            if (replacements.ContainsKey(item.Name))
            {
                Data = replacements[item.Name];
            }
        }

        public ISpell Copy()
        {
            return new MoltenSwing() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
