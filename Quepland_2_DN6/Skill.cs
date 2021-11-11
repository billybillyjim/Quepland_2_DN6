﻿using System;
using System.Text.Json.Serialization;

public class Skill
{
    public static long MaxEXP = 9000000000000000000;
    public string Name { get; set; }

    public bool GainedXPLastTick { get; set; }

    private ExperienceTracker? expTracker;
    public ExperienceTracker EXPTracker
    {
        get
        {
            if(expTracker == null) 
            {  
                expTracker = new ExperienceTracker();
                expTracker.TimeSinceTrackerStarted = TimeSpan.Zero;
                expTracker.Skill = this;
            }
            return expTracker;
        }
    }

    public int Level { get; set; }

    public void ResetExperience()
    {
        _experience = 0;
    }
    public void GainExperience(long amount)
    {
        if (!EXPTracker.Show)
        {
            EXPTracker.StartExperience = _experience;
            EXPTracker.TimeSinceTrackerStarted = TimeSpan.Zero;
        }
        if(amount > 0)
        {
            if(amount + _experience > _experience && amount + _experience < MaxEXP)
            {
                _experience += amount;
            }
        }
        EXPTracker.Show = true;
    }
    
    
    private long _experience;



    public long Experience {
        get { return _experience; }     
    }
    public string Description { get; set; }
    public int Boost { get; set; }
    public double Progress
    {
        get
        {
            double expLastLevel = GetExperienceRequired(Level - 1);
            double expToLevel = GetExperienceRequired(Level) - expLastLevel;
            double expProgress = _experience - expLastLevel;

            return ((expProgress / expToLevel) * 100);
            
        }
    }
    public static double GetExperienceRequired(long level)
    {
        double exp = 0;

        for (int i = 0; i < level; i++)
        {
            exp += (100.0d * Math.Pow(1.1, i));
        }
        return exp;
    }
    /// <summary>
    /// Returns a skill's level including boost and bed boost. Use GetSkillLevelUnboosted() for the real level.
    /// </summary>
    /// <returns></returns>
    public int GetSkillLevel()
    {
        return Level + Boost;
    }
    public int GetSkillLevelUnboosted()
    {
        return Level;
    }
    public void SetSkillLevel(int level)
    {
        Level = level;
    }
    public override string ToString()
    {
        return Name;
    }
    public void LoadExperience(long amount)
    {
        //Console.WriteLine("Loading EXP for " + Name + ":" + amount);
        if (amount < 0)
        {
            return;
        }
        Level = 1;
        _experience = 0;
        _experience += amount;

        while(Experience >= (long)Skill.GetExperienceRequired(GetSkillLevelUnboosted()))
        {

            if(Level > 350)
            {
                break;
            }
            Level++;
        }
    }
}
