using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class SelfHealEffect : IStatusEffect
{
    public string Name { get; set; } = "Self Heal";
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
        return "Has a " + (ProcOdds * 100) + "% chance to heal you every attack for " + Power + " HP.";
    }
    public void DoEffect(Monster m)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            int heal = Math.Min(Power, m.HP - m.CurrentHP);
            m.CurrentHP += heal;
            MessageManager.AddMessage(Message);
        }
    }
    public void DoEffect(Player p)
    {
        if (RemainingTime % Speed == 0 && RemainingTime > 0)
        {
            int heal = Math.Min(Power, p.MaxHP - p.CurrentHP);
            p.CurrentHP += heal;

            MessageManager.AddMessage("You recover " + heal + " HP!");
        }
    }
    public IStatusEffect Copy()
    {
        return new SelfHealEffect(d);
    }
}

