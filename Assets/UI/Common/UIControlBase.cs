using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class UIControlBase : MonoBehaviour, IButtonEvents
{
    [Header("Common")]
    public GameObject ButtonEnabled;
    public GameObject ButtonDisabled;
    public GameObject ButtonFocussed;
    //public GameObject ButtonClicked;
    public ButtonState State;

    public AudioSource FocusSound;
    public AudioSource ClickSound;

    public event ButtonClick OnButtonClick;
    public event ButtonGazeFocus OnButtonGazeFocus;
    public event ButtonStateChange OnButtonStateChange;

    protected GameObject buttonEnabledUI = null;
    protected GameObject buttonDisabledUI = null;
    protected GameObject buttonFocussedUI = null;

    private GestureRecognizer gestures = null;

    public virtual void OnButtonClicked()
    {
        if (OnButtonClick != null)
        {
            OnButtonClick();
        }
    }

    public virtual void OnButtonGazeFocussed()
    {
        if (OnButtonGazeFocus != null)
        {
            OnButtonGazeFocus();
        }
    }

    public virtual void OnButtonStateChanged(ButtonStateEventProperties properties)
    {
        if (OnButtonStateChange != null)
        {
            OnButtonStateChange(properties);
        }
    }

    protected virtual void OnInit()
    {
        LoadButtonUIElements();
        OnInitialiseGestures();
        SetState(State);
    }

    protected virtual void OnInitialiseGestures()
    {
        gestures = new GestureRecognizer();

        gestures.TappedEvent += (source, tapCount, ray) =>
        {
            if (IsState(ButtonState.Focussed))
            {
                if (ClickSound != null)
                {
                    ClickSound.Play();
                }

                // clickevent
                OnButtonClicked();         
            }
        };

        gestures.StartCapturingGestures();
    }

    protected virtual void LoadButtonUIElements()
    {
        if (ButtonEnabled != null)
        {
            buttonEnabledUI = Instantiate(ButtonEnabled, gameObject.transform);

            if (ButtonDisabled != null)
            {
                buttonDisabledUI = Instantiate(ButtonDisabled, gameObject.transform);
            }
            else
            {
                buttonDisabledUI = Instantiate(ButtonEnabled, gameObject.transform);
            }

            if (ButtonFocussed != null)
            {
                buttonFocussedUI = Instantiate(ButtonFocussed, gameObject.transform);
            }
            else
            {
                buttonFocussedUI = Instantiate(ButtonEnabled, gameObject.transform);
            }

            buttonEnabledUI.name = "ButtonEnabled";
            buttonDisabledUI.name = "ButtonDisabled";
            buttonFocussedUI.name = "ButtonFocussed";

        }
    }

    protected virtual void UpdateUI()
    {
        buttonFocussedUI.SetActive(false);
        buttonEnabledUI.SetActive(false);
        buttonDisabledUI.SetActive(false);

        if (IsState(ButtonState.Focussed))
        {
            buttonFocussedUI.SetActive(true);
        }
        else
        {
            if (IsState(ButtonState.Enabled))
            {
                buttonEnabledUI.SetActive(true);
            }
            else
            {
                buttonDisabledUI.SetActive(true);
            }
        }
    }

    protected virtual void OnUpdate()
    {
        if (IsState(ButtonState.Disabled))
        {
            return;
        }

        // Do a raycast into the world based on the user's
        // head position and orientation.
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            if (hitInfo.collider.gameObject == buttonEnabledUI)
            {
                if (!IsState(ButtonState.Focussed))
                {
                    // focussed
                    SetState(ButtonState.Focussed);

                    if (FocusSound != null)
                    {
                        FocusSound.Play();
                    }

                    UpdateUI();

                    OnButtonGazeFocussed();
                }
            }
        }
        else
        {
            if (IsState(ButtonState.Focussed))
            {
                // not focussed
                SetState(ButtonState.Enabled);

                UpdateUI();
            }
        }
    }

    public virtual void SetState(ButtonState state)
    {
        ButtonStateEventProperties properties = new ButtonStateEventProperties();
        properties.PreviousState = State;
        properties.State = state;
      
        State = state;

        OnButtonStateChanged(properties);

        UpdateUI();
    }

    public bool IsState(ButtonState state)
    {
        return State == state;
    }

    // Use this for initialization
    void Start()
    {
        OnInit();
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdate();
    }
}
