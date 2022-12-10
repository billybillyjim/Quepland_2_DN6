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
		public bool Unlocked { get; set; } = false;
        public Enchant() { }

        private Dictionary<string, string> enchantmentTable = new Dictionary<string, string>()
        {
            { "Silver Necklace" , "Necklace of Purity" },
            { "Gold Necklace" , "Necklace of Courage" },
            { "Platinum Necklace" , "Necklace of Valor" },
            { "Amberite Necklace" , "Incubation Necklace" },
            { "Lizardite Necklace" , "Miasmic Necklace" },
            { "Purpurite Necklace" , "Reverberation Necklace" },
            { "Opal Necklace" , "Oscillation Necklace" },
            { "Garnet Necklace" , "Piercing Necklace" },
            { "Amethyst Necklace" , "Sleeping Necklace" },
            { "Emerald Necklace" , "Perception Necklace" },
            { "Aquamarine Necklace" , "Restoration Necklace" },
            { "Topaz Necklace" , "Protection Necklace" },
            { "Chrysoberyl Necklace" , "Anticipatory Necklace" },
            { "Sapphire Necklace" , "Illusion Necklace" },
            { "Ruby Necklace" , "Incineration Necklace" },
            { "Diamond Necklace" , "Demolition Necklace" },
            { "Potaki's Tear" , "Potaki's Blessing" },
            { "Intricate Necklace" , "Labyrinthine Necklace" }
        };

        public void Cast(Inventory inventory, GameItem item)
        {
            if (item.Name.Contains("Enchanted"))
            {
                MessageManager.AddMessage("This item is already enchanted.");
                return;
            }
            if (inventory.HasItem(item))
            {
                if (enchantmentTable.TryGetValue(item.Name, out var newItemName))
                {
                    var newItem = ItemManager.Instance.GetItemByUniqueID(newItemName + "0");
                
                    if(inventory.RemoveItems(item, 1) == 1)
                    {
                        inventory.AddItem(newItem);
                        MessageManager.AddMessage("You enchanted the " + item.Name + " into a " + newItem.Name);
                        Player.Instance.GainExperience("Magic", item.Value / 8);
                    }
                }
                else
                {
                    MessageManager.AddMessage("You cannot seem to enchant this item.");
                }
            }
            

        }


        public ISpell Copy()
        {
            return new Enchant() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
