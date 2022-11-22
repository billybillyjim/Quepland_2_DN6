using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class StatusEffectData
{
	public string Name { get; set; }
	/// <summary>
	/// The duration of the effect in game ticks.
	/// </summary>
	public int Duration { get; set; }
	/// <summary>
	/// The number of game ticks between effects.
	/// </summary>
	public int Speed { get; set; }
	/// <summary>
	/// Remaining time in game ticks.
	/// </summary>
	public int RemainingTime { get; set; }
	/// <summary>
	/// The odds of the effect being attached to the player or monster. Ranges from 0-1.
	/// </summary>
	public double ProcOdds { get; set; }
	/// <summary>
	/// Describes the amount of damage/stun/reduction/etc. the effect causes.
	/// </summary>
	public int Power { get; set; }
	/// <summary>
	/// The message sent when the effect is first applied.
	/// </summary>
	public string Message { get; set; } = "";
	/// <summary>
	/// Any custom text for use in the effect.
	/// </summary>
	public string CustomData { get; set; } = "";
	public bool SelfInflicted { get; set; }
    public bool OnProc { get; set; } = false;
}

