using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : UIControlBase
{
    [Header("Button")]
    public GameObject Target;
    public string TargetMethod;
    public string EnabledText;
    public string DisabledText;
    public string FocussedText;

    protected override void OnInit()
    {
        base.OnInit();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    protected override void UpdateUI()
    {
        base.UpdateUI();

        TextMesh textMesh = buttonEnabledUI.GetComponentInChildren<TextMesh>();
        if (textMesh != null)
        {
            textMesh.text = EnabledText;
        }

        textMesh = buttonDisabledUI.GetComponentInChildren<TextMesh>();
        if (textMesh != null)
        {
            textMesh.text = DisabledText;
        }

        textMesh = buttonFocussedUI.GetComponentInChildren<TextMesh>();
        if (textMesh != null)
        {
            textMesh.text = FocussedText;
        }
    }

    protected override void LoadButtonUIElements()
    {
        base.LoadButtonUIElements();
    }

    public virtual object MessageValue
    {
        get
        {
            return this as UIControlBase;
        }
    }

    public override void OnButtonClicked()
    {
        base.OnButtonClicked();

        if (Target != null && TargetMethod != null)
        {
            Target.SendMessage(TargetMethod, MessageValue);
        }
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
