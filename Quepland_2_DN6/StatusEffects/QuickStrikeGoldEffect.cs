﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class QuickStrikeGoldEffect : IStatusEffect
{
    public string Name { get; set; } = "Quick Strike Gold";
    public int Duration { get; set; }
    public int Speed { get; set; }
    public int Power { get; set; }
    public int RemainingTime { get; set; }

    public double ProcOdds { get; set; }
    public bool SelfInflicted { get; set; }

    public string Message { get; set; }
    private StatusEffectData d;
    public QuickStrikeGoldEffect(StatusEffectData data)
    {
        Name = data.Name;
        Duration = data.Duration;
        Speed = data.Speed;
        ProcOdds = data.ProcOdds;
        Power = data.Power;
        Message = data.Message;
        RemainingTime = data.Duration;
        SelfInflicted = data.SelfInflicted;
        d = data;
    }
    public void DoEffect(Monster m)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            m.TicksToNextAttack = 0;
            MessageManager.AddMessage(m.Name + " quickly counters your attack!");
        }
    }
    public void DoEffect(Player p)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            
            if(p.Inventory.RemoveItems(ItemManager.Instance.GetItemByName("Coins"), Power) == Power)
            {
                p.TicksToNextAttack = 0;
                MessageManager.AddMessage("A clear 'ka-ching' fills the air as your hammer counters the enemy's attack!");
            }
            
        }
    }
    public IStatusEffect Copy()
    {
        return new QuickStrikeGoldEffect(d);
    }
}

