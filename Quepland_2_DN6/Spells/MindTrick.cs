namespace Quepland_2_DN6.Spells
{
    public class MindTrick : ISpell
    {
        public string Name { get; set; } = "Mind Trick";
        public string Description { get; set; }
        public int Power { get; set; }
        public string Message { get; set; } = "";
        public int Duration { get; set; }
        public string Target { get; set; } = "Monster";
        public int TimeRemaining { get; set; } 
		public int Cooldown { get; set; } 
		public int CooldownRemaining { get; set; }
        public string Data { get; set; } 
		public bool Unlocked { get; set; } = false;
        public List<Ingredient> Cost { get; set; }
        public MindTrick() { }
        

        public void Cast(Monster m)
        {
            if (CooldownRemaining > 0)
            {
                MessageManager.AddMessage($"You aren't quite ready to cast that spell again. ({Math.Round(CooldownRemaining / 5f, 2)})");
                return;
            }
            if (m == null)
            {
                MessageManager.AddMessage($"Nothing needs to be tricked!");
                return;
            }
            if (m.CurrentHP <= 0)
            {
                MessageManager.AddMessage($"{m.Name} is in no state to be mind tricked!");
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
            m.AddStatusEffect(new StunEffect(new StatusEffectData() { Name = Name, Duration = Duration, Power = Power, Speed = 5 }));
            CooldownRemaining = Cooldown;
            Player.Instance.GainExperience("Magic", 100);
            MessageManager.AddMessage(Message);
        }
        public ISpell Copy()
        {
            return new MindTrick() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
