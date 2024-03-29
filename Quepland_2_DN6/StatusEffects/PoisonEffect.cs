﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PoisonEffect : IStatusEffect
{
    public string Name { get; set; } = "Poison";
    public int Duration { get; set; }
	public int Speed { get; set; }
    public int Power { get; set; }
	public int RemainingTime { get; set; }
    public bool OnProc { get; set; } = false;
    public string CustomData { get; set; }
    public double ProcOdds { get; set; }
    public bool SelfInflicted { get; set; }
    private StatusEffectData d;
    public string Message { get; set; }
    public PoisonEffect(StatusEffectData data)
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
        return "Poisons an enemy for " + Power + " damage every " + Speed + " game ticks, for " + Duration + " total ticks.";
    }
    public void DoEffect(Monster m)
    {
        if(RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            m.CurrentHP -= Power;
            MessageManager.AddMessage(Message);
        }
    }
	public void DoEffect(Player p)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            MessageManager.AddMessage("You took " + Power + " damage from the poison.");
            p.CurrentHP -= Power;
            if (Player.Instance.CurrentHP <= 0)
            {
                Player.Instance.Die("Poison");
            }
        }
    }
    public IStatusEffect Copy()
    {
        return new PoisonEffect(d);
    }
}

