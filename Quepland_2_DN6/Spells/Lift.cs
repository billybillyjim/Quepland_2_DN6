namespace Quepland_2_DN6.Spells
{
    public class Lift : ISpell
    {
        public string Name { get; set; } = "Lift";
        public string Description { get; set; }
        public int Power { get; set; } = 15;
        public string Message { get; set; } = "Your weapon feels lighter now. You feel like you can really swing it around.";
        public int Duration { get; set; } = 30;
        public string Target { get; set; } = "Player";
        public int TimeRemaining { get; set; }
        public string Data { get; set; }
        public Lift() { }
        

        public void Cast(Player player)
        {
            if(Player.Instance.GetWeaponAttackSpeed() > 15)
            {
                GameState.AddActiveSpell(this.Copy(), Duration);
                MessageManager.AddMessage(Message);
            }
            else
            {
                MessageManager.AddMessage("Your weapon feels lighter now, but it doesn't seem to make much of a difference. Maybe with a heavier weapon...");
            }
        }
        public void Tick(Player player)
        {
            player.TicksToNextAttack = Math.Min(player.TicksToNextAttack, Power);
        }
        public ISpell Copy()
        {
            return new Lift() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
