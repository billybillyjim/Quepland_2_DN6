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
		public int Cooldown { get; set; } 
		public int CooldownRemaining { get; set; }
        public string Data { get; set; } 
		public bool Unlocked { get; set; } = false;
        public List<Ingredient> Cost { get; set; }
        public Extract() { }
        

        public void Cast(Inventory inventory, GameItem item)
        {
            if (CooldownRemaining > 0)
            {
                MessageManager.AddMessage($"You aren't quite ready to cast that spell again. ({Math.Round(CooldownRemaining / 5f, 2)})");
                return;
            }
            if(item == null)
            {
                MessageManager.AddMessage($"WARNING:Item was null.");
                return;
            }
            if(item.IsSellable == false || item.Category == "QuestItems")
            {
                MessageManager.AddMessage($"You aren't sure why, but you feel like it's a bad idea to cast this spell on this item, so you decide not to.");
                return;
            }
            ISpell spell = this;
            if (!spell.CanPayCost())
            {
                MessageManager.AddMessage($"You don't have the seeds or MP to cast this spell.");
                GameState.RemoveActiveSpellByName(spell.Name);
                return;
            }
            if(item.Value >= GameState.ExtractWarningValue)
            {
                MessageManager.AddMessage($"This item is worth more than " + GameState.ExtractWarningValue + ". If you are sure you want to extract it into magic seeds, you can increase the limit in the settings.");
                GameState.RemoveActiveSpellByName(spell.Name);
                return;
            }
            if (!inventory.HasItem(item))
            {
                MessageManager.AddMessage($"You have run out of {item.GetName(2)}.");
                GameState.RemoveActiveSpellByName(spell.Name);
                return;
            }
            if (inventory.GetNumberOfUnlockedItem(item) == 0)
            {
                MessageManager.AddMessage($"You can't extract items that are locked.");
                GameState.RemoveActiveSpellByName(spell.Name);
                return;
            }
            var components = GetComponents(item);
            int req = components.Count;

            if (req == 0)
            {
                MessageManager.AddMessage($"{item.Name} does not extract into anything.");
                return;
            }
            foreach (KeyValuePair<GameItem, int> i in components)
            {
                if (inventory.HasItem(i.Key))
                {
                    req--;
                }
            }
            if (inventory.GetAvailableSpaces() < req)
            {
                MessageManager.AddMessage($"You don't have enough inventory space to cast this spell.");
                GameState.RemoveActiveSpellByName(spell.Name);
                return;
            }  
            if(inventory.RemoveItems(item, 1) != 1)
            {
                MessageManager.AddMessage($"You can't extract items that are locked.");
                GameState.RemoveActiveSpellByName(spell.Name);
                return;
            }
            foreach (KeyValuePair<GameItem, int> i in components)
            {
                inventory.AddMultipleOfItem(i.Key, i.Value);
            }

            spell.PayCost();

            Player.Instance.GainExperience("Magic", item.Value / 20);
            MessageManager.AddMessage("You get " + GetComponentsString(components) + ", destroying the " + item.GetName(1) + " in the process.");      
       
            CooldownRemaining = Cooldown;
        }

        private string GetComponentsString(Dictionary<GameItem, int> components)
        {
            string output = "";
            foreach(KeyValuePair<GameItem, int> pair in components)
            {
                output += pair.Value + " " + pair.Key.GetName(pair.Value);
            }
            return output;
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
                components.Add(ItemManager.Instance.GetItem("Rejuvinal Seed", 0, ""), amt);
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
                components.Add(ItemManager.Instance.GetItem("Rejuvinal Seed", 0, ""), amt);

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
                components.Add(ItemManager.Instance.GetItem("Rejuvinal Seed", 0, ""), amt);

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
                components.Add(ItemManager.Instance.GetItem("Rejuvinal Seed", 0, ""), amt);
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
