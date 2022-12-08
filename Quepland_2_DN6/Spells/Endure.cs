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
        public string Data { get; set; }
        public Endure() { }


        public void Cast(Player player)
        {
            GameState.AddActiveSpell(this.Copy(), Duration);
            MessageManager.AddMessage(Message);
        }

        public ISpell Copy()
        {
            return new Endure() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
