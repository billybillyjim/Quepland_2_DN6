
using Quepland_2_DN6.Spells;
using System.Drawing;
using System.Net.Http.Json;
using System.Xml;

public class MagicManager
{
    private static readonly MagicManager instance = new MagicManager();
    private MagicManager() { }
    static MagicManager() { }
    public static MagicManager Instance
    {
        get
        {
            return instance;
        }
    }
    public List<ISpell> Spells = new List<ISpell>();

    public List<ISpell> CombatSpells = new List<ISpell>();

    public List<ISpell> ItemSpells = new List<ISpell>();

    public List<ISpell> InventorySpells = new List<ISpell>();

    public List<ISpell> GatherSpells = new List<ISpell>();

    public async Task LoadSpells(HttpClient Http)
    {
        List<SpellData> spellData = new List<SpellData>();
        spellData.AddRange(await Http.GetFromJsonAsync<SpellData[]>("data/Spells.json"));
        foreach(SpellData sd in spellData)
        {
            var spell = GetSpellFromData(sd);
            Spells.Add(spell);
            Console.WriteLine(sd);
            if(sd.Target == "Monster" || sd.Target == "Player")
            {
                if(sd.Name != "Breathe")
                {
                    CombatSpells.Add(spell);
                }
                
            }
        }
        
    }

    public ISpell? GetSpell(string name)
    {
        foreach (ISpell spell in Spells)
        {
            if (spell.Name == name)
            {
                return spell;
            }
        }
        return null;
    }

    public ISpell GetRandomSpell()
    {
        return Spells[GameState.Random.Next(Spells.Count)];

    }

    public bool NoSpellsUnlocked()
    {
        foreach(ISpell spell in Spells)
        {
            if (spell.Unlocked)
            {
                return false;
            }
        }
        return true;
    }
    public void LoadSpellUnlocks(List<string> data)
    {
        if(data != null)
        {
            Console.WriteLine(data);
            foreach (string spell in data)
            {
                var s = Spells.FirstOrDefault(x => x.Name == spell);
                if(s != null)
                {
                    s.Unlocked = true;
                }
                else
                {
                    Console.WriteLine("No spell with name: " + spell);
                }
            }
        }
        
    }
    public List<string> GetMagicUnlocks()
    {
        List<string> data = new List<string>();
        foreach (ISpell spell in Spells)
        {
            if (spell.Unlocked)
            {
                data.Add(spell.Name);
            }
        }
        return data;
    }
    public bool UnlockSpell(string spellName)
    {
        foreach(ISpell spell in Spells)
        {
            if(spell.Name == spellName)
            {
                Console.WriteLine("Unlocking " + spell.Name);
                spell.Unlocked = true;
                return true;
            }
        }
        return false;
    }
    public ISpell GetSpellFromData(SpellData data)
    {
        ISpell spell = null;    
        if (data.Name == "Axe Fletch")
        {
            spell = new AxeFletch();
           
        }
        else if (data.Name == "Blast")
        {
            spell = new Blast();
        }
        else if(data.Name == "Breathe")
        {
            spell = new Breathe();
        }
        else if (data.Name == "Dehydrate")
        {
            spell = new Dehydrate();
        }
        else if (data.Name == "Drain")
        {
            spell = new Drain();
        }
        else if (data.Name == "Enchant")
        {
            spell = new Enchant();
        }
        else if (data.Name == "Endure")
        {
            spell = new Endure();
        }
        else if (data.Name == "Entangle")
        {
            spell = new Entangle();
        }
        else if (data.Name == "Extract")
        {
            spell = new Extract();
        }
        else if (data.Name == "Hypnotize")
        {
            spell = new Hypnotize();
        }
        else if (data.Name == "Introspect")
        {
            spell = new Introspect();
        }
        else if (data.Name == "Lift")
        {
            spell = new Lift();
        }
        else if (data.Name == "Liquify")
        {
            spell = new Liquify();
        }
        else if (data.Name == "Lure")
        {
            spell = new Lure();
        }
        else if (data.Name == "Magic Hand")
        {
            spell = new MagicHand();
        }
        else if (data.Name == "Magic Needle")
        {
            spell = new MagicNeedle();
        }
        else if (data.Name == "Mind Trick")
        {
            spell = new MindTrick();
        }
        else if (data.Name == "Molten Swing")
        {
            spell = new MoltenSwing();
        }
        else if (data.Name == "Rain")
        {
            spell = new Rain();
        }
        else if (data.Name == "Rally")
        {
            spell = new Rally();
        }
        else if (data.Name == "Reflect")
        {
            spell = new Reflect();
        }
        else if (data.Name == "Consume")
        {
            spell = new Consume();
        }
        else if (data.Name == "Rice to Rolls")
        {
            spell = new RiceToRolls();
        }
        else if (data.Name == "Teleport Branch")
        {
            spell = new TeleportBranch();
        }
        else if (data.Name == "Temporal Leap")
        {
            spell = new TemporalLeap();
        }

        if (spell == null)
        {
            Console.WriteLine("Warning:" + data.Name + " not in list of status effects in Magic Manager.");
            return null;
        }
        spell.LoadData(data);
        return spell;
    }
}

