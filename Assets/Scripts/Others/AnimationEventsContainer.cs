using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventsContainer : MonoBehaviour
{
    Action OnAnimationEvent;
    public void LaunchAnimationEvent()
    {
        OnAnimationEvent?.Invoke();
    }

    public void SetEvent(Action animEvent)
    {
        OnAnimationEvent = animEvent;
    }
}
