﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class HypnotizeEffect : IStatusEffect
{
    public string Name { get; set; } = "Hypnotize";
    public int Duration { get; set; }
    public int Speed { get; set; }
    public int Power { get; set; }
    public int RemainingTime { get; set; }
    public string CustomData { get; set; }
    public double ProcOdds { get; set; }
    public bool OnProc { get; set; } = false;
    public string Message { get; set; }
    public bool SelfInflicted { get; set; }
    private StatusEffectData d;
    public HypnotizeEffect(StatusEffectData data)
    {
        Name = data.Name;
        Duration = data.Duration;
        Speed = data.Speed;
        ProcOdds = data.ProcOdds;
        Power = data.Power;
        CustomData = data.CustomData;
        Message = data.Message;
        RemainingTime = data.Duration;
        SelfInflicted = data.SelfInflicted;
        d = data;
    }
    public string GetDescription()
    {
        return "Has a " + (ProcOdds * 100) + "% chance to hypnotize an enemy for " + Duration + " total ticks.";
    }
    public void DoEffect(Monster m)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            m.TicksToNextAttack = m.AttackSpeed;
            MessageManager.AddMessage(Message);
        }
    }
    public void DoEffect(Player p)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            p.TicksToNextAttack = p.GetWeaponAttackSpeed();
            MessageManager.AddMessage("You are hypnotized and cannot move!");
        }
    }
    public IStatusEffect Copy()
    {
        return new HypnotizeEffect(d);
    }
}

