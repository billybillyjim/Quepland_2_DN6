namespace Quepland_2_DN6.Spells
{
    public class Endure : ISpell
    {
        public string Name { get; set; } = "Endure";
        public string Description { get; set; }
        public int Power { get; set; } = 1;
        public string Message { get; set; } = "You feel prepared to handle the next hit!";
        public int Duration { get; set; } = 30;
        public string Target { get; set; } = "Player";
        public int TimeRemaining { get; set; } 
		public int Cooldown { get; set; } 
		public int CooldownRemaining { get; set; }
        public string Data { get; set; } 
		public bool Unlocked { get; set; } = false;
        public List<Ingredient> Cost { get; set; }
        public Endure() { }


        public void Cast(Player player)
        {
            if (CooldownRemaining > 0)
            {
                MessageManager.AddMessage($"You aren't quite ready to cast that spell again. ({Math.Round(CooldownRemaining / 5f, 2)})");
                return;
            }
            if (BattleManager.Instance.BattleHasEnded)
            {
                MessageManager.AddMessage($"There's nothing you need to endure right now!");
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
            MessageManager.AddMessage(Message);
            Player.Instance.GainExperience("Magic", 120);
        }

        public ISpell Copy()
        {
            return new Endure() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
