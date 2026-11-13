
using UnityEngine;
using System;

[CreateAssetMenu(
    fileName = "NewGameEvent",
    menuName = "CCT382/Game Event"
)]
public class GameEventSO : ScriptableObject
{
    public event Action OnEventRaised;

    public void Raise()
    {
        OnEventRaised?.Invoke();
    }
}
