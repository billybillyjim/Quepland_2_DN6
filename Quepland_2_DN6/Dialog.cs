using System;
using System.Collections.Generic;
using System.Linq;

public class Dialog
{ 
	public string ButtonText { get; set; } = "Unset";
	public string ResponseText { get; set; } = "Unset";

	public string ResponseWithParameter
    {
        get
        {
			return ResponseText + Parameter;
		}
    }
	public string ItemOnTalk { get; set; } = "None";
	public string Quest { get; set; } = "None";
	public string UnlockedFollower { get; set; } = "None";
	public string Parameter { get; set; } = "";
	public bool ConsumeRequiredItems { get; set; }
	public bool CompleteQuest { get; set; } = false;
	public int NewQuestProgressValue { get; set; } = -1;
	public List<Requirement> Requirements { get; set; } = new List<Requirement>();
	private bool HasStartedQuest;
	public int Depth { get; set; }
	public int MinDepth { get; set; } = -1;
	public int MaxDepth { get; set; } = -1;
	public int NewDepthOnTalk { get; set; }
	public string SetFlag { get; set; } = "None";
	public bool SetFlagValue { get; set; } = true;
	public string SetProgressFlag { get; set; } = "None";
	public bool SetProgressFlagValue { get; set; } = true;

	public bool HasRequirements()
	{
		if(Requirements.Count == 0)
        {
			return true;
        }
		foreach (Requirement r in Requirements)
		{
			if (r.IsMet() == false)
			{
				return false;
			}
		}
		if(UnlockedFollower != "None")
        {
            if (FollowerManager.Instance.GetFollowerByName(UnlockedFollower).IsUnlocked)
            {
				return false;
            }
        }
		return true;
	}
	public void Talk()
    {
		if (NPCManager.Instance.CustomDialogFunctions.TryGetValue(ResponseWithParameter, out Action a))
		{
			a.Invoke();
			DoQuestCheck();
			return;
		}
        if (ConsumeRequiredItems)
        {
			foreach(Requirement r in Requirements)
            {
				if(r.Item != "None")
                {
					GameItem i = ItemManager.Instance.GetItemByName(r.Item);
					if (Player.Instance.Inventory.GetNumberOfItem(i) < r.ItemAmount)
					{
						if(r.ItemAmount == 1)
                        {
							MessageManager.AddMessage("You need a " + i.Name + ".", "red");
						}
                        else
                        {
							MessageManager.AddMessage("You don't have enough " + i.GetPlural() + ".(Need " + r.ItemAmount + ")", "red");
						}
						
						return;
                    }
					if (Player.Instance.Inventory.GetNumberOfUnlockedItem(i) < r.ItemAmount)
					{
						if (r.ItemAmount == 1)
						{
							MessageManager.AddMessage("Your " + i.Name + " is locked.", "red");
						}
						else
						{
							MessageManager.AddMessage("You don't have enough unlocked" + i.GetPlural() + ".(Need " + r.ItemAmount + ")", "red");
						}

						return;
					}
				}
            }
			foreach (Requirement r in Requirements)
			{
				if (r.Item != "None")
				{
					Player.Instance.Inventory.RemoveItems(ItemManager.Instance.GetItemByName(r.Item), r.ItemAmount);

				}
			}
		}
		if (ItemOnTalk != "None")
		{
			if (Player.Instance.Inventory.AddItem(ItemManager.Instance.GetItemByName(ItemOnTalk).Copy()) == false)
			{
				MessageManager.AddMessage("Your inventory is full! Come back after you store something in your bank.", "red");
				return;
			}

		}
		if (UnlockedFollower != "None")
        {
			FollowerManager.Instance.GetFollowerByName(UnlockedFollower).IsUnlocked = true;
        }
		if(SetProgressFlag != "None")
        {
			GameState.GetFlagByName(SetProgressFlag).Completed = SetProgressFlagValue;
        }
		DoQuestCheck();
		MessageManager.AddMessage(ResponseText, "white", "Dialogue");
	}

	private void DoQuestCheck()
	{
		if (Quest != "None")
		{
			if(NewQuestProgressValue != -1)
			{
                if (NewQuestProgressValue == 1 && QuestManager.Instance.GetQuestByName(Quest).Progress == 0 && HasStartedQuest == false)
                {
                    HasStartedQuest = true;
                    MessageManager.AddMessage("You've started the quest " + Quest + ".", "#00ff00");
                }
                QuestManager.Instance.GetQuestByName(Quest).Progress = NewQuestProgressValue;
            }
			
			if (CompleteQuest)
			{
				QuestManager.Instance.GetQuestByName(Quest).Complete();

			}
			if (SetFlag != "None")
			{
				
				QuestManager.Instance.GetQuestByName(Quest).SetFlag(SetFlag, SetFlagValue);
			}
		}
	}

}
