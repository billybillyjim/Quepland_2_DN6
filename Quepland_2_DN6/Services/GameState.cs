using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Pluralize.NET;
using Quepland_2_DN6.Components;
using Quepland_2_DN6.Pages;
using Quepland_2_DN6.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
//using Pluralize.NET;


    public class GameState
    {
    public enum GameType
    {
        Normal,
        Hardcore,
        Ultimate
    }
    public static GameType CurrentGameMode;
    public event EventHandler StateChanged;
    public IJSRuntime JSRuntime { get; set; }

    public static string Version { get; set; } = "1.2.0";
    public static List<Update> Updates { get; set; } = new List<Update>();
    public static Pluralizer Pluralizer = new Pluralizer();

    public static string Location { get; set; } = "";
    public static string BGColor { get; set; } = "#2d2d2d";
    public static bool InitCompleted { get; set; } = false;
    public static bool ShowStartMenu { get; set; } = true;
    public static bool ShowSettings { get; set; }
    public static bool ShowExpTrackerSettings { get; set; }
    private bool stopActions = false;
    private bool stopNoncombatActions = false;
    public bool IsStoppingNextTick { get; set; }
    private static bool cancelTask;
    public static bool IsOnHuntingTrip { get; set; }
    public static bool UseNewSaveCompression { get; set; }
    public bool ShowMap { get; set; }
    public static AFKAction CurrentAFKAction { get; set; }
    private Timer GameTimer { get; set; }
    public int testInt = 0;
    private static Guid _guid;
    public static Guid Guid
    {
        get
        {
            if (_guid == Guid.Empty)
            {
                _guid = Guid.NewGuid();
                return _guid;
            }
            else
            {
                return _guid;
            }
        }
        set
        {
            _guid = value;
        }
    }

    public GameItem NewGatherItem;
    public GameItem NewRequiredItem;
    public Recipe NewCraftingRecipe;
    public Recipe NewSmeltingRecipe;
    public Recipe NewSmithingRecipe;
    public Book NewBook;

    public GameItem? CurrentGatherItem;
    public GameItem RequiredForGatherItem;
    public List<GameItem> PossibleGatherItems = new List<GameItem>();
    public Recipe CurrentSmeltingRecipe;
    public Recipe CurrentSmithingRecipe;
    public Book CurrentBook;

    public AlchemicalFormula CurrentAlchemyFormula;
    public static List<ProgressFlag> ProgressFlags { get; set; } = new List<ProgressFlag>();
    
    public static GameItem? CurrentFood { get; set; }
    public static bool CancelEating;
    public Recipe? CurrentRecipe;
    public static Land CurrentLand;
    public ItemViewerComponent itemViewer { get; set; }
    public InventoryViewerComponent inventoryViewer { get; set; }
    public SmithyComponent SmithingComponent { get; set; }
    public OvenComponent OvenComponent { get; set; }
    public CraftingComponent CraftingComponent { get; set; }
    public AlchemicalHallComponent AlchemicalHallComponent { get; set; }
    public ContextMenu CurrentContextMenu { get; set; }
    public RightSidebarComponent RightSidebarComponent { get; set; }
    public static NavigationManager UriHelper;
    public static ArtisanTask CurrentArtisanTask;

    public static int TicksToNextAction;
    public static readonly int GameSpeed = 200;
    public static bool CompactInventoryView;
    public static bool HideLockedItems;
    public static bool ShowBackgrounds = true;
    public static int ExtractWarningValue = 1000;

    public static int TicksToNextHeal;
    public static int HealingTicks;
    public static int CurrentTick { get; set; }
    public static int AutoSaveInterval { get; set; } = 4500;
    public static int GameWindowWidth { get; set; }
    public static int GameWindowHeight { get; set; }
    public static double MainWindowWidth { get; set; }
    public static double RightSidebarWidth { get; set; }
    public static int GameLoadProgress { get; set; }
    public static bool ShowTome { get; set; }
    public static bool UseOldBank { get; set; }
    public static string CurrentTome { get; set; } = "None";
    public int MinWindowWidth { get; set; } = 600;
    public double BankWindowWidth { get; set; } = 550;
    public double BankWindowHeight { get; set; } = 540;
    public int AlchemyStage;
    public int AutoSmithedItemCount { get; set; } = 0;

    private QuestTester QuestTester = new QuestTester();
    private RecipeTester RecipeTester = new RecipeTester();
    public static Random Random = new Random();

    public static bool SaveGame = false;
    public static bool DieNextTick = false;
    public static string DieNextTickMessage = "";
    public static bool IsSaving = false;
    private Stopwatch stopwatch = new Stopwatch();
    public static HCDeathInfo HCDeathInfo;

    public static List<ISpell> ActiveSpells = new List<ISpell>();
    public static List<string> CancelActiveSpells = new List<string>();
    public static List<ISpell> AutoCastSpells = new List<ISpell>();
    public static List<ISpell> CancelAutoCastSpells = new List<ISpell>();
    public static GameItem LastAutoCastItemTarget { get; set; }
    public void Start()
    {
        //Console.WriteLine(CheckVersion("1.1.1b", "1.1.1b") + ":1.1.1b");
        //Console.WriteLine(CheckVersion("1.1.1b", "1.1.1") + ":1.1.1");
        if (GameTimer != null)
        {
            return;
        }
        //QuestTester.TestQuests();
        //RecipeTester.TestRecipes();
        GameTimer = new Timer(new TimerCallback(async _ =>
        {
            await Tick();
            StateHasChanged();
        }), null, GameSpeed, GameSpeed);
        StateHasChanged();
        //QuestTester.TestQuests();
        //RecipeTester.TestRecipes();
    }

    private async Task Tick()
    {
        if (IsOnHuntingTrip || CurrentAFKAction != null)
        {
            if (SaveGame)
            {
                IsSaving = true;
                await SaveManager.SaveGame();
                SaveGame = false;
            }
            else if (CurrentTick % AutoSaveInterval == 0)
            {
                await SaveManager.SaveGame();
            }
            CurrentTick++;
            StateHasChanged();
            return;
        }
        if (DieNextTick)
        {
            Player.Instance.Die(DieNextTickMessage);
            DieNextTick = false;
            DieNextTickMessage = "Unknown";
        }

        TickActiveSpells();

        if (stopActions)
        {
            ClearActions();
        }
        else if (stopNoncombatActions)
        {
            ClearNonCombatActions(); 
        }
        if (TicksToNextAction <= 0 && CurrentGatherItem != null)
        {
            GatherItem();
        }
        else if (TicksToNextAction <= 0 && CurrentRecipe != null)
        {
            CraftItem();
        }
        else if (TicksToNextAction <= 0 && CurrentSmithingRecipe != null && CurrentSmeltingRecipe != null)
        {
            SmithItem();
        }
        else if (TicksToNextAction <= 0 && CurrentAlchemyFormula != null)
        {
            AlchItem();
        }
        else if (BattleManager.Instance.CurrentOpponents != null && BattleManager.Instance.CurrentOpponents.Count > 0)
        {
            if (BattleManager.Instance.WaitedAutoBattleGameTick)
            {
                if (BattleManager.Instance.SelectedOpponent != null)
                {
                    BattleManager.Instance.StartBattle(BattleManager.Instance.SelectedOpponent);
                }
                else
                {
                    BattleManager.Instance.StartBattle(BattleManager.Instance.CurrentArea);
                }
            }
            else
            {
                BattleManager.Instance.DoBattle();
            }

        }
        else if(TicksToNextAction <= 0 && CurrentBook != null)
        {
            ReadBook();
        }
        if (Player.Instance.CurrentFollower != null && Player.Instance.CurrentFollower.IsBanking && (CurrentGameMode != GameType.Ultimate))
        {
            Player.Instance.CurrentFollower.TicksToNextAction--;
            if (Player.Instance.CurrentFollower.TicksToNextAction <= 0)
            {
                Player.Instance.CurrentFollower.BankItems();
            }
        }
        if (!Player.Instance.JustDied)
        {
            Player.Instance.TickStatusEffects();
            if(CurrentTick % Player.Instance.GetMagicRestoreRate() == 0)
            {
                Player.Instance.CurrentMP++;
                if(Player.Instance.CurrentMP > Player.Instance.MaxMP)
                {
                    Player.Instance.CurrentMP = Player.Instance.MaxMP;
                }
            }
        }
        
        if (CurrentFood != null && CurrentTick % CurrentFood.FoodInfo.HealSpeed == 0)
        {
            if (CancelEating)
            {
                CurrentFood = null;
                CancelEating = false;
                Player.Instance.ClearBoosts();
            }
            else
            {
                HealPlayer();
            }
        }

            if (CurrentTick % 5 == 0)
            {
                foreach (Skill s in Player.Instance.Skills)
                {
                    if (!s.EXPTracker.IsPaused)
                    {
                        s.EXPTracker.TimeSinceTrackerStarted += TimeSpan.FromMilliseconds(GameSpeed * 5);
                    }                
                }
            }
        
        if (Player.Instance.JustDied)
        {
            CurrentFood = null;
            HealingTicks = 0;
            Player.Instance.ClearBoosts();
            Player.Instance.JustDied = false;
        }
        if (cancelTask)
        {
            CurrentArtisanTask = null;
            cancelTask = false;
        }
        
        await GetDimensions();
        TicksToNextAction--;
        CurrentTick++;
        TooltipManager.currentDelay++;
        if (SaveGame)
        {
            IsSaving = true;
            await SaveManager.SaveGame();
            SaveGame = false;
        }
        else if (CurrentTick % AutoSaveInterval == 0)
        {
            await SaveManager.SaveGame();
        }
    }
    /// <summary>
    /// Pauses actions at the beginning of the next game tick.
    /// </summary>
    public void StopActions()
    {
        stopActions = true;
        IsStoppingNextTick = true;
    }
    public void StopNonCombatActions()
    {
        stopNoncombatActions = true;
        IsStoppingNextTick = true;
    }
    private void ClearActions()
    {
        CurrentGatherItem = null;
        RequiredForGatherItem = null;
        CurrentRecipe = null;
        BattleManager.Instance.CurrentOpponents.Clear();
        CurrentSmithingRecipe = null;
        CurrentAlchemyFormula = null;
        AlchemyStage = 0;
        SmithingManager.SmithingStage = 0;
        CurrentSmeltingRecipe = null;
        CurrentBook = null;
        stopActions = false;
        IsStoppingNextTick = false;
        if(NewGatherItem != null)
        {
            CurrentGatherItem = NewGatherItem;
            NewGatherItem = null;
        }
        if(NewRequiredItem != null)
        {
            RequiredForGatherItem = NewRequiredItem;
            NewRequiredItem = null;
        }
        if(NewSmithingRecipe != null)
        {
            CurrentSmithingRecipe = NewSmithingRecipe;
            NewSmithingRecipe = null; 

        }
        if (NewSmeltingRecipe != null)
        {
            CurrentSmeltingRecipe = NewSmeltingRecipe;
            NewSmeltingRecipe = null;
        }
        if(NewCraftingRecipe != null)
        {
            CurrentRecipe = NewCraftingRecipe;
            NewCraftingRecipe = null;
        }
        if (NewBook != null)
        {
            CurrentBook = NewBook;
            NewBook = null;
        }    
    }
    public static void CancelTask()
    {
        cancelTask = true;
    }
    private void ClearNonCombatActions()
    {
        CurrentGatherItem = null;
        CurrentRecipe = null;
        CurrentSmithingRecipe = null;
        CurrentSmeltingRecipe = null;
        stopNoncombatActions = false;
        IsStoppingNextTick = false;
    }

    public GameItem? MagicalGather()
    {
        if(CurrentGatherItem != null)
        {
            foreach(ISpell spell in ActiveSpells)
            {
                if (spell.Target == "Gather")
                {
                    //Updates Data for spell
                    spell.Cast(Player.Instance.Inventory, CurrentGatherItem);
                    return ItemManager.Instance.GetItemByUniqueID(spell.Data);
                }
            }
        }
        return null;
    }

    public void TickActiveSpells()
    {
        ActiveSpells.RemoveAll(x => CancelActiveSpells.Contains(x.Name));
        CancelActiveSpells = new List<string>();

        var spellsToRemove = new List<ISpell>();

        foreach(ISpell spell in ActiveSpells)
        {
            if(spell.Target == "Player")
            {
                spell.Tick(Player.Instance);
            }
            else if(spell.Target == "Monster")
            {

            }
            else if(spell.Target == "Inventory")
            {
                
            }
            else
            {
                spell.Tick();
            }

            
            spell.TimeRemaining--;
            if(spell.TimeRemaining <= 0)
            {
                spellsToRemove.Add(spell);
                MessageManager.AddMessage(spell.Name + " has run out.");
            }
            else if(spell.TimeRemaining == 60)
            {
                MessageManager.AddMessage(spell.Name + " will run out soon.");
            }
        }
        ActiveSpells.RemoveAll(x => spellsToRemove.Contains(x));
        foreach(ISpell spell in CancelAutoCastSpells)
        {
            MessageManager.AddMessage($"You stop autocasting {spell.Name}");
        }
        AutoCastSpells.RemoveAll(x => CancelAutoCastSpells.Contains(x));
        CancelAutoCastSpells = new List<ISpell>();

        foreach(ISpell spell in MagicManager.Instance.Spells)
        {
            spell.CooldownRemaining--;
        }

        foreach(ISpell spell in AutoCastSpells)
        {
            if (spell.CooldownRemaining <= 0 && spell.CanPayCost())
            {
                if (spell.Target == "Inventory")
                {
                    spell.Cast(Player.Instance.Inventory);
                }
                else if (spell.Target == "Item")
                {
                    spell.Cast(Player.Instance.Inventory, LastAutoCastItemTarget);                    
                }
                else if (spell.Target == "Player")
                {
                    spell.Cast(Player.Instance);
                }
                else if (spell.Target == "Monster")
                {
                    if(BattleManager.Instance.BattleHasEnded == false && BattleManager.Instance.Target != null)
                    {
                        spell.Cast(BattleManager.Instance.Target);
                    }                     
                }
                else if (spell.Target == "Gather" || spell.Target == "None")
                {
                    spell.Cast();
                }
            }
        }
    }

    public static bool PlayerIsUnderSpell(string spellName)
    {
        foreach(ISpell spell in ActiveSpells)
        {
            if(spell.Target == "Player")
            {
                if(spell.Name == spellName)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Pause()
    {
        GameTimer.Change(Timeout.Infinite, Timeout.Infinite);
    }

    private void GatherItem()
    {
        if (HasRequiredItemForGather())
        {
            GameItem? magicGatherSwap = MagicalGather();
            if (Player.Instance.FollowerGatherItem(CurrentGatherItem, magicGatherSwap) == false)
            {
                PlayerGatherItem(magicGatherSwap);
            }
            if (CurrentGatherItem != null)
            {
                if (Player.Instance.CurrentFollower != null && Player.Instance.CurrentFollower.IsBanking && Player.Instance.CurrentFollower.InventorySize > 0 && Player.Instance.Inventory.GetAvailableSpaces() == 0)
                {

                    TicksToNextAction = Player.Instance.CurrentFollower.TicksToNextAction;

                }
                else
                {
                    if (PossibleGatherItems.Count > 1)
                    {
                        int roll = Random.Next(0, PossibleGatherItems.Count);
                        CurrentGatherItem = PossibleGatherItems[roll];
                    }
                    TicksToNextAction = GetTicksToNextGather();
                }
            }
        }
    }
    private bool PlayerGatherItem(GameItem magicGatherSwap)
    {

        if (Player.Instance.PlayerGatherItem(CurrentGatherItem, magicGatherSwap) == false)
        {
            if(Player.Instance.CurrentFollower != null && Player.Instance.CurrentFollower.InventorySize > 0)
            {
                if (Player.Instance.CurrentFollower.IsBanking == false)
                {
                    CurrentGatherItem = null;
                    return false;
                }
            }

        }
        if(Player.Instance.Inventory.GetAvailableSpaces() == 0 && RequiredForGatherItem == null && 
            !(CurrentGatherItem != null && CurrentGatherItem.IsStackable && Player.Instance.Inventory.HasItem(CurrentGatherItem)))
        {
            if(Player.Instance.CurrentFollower != null && Player.Instance.CurrentFollower.IsBanking == false)
            {
                CurrentGatherItem = null;
            }
            else if(Player.Instance.CurrentFollower != null)
            {
                TicksToNextAction = Player.Instance.CurrentFollower.TicksToNextAction;
            }
            else
            {
                CurrentGatherItem = null;
            }
        }
        return true;
        
    }
    private bool HasRequiredItemForGather()
    {
        if (RequiredForGatherItem != null)
        {
            bool hasReq = false;
            if (Player.Instance.CurrentFollower != null)
            {
                hasReq = Player.Instance.CurrentFollower.Inventory.HasItem(RequiredForGatherItem);
            }
            if (hasReq == false)
            {
                hasReq = Player.Instance.Inventory.HasItem(RequiredForGatherItem);
            }
            if (hasReq == false)
            {
                CurrentGatherItem = null;
                MessageManager.AddMessage("You have run out of " + RequiredForGatherItem);
                RequiredForGatherItem = null;
                return false;
            }
            else
            {
                Player.Instance.Inventory.RemoveItems(RequiredForGatherItem, 1);
            }
        }
        return true;
    }
    private void ReadBook()
    {

        float multiplier = 500;
        string skill = CurrentBook.Skill.Name;
        int lvl = Math.Max(1, Player.Instance.GetLevel(skill));
        if (lvl > 50)
        {
            multiplier *= 0.9f;
            if (lvl > 80)
            {
                multiplier *= 0.8f;
                if (lvl > 100)
                {
                    multiplier *= 0.75f;
                }
            }
        }
        try
        {
            Player.Instance.GainExperience(skill, (long)((lvl / 90d) * multiplier));
            MessageManager.AddMessage("You read another page of the book. You feel more knowledgable about " + skill + ".");
            CurrentBook.Progress++;

            TicksToNextAction = (int)Math.Max(100, ((2 + CurrentBook.Difficulty * 5d) / lvl) * 100);
            if (Random.Next(0, CurrentBook.Length) == CurrentBook.Progress)
            {
                MessageManager.AddMessage("A small key falls out of the book as you turn the page.");
                if (BGColor == "#2e1b1b")
                {
                    Player.Instance.Inventory.AddItem("Small Red Library Key");
                }
                else if (BGColor == "#463513")
                {
                    Player.Instance.Inventory.AddItem("Small Orange Library Key");
                }
                else
                {
                    Player.Instance.Inventory.AddItem("Small Green Library Key");
                }

            }
            if (CurrentBook.Progress >= CurrentBook.Length)
            {
                MessageManager.AddMessage("You finish reading the book and return it to the shelf.");
                CurrentBook = null;

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
        
    }
    private void CraftItem()
    {
        if (CurrentRecipe == null)
        {
            return;
        }
        else if(BattleManager.Instance.BattleHasEnded == false)
        {
            MessageManager.AddMessage("You can't make that while fighting!");
            CurrentRecipe = null;
            return;
        }
        if (CurrentRecipe.Create(out int created))
        {
            TicksToNextAction = CurrentRecipe.CraftingSpeed;
            MessageManager.AddMessage(CurrentRecipe.GetOutputsString().Replace("$", (created * CurrentRecipe.OutputAmount).ToString()));
            if(CurrentRecipe.CanCreate() == false)
            {
                
                if (CurrentRecipe.HasSpace())
                {
                    CurrentRecipe.Output.Rerender = true;
                    MessageManager.AddMessage("You have run out of materials.");
                    if(OvenComponent != null)
                    {
                        OvenComponent.Source = "";
                    }
                }
                else
                {
                    MessageManager.AddMessage("You don't have enough inventory space to do it again.");
                }
                if (CurrentRecipe != null)
                {
                    foreach (Ingredient i in CurrentRecipe.Ingredients)
                    {
                        i.Item.Rerender = true;
                    }
                }
                CurrentRecipe = null;
                itemViewer.ClearItem();
            }
            if(CurrentRecipe != null)
            {
                foreach (Ingredient i in CurrentRecipe.Ingredients)
                {
                    i.Item.Rerender = true;
                }
            }

        }
        else
        {
            if(CurrentRecipe.Ingredients.Count == 1 && CurrentRecipe.Ingredients[0].Item.Name == itemViewer.Item.Name && CurrentRecipe.HasSpace())
            {
                itemViewer.ClearItem();                
                MessageManager.AddMessage("You have run out of materials.");
                CurrentRecipe.Output.Rerender = true;
                if (OvenComponent != null)
                {
                    OvenComponent.Source = "";
                }
            }
            foreach(Ingredient i in CurrentRecipe.Ingredients)
            {
                i.Item.Rerender = true;
            }
            CurrentRecipe = null;
        }
        if(CraftingComponent != null)
        {
            CraftingComponent.ReloadRecipes();
        }
        

    }
    public static void Eat(GameItem item, int customAmount, int customDuration)
    {
        if (item.FoodInfo != null)
        {

            if (Player.Instance.Inventory.RemoveItems(item, customAmount) == customAmount)
            {
                Player.Instance.ClearBoosts();
                CurrentFood = item;
                HealingTicks = customDuration;
                if (CurrentFood.FoodInfo.BuffedSkill != null)
                {
                    var s = Player.Instance.GetSkill(item.FoodInfo.BuffedSkill);
                    if (s == null)
                    {
                        Console.WriteLine("Failed to find food buff for " + item.Name);
                        return;
                    }
                    Player.Instance.ClearBoosts();
                    s.Boost = CurrentFood.FoodInfo.BuffAmount;
                    MessageManager.AddMessage($"You eat a {customAmount} {CurrentFood.GetName(customAmount)} It somehow makes you feel better at {CurrentFood.FoodInfo.BuffedSkill }.");

                }
                else
                {
                    MessageManager.AddMessage($"You eat {customAmount} {CurrentFood.GetName(customAmount)}.");

                }
            }
            else
            {
                MessageManager.AddMessage("Please unlock " + CurrentFood + " before eating it.");
            }

        }
        else
        {
            Console.WriteLine("Item has no food info:" + item.Name);
        }
    }
    public static void Eat(GameItem item)
    {
        if(item.FoodInfo != null)
        {
            Eat(item, 1, item.FoodInfo.HealDuration);
        }
    }
    private void HealPlayer()
    {
        if(CurrentFood == null)
        {
            return;
        }
        int healAmount = Math.Min(Player.Instance.MaxHP - Player.Instance.CurrentHP, CurrentFood.FoodInfo.HealAmount);
        Player.Instance.CurrentHP += healAmount;
        Player.Instance.GainExperience("HP", healAmount * 3);

        int magicAmount = Math.Min(Player.Instance.MaxMP - Player.Instance.CurrentMP, CurrentFood.FoodInfo.MagicAmount);
        Player.Instance.CurrentMP += magicAmount;

        if (Player.Instance.CurrentHP <= 0)
        {
            Player.Instance.Die("Food Poisoning");
        }
        HealingTicks--;
        if (HealingTicks <= 0)
        {
            if (CurrentFood.FoodInfo.BuffedSkill != null)
            {
                Player.Instance.ClearBoosts();
            }
            CurrentFood = null;
        }
    }

    private void SmithItem()
    {
        if(CurrentSmeltingRecipe == null || CurrentSmithingRecipe == null)
        {
            Console.WriteLine("Failed to start smithing, Smelting Item is null:" + (CurrentSmeltingRecipe == null) + " and Smithing Item is null:" + (CurrentSmithingRecipe == null));
            return;
        }
        if(SmithingManager.SmithingStage == 0)
        {
            if (Player.Instance.CurrentFollower != null && Player.Instance.CurrentFollower.AutoCollectSkill == "Smithing")
            {
                SmithingManager.AutoSmithedItemCount = 0;
                SmithingManager.GetAutoSmeltingMaterials(CurrentSmeltingRecipe);
            }
            else if (SmithingManager.DoSmelting(CurrentSmeltingRecipe))
            {
                TicksToNextAction = CurrentSmeltingRecipe.CraftingSpeed;
            }
            else
            {
                if (SmithingComponent != null)
                {
                    SmithingComponent.UpdateSmeltables();
                }
                MessageManager.AddMessage("You have run out of ores.");
                StopActions();
                SmithingManager.SmithingStage = 0;
            }
        }
        else if(SmithingManager.SmithingStage == 1)
        {
            if (Player.Instance.CurrentFollower != null && Player.Instance.CurrentFollower.AutoCollectSkill == "Smithing")
            {
                SmithingManager.DoAutoSmelting(CurrentSmeltingRecipe, CurrentSmithingRecipe);
            }
            else if (SmithingManager.DoSmithing(CurrentSmeltingRecipe, CurrentSmithingRecipe))
            {
                TicksToNextAction = CurrentSmithingRecipe.CraftingSpeed;
            }
            else
            {
                if (SmithingComponent != null)
                {
                    SmithingComponent.UpdateSmeltables();
                }
                StopActions();
            }
        }
        else if(SmithingManager.SmithingStage == 2)
        {
            if (Player.Instance.CurrentFollower != null && Player.Instance.CurrentFollower.AutoCollectSkill == "Smithing")
            {
                SmithingManager.DoAutoSmithing(CurrentSmithingRecipe);
            }
            else if (Player.Instance.Inventory.AddMultipleOfItem(CurrentSmithingRecipe.Output, CurrentSmithingRecipe.OutputAmount))
            {
                MessageManager.AddMessage("You withdraw " + CurrentSmithingRecipe.OutputAmount + " " + CurrentSmithingRecipe.Output.Name);
                Player.Instance.GainExperience(CurrentSmithingRecipe.ExperienceGained);
                if(CurrentArtisanTask != null)
                {
                    if(CurrentArtisanTask.ItemName == CurrentSmithingRecipe.OutputItemName)
                    {
                        if (long.TryParse(CurrentSmithingRecipe.ExperienceGained.Split(':')[1], out long xp))
                        {
                            Player.Instance.GainExperience("Artisan", xp / 5);
                        }
                        else
                        {
                            Player.Instance.GainExperience("Artisan", 15);
                        }
                    }
                }
                TicksToNextAction = 12;
                SmithingManager.SmithingStage = 0;
            }
        }
        else if(SmithingManager.SmithingStage == 3)
        {
            if (Player.Instance.CurrentFollower != null && Player.Instance.CurrentFollower.AutoCollectSkill == "Smithing")
            {
                SmithingManager.DoAutoWithdrawal(CurrentSmithingRecipe);
            }
        }
        else if(SmithingManager.SmithingStage == 4)
        {
            if (Player.Instance.CurrentFollower != null && Player.Instance.CurrentFollower.AutoCollectSkill == "Smithing")
            {
                SmithingManager.DepositOutputs();
            }
        }

    }

    public void SellItem(GameItem item)
    {
        if (item.IsSellable == false)
        {
            MessageManager.AddMessage("You can't seem to sell this...");
            return;
        }
        if (item.IsEquipped)
        {
            MessageManager.AddMessage("You need to unequip this item before selling it.");
            return;
        }
        if (ItemManager.Instance.CurrentShop != null)
        {
            int sellAmt = Math.Min(ItemManager.Instance.SellAmount, Player.Instance.Inventory.GetNumberOfUnlockedItem(item));
            if (Player.Instance.Inventory.GetAvailableSpaces() > 0 || Player.Instance.Inventory.GetNumberOfItem(ItemManager.Instance.CurrentShop.Currency) > 0 || (item.IsStackable == false))
            {
                int amtRemoved = Player.Instance.Inventory.RemoveUnlockedItems(item, sellAmt);
                Player.Instance.Inventory.AddMultipleOfItem(ItemManager.Instance.CurrentShop.Currency, (amtRemoved * item.Value / 2));
                MessageManager.AddMessage("You sold " + amtRemoved + " " + item.Name + " for " + (amtRemoved * item.Value / 2) + " " + ItemManager.Instance.CurrentShop.Currency.Name + ".");
            }
            else
            {
                if(Player.Instance.Inventory.GetNumberOfUnlockedItem(item) == 0)
                {
                    MessageManager.AddMessage("You dont have any unlocked " + item.Name + ".");

                }
                else
                {
                    MessageManager.AddMessage("You sold dont have the inventory space to sell that.");

                }
            }
        }
        else if(Bank.Instance.IsBanking && Player.Instance.CurrentFollower != null && Player.Instance.CurrentFollower.AutoCollectSkill == "Banking")
        {
            Console.WriteLine("Code was run!");
            int sellAmt = Math.Min(Bank.Instance.Amount, Player.Instance.Inventory.GetNumberOfUnlockedItem(item));
            if (Player.Instance.Inventory.GetAvailableSpaces() > 0 || Player.Instance.Inventory.GetNumberOfItem(ItemManager.Instance.GetItemByName("Coins")) > 0 || (item.IsStackable == false))
            {
                Player.Instance.Inventory.RemoveItems(item, sellAmt);
                Player.Instance.Inventory.AddMultipleOfItem("Coins", (sellAmt * item.Value / 2));
                MessageManager.AddMessage("You sold " + sellAmt + " " + item.Name + " for " + (sellAmt * item.Value / 2) + " Coins.");
            }
            else
            {
                if (Player.Instance.Inventory.GetNumberOfUnlockedItem(item) == 0)
                {
                    MessageManager.AddMessage("You dont have any unlocked " + item.Name + ".");

                }
                else
                {
                    MessageManager.AddMessage("You sold dont have the inventory space to sell that.");

                }
            }
        }
        else
        {
            Console.WriteLine("Shop was null");
        }
    }
    public void SellItemFromBank(GameItem item)
    {
        if (item.IsSellable == false)
        {
            MessageManager.AddMessage("You can't seem to sell this...");
            return;
        }
        if (item.IsEquipped)
        {
            MessageManager.AddMessage("You need to unequip this item before selling it.");
            return;
        }
        if (Bank.Instance.IsBanking && Player.Instance.CurrentFollower != null && Player.Instance.CurrentFollower.AutoCollectSkill == "Banking")
        {
            int sellAmt = Math.Min(Bank.Instance.Amount, Bank.Instance.Inventory.GetNumberOfItem(item));

                Bank.Instance.Inventory.RemoveItems(item, sellAmt);
                Bank.Instance.Inventory.AddMultipleOfItem("Coins", (sellAmt * item.Value / 2));
                MessageManager.AddMessage(Player.Instance.CurrentFollower.Name + " sold " + sellAmt + " " + item.Name + " for " + (sellAmt * item.Value / 2) + " Coins and deposited them into your bank.");
        }
        else
        {
            Console.WriteLine("Shop was null");
        }
        Bank.Instance.HasChanged = true;
    }
    private void AlchItem()
    {
        if(AlchemyStage == 0)
        {
            if (Player.Instance.Inventory.HasItem(CurrentAlchemyFormula.InputMetal))
            {
                if(Player.Instance.Inventory.RemoveItems(CurrentAlchemyFormula.InputMetal, 1) == 1)
                {
                    AlchemyStage = 1;
                    TicksToNextAction = 10;
                    MessageManager.AddMessage("You place the " + CurrentAlchemyFormula.InputMetal.Name + " into the pool.");
                }
                else
                {
                    MessageManager.AddMessage("You need to unlock the ingredients to use them.");
                }
            }
            else
            {
                if(AlchemicalHallComponent != null)
                {
                    AlchemicalHallComponent.UpdateLists();
                }
                MessageManager.AddMessage("You don't have any " + CurrentAlchemyFormula.InputMetal.Name + "s.");
                CurrentAlchemyFormula = null;
                AlchemyStage = 0;
            }
        }
        else if(AlchemyStage == 1)
        {
            if (Player.Instance.Inventory.HasItem(CurrentAlchemyFormula.Element))
            {
                if(Player.Instance.Inventory.RemoveItems(CurrentAlchemyFormula.Element, 1) == 1)
                {
                    AlchemyStage = 2;
                    TicksToNextAction = 10;
                    MessageManager.AddMessage("You apply the " + CurrentAlchemyFormula.Element.Name + " to the metal mixture in the pool.");
                }
                else
                {
                    MessageManager.AddMessage("You need to unlock the ingredients to use them.");
                }
            }
            else
            {
                if (AlchemicalHallComponent != null)
                {
                    AlchemicalHallComponent.UpdateLists();
                }
                MessageManager.AddMessage("You don't have any " + CurrentAlchemyFormula.Element.Name + ".");
                CurrentAlchemyFormula = null;
                AlchemyStage = 0;
            }
        }
        else if(AlchemyStage == 2)
        {
            MessageManager.AddMessage("You " + CurrentAlchemyFormula.ActionString + " the combined element and metal.");
            AlchemyStage = 3;
            TicksToNextAction = 10;
            if (AlchemicalHallComponent != null)
            {
                AlchemicalHallComponent.UpdateLists();
            }
        }
        else if(AlchemyStage == 3)
        {
            GameItem reward = ItemManager.Instance.GetItemFromFormula(CurrentAlchemyFormula);
            if(reward == null)
            {
                reward = ItemManager.Instance.GetItemByName("Alchemical Dust");
            }
            MessageManager.AddMessage("You withdraw the " + reward.Name + " from the release valve.");
            Player.Instance.GainExperience("Alchemy", reward.Value);
            Player.Instance.Inventory.AddItem(reward);
            Player.Instance.KnownAlchemicalFormulae.Add(CurrentAlchemyFormula);
            if(reward.Name == "Alchemical Dust")
            {
                StopActions();
            }
            AlchemyStage = 0;
            TicksToNextAction = 10;
            if (AlchemicalHallComponent != null)
            {
                AlchemicalHallComponent.UpdateLists();
            }
        }
    }
    public void CompleteArtisanTask()
    {
        try
        {
            Recipe r = ItemManager.Instance.GetArtisanRecipeByOutput(CurrentArtisanTask.ItemName);
            string skill = r.ExperienceGained.Split(',')[0].Split(':')[0];
            long xp = long.Parse(r.ExperienceGained.Split(',')[0].Split(':')[1]);
            Player.Instance.GainExperience(skill, xp * CurrentArtisanTask.AmountRequired / 10);
            Player.Instance.ArtisanPoints += CurrentArtisanTask.PointsToEarn;
            MessageManager.AddMessage("You completed your artisan task! You've earned " + CurrentArtisanTask.PointsToEarn + " artisan points with the guild and now have a total of " + Player.Instance.ArtisanPoints + ". Speak to a guild member to get another task.");          
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }
        CurrentArtisanTask = null;
    }
    public void SetCraftingItem(Recipe recipe)
    {
        StopActions();
        NewCraftingRecipe = recipe;
        TicksToNextAction = recipe.CraftingSpeed;
        MessageManager.AddMessage(recipe.RecipeActionString);
        UpdateState();
    }
    public static ProgressFlag GetFlagByName(string name)
    {
        ProgressFlag? flag = ProgressFlags.FirstOrDefault(x => x.Name == name);
        if(flag == null)
        {
            Console.WriteLine("Failed to find flag with name:" + name);
        }
        return flag;
    }

    public static bool FlagIsMet(string name)
    {
        ProgressFlag f = GetFlagByName(name);
        return f.Completed;
    }
    public int GetTicksToNextGather()
    {
        if(CurrentGatherItem != null)
        {
            
            int baseValue = CurrentGatherItem.GatherSpeed.ToGaussianRandom();
           
            baseValue = (int)Math.Max(1, baseValue * Player.Instance.GetGearMultiplier(CurrentGatherItem));      
            baseValue = (int)Math.Max(1, baseValue * Player.Instance.GetLevelMultiplier(CurrentGatherItem));

            return baseValue;
        }
        return int.MaxValue;
       
    }
    public int GetTicksToNextGather(GameItem item, int gatherSpeed)
    {
        if (item != null)
        {
            int baseValue = gatherSpeed.ToGaussianRandom();
            // Console.WriteLine("Ticks to next gather:" + baseValue);
            baseValue = (int)Math.Max(1, (double)baseValue * Player.Instance.GetGearMultiplier(item));
            //Console.WriteLine("Ticks to next gather with gear:" + baseValue);
            baseValue = (int)Math.Max(1, (double)baseValue * Player.Instance.GetLevelMultiplier(item));
            //Console.WriteLine("Ticks to next gather with gear and level:" + baseValue);
            return baseValue;
        }
        return int.MaxValue;

    }
    public void CancelHuntingTrip()
    {
        foreach(Area a in AreaManager.Instance.Areas)
        {
            if(a.HuntingTripInfo != null && a.HuntingTripInfo.IsActive)
            {
                double amountCompleted = DateTime.UtcNow.Subtract(a.HuntingTripInfo.StartTime).TotalHours / a.HuntingTripInfo.ReturnTime.Subtract(a.HuntingTripInfo.StartTime).TotalHours;
                HuntingManager.EndHunt(a.HuntingTripInfo, amountCompleted, false);
            }
        }
        IsOnHuntingTrip = false;
    }
    public void ShowTooltip(MouseEventArgs args, string tipName, bool alignRight)
    {
        if (alignRight)
        {
            TooltipManager.ShowTip(args, tipName, alignRight);
            UpdateState();
        }
        else
        {
            ShowTooltip(args, tipName);
        }
    }
    public void ShowTooltip(MouseEventArgs args, string tipName, bool alignRight, bool showAbove)
    {
        TooltipManager.ShowTip(args, tipName, alignRight, showAbove);
        UpdateState();
    }
    public async Task LoadData(System.Net.Http.HttpClient Http, NavigationManager URIHelper, IJSRuntime Jsruntime)
    {
        await AreaManager.Instance.LoadAreas(Http);
        CurrentLand = AreaManager.Instance.GetLandByName("Quepland");
        UriHelper = URIHelper;
        Updates = await Http.GetFromJsonAsync<List<Update>>("data/Updates.json");
        await MagicManager.Instance.LoadSpells(Http);
        await ItemManager.Instance.LoadItems(Http);
        await Player.Instance.LoadSkills(Http);
        await NPCManager.Instance.LoadNPCs(Http);
        await QuestManager.Instance.LoadQuests(Http);
        await BattleManager.Instance.LoadMonsters(Http);
        await FollowerManager.Instance.LoadFollowers(Http);
        await LoadProgressFlags(Http);
        JSRuntime = Jsruntime;
        await GetDimensions();
        Start();
        InitCompleted = true;
    }

    public async Task LoadProgressFlags(HttpClient Http)
    {
        ProgressFlags = await Http.GetFromJsonAsync<List<ProgressFlag>>("data/ProgressFlags.json");
        foreach(ProgressFlag pf in ProgressFlags)
        {
            Console.WriteLine(pf.Name);
        }
        
    }
    public async Task<int> GetLoadingProgress()
    {
        return GameLoadProgress;
    }
    public void ShowTooltip(MouseEventArgs args, string tipName, string tipData)
    {
        TooltipManager.ShowTip(args, tipName, tipData);
        //UpdateState();
    }
    public void ShowTooltip(MouseEventArgs args, string tipName)
    {
        TooltipManager.ShowTip(args, tipName);
        //UpdateState();
    }
    public void ShowTooltip(MouseEventArgs args, Tooltip tip)
    {
        TooltipManager.ShowTip(args, tip);
        //UpdateState();
    }
    public void ShowItemTooltip(MouseEventArgs args, string itemName, string itemDesc)
    {
        TooltipManager.ShowItemTip(args, itemName, itemDesc);
        //UpdateState();
    }
    public void ShowCraftingTooltip(MouseEventArgs args, string itemName, string itemDesc)
    {
        TooltipManager.ShowCraftingTip(args, itemName, itemDesc);
        //UpdateState();
    }
    public void ShowContextMenu(MouseEventArgs args)
    {
        
        if (CurrentContextMenu != null && CurrentContextMenu.Buttons.Count > 0)
        {
            Console.WriteLine(CurrentContextMenu.Buttons.Count);
            TooltipManager.xPos = args.ClientX;
            TooltipManager.yPos = args.ClientY;
            TooltipManager.ShowContextMenu(args);
        }
        else
        {
            Console.WriteLine("Menu was nulled");
        }
    }
    public static void ResetGame()
    {
        Player.Instance.ResetStats();
        Player.Instance.Inventory.Clear();
        Player.Instance.GetEquippedItems().Clear();
        Bank.Instance.Inventory.Clear();
        foreach (Area a in AreaManager.Instance.Areas)
        {
            a.IsUnlocked = false;
        }
        foreach (Region r in AreaManager.Instance.Regions)
        {
            r.IsUnlocked = false;
        }
        foreach(Quest q in QuestManager.Instance.Quests)
        {
            q.Progress = 0;
            q.IsComplete = false;
            
        }

    }
    public static void GoTo(string url)
    {
        if (url.Contains("World/"))
        {
            Location = url.Split("World/")[1];
        }
        else
        {
            Location = url;
        }
         
        UriHelper.NavigateTo(url);
    }

    public void GoToArea(string url)
    {
        if (url == "Nowhere")
        {
            return;
        }
        if (Location != url)
        {
            StopActions();
            Location = url;
            if (url != "Bank")
            {
                try
                {
                    CurrentLand = AreaManager.Instance.GetLandForArea(AreaManager.Instance.GetAreaByURL(url));

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }
            UriHelper.NavigateTo("World/" + url + "/");
            UpdateState();
        }

    }
    public async Task GetDimensions()
    {
        GameWindowWidth = await JSRuntime.InvokeAsync<int>("getWidth");
        GameWindowHeight = await JSRuntime.InvokeAsync<int>("getHeight");
        BankWindowWidth = await JSRuntime.InvokeAsync<double>("getBankWidth");
        BankWindowHeight = await JSRuntime.InvokeAsync<double>("getBankHeight");
        MainWindowWidth = await JSRuntime.InvokeAsync<double>("getMainWindowWidth");
        RightSidebarWidth = await JSRuntime.InvokeAsync<double>("getRightSidebarWidth");
        
    }

    public void HideTooltip()
    {
        TooltipManager.HideTip();
    }

    public static GameStateSaveData GetSaveData()
    {
        return new GameStateSaveData { IsHunting = IsOnHuntingTrip,
            Location = Location,
            CurrentLand = CurrentLand.Name,
            CurrentTask = CurrentArtisanTask,
            HideLockedItems = HideLockedItems,
            CompactInventory = CompactInventoryView,
            ShowBackgrounds = ShowBackgrounds,
            UseOldBankView = UseOldBank,
            ExtractWarningValue = ExtractWarningValue
        };
    }
    public static void LoadSaveData(GameStateSaveData data)
    {
        IsOnHuntingTrip = AreaManager.LoadedHuntingInfo;
        HideLockedItems = data.HideLockedItems;
        CompactInventoryView = data.CompactInventory;
        ShowBackgrounds = data.ShowBackgrounds;
        CurrentArtisanTask = data.CurrentTask;
        UseOldBank = data.UseOldBankView;      
        ExtractWarningValue = data.ExtractWarningValue;
        if (data.Location == null || data.Location == "" || data.Location == "Battle")
        {
            Location = "QueplandFields";
            CurrentLand = AreaManager.Instance.GetLandByName("Quepland");
        }
        else
        {
            Location = data.Location;
            CurrentLand = AreaManager.Instance.GetLandByName(data.CurrentLand);
        }
    }
    public static void LoadProgressFlagSave(List<ProgressFlag> flags)
    {
        foreach(ProgressFlag f in ProgressFlags)
        {
            foreach(ProgressFlag f2 in flags)
            {
                if(f.Name == f2.Name)
                {
                    f.ProgressValue = f2.ProgressValue;
                    f.ProgressData = f2.ProgressData;
                    f.Completed = f2.Completed;
                }
            }
        }
    }
    public static void AddActiveSpell(ISpell spell, int duration)
    {
        spell.TimeRemaining = duration;
        ActiveSpells.Add(spell);
    }
    public static void RemoveActiveSpellByName(string spell)
    {
        CancelActiveSpells.Add(spell);
    }
    public static void LoadAFKActionData(AFKAction action)
    {       
        if(action != null)
        {
            try
            {
                AreaManager.Instance.GetAFKActionByUniqueID(action.UniqueID).ReturnTime = action.ReturnTime;
                AreaManager.Instance.GetAFKActionByUniqueID(action.UniqueID).StartTime = action.StartTime;
                AreaManager.Instance.GetAFKActionByUniqueID(action.UniqueID).IsActive = action.IsActive;
                CurrentAFKAction = AreaManager.Instance.GetAFKActionByUniqueID(action.UniqueID);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                
            }
        }

    }
    public static bool CheckVersion(string minimumVersion, string versionToCheck)
    {
        string[] compVersions = versionToCheck.Split(".");
        string[] versions = minimumVersion.Split(".");
        if(compVersions.Length < 3 || versions.Length < 3)
        {
            
            return false;
        }
        string bigNum = versions[0];
        string major = versions[1];
        string minor = versions[2];

        string compBigNum = compVersions[0];
        string compMajor = compVersions[1];
        string compMinor = compVersions[2];

        int bn, maj, min = 0;
        string rev = "";

        int cbn, cmaj, cmin = 0;
        string cRev = "";

        if (int.TryParse(bigNum, out bn))
        {
            if (int.TryParse(major, out maj))
            {
                string min1 = Regex.Match(minor, @"\d+").Value;
                if (int.TryParse(min1, out min))
                {
                    rev = Regex.Match(minor, @"[a-zA-Z]").Value;
                    if (int.TryParse(compBigNum, out cbn))
                    {
                        if (int.TryParse(compMajor, out cmaj))
                        {
                            string min2 = Regex.Match(compMinor, @"\d+").Value;
                            if (int.TryParse(min2, out cmin))
                            {
                                cRev = Regex.Match(compMinor, @"[a-zA-Z]").Value;
                            }
                            else
                            {
                                Console.Error.WriteLine("Invalid minor version number:" + compMinor + " in " + versionToCheck);
                                return false;
                            }
                        }
                        else
                        {
                            Console.Error.WriteLine("Invalid major version number:" + compMajor + " in " + versionToCheck);
                            return false;
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine("Invalid big version number:" + compBigNum + " in " + versionToCheck);
                        return false;
                    }
                }
                else
                {
                    Console.Error.WriteLine("Invalid minor version number:" + minor + " in " + minimumVersion);
                    return false;
                }
            }
            else
            {
                Console.Error.WriteLine("Invalid major version number:" + major + " in " + minimumVersion);
                return false;
            }
        }
        else
        {
            Console.Error.WriteLine("Invalid big version number:" + bigNum + " in " + minimumVersion);
            return false;
        }
        if (cbn > bn)
        {
            return true;
        }
        else if (cbn == bn)
        {
            if (cmaj > maj)
            {
                return true;
            }
            else if (cmaj == maj)
            {
                if (cmin > min)
                {
                    return true;
                }
                else if (cmin == min)
                {
                    return string.Compare(cRev, rev) >= 0;
                }
            }
        }
        return false;
    }
    public static bool CheckVersion(string compareVersion)
    {
        return CheckVersion(compareVersion, Version);
    }
    private void StateHasChanged()
    {
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
    public void UpdateState()
    {
        StateHasChanged();
    }
}

