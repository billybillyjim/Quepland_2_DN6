using Ionic.Zip;
using Ionic.Zlib;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public static class SaveManager
{
    public static string SaveVersion = "";
    public static DateTime LastSave;
    public static IJSRuntime jSRuntime;
    public static bool DebugMode = false;

    public static async Task SaveGame(bool manual)
    {

        if (!manual && (GameState.ShowStartMenu || GameState.InitCompleted == false))
        {
            return;
        }
        GameState.UseNewSaveCompression = true;
        string mode = GameState.CurrentGameMode.ToString();
        try
        {
            await SetItemAsync("Version:" + mode, Compress(GameState.Version));
        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to save game version, please contact the developer.", "red");
            Console.WriteLine("Failed to save.");
            Console.WriteLine(e.Message);
        }
        try
        {
            await SetItemAsync("Playtime:" + mode, GetSaveString(GameState.CurrentTick));
        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to save playtime, please contact the developer.", "red");
            Console.WriteLine("Failed to save.");
            Console.WriteLine(e.Message);
        }
        try
        {
            await SetItemAsync("LastSave:" + mode, DateTime.UtcNow.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to save last save time, please contact the developer.", "red");
            Console.WriteLine("Failed to save.");
            Console.WriteLine(e.Message);
        }
        try
        {
            await SetItemAsync("Skills:" + mode, Compress(GetSkillsSave()));
        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to save skill data, please contact the developer.", "red");
            Console.WriteLine("Failed to save.");
            Console.WriteLine(e.Message);
        }
        try
        {
            await SetItemAsync("Inventory:" + mode, Compress(GetItemSave(Player.Instance.Inventory)));
        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to save inventory data, please contact the developer.", "red");
            Console.WriteLine("Failed to save.");
            Console.WriteLine(e.Message);
        }
        try
        {
            await SetItemAsync("Bank:" + mode, Compress(GetItemSave(Bank.Instance.Inventory)));
        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to save bank data, please contact the developer.", "red");
            Console.WriteLine("Failed to save.");
            Console.WriteLine(e.Message);
        }
        try
        {
            await SetItemAsync("BankTabs:" + mode, GetSaveString(Bank.Instance.Tabs));
        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to save bank tabs, please contact the developer.", "red");
            Console.WriteLine("Failed to save.");
            Console.WriteLine(e.Message);
        }
        try
        {
            await SetItemAsync("Areas:" + mode, Compress(GetAreaSave()));
        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to save areas, please contact the developer.", "red");
            Console.WriteLine("Failed to save.");
            Console.WriteLine(e.Message);
        }
        try
        {
            await SetItemAsync("Regions:" + mode, Compress(GetRegionSave()));
        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to save regions, please contact the developer.", "red");
            Console.WriteLine("Failed to save.");
            Console.WriteLine(e.Message);
        }
        try
        {
            await SetItemAsync("Dungeons:" + mode, GetSaveString(AreaManager.Instance.GetDungeonSaveData()));
        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to save dungeon progress, please contact the developer.", "red");
            Console.WriteLine("Failed to save.");
            Console.WriteLine(e.Message);
        }
        try
        {
            await SetItemAsync("Quests:" + mode, Compress(GetQuestSave()));
        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to save quest progress, please contact the developer.", "red");
            Console.WriteLine("Failed to save.");
            Console.WriteLine(e.Message);
        }
        try
        {
            await SetItemAsync("GameState:" + mode, GetSaveString(GameState.GetSaveData()));
        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to save game state, please contact the developer.", "red");
            Console.WriteLine("Failed to save.");
            Console.WriteLine(e.Message);
        }
        try
        {
            await SetItemAsync("Player:" + mode, GetSaveString(Player.Instance.GetSaveData()));
        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to save player info, please contact the developer.", "red");
            Console.WriteLine("Failed to save.");
            Console.WriteLine(e.Message);
        }
        try
        {
            await SetItemAsync("Followers:" + mode, Compress(FollowerManager.Instance.GetNewSaveData()));
        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to save followers, please contact the developer.", "red");
            Console.WriteLine("Failed to save.");
            Console.WriteLine(e.Message);
        }
        try
        {
            await SetItemAsync("TanningInfo:" + mode, Compress(GetTanningSave()));
        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to save tanning information, please contact the developer.", "red");
            Console.WriteLine("Failed to save.");
            Console.WriteLine(e.Message);
        }
        try
        {
            await SetItemAsync("Dojos:" + mode, GetSaveString(AreaManager.Instance.GetDojoSaveData()));
        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to save dojo information, please contact the developer.", "red");
            Console.WriteLine("Failed to save.");
            Console.WriteLine(e.Message);
        }
        try
        {
            await SetItemAsync("AFKAction:" + mode, GetSaveString(GameState.CurrentAFKAction));
        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to save current AFK action, please contact the developer.", "red");
            Console.WriteLine("Failed to save.");
            Console.WriteLine(e.Message);
        }
        try
        {
            await SetItemAsync("Tomes:" + mode, GetSaveString(ItemManager.Instance.Tomes));
        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to save tome travel data, please contact the developer.", "red");
            Console.WriteLine("Failed to save.");
            Console.WriteLine(e.Message);
        }
        try
        {
            await SetItemAsync("KC:" + mode, GetSaveString(BattleManager.Instance.GetKillCounts()));

        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to save monster kill counts, please contact the developer.", "red");
            Console.WriteLine("Failed to save.");
            Console.WriteLine(e.Message);
        }
        try
        {
            await SetItemAsync("Flags:" + mode, GetSaveString(GameState.ProgressFlags));

        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to save progress flags, please contact the developer.", "red");
            Console.WriteLine("Failed to save.");
            Console.WriteLine(e.Message);
        }
        await SetItemAsync("NewSaveCompression", "true");
        LastSave = DateTime.UtcNow;
        GameState.IsSaving = false;

    }
    public static async Task SaveGame()
    {
        await SaveGame(false);
    }

    public static async Task DeleteHCSave()
    {
        string mode = "Hardcore";
        await RemoveItemAsync("Version:" + mode);
        await RemoveItemAsync("Playtime:" + mode);
        await RemoveItemAsync("LastSave:" + mode);
        await RemoveItemAsync("Skills:" + mode);
        await RemoveItemAsync("Inventory:" + mode);
        await RemoveItemAsync("Bank:" + mode);
        await RemoveItemAsync("BankTabs:" + mode);
        await RemoveItemAsync("Areas:" + mode);
        await RemoveItemAsync("Regions:" + mode);
        await RemoveItemAsync("Dungeons:" + mode);
        await RemoveItemAsync("Quests:" + mode);
        await RemoveItemAsync("GameState:" + mode);
        await RemoveItemAsync("Player:" + mode);
        await RemoveItemAsync("Followers:" + mode);
        await RemoveItemAsync("TanningInfo:" + mode);
        await RemoveItemAsync("Dojos:" + mode);
        await RemoveItemAsync("AFKAction:" + mode);
        await RemoveItemAsync("Tomes:" + mode);
        await RemoveItemAsync("KC:" + mode);
        await RemoveItemAsync("Flags:" + mode);
    }
    public static async Task LoadSaveGame(string mode)
    {

        var serializerSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
        int errorcode = 0;
        string version = await GetItemAsync<string>("Version:" + mode);
        bool error = false;
        try
        {
            if (await ContainsKeyAsync("NewSaveCompression"))
            {
                GameState.UseNewSaveCompression = true;
            }
            if (mode == "Normal")
            {
                GameState.CurrentGameMode = GameState.GameType.Normal;
            }
            else if (mode == "Hardcore")
            {
                GameState.CurrentGameMode = GameState.GameType.Hardcore;
            }
            else if (mode == "Ultimate")
            {
                GameState.CurrentGameMode = GameState.GameType.Ultimate;
            }

            if (await ContainsKeyAsync("Playtime:" + mode))
            {
                GameState.CurrentTick = int.Parse(Decompress(await GetItemAsync<string>("Playtime:" + mode)));
            }
            errorcode++; //1
            if (await ContainsKeyAsync("LastSave:" + mode))
            {
                LastSave = DateTime.Parse(await GetItemAsync<string>("LastSave:" + mode), System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            MessageManager.AddMessage("Failed to load save. Error code:" + errorcode);
            error = true;
        }
        try
        {
            errorcode++; //2
            if (await ContainsKeyAsync("Skills:" + mode))
            {
                string[] data = Decompress(await GetItemAsync<string>("Skills:" + mode)).Split(',');
                foreach (string d in data)
                {
                    if (d.Length > 1)
                    {
                        Player.Instance.Skills.Find(x => x.Name == d.Split(':')[0]).LoadExperience(long.Parse(d.Split(':')[1]));

                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            MessageManager.AddMessage("Failed to load save. Error code:" + errorcode);
            error = true;
        }
        errorcode++; //3
        try
        {
            if (await ContainsKeyAsync("Inventory:" + mode))
            {
                Player.Instance.Inventory.Clear();
                Player.Instance.Inventory.LoadData(Decompress(await GetItemAsync<string>("Inventory:" + mode)));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            MessageManager.AddMessage("Failed to load save. Error code:" + errorcode);
            error = true;
        }
        errorcode++; //4
        try
        {
            if (await ContainsKeyAsync("Bank:" + mode))
            {
                Bank.Instance.Inventory.Clear();
                Bank.Instance.Inventory.LoadData(Decompress(await GetItemAsync<string>("Bank:" + mode)));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            MessageManager.AddMessage("Failed to load save. Error code:" + errorcode);
            error = true;
        }
        errorcode++; //5
        try
        {
            if (await ContainsKeyAsync("BankTabs:" + mode))
            {
                Bank.Instance.LoadTabs(DeserializeToList(Decompress(await GetItemAsync<string>("BankTabs:" + mode))));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            MessageManager.AddMessage("Failed to load save. Error code:" + errorcode);
            error = true;
        }
        try
        {
            errorcode++; //6
            if (await ContainsKeyAsync("Areas:" + mode))
            {
                AreaManager.Instance.LoadAreaSave(JsonConvert.DeserializeObject<List<AreaSaveData>>(Decompress(await GetItemAsync<string>("Areas:" + mode))));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            MessageManager.AddMessage("Failed to load save. Error code:" + errorcode);
            error = true;
        }

        errorcode++; //7
        try
        {
            if (await ContainsKeyAsync("Regions:" + mode))
            {
                AreaManager.Instance.LoadRegionSave(JsonConvert.DeserializeObject<List<RegionSaveData>>(Decompress(await GetItemAsync<string>("Regions:" + mode))));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            MessageManager.AddMessage("Failed to load save. Error code:" + errorcode);
            error = true;
        }
        errorcode++; //8
        try
        {
            if (await ContainsKeyAsync("Dungeons:" + mode))
            {
                AreaManager.Instance.LoadDungeonSaveData(JsonConvert.DeserializeObject<List<DungeonSaveData>>(Decompress(await GetItemAsync<string>("Dungeons:" + mode))));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            MessageManager.AddMessage("Failed to load save. Error code:" + errorcode);
            error = true;
        }
        errorcode++; //9
        try
        {
            if (await ContainsKeyAsync("Quests:" + mode))
            {
                QuestManager.Instance.LoadQuestSave(JsonConvert.DeserializeObject<List<QuestSaveData>>(Decompress(await GetItemAsync<string>("Quests:" + mode))));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            MessageManager.AddMessage("Failed to load save. Error code:" + errorcode);
            error = true;
        }
        errorcode++; //10
        try
        {
            if (await ContainsKeyAsync("GameState:" + mode))
            {
                GameState.LoadSaveData(JsonConvert.DeserializeObject<GameStateSaveData>(Decompress(await GetItemAsync<string>("GameState:" + mode))));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            MessageManager.AddMessage("Failed to load save. Error code:" + errorcode);
            error = true;
        }
        errorcode++; //11
        try
        {
            if (await ContainsKeyAsync("Followers:" + mode))
            {
                if (GameState.CheckVersion("1.1.0", Decompress(await GetItemAsync<string>("Version:" + mode))))
                {
                    FollowerManager.Instance.LoadNewSaveData(Decompress2(await GetItemAsync<string>("Followers:" + mode)));
                }
                else
                {
                    FollowerManager.Instance.LoadSaveData(Decompress2(await GetItemAsync<string>("Followers:" + mode)));
                }

            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            MessageManager.AddMessage("Failed to load save. Error code:" + errorcode);
            error = true;
        }
        errorcode++; //12
        try
        {
            if (await ContainsKeyAsync("Player:" + mode))
            {
                Player.Instance.LoadSaveData(JsonConvert.DeserializeObject<PlayerSaveData>(Decompress(await GetItemAsync<string>("Player:" + mode))), version);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            MessageManager.AddMessage("Failed to load save. Error code:" + errorcode);
            error = true;
        }
        errorcode++; //13
        try
        {
            if (await ContainsKeyAsync("TanningInfo:" + mode))
            {
                AreaManager.Instance.LoadTanningSave(JsonConvert.DeserializeObject<List<TanningSaveData>>(Decompress(await GetItemAsync<string>("TanningInfo:" + mode))));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            MessageManager.AddMessage("Failed to load save. Error code:" + errorcode);
            error = true;
        }
        errorcode++; //14
        try
        {
            if (await ContainsKeyAsync("Dojos:" + mode))
            {
                AreaManager.Instance.LoadDojoSaveData(JsonConvert.DeserializeObject<List<DojoSaveData>>(Decompress(await GetItemAsync<string>("Dojos:" + mode))));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            MessageManager.AddMessage("Failed to load save. Error code:" + errorcode);
            error = true;
        }
        errorcode++; //15
        try
        {
            if (await ContainsKeyAsync("AFKAction:" + mode))
            {
                GameState.LoadAFKActionData(JsonConvert.DeserializeObject<AFKAction>(Decompress(await GetItemAsync<string>("AFKAction:" + mode))));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            MessageManager.AddMessage("Failed to load save. Error code:" + errorcode);
            error = true;
        }
        errorcode++; //16
        try
        {
            if (await ContainsKeyAsync("Tomes:" + mode))
            {
                ItemManager.Instance.Tomes = JsonConvert.DeserializeObject<List<TomeData>>(Decompress(await GetItemAsync<string>("Tomes:" + mode)));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            MessageManager.AddMessage("Failed to load save. Error code:" + errorcode);
            error = true;
        }
        errorcode++; //17
        try
        {
            if (await ContainsKeyAsync("KC:" + mode))
            {
                BattleManager.Instance.LoadKillCounts(JsonConvert.DeserializeObject<string>(Decompress(await GetItemAsync<string>("KC:" + mode))));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            MessageManager.AddMessage("Failed to load save. Error code:" + errorcode);
            error = true;
        }
        errorcode++; //18
        try
        {
            if (await ContainsKeyAsync("Flags:" + mode))
            {
                GameState.LoadProgressFlagSave(JsonConvert.DeserializeObject<List<ProgressFlag>>(Decompress(await GetItemAsync<string>("Flags:" + mode))));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            MessageManager.AddMessage("Failed to load save. Error code:" + errorcode);
            error = true;
        }
        if (error)
        {
            string buggedSave = "";
            buggedSave += await GetItemAsync<string>("Version:" + mode) + ",";
            buggedSave += await GetItemAsync<string>("Playtime:" + mode) + ",";
            buggedSave += await GetItemAsync<string>("LastSave:" + mode) + ",";
            buggedSave += await GetItemAsync<string>("Skills:" + mode) + ",";
            buggedSave += await GetItemAsync<string>("Inventory:" + mode) + ",";
            buggedSave += await GetItemAsync<string>("Bank:" + mode) + ",";
            buggedSave += await GetItemAsync<string>("BankTabs:" + mode) + ",";
            buggedSave += await GetItemAsync<string>("Areas:" + mode) + ",";
            buggedSave += await GetItemAsync<string>("Regions:" + mode) + ",";
            buggedSave += await GetItemAsync<string>("Dungeons:" + mode) + ",";
            buggedSave += await GetItemAsync<string>("Quests:" + mode) + ",";
            buggedSave += await GetItemAsync<string>("GameState:" + mode) + ",";
            buggedSave += await GetItemAsync<string>("Player:" + mode) + ",";
            buggedSave += await GetItemAsync<string>("Followers:" + mode) + ",";
            buggedSave += await GetItemAsync<string>("TanningInfo:" + mode) + ",";
            buggedSave += await GetItemAsync<string>("Dojos:" + mode) + ",";
            buggedSave += await GetItemAsync<string>("AFKAction:" + mode) + ",";
            buggedSave += await GetItemAsync<string>("Tomes:" + mode) + ",";
            buggedSave += await GetItemAsync<string>("KC:" + mode) + ",";
            buggedSave += await GetItemAsync<string>("Flags:" + mode);
            await SetItemAsync("SaveError", buggedSave);
        }
    }

    public static string GetSaveExport()
    {
        string file = "";
        //0
        file += Compress(GameState.CurrentGameMode.ToString()) + ",";
        Console.WriteLine(GameState.CurrentGameMode.ToString());
        //1
        file += Compress(GameState.Version) + ",";
        //2
        file += Compress(GameState.CurrentTick.ToString()) + ",";
        //3
        file += Compress(DateTime.UtcNow.ToString(System.Globalization.CultureInfo.InvariantCulture)) + ",";
        //4
        file += Compress(GetSkillsSave()) + ",";
        //5
        file += Compress(GetItemSave(Player.Instance.Inventory)) + ",";
        //6
        file += Compress(GetItemSave(Bank.Instance.Inventory)) + ",";
        //7
        file += Compress(GetAreaSave()) + ",";
        //8
        file += Compress(GetRegionSave()) + ",";
        //9
        file += Compress(GetQuestSave()) + ",";
        //10
        file += Compress(JsonConvert.SerializeObject(GameState.GetSaveData())) + ",";
        //11
        file += Compress(JsonConvert.SerializeObject(Player.Instance.GetSaveData())) + ",";
        //12
        if (GameState.CheckVersion("1.1.0"))
        {
            file += Compress(FollowerManager.Instance.GetNewSaveData()) + ",";
            //file += Compress(FollowerManager.Instance.GetSaveData()) + ",";
        }
        else
        {
            file += Compress(FollowerManager.Instance.GetSaveData()) + ",";
        }

        //13
        file += Compress(GetTanningSave()) + ",";
        //14
        file += Compress(JsonConvert.SerializeObject(AreaManager.Instance.GetDojoSaveData())) + ",";
        //15
        file += Compress(JsonConvert.SerializeObject(Bank.Instance.Tabs)) + ",";
        //16
        file += GetSaveString(AreaManager.Instance.GetDungeonSaveData()) + ",";
        //17
        file += GetSaveString(ItemManager.Instance.Tomes) + ",";
        //18
        file += GetSaveString(BattleManager.Instance.GetKillCounts()) + ",";
        //19
        file += GetSaveString(GameState.ProgressFlags);
        return file;
    }
    public static void ImportSave(string file)
    {
        string[] data = file.Split(',');
        string version = "";
        if (DebugMode)
        {
            foreach (string line in data)
            {
                string dc = Decompress(line);
                if (dc != null)
                {
                    Console.WriteLine(dc);
                }
            }
        }
        if (Decompress(data[0]) == "Normal")
        {
            GameState.CurrentGameMode = GameState.GameType.Normal;
        }
        else if (Decompress(data[0]) == "Hardcore")
        {
            GameState.CurrentGameMode = GameState.GameType.Hardcore;
        }
        else if (Decompress(data[0]) == "Ultimate")
        {
            GameState.CurrentGameMode = GameState.GameType.Ultimate;
        }
        if (data.Length > 1)
        {
            version = Decompress(data[1]);
        }
        if (data.Length > 2)
        {
            try
            {
                GameState.CurrentTick = int.Parse(Decompress(data[2]));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                MessageManager.AddMessage("Failed to load game's current tick. This may mean your save file has been corrupted.", "red");
            }

        }
        if (data.Length > 3)
        {
            try
            {
                LastSave = DateTime.Parse(Decompress(data[3]));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                LastSave = DateTime.MinValue;
            }

        }
        if (data.Length > 4)
        {
            try
            {
                string[] skillData = (Decompress(data[4])).Split(',');
                foreach (string d in skillData)
                {
                    if (d.Length > 1)
                    {
                        Player.Instance.Skills.Find(x => x.Name == d.Split(':')[0]).LoadExperience(long.Parse(d.Split(':')[1]));

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        if (data.Length > 5)
        {
            try
            {
                Player.Instance.Inventory.Clear();
                Player.Instance.Inventory.LoadData(Decompress(data[5]));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

        }
        if (data.Length > 6)
        {
            try
            {
                Bank.Instance.Inventory.Clear();
                Bank.Instance.Inventory.FixItems = true;
                Bank.Instance.Inventory.LoadData(Decompress(data[6]));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

        }
        if (data.Length > 7)
        {
            try
            {
                AreaManager.Instance.LoadAreaSave(JsonConvert.DeserializeObject<List<AreaSaveData>>(Decompress(data[7])));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        if (data.Length > 8)
        {
            try
            {
                AreaManager.Instance.LoadRegionSave(JsonConvert.DeserializeObject<List<RegionSaveData>>(Decompress(data[8])));

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        if (data.Length > 9)
        {
            try
            {
                QuestManager.Instance.LoadQuestSave(JsonConvert.DeserializeObject<List<QuestSaveData>>(Decompress(data[9])));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        if (data.Length > 10)
        {
            try
            {
                GameState.LoadSaveData(JsonConvert.DeserializeObject<GameStateSaveData>(Decompress(data[10])));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        if (data.Length > 11)
        {
            try
            {
                Player.Instance.LoadSaveData(JsonConvert.DeserializeObject<PlayerSaveData>(Decompress(data[11])), version);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        if (data.Length > 12)
        {
            try
            {
                if (GameState.CheckVersion("1.1.0", version))
                {
                    FollowerManager.Instance.LoadNewSaveData(Decompress(data[12]));
                }
                else
                {
                    FollowerManager.Instance.LoadSaveData(Decompress(data[12]));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        if (data.Length > 13)
        {
            try
            {
                AreaManager.Instance.LoadTanningSave(JsonConvert.DeserializeObject<List<TanningSaveData>>(Decompress(data[13])));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        if (data.Length > 14)
        {
            try
            {
                AreaManager.Instance.LoadDojoSaveData(JsonConvert.DeserializeObject<List<DojoSaveData>>(Decompress(data[14])));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        if (data.Length > 15)
        {
            try
            {
                Bank.Instance.LoadTabs(DeserializeToList(Decompress(data[15])));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        if (data.Length > 16)
        {
            try
            {
                AreaManager.Instance.LoadDungeonSaveData(JsonConvert.DeserializeObject<List<DungeonSaveData>>(Decompress(data[16])));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        if (data.Length > 17)
        {
            try
            {
                ItemManager.Instance.Tomes = JsonConvert.DeserializeObject<List<TomeData>>(Decompress(data[17]));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

        }
        if (data.Length > 18)
        {
            try
            {
                BattleManager.Instance.LoadKillCounts(JsonConvert.DeserializeObject<string>(Decompress(data[18])));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

        }
        if(data.Length > 19)
        {
            try
            {
                GameState.LoadProgressFlagSave(JsonConvert.DeserializeObject<List<ProgressFlag>>(Decompress(data[19])));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }

    public static string GetItemSave(Inventory i)
    {
        string data = "";
        foreach (KeyValuePair<GameItem, int> pair in i.GetItems())
        {
            data += pair.Key.UniqueID + "_" + pair.Value + "_" + JsonConvert.SerializeObject(pair.Key.Tabs) + "_" + pair.Key.IsLocked + "/";
        }
        return data;
    }
    public static string GetFollowerItemSave(Inventory i)
    {
        return GetItemSave(i).Replace(',', char.MaxValue);
    }
    public static string FixFollowerItemSave(string data)
    {
        return data.Replace(char.MaxValue, ',');
    }
    public static string GetAreaSave()
    {
        return JsonConvert.SerializeObject(AreaManager.Instance.GetAreaSave());
    }
    public static string GetRegionSave()
    {
        return JsonConvert.SerializeObject(AreaManager.Instance.GetRegionSaveData());
    }
    public static string GetQuestSave()
    {
        return JsonConvert.SerializeObject(QuestManager.Instance.GetQuestSaveData());
    }
    public static string GetSkillsSave()
    {
        string s = "";
        foreach (Skill skill in Player.Instance.Skills)
        {
            s += skill.Name + ":" + skill.Experience + ",";
        }
        return s;
    }
    public static string GetFollowerSkillSave(Follower f)
    {
        string s = "";
        s += f.Name + ":" + f.Banking.Experience + ",";
        return s;
    }
    public static string GetTanningSave()
    {
        return JsonConvert.SerializeObject(AreaManager.Instance.GetTanningSaveData());
    }
    public static async Task<bool> HasSaveFile(string mode)
    {
        return await ContainsKeyAsync("Version:" + mode);
    }

    public static string GetSaveString(Object o)
    {
        return Compress(JsonConvert.SerializeObject(o));
    }
    public static List<string> DeserializeToList(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return new List<string>();
        }
        return JsonConvert.DeserializeObject<List<string>>(s);
    }
    public static string Compress(string s)
    {
        if (GameState.UseNewSaveCompression)
        {
            return Compress2(s);
        }
        byte[] bytes = Encoding.UTF8.GetBytes(s);
        using (var stream = new MemoryStream())
        {
            var len = BitConverter.GetBytes(bytes.Length);
            stream.Write(len, 0, 4);
            using (var compressionStream = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Compress))
            {
                compressionStream.Write(bytes, 0, bytes.Length);
            }
            return Convert.ToBase64String(stream.ToArray());
        }
    }
    public static string Compress2(string s)
    {
        return Convert.ToBase64String(Ionic.Zlib.GZipStream.CompressString(s));
    }
    public static string Decompress2(string s)
    {
        try
        {
            return Ionic.Zlib.GZipStream.UncompressString(Convert.FromBase64String(s));
        }
        catch (Exception e)
        {
            MessageManager.AddMessage("Failed to Decompress save data. This usually means the save is incomplete or has been altered in some way.");
            Console.WriteLine(e.ToString());
        }
        return "";
    }
    public static string Decompress(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return "";
        }
        if (GameState.UseNewSaveCompression)
        {
            return Decompress2(s);
        }
        try
        {
            Span<byte> buffer = new Span<byte>(new byte[s.Length]);
            if (Convert.TryFromBase64String(s, buffer, out int b) == false)
            {
                if (Convert.TryFromBase64String(s.PadRight(s.Length / 4 * 4 + (s.Length % 4 == 0 ? 0 : 4), '='), new Span<byte>(new byte[s.Length]), out int pad))
                {
                    using (var source = new MemoryStream(Convert.FromBase64String(s)))
                    {
                        byte[] len = new byte[4];
                        source.Read(len, 0, 4);
                        var l = BitConverter.ToInt32(len, 0);
                        using (var decompressionStream = new System.IO.Compression.GZipStream(source, System.IO.Compression.CompressionMode.Decompress))
                        {
                            var result = new byte[l];
                            decompressionStream.Read(result, 0, l);
                            return Encoding.UTF8.GetString(result);
                        }
                    }

                }
                else
                {
                    Console.WriteLine("Padding failed.");
                    Console.WriteLine("String Length:" + s.Length);
                    Console.WriteLine("Bytes parsed:" + b);
                    Console.WriteLine("End padding:" + s.Substring(s.Length - 10));
                }
            }
            using (var source = new MemoryStream(Convert.FromBase64String(s)))
            {
                byte[] len = new byte[4];
                source.Read(len, 0, 4);
                var l = BitConverter.ToInt32(len, 0);
                using (var decompressionStream = new System.IO.Compression.GZipStream(source, System.IO.Compression.CompressionMode.Decompress))
                {
                    var result = new byte[l];
                    decompressionStream.Read(result, 0, l);
                    return Encoding.UTF8.GetString(result);
                }
            }
        }

        catch
        {
            return Decompress2(s);
        }

    }
    public async static Task SetItemAsync(string key, object data)
    {
        if (key == null || key.Length == 0)
        {
            return;
        }
        await jSRuntime.InvokeVoidAsync("localStorage.setItem", key, data);
    }
    public async static Task<string> GetItemAsync<T>(string key)
    {
        if (key == null || key.Length == 0)
        {
            throw new ArgumentNullException(nameof(key));
        }
        string data = await jSRuntime.InvokeAsync<string>("localStorage.getItem", key);
        if (data == null || data.Length == 0)
        {
            return "";
        }
        return data;
    }
    public async static Task<bool> ContainsKeyAsync(string key)
    {
        try
        {
            return await jSRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", key);
        }
        catch
        {
            return false;
        }
    }
    public async static Task RemoveItemAsync(string key)
    {
        if (key == null || key.Length == 0)
        {
            throw new ArgumentNullException(nameof(key));
        }
        await jSRuntime.InvokeVoidAsync("localStorage.removeItem", key);

    }
}

