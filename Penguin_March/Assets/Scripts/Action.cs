using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action {

    public string ActionName;
    public int penguinsAssigned;

    // Constructor
    public Action(string name)
    {
        ActionName = name;
    }

    public void AddPenguins(int num)
    {
        penguinsAssigned += num;
    }

    public void Reset()
    {
        penguinsAssigned = 0;
    }
}
