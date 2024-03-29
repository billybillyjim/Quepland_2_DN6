﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class FreezeEffect : IStatusEffect
{
    public string Name { get; set; } = "Freeze";
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
    public FreezeEffect(StatusEffectData data)
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
        return "Has a " + (ProcOdds * 100) + "% chance to freeze an enemy for " + Duration + " total ticks.";
    }
    public void DoEffect(Monster m)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            m.TicksToNextAttack = m.AttackSpeed;
            MessageManager.AddMessage(m.Name + " is frozen in its tracks!");
        }
    }
    public void DoEffect(Player p)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            p.TicksToNextAttack = p.GetWeaponAttackSpeed();
            MessageManager.AddMessage("You are frozen in your tracks!");
        }
    }
    public IStatusEffect Copy()
    {
        return new FreezeEffect(d);
    }
}

