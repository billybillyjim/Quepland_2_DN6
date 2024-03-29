﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


public class QuestManager
{
    private static readonly QuestManager instance = new QuestManager();
    private QuestManager() { }
    static QuestManager() { }
    public static QuestManager Instance
    {
        get
        {
            return instance;
        }
    }
    public List<Quest> Quests = new List<Quest>();

    public async Task LoadQuests(HttpClient Http)
    {
        Quests.Add(await Http.GetFromJsonAsync<Quest>("data/Quests/OrioleOrnithology.json"));
        Quests.Add(await Http.GetFromJsonAsync<Quest>("data/Quests/Mayor'sAssistant.json"));
        //Quests.Add(await Http.GetFromJsonAsync<Quest>("data/Quests/ArborealConundrum.json"));
        Quests.Add(await Http.GetFromJsonAsync<Quest>("data/Quests/CaseOfTheDeadPirate.json"));
        Quests.Add(await Http.GetFromJsonAsync<Quest>("data/Quests/GrandTheftCamel.json"));
        Quests.Add(await Http.GetFromJsonAsync<Quest>("data/Quests/MoundIsMissing.json"));
        Quests.Add(await Http.GetFromJsonAsync<Quest>("data/Quests/Queple'sSword.json"));
        Quests.Add(await Http.GetFromJsonAsync<Quest>("data/Quests/ChrysopoeianCrisis.json"));

    }

    public Quest GetQuestByName(string name)
    {
        return Quests.FirstOrDefault(x => x.Name == name);
    }
    public List<QuestSaveData> GetQuestSaveData()
    {
        List<QuestSaveData> data = new List<QuestSaveData>();
        foreach(Quest q in Quests)
        {
            data.Add(q.GetSaveData());
        }
        return data;
    }
    public void LoadQuestSave(List<QuestSaveData> data)
    {
        foreach(QuestSaveData d in data)
        {
            Quests.Find(x => x.ID == d.ID).LoadFromSave(d);
        }
    }
}

