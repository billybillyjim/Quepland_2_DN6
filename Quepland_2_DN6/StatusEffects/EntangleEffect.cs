using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class EntangleEffect : IStatusEffect
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
    public EntangleEffect(StatusEffectData data)
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
        return "Has a " + (ProcOdds * 100) + "% chance to trap an enemy for " + Duration + " total ticks.";
    }
    public void DoEffect(Monster m)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            m.TicksToNextAttack = m.AttackSpeed;
            m.CurrentHP -= Power;
            MessageManager.AddMessage(m.Name + " is entangled and cannot move! The vines grip hard for " + Power + " damage!");
        } 
    }
    public void DoEffect(Player p)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            p.TicksToNextAttack = p.GetWeaponAttackSpeed();
            p.CurrentHP -= Power;
            MessageManager.AddMessage("You are entangled and cannot move! The vines grip hard for " + Power + " damage!");
        }
    }
    public IStatusEffect Copy()
    {
        return new EntangleEffect(d);
    }
}

