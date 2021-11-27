using System;

public class ExperienceTracker
{
	public TimeSpan TimeSinceTrackerStarted;
	public Skill Skill;
	public bool IsPaused;
	public long StartExperience;
	public long GoalExperience;
	public bool Show;
	public bool UpdateGoal = false;
	public double GetProgress()
    {
		if(GoalExperience == 0)
        {
			return Skill.Progress;
        }
		return ((double)(Skill.Experience - StartExperience) / GoalExperience) * 100d;
    }
}
