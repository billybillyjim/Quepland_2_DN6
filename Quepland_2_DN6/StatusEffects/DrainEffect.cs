﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class DrainEffect : IStatusEffect
{
    public string Name { get; set; } = "Drain";
    public int Duration { get; set; }
	public int Speed { get; set; }
    public int Power { get; set; }
	public int RemainingTime { get; set; }
    public string CustomData { get; set; }
    public bool OnProc { get; set; } = false;
    public double ProcOdds { get; set; }
    public bool SelfInflicted { get; set; }
    private StatusEffectData d;
    public string Message { get; set; }
    public DrainEffect(StatusEffectData data)
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
        return "Drains an enemy for their HP / " + Power + " damage every " + Speed + " game ticks, for " + Duration + " total ticks, healing you in the process.";
    }
    public void DoEffect(Monster m)
    {
        if(RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            MessageManager.AddMessage(Message);
            int drain = Math.Min(m.HP / Power, m.CurrentHP);
            m.CurrentHP -= drain;

            Player.Instance.CurrentHP = Math.Min(Player.Instance.MaxHP, Player.Instance.CurrentHP + drain);

        }
    }
	public void DoEffect(Player p)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            foreach (GameItem item in Player.Instance.GetEquippedItems())
            {
                if (item.EnabledActions.Contains("Drain Protection"))
                {
                    return;
                }
            }
            if (RemainingTime == Duration)
            {
                MessageManager.AddMessage(Message, "red");
            }
            p.CurrentHP -= p.MaxHP / Power;
            if (Player.Instance.CurrentHP <= 0)
            {
                GameState.DieNextTick = true;
                GameState.DieNextTickMessage = "A Ruwohane swallowed your soul.";
                
            }
        }
    }
    public IStatusEffect Copy()
    {
        return new DrainEffect(d);
    }
}

