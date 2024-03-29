﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UndulateEffect : IStatusEffect
{
    public string Name { get; set; } = "Undulate";
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
    public UndulateEffect(StatusEffectData data)
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
        return "Has a " + (ProcOdds * 100) + "% chance to strike the enemy an extra time.";
    }
    public void DoEffect(Monster m)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            MessageManager.AddMessage(Message, "#34baeb");
            int total = BattleManager.Instance.GetTotalPlayerDamage(m, out _);
            m.CurrentHP -= total;
            BattleManager.Instance.GainCombatExperience(total, "None");
        }
    }
    public void DoEffect(Player p)
    {

    }
    public IStatusEffect Copy()
    {
        return new UndulateEffect(d);
    }
}

