using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnButtonClicked : EventCallbacks.Event<OnButtonClicked>
{
    public readonly string ButtonName;
    public OnButtonClicked(string _buttonName) : base("Event, that will be fired when a button is clicked.")
    {
        ButtonName = _buttonName;
        FireEvent(this);
    }
}
