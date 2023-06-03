using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class HatchEffect : IStatusEffect
{
    public string Name { get; set; } = "Hatch";
    public int Duration { get; set; }
    public int Speed { get; set; }
    public int Power { get; set; }
    public int RemainingTime { get; set; }
    public string CustomData { get; set; }
    public bool OnProc { get; set; } = true;
    public double ProcOdds { get; set; }
    public bool SelfInflicted { get; set; }
    private StatusEffectData d;
    public string Message { get; set; }
    public HatchEffect(StatusEffectData data)
    {
        Name = data.Name;
        Duration = data.Duration;
        Speed = data.Speed;
        ProcOdds = data.ProcOdds;
        Power = data.Power;
        Message = data.Message;
        RemainingTime = data.Duration;
        CustomData = data.CustomData;
        SelfInflicted = data.SelfInflicted; 
        d = data;
    }
    public string GetDescription()
    {
        return "Hatches a chicken from " + CustomData;
    }
    public void DoEffect(Monster m)
    {
        MessageManager.AddMessage(Message);
        BattleManager.Instance.RemoveOpponentMidBattle(BattleManager.Instance.GetMonsterByName(CustomData.Split(":")[0]));
        BattleManager.Instance.SpawnOpponentMidBattle(BattleManager.Instance.GetMonsterByName(CustomData.Split(":")[1]));
        RemainingTime = 0;
    }
    public void DoEffect(Player p)
    {
        BattleManager.Instance.AddAlly(BattleManager.Instance.GetMonsterByName(CustomData));
        RemainingTime = 0;

    }
    public IStatusEffect Copy()
    {
        return new HatchEffect(d);
    }
}

