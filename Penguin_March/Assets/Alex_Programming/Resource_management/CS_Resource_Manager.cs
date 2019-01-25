using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Resource_Manager : MonoBehaviour
{
    Resource_Containter resourceContainer;

    private void Start()
    {
        resourceContainer.resource1 = "Fish";
        resourceContainer.resource2 = "hats";
        resourceContainer.resource3 = "Bowties";
        resourceContainer.resource4 = "Neckties";

        resourceContainer.r1 = 0;
        resourceContainer.r2 = 0;
        resourceContainer.r3 = 0;
        resourceContainer.r4 = 0;

    }

    public void SetResourceValue(int resourceNo, int changeValue)
    {
        switch (resourceNo)
        {
            case 1:
                resourceContainer.r1 += changeValue;
                break;
            case 2:
                resourceContainer.r1 += changeValue;
                break;
            case 3:
                resourceContainer.r1 += changeValue;
                break;
            case 4:
                resourceContainer.r1 += changeValue;
                break;
        }
    }
}
