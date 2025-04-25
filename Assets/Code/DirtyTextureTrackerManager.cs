using System;
using UnityEngine;

public class DirtyTextureTrackerManager : TrackableManager<TextureCleaning>
{
    private int numOfItemsCleaned;
    private bool allItemsAreCleaned;

    protected override void Awake()
    {
        base.Awake();
        if (trackedItems.Count > 0)
        {
            for (int i = 0; i < trackedItems.Count; i++)
            {
                trackedItems[i].OnProgressCompleted += HandleProgressCompletion;
            }
        }
    }

    public override bool AllItemsCleared()
    {
        return allItemsAreCleaned;
    }

    private void HandleProgressCompletion()
    {
        numOfItemsCleaned++;
        if (numOfItemsCleaned >= trackedItems.Count)
        {
            allItemsAreCleaned = true;
        }
    }
}