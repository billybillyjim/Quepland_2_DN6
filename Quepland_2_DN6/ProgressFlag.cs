using System.Collections;
using System.Collections.Generic;


public class ProgressFlag
{
    public string Name { get; set; } = "Unset";
    public bool Completed { get; set; }
    public string Description { get; set; } = "Unset";
    public int ProgressValue { get; set; }
    public string ProgressData { get; set; } = "";

    public ProgressFlag() { }
    public ProgressFlag(string name, string description, int value = 0, string data = "")
    {
        Name = name;
        Description = description;
        ProgressValue = value;
        ProgressData = data;
    }
}
