using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Transactions;

public class Quest
{
	public string Name { get; set; }
	private int _progress;
	public int Progress { get { return _progress; } 
		set {
			_progress = value;
			if (_progress >= ProgressToComplete) { Complete(); } 
		} 
	}
	public int ProgressToComplete { get; set; }
	public List<Requirement> Requirements { get; set; } = new List<Requirement>();
	public List<Reward> Rewards { get; set; } = new List<Reward>();
	public List<QuestFlag> Flags { get; set; } = new List<QuestFlag>();
	public string Description { get; set; } = "Unset";
	public string CompletionText { get; set; } = "Unset";
	public bool IsComplete { get; set; }
	public int ID { get; set; }
	public List<string> ProgressStrings { get; set; } = new List<string>();

	public void Complete()
    {
		if(IsComplete == false)
        {
			IsComplete = true;
			foreach (Reward reward in Rewards)
			{
				reward.Award();
				MessageManager.AddMessage("You earned " + reward.ToString());
			}
			MessageManager.AddMessage(CompletionText);
		}
        else
        {
			Console.WriteLine("Game attempted to complete Quest:" + Name + " more than once. Progress:" + Progress);
        }
    }
	public QuestSaveData GetSaveData()
    {
		return new QuestSaveData { ID = ID, IsCompleted = IsComplete, Progress = Progress, Flags = Flags };
    }
	public void LoadFromSave(QuestSaveData data)
    {
		IsComplete = data.IsCompleted;
		_progress = data.Progress;
		if(data.Flags.Count > 0)
        {
			Flags = data.Flags;
		}
    }
	public string GetProgressString()
    {
		if(ProgressStrings.Count == 0)
        {
			return "The progress log for this quest hasn't been completed yet.";
        }
		if(Progress >= ProgressStrings.Count)
        {
			return ProgressStrings[ProgressStrings.Count - 1];
        }
		return ProgressStrings[Progress];
    }
	public void SetFlag(string name)
    {
		SetFlag(name, true);
    }
	public void SetFlag(string name, bool value)
	{

		QuestFlag? f = Flags.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
		if (f == null)
		{
			Console.WriteLine("No Flag with name:" + name + " exists");
			return;
		}
		f.Completed = value;
	}
	public bool CheckFlag(string name)
	{
		QuestFlag? f = Flags.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
		if (f == null)
		{
			Console.WriteLine("No Flag with name:" + name + " exists");
			return false;
		}
		return f.Completed;
	}
}
