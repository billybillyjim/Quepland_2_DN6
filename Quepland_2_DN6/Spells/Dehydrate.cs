namespace Quepland_2_DN6.Spells
{
    public class Dehydrate : ISpell
    {
        public string Name { get; set; } = "Dehydrate";
        public string Description { get; set; }
        public int Power { get; set; }
        public string Message { get; set; } = "You draw all the water out from the ";
        public int Duration { get; set; }
        public string Target { get; set; } = "Item";
        public int TimeRemaining { get; set; }
        public string Data { get; set; }
        public bool Unlocked { get; set; } = false;
        public Dehydrate() { }
        

        public void Cast(Inventory inventory, GameItem item)
        {
            if (inventory.HasItem(item))
            {
                if(item.TanningInfo != null)
                {
                    if(inventory.RemoveItems(item, 1) == 1)
                    {
                        inventory.AddItem(item.TanningInfo.TansInto);
                        MessageManager.AddMessage(Message + item.Name);
                        Player.Instance.GainExperience("Magic", item.Value / 10);
                        return;
                    }
                }
                else if(item.Name == "Bottle of Water")
                {
                    if (inventory.RemoveItems(item, 1) == 1)
                    {
                        inventory.AddItem(ItemManager.Instance.GetItemByUniqueID("Empty Bottle0"));
                        MessageManager.AddMessage(Message + item.Name);
                        Player.Instance.GainExperience("Magic", item.Value / 10);
                        return;
                    }
                }
                else if(item.Name == "Bucket of Water")
                {
                    if (inventory.RemoveItems(item, 1) == 1)
                    {
                        inventory.AddItem(ItemManager.Instance.GetItemByUniqueID("Empty Bucket0"));
                        MessageManager.AddMessage(Message + item.Name);
                        Player.Instance.GainExperience("Magic", item.Value / 10);
                        return;
                    }
                }
            }
            
            MessageManager.AddMessage("You draw all the water out of the " + item.Name + ". Nothing interesting happens.");

        }
        public ISpell Copy()
        {
            return new Dehydrate() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
