using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventsContainer : MonoBehaviour
{
    Action OnAnimationEvent;
    Action OnAnimationEvent2;
    public void LaunchAnimationEvent()
    {
        OnAnimationEvent?.Invoke();
    }
    public void LaunchAnimationEvent2()
    {
        OnAnimationEvent2?.Invoke();
    }

    public void SetEvent(Action animEvent)
    {
        OnAnimationEvent = animEvent;
    }
    public void SetEvent2(Action animEvent2)
    {
        OnAnimationEvent2 = animEvent2;
    }
}
