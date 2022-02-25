using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class SelfHealEffect : IStatusEffect
{
    public string Name { get; set; } = "SelfHeal";
    public int Duration { get; set; }
    public int Speed { get; set; }
    public int Power { get; set; }
    public int RemainingTime { get; set; }
    public string CustomData { get; set; }
    public double ProcOdds { get; set; }
    private StatusEffectData d;
    public string Message { get; set; }
    public bool SelfInflicted { get; set; }
    public SelfHealEffect(StatusEffectData data)
    {
        Name = data.Name;
        Duration = data.Duration;
        Speed = data.Speed;
        ProcOdds = data.ProcOdds;
        CustomData = data.CustomData;
        Power = data.Power;
        Message = data.Message;
        RemainingTime = data.Duration;
        SelfInflicted = data.SelfInflicted; 
        d = data;
    }
    public string GetDescription()
    {
        return "Has a " + (ProcOdds * 100) + "% chance to heal you every attack for " + Power + ".";
    }
    public void DoEffect(Monster m)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            m.CurrentHP += Power;
            MessageManager.AddMessage(Message);
        }
    }
    public void DoEffect(Player p)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            p.CurrentHP += Power;
            MessageManager.AddMessage("You recover " + Power + " HP!");
        }
    }
    public IStatusEffect Copy()
    {
        return new SelfHealEffect(d);
    }
}

