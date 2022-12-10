namespace Quepland_2_DN6.Spells
{
    public class Breathe : ISpell
    {
        public string Name { get; set; } = "Breathe";
        public string Description { get; set; }
        public int Power { get; set; }
        public string Message { get; set; } = "Your lungs fill and are satisfied. For a while, you don't feel the need to inhale or exhale at all.";
        public int Duration { get; set; } = 600;
        public string Target { get; set; } = "Player";
        public int TimeRemaining { get; set; }
        public string Data { get; set; }
        public bool Unlocked { get; set; } = false;
        public Breathe() { }
        

        public void Cast(Player player)
        {

            Player.Instance.Air = 100;
            GameState.AddActiveSpell(this.Copy(), Duration);
            MessageManager.AddMessage(Message);
            Player.Instance.GainExperience("Magic", 70);
        }
        public void Tick(Player player)
        {
            Player.Instance.Air = 101;
        }
        public ISpell Copy()
        {
            return new Breathe() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
