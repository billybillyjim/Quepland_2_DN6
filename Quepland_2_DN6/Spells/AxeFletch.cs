namespace Quepland_2_DN6.Spells
{
    public class AxeFletch : ISpell
    {
        public string Name { get; set; } = "Axe Fletch";
        public string Description { get; set; }
        public int Power { get; set; } = 3;
        public string Message { get; set; } = "The axe in your hand begins to quiver. It seems to have taken on a mind of its own.";
        public int Duration { get; set; } = 600;
        public string Target { get; set; } = "Gather";
        public int TimeRemaining { get; set; }
        public int Cooldown { get; set; }
        public int CooldownRemaining { get; set; }
        public string Data { get; set; }

        public bool Unlocked { get; set; } = false;

        public List<Ingredient> Cost { get; set; }
        public AxeFletch() { }


        public void Cast()
        {
            if (CooldownRemaining > 0)
            {
                MessageManager.AddMessage($"You aren't quite ready to cast that spell again. ({Math.Round(CooldownRemaining / 5f, 2)})");
                return;
            }
            if (!Player.Instance.HasToolRequirement("Woodcutting"))
            {
                MessageManager.AddMessage("You'll need some kind of axe in your inventory to activate this spell.");
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
            MessageManager.AddMessage(Message);
            Player.Instance.GainExperience("Magic", 50);
        }
        private Dictionary<string, string> replacements = new Dictionary<string, string>()
        {
            {"White Pine Logs", "White Pine Arrow0"},
            {"Poplar Logs", "Poplar Arrow0"},
            {"Oak Logs", "Oak Arrow0"},
            {"Maple Logs", "Maple Arrow0"},
            {"Birch Logs", "Birch Arrow0"},
            {"Mesquite Logs", "Mesquite Arrow0"},
            {"Mahogany Logs", "Mahogany Arrow0"},
            {"Ironwood Logs", "Ironwood Arrow0"},
            {"Snakewood Logs", "Snakewood Arrow0"},
            {"Lignum Vitae Logs", "Lignum Vitae Arrow0"},
            {"Buloke Logs", "Buloke Arrow0"},
            {"Ice Logs", "Ice Arrow0"},
            {"Great Tree Logs", "Great Tree Arrow0"}
        };

        public void Cast(Inventory inventory, GameItem item)
        {
            if (replacements.ContainsKey(item.Name))
            {
                Data = replacements[item.Name];
            }
        }

        public ISpell Copy()
        {
            return new AxeFletch() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
