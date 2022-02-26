using System;
using System.Collections.Generic;

public class DPSCalc
{

	public static int NumOfBattles = 100;
	public List<Monster> Opponents = new List<Monster>();
    public int TotalTicksTaken = 0;
    public double AverageKillTime = 0;
    public int TotalKills;
    public int TotalDeaths;

    public int TotalTicksTakenSecond = 0;
    public double AverageKillTimeSeconds = 0;
    public int TotalKillsSecond;
    public int TotalDeathsSecond;
    public GameItem? CurrentFood;
	public void CalculateDPS()
    {
        TotalTicksTaken = 0;
        TotalKills = 0;
        TotalDeaths = 0;
        LootTracker.Instance.TrackLoot = true;
		for(int i = 0; i < NumOfBattles; i++)
        {
            foreach(Monster o in Opponents)
            {
                o.CurrentHP = o.HP;
            }
            BattleManager.Instance.StartBattle(Opponents);
            while (BattleManager.Instance.BattleHasEnded == false)
            {
                BattleManager.Instance.DoBattle();
                if(CurrentFood != null)
                {
                    if (CurrentFood.FoodInfo.HealSpeed % TotalTicksTaken == 0)
                    {
                        Player.Instance.CurrentHP += CurrentFood.FoodInfo.HealAmount;
                    }
                }
                TotalTicksTaken++;
            }
            if (BattleManager.Instance.AllOpponentsDefeated())
            {
                TotalKills++;
            }
            else
            {
                TotalDeaths++;
            }
        }
        if(NumOfBattles == 0)
        {
            AverageKillTime = 0;
            return;
        }
        AverageKillTime = (double)TotalTicksTaken / NumOfBattles;
    }
}
