using System.Collections;
using System.Collections.Generic;


public class QuestFlag
{
    public string Name { get; set; } = "Unset";
    public bool Completed { get; set; }
    public bool CompletesQuest { get; set; }
    public string Description { get; set; } = "Unset";
    public int ProgressValue { get; set; }
}
