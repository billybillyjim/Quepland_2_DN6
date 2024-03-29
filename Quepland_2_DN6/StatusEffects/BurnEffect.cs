﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class BurnEffect : IStatusEffect
{
    public string Name { get; set; } = "Burn";
    public int Duration { get; set; }
    public int Speed { get; set; }
    public int Power { get; set; }
    public int RemainingTime { get; set; }
    public string CustomData { get; set; }
    public bool OnProc { get; set; } = false;

    public double ProcOdds { get; set; }
    public bool SelfInflicted { get; set; }

    public string Message { get; set; }
    private StatusEffectData d;
    public BurnEffect(StatusEffectData data)
    {
        Name = data.Name;
        Duration = data.Duration;
        Speed = data.Speed;
        ProcOdds = data.ProcOdds;
        Power = data.Power;
        Message = data.Message;
        CustomData = data.CustomData;
        RemainingTime = data.Duration;
        SelfInflicted = data.SelfInflicted;
        d = data;
    }
    public string GetDescription()
    {
        return "Burns an enemy for " + Power + " damage every " + Speed + " game ticks, for " + Duration + " total ticks.";
    }
    public void DoEffect(Monster m)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0 && Duration > 0)
        {
            int dmg = (int)(Power * (RemainingTime / (float)Duration));
            MessageManager.AddMessage(m.Name + " took " + dmg + " damage from being on fire.");
            m.CurrentHP -= dmg;
        }
    }
    public void DoEffect(Player p)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0 && Duration > 0)
        {
            int dmg = (int)(Power * (RemainingTime / (float)Duration));
            MessageManager.AddMessage("You took " + dmg + " damage from being on fire.");
            p.CurrentHP -= dmg;
        }
    }
    public IStatusEffect Copy()
    {
        return new BurnEffect(d);
    }
}

