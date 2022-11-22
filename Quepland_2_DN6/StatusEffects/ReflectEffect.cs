using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ReflectEffect : IStatusEffect
{
    public string Name { get; set; } = "Reflect";
    public int Duration { get; set; }
    public int Speed { get; set; }
    public int Power { get; set; }
    public int RemainingTime { get; set; }
    public string CustomData { get; set; }
    public bool OnProc { get; set; } = true;
    public double ProcOdds { get; set; }
    public bool SelfInflicted { get; set; }

    public string Message { get; set; }
    private StatusEffectData d;
    public ReflectEffect(StatusEffectData data)
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
        return "Reflects damage to an enemy for " + Power + "% of the damage they dealt.";
    }
    public void DoEffect(Monster m)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            MessageManager.AddMessage(m.Name + " took " + Power + " damage from being on fire.");
            m.CurrentHP -= Power;
        }
    }
    public void DoEffect(Player p)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            MessageManager.AddMessage("You took " + Power + " damage from being on fire.");
            p.CurrentHP -= Power;
        }
    }

    public IStatusEffect Copy()
    {
        return new ReflectEffect(d);
    }
}

