using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class QuickStrikeEffect : IStatusEffect
{
    public string Name { get; set; } = "Quick Strike";
    public int Duration { get; set; }
    public int Speed { get; set; }
    public int Power { get; set; }
    public int RemainingTime { get; set; }
    public string CustomData { get; set; }
    public double ProcOdds { get; set; }
    public bool SelfInflicted { get; set; }

    public string Message { get; set; }
    private StatusEffectData d;
    public QuickStrikeEffect(StatusEffectData data)
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
        return "Has a " + (ProcOdds * 100) + "% chance to counter-strike the enemy.";
    }
    public void DoEffect(Monster m)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            m.TicksToNextAttack = 0;
            MessageManager.AddMessage(m.Name + " strikes with a second quick attack!");
        }
    }
    public void DoEffect(Player p)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            p.TicksToNextAttack = 0;
            MessageManager.AddMessage("You strike with a second quick attack!");
        }
    }
    public IStatusEffect Copy()
    {
        return new QuickStrikeEffect(d);
    }
}

