using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ButtonClick();
public delegate void ButtonGazeFocus();
public delegate void ButtonStateChange(ButtonStateEventProperties properties);

public interface IButtonEvents
{
    event ButtonClick OnButtonClick;

    event ButtonGazeFocus OnButtonGazeFocus;

    event ButtonStateChange OnButtonStateChange;

    void OnButtonClicked();

    void OnButtonGazeFocussed();

    void OnButtonStateChanged(ButtonStateEventProperties properties);

}
