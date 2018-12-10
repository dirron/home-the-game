using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelEventManager : MonoBehaviour {
    private Dictionary<string, Action> eventDictionary;
    private static LevelEventManager levelEventManager;

    public static LevelEventManager instance
    {
        get
        {
            if (!levelEventManager)
            {
                levelEventManager = FindObjectOfType(typeof(LevelEventManager)) as LevelEventManager;

                if (!levelEventManager)
                {
                    Debug.LogError("There must be an active LevelEventManager script on a GameObject");
                }
                else
                {
                    levelEventManager.init();
                }
            }

            return levelEventManager;
        }
    }

    void init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, Action>();
        }
    }

    public static void StartListening(string eventName, Action listener)
    {
        Action thisEvent;

        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            // add the listener to the existing event
            thisEvent += listener;

            // update dictionary entry
            instance.eventDictionary[eventName] = thisEvent;
        }
        else
        {
            // add listener to event
            thisEvent += listener;

            // add new dictionary entry
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, Action listener)
    {
        if (levelEventManager == null) { return; }

        Action thisEvent;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            // remove event from existing events
            thisEvent -= listener;

            // update dictionary entry
            instance.eventDictionary[eventName] = thisEvent;
        }
    }

    public static void TriggerEvent(string eventName)
    {
        Action thisEvent = null;

        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}
