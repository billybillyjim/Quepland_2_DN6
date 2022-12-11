namespace Quepland_2_DN6.Spells
{
    public class Rain : ISpell
    {
        public string Name { get; set; } = "Rain";
        public string Description { get; set; }
        public int Power { get; set; }
        public string Message { get; set; } = "The air grows thick with humidity, enveloping you in a thick fog. A sudden updraft clears your view and it begins to rain.";
        public int Duration { get; set; }
        public string Target { get; set; } = "Inventory";
        public int TimeRemaining { get; set; } 
		public int Cooldown { get; set; } 
		public int CooldownRemaining { get; set; }
        public string Data { get; set; } 
		public bool Unlocked { get; set; } = false;
        public Rain() { }
        

        public void Cast(Inventory inventory)
        {
            if (CooldownRemaining > 0)
            {
                MessageManager.AddMessage($"You aren't quite ready to cast that spell again. ({Math.Round(CooldownRemaining / 5f, 2)})");
                return;
            }
            var buckets = ItemManager.Instance.GetItemByUniqueID("Empty Bucket0");
            var filled = ItemManager.Instance.GetItemByUniqueID("Bucket of Water0");
            var amt = inventory.RemoveItems(buckets, 1000);
            inventory.AddMultipleOfItem(filled, amt);

            var bucketmsg = amt > 0 ? $" The water pours down and fills {amt} {(amt != 1 ? "buckets" : "bucket")} with water." : "";

            var bottles = ItemManager.Instance.GetItemByUniqueID("Empty Bottle0");
            var filledBottles = ItemManager.Instance.GetItemByUniqueID("Bottle of Water0");
            var amount = inventory.RemoveItems(bottles, 1000);
            inventory.AddMultipleOfItem(filledBottles, amount);

            var bottlemsg = amt > 0 ? $" The water pours down and fills {amt} {(amt != 1 ? "bottles" : "bottle")} with water." : "";
            CooldownRemaining = Cooldown;
            MessageManager.AddMessage(Message + bucketmsg + bottlemsg);
        }
        public ISpell Copy()
        {
            return new Rain() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
