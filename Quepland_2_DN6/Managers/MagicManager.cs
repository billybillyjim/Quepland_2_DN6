
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
        Spells.AddRange(await Http.GetFromJsonAsync<ISpell[]>("data/Spells.json"));

    }
    public void LoadSpellUnlocks(string data)
    {

    }
    public List<string> GetMagicUnlocks()
    {
        List<string> data = new List<string>();

        return data;
    }
    public bool UnlockSpell(string spellName)
    {
        foreach(ISpell spell in Spells)
        {
            if(spell.Name == spellName)
            {
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
        else if (data.Name == "Rice To Rolls")
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
            Console.WriteLine("Warning:" + data.Name + " not in list of status effects in Battle Manager.");
            return null;
        }
        spell.LoadData(data);
        return spell;
    }
}

