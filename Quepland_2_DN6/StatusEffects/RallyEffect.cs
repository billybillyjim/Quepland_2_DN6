using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class RallyEffect : IStatusEffect
{
    public string Name { get; set; } = "Rally";
    public int Duration { get; set; }
    public int Speed { get; set; }
    public int Power { get; set; }
    public int RemainingTime { get; set; }
    public bool OnProc { get; set; } = false;
    public string CustomData { get; set; }
    public double ProcOdds { get; set; }
    private StatusEffectData d;
    public string Message { get; set; }
    public bool SelfInflicted { get; set; }
    public RallyEffect(StatusEffectData data)
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
        return "The lower your HP, the more damage you do.";
    }
    public void DoEffect(Monster m)
    {

    }
    public void DoEffect(Player p)
    {

    }

    public IStatusEffect Copy()
    {
        return new RallyEffect(d);
    }
}

