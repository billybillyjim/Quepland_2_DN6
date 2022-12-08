namespace Quepland_2_DN6.Spells
{
    public class Extract : ISpell
    {
        public string Name { get; set; } = "Extract";
        public string Description { get; set; } = "Draw the essence out of an item, concentrating it into magical seeds.";
        public int Power { get; set; }
        public string Message { get; set; } = "";
        public int Duration { get; set; } = 0;
        public string Target { get; set; } = "Item";
        public int TimeRemaining { get; set; }
        public string Data { get; set; }
        public Extract() { }
        

        public void Cast(Inventory inventory, GameItem item)
        {
            if (inventory.HasItem(item))
            {
                var components = GetComponents(item);

                if(components.Count > 0)
                {
                    int req = components.Count;
                    foreach(KeyValuePair<GameItem, int> i in components)
                    {
                        if (inventory.HasItem(i.Key))
                        {
                            req--;
                        }
                    }
                    if(inventory.GetAvailableSpaces() >= req)
                    {
                        if(inventory.RemoveItems(item, 1) == 1)
                        {
                            foreach(KeyValuePair<GameItem, int> i in components)
                            {
                                inventory.AddMultipleOfItem(i.Key, i.Value);
                            }
                        }
                    }
                }
            }

            MessageManager.AddMessage(Message);
        }

        private Dictionary<GameItem, int> GetComponents(GameItem item)
        {
            Dictionary<GameItem, int> components = new Dictionary<GameItem, int>();
            if (item.IsSellable == false ||
                item.IsLocked ||
                item.IsEquipped)
            {
                MessageManager.AddMessage("");
                return components;

            }
            if(item.Category == "Armors")
            {
                int amt = (int)Math.Log10(item.Value) * 3;
                components.Add(ItemManager.Instance.GetItem("Perpetual Seed", 0, ""), amt);
            }
            else if(item.Category == "Arrows")
            {
                int amt = (int)Math.Log10(item.Value) * 2;
                components.Add(ItemManager.Instance.GetItem("Fierce Seed", 0, ""), amt);
            }
            else if (item.Category == "Arrowtips")
            {
                int amt = (int)Math.Log10(item.Value);
                components.Add(ItemManager.Instance.GetItem("Fierce Seed", 0, ""), amt);
            }
            else if (item.Category == "Bars")
            {
                int amt = (int)Math.Log10(item.Value);
                components.Add(ItemManager.Instance.GetItem("Alimental Seed", 0, ""), amt);
            }
            else if (item.Category == "Bows")
            {
                int amt = (int)Math.Log10(item.Value);
                components.Add(ItemManager.Instance.GetItem("Fierce Seed", 0, ""), amt);
            }
            else if (item.Category == "Bread")
            {
                int amt = (int)Math.Log10(item.Value) * 2 ;
                components.Add(ItemManager.Instance.GetItem("Warmth Seed", 0, ""), amt);
                components.Add(ItemManager.Instance.GetItem("Rejuvinating Seed", 0, ""), amt);
            }
            else if (item.Category == "Elements")
            {
                int amt = (int)Math.Log10(item.Value) * 2;
                components.Add(ItemManager.Instance.GetItem("Etheral Seed", 0, ""), amt);
            }
            else if (item.Category == "Fishing")
            {
                int amt = (int)Math.Log10(item.Value) * 2;
                components.Add(ItemManager.Instance.GetItem("Alluvial Seed", 0, ""), amt);
            }
            else if (item.Category == "Gems")
            {
                int amt = (int)Math.Log10(item.Value);
                components.Add(ItemManager.Instance.GetItem("Perpetual Seed", 0, ""), amt);
            }
            else if (item.Category == "General")
            {
                int amt = (int)Math.Log10(item.Value);
                components.Add(ItemManager.Instance.GetItem("Alluvial Seed", 0, ""), amt);
            }
            else if (item.Category == "Hunting")
            {
                int amt = (int)Math.Log10(item.Value);
                components.Add(ItemManager.Instance.GetItem("Rejuvinating Seed", 0, ""), amt);

            }
            else if (item.Category == "Jerkies")
            {
                int amt = (int)Math.Log10(item.Value);
                components.Add(ItemManager.Instance.GetItem("Alimental Seed", 0, ""), amt);
            }
            else if (item.Category == "Logs")
            {
                int amt = (int)Math.Log10(item.Value);
                components.Add(ItemManager.Instance.GetItem("Warmth Seed", 0, ""), amt);
                components.Add(ItemManager.Instance.GetItem("Rejuvinating Seed", 0, ""), amt);

            }
            else if (item.Category == "Magic")
            {
                int amt = (int)Math.Log10(item.Value);
                components.Add(ItemManager.Instance.GetItem("Thought Seed", 0, ""), amt);
            }
            else if (item.Category == "Necklaces")
            {
                int amt = (int)Math.Log10(item.Value);
                components.Add(ItemManager.Instance.GetItem("Perpetual Seed", 0, ""), amt);
            }
            else if (item.Category == "Ores")
            {
                int amt = (int)Math.Log10(item.Value);
                components.Add(ItemManager.Instance.GetItem("Alimental Seed", 0, ""), amt);
            }
            else if (item.Category == "QuestItems")
            {
                MessageManager.AddMessage("You probably shouldn't do that...");
                return new Dictionary<GameItem, int>();

            }
            else if (item.Category == "Shields")
            {
                int amt = (int)Math.Log10(item.Value);
                components.Add(ItemManager.Instance.GetItem("Perpetual Seed", 0, ""), amt);
            }
            else if (item.Category == "Sushi")
            {
                int amt = (int)Math.Log10(item.Value);
                components.Add(ItemManager.Instance.GetItem("Rejuvinating Seed", 0, ""), amt);
            }
            else if (item.Category == "Textiles")
            {
                int amt = (int)Math.Log10(item.Value);
                components.Add(ItemManager.Instance.GetItem("Thought Seed", 0, ""), amt);
            }
            else if (item.Category == "Weapons")
            {
                int amt = (int)Math.Log10(item.Value);
                components.Add(ItemManager.Instance.GetItem("Fierce Seed", 0, ""), amt);
            }
            else if (item.Category == "WoodworkingItems")
            {
                int amt = (int)Math.Log10(item.Value);
                components.Add(ItemManager.Instance.GetItem("Alimental Seed", 0, ""), amt);
            }


            return components;
        }

        public void Tick(Player player)
        {
            Player.Instance.Air = 101;
        }
        public ISpell Copy()
        {
            return new Extract() {Name=Name, Description=Description, Duration=Duration, TimeRemaining=TimeRemaining,Target=Target, Message=Message, Power=Power };
        }
    }
}
