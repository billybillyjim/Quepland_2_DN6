﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CleaveEffect : IStatusEffect
{
    public string Name { get; set; } = "Cleave";
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
    public CleaveEffect(StatusEffectData data)
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
        return "Ignores enemy armor " + (100 * ProcOdds) + "% of the time.";
    }
    public void DoEffect(Monster m)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            MessageManager.AddMessage(m.Name + " takes the hit as if it had no armor!");
        }
    }
    public void DoEffect(Player p)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            
        }
    }
    public IStatusEffect Copy()
    {
        return new CleaveEffect(d);
    }
}

