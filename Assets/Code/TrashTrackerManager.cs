using System.Collections;
using Fusion;

public class TrashTrackerManager : TrackableManager<Trash>
{
    private bool allItemsCleared;
    private int numOfItemsDestroyed;

    public override bool AllItemsCleared()
    {
        return allItemsCleared;
    }

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < trackedItems.Count; i++)
        {
            trackedItems[i].OnItemDestroyed += HandleItemDestroyed;
        }
    }

    protected virtual void HandleItemDestroyed()
    {
        StartCoroutine(CleanUpDelay());
    }

    private IEnumerator CleanUpDelay()
    {
        yield return null;
        trackedItems.RemoveAll(t => t == null);
        CheckTrashes();
    }

    private void CheckTrashes()
    {
        if (numOfItemsDestroyed >= trackedItems.Count)
        {
            allItemsCleared = true;
        }
        else
        {
            numOfItemsDestroyed++;
        }
    }
}