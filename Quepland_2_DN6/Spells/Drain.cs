namespace Quepland_2_DN6.Spells
{
    public class Drain : ISpell
    {
        public string Name { get; set; } = "Drain";
        public string Description { get; set; }
        public int Power { get; set; } = 15;
        public string Message { get; set; } = "";
        public int Duration { get; set; } = 30;
        public string Target { get; set; } = "Monster";
        public int TimeRemaining { get; set; } 
		public int Cooldown { get; set; } 
		public int CooldownRemaining { get; set; }
        public string Data { get; set; } 
		public bool Unlocked { get; set; } = false;
        public List<Ingredient> Cost { get; set; }

        public Drain() { }
        

        public void Cast(Monster m)
        {
            if (CooldownRemaining > 0)
            {
                MessageManager.AddMessage($"You aren't quite ready to cast that spell again. ({Math.Round(CooldownRemaining / 5f, 2)})");
                return;
            }
            if (m.CurrentHP <= 0)
            {
                MessageManager.AddMessage($"{m.Name} doesn't have any energy left to drain!");
                return;
            }
            if (BattleManager.Instance.BattleHasEnded)
            {
                MessageManager.AddMessage($"{m.Name} is long gone!");
                return;
            }
            ISpell spell = this;
            if (!spell.CanPayCost())
            {
                MessageManager.AddMessage($"You don't have the seeds or MP to cast this spell.");
                return;
            }
            spell.PayCost();
            var dmg = Power * Player.Instance.GetLevel("Magic");
            m.AddStatusEffect(new DrainEffect(new StatusEffectData() {  Name=Name, Duration = Duration, Power = dmg, Speed = 5}));
            CooldownRemaining = Cooldown;
            MessageManager.AddMessage("You sap the " + m.Name + " of its energy!");
            Player.Instance.GainExperience("Magic", dmg);
        }

        public ISpell Copy()
        {
            return new Drain() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
