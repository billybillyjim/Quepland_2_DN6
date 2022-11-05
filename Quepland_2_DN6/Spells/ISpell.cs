﻿
public interface ISpell
{
    /// <summary>
    /// The name of the spell.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// What the spell does. Appears on the spell screen.
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Used by Cast to determine how strong the spell is at what it does. Allows for different power levels of a similar effect.
    /// </summary>
    public int Power { get; set; }
    /// <summary>
    /// The message sent when the effect is first applied.
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// How long the spell should last, in game ticks.
    /// </summary>
    public int Duration { get; set; }
    /// <summary>
    /// Time until spell expires, in seconds.
    /// </summary>
    public int TimeRemaining { get; set; }
    public string Target { get; set; }
    public void Cast() { }
    public void Cast(Monster m) { }
    public void Cast(Player player) { }
    public void Cast(Inventory inventory) { }

    public void Tick() { }
    public void Tick(Monster m) { }
    public void Tick(Player player) { }
    public void Tick(Inventory inventory) { }
    public ISpell Copy();

}

