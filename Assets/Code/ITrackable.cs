using System;
using UnityEngine;

public interface ITrackable
{
    public Action OnItemDestroyed { get; set; }
    public Action OnProgressCompleted { get; set; }
}