namespace Quepland_2_DN6.Spells
{
    public class Entangle : ISpell
    {
        public string Name { get; set; } = "Entangle";
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
        public Entangle() { }
        

        public void Cast(Monster m)
        {
            if (CooldownRemaining > 0)
            {
                MessageManager.AddMessage($"You aren't quite ready to cast that spell again. ({Math.Round(CooldownRemaining / 5f, 2)})");
                return;
            }
            if(m == null)
            {
                MessageManager.AddMessage($"Nothing needs to be entangled!");
                return;
            }
            if (m.CurrentHP <= 0)
            {
                MessageManager.AddMessage($"{m.Name} doesn't need to be entangled to not go anywhere!");
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
            var dmg = (int)(Power * Player.Instance.GetLevel("Magic") * Player.Instance.GetMagicDamage());
            m.AddStatusEffect(new EntangleEffect(new StatusEffectData() {  Name=Name, Duration = Duration, Power = dmg, Speed = 5}));
            CooldownRemaining = Cooldown;
            MessageManager.AddMessage(m.Name + Message);
            Player.Instance.GainExperience("Magic", Power);
        } 

        public ISpell Copy()
        {
            return new Entangle() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
