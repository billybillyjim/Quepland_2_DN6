﻿using System;

public class QuestSaveData
{
	public int ID { get; set; }
	public int Progress { get; set; }
	public bool IsCompleted { get; set; }
	public List<QuestFlag> Flags { get; set; } = new List<QuestFlag>();
}
