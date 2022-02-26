using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class Skill
{
    public static long MaxEXP = 9000000000000000000;
    public static int MaxLevel = 350;
    [JsonProperty]
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
    [JsonProperty]
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
            _experience = Math.Min(_experience, MaxEXP);
        }
        EXPTracker.Show = true;
    }

    [JsonProperty]
    private long _experience;



    public long Experience {
        get { return _experience; }     
    }
    [JsonProperty]
    public string Description { get; set; }
    public int Boost { get; set; }
    public double Progress
    {
        get
        {
            if(Level >= MaxLevel)
            {
                return 100;
            }
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
        if(exp < 0)
        {
            exp = 0;
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
        if(level > 350)
        {
            level = 350;
        }
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

            if(Level >= MaxLevel)
            {
                break;
            }
            Level++;
        }
    }
}
