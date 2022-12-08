namespace Quepland_2_DN6.Spells
{
    public class TeleportBranch : ISpell
    {
        public string Name { get; set; } = "Teleport Branch";
        public string Description { get; set; }
        public int Power { get; set; }
        public string Message { get; set; } = "";
        public int Duration { get; set; }
        public string Target { get; set; } = "Item";
        public int TimeRemaining { get; set; }
        public string Data { get; set; }
        public TeleportBranch() { }

        public void Cast(Inventory inventory, GameItem item)
        {
            if (inventory.HasItem(item))
            {
                var newItem = ItemManager.Instance.GetItemByUniqueID("Transit Branch0");

                if (inventory.RemoveItems(item, 1) == 1)
                {
                    inventory.AddItem(newItem);
                    MessageManager.AddMessage("You imbued the " + item.Name + " with magic. Now it's a " + newItem.Name);
                }
            }
        }

        public ISpell Copy()
        {
            return new TeleportBranch() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
