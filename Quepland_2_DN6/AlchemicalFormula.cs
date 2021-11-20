using System;

public class AlchemicalFormula
{
	public GameItem InputMetal { get; set; }
	public double LocationMultiplier { get; set; }
	public GameItem Element { get; set; }
	public double ActionMultiplier { get; set; }
	public string ActionString { get; set; }

	public AlchemicalFormula() { }
	public AlchemicalFormula(AlchemicalFormula data)
    {
		InputMetal = data.InputMetal;
		LocationMultiplier = data.LocationMultiplier;
		Element = data.Element;
		ActionMultiplier = data.ActionMultiplier;
		ActionString = data.ActionString;
    }
}
