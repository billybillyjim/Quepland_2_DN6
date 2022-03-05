using System;

public static class GreatTree
{
	public static int Progress { get; set; }
	public static int Height { get; set; } = 300;
	private static bool init = false;

	public static void Climb(int amount)
    {
		Progress += amount;
		GameState.GetFlagByName("Great Tree Climb").ProgressValue = Progress;
		if(Progress >= Height)
        {
			GameState.GetFlagByName("Great Tree Climb").Completed = true;

		}
    }
	public static void Initialize()
    {
        if (!init)
        {
			Progress = GameState.GetFlagByName("Great Tree Climb").ProgressValue;

		}
		init = true;
    }
}
