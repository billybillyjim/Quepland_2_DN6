namespace Quepland_2_DN6.Spells
{
    public class Enchant : ISpell
    {
        public string Name { get; set; } = "Enchant";
        public string Description { get; set; }
        public int Power { get; set; }
        public string Message { get; set; } = "";
        public int Duration { get; set; }
        public string Target { get; set; } = "Item";
        public int TimeRemaining { get; set; }
        public string Data { get; set; }
        public Enchant() { }


        public void Cast(Inventory inventory, GameItem item)
        {
            if (item.Name.Contains("Enchanted"))
            {
                MessageManager.AddMessage("This item is already enchanted.");
                return;
            }
            if (inventory.HasItem(item))
            {
                if(item.Category == "Necklaces")
                {
                    if(inventory.RemoveItems(item, 1) == 1)
                    {
                        var newItem = ItemManager.Instance.GetItemByUniqueID(item.Name + " (Enchanted)0");
                        inventory.AddItem(newItem);
                        MessageManager.AddMessage("You enchanted the " + item.Name + " into a " + newItem.Name);
                    }
                }
            }
            

        }


        public ISpell Copy()
        {
            return new Enchant() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
