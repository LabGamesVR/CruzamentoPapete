using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResizeObserver : MonoBehaviour
{
    public UnityAction ResizeObserverAction;
    private void OnRectTransformDimensionsChange()
    {
        if(gameObject.activeSelf && ResizeObserverAction != null)
            ResizeObserverAction.Invoke();
    }
}
