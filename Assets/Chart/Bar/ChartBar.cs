using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChartDirection
{
    Horizontal = 0,
    Vertical = 1
}

public class ChartBar : MonoBehaviour
{
    [Header("Configuration")]
    public float MinimumValue = 0f;

    public float MaximumValue = 100f;

    public ChartDirection Direction = ChartDirection.Horizontal;

    public Color BarColor = Color.white;

    public Vector3 Size = new Vector3(.1f, .02f, .005f);

    [Header("Value")]
    public float Value = 30f;

    public Color ValueColor = Color.white;

    public int ValueFontSize = 100;

    public string Prefix = "%";

    [Header("Description")]

    public string Description = "";

    public Color TextColor = Color.white;

    public int TextFontSize = 100;

    private float space = 0.01f;

    private Renderer barRenderer = null;

    private Vector3 initialPosition;

    private GameObject textValueObject;

    private GameObject textDescriptionObject;

    // Use this for initialization
    void Start()
    {
        barRenderer = gameObject.GetComponent<Renderer>();
        initialPosition = gameObject.transform.position;

        barRenderer.material.color = BarColor;

        textValueObject = new GameObject();
        //textObject.transform.SetParent(gameObject.transform);
        TextMesh textMesh = textValueObject.AddComponent<TextMesh>();
        textValueObject.transform.localScale = new Vector3(1f, 1f, 1f);
        textMesh.color = ValueColor;
        textMesh.characterSize = .001f;
        textMesh.fontSize = ValueFontSize;

        textDescriptionObject = new GameObject();
        TextMesh textDescription = textDescriptionObject.AddComponent<TextMesh>();
        textDescriptionObject.transform.localScale = new Vector3(1f, 1f, 1f);
        textDescription.color = TextColor;
        textDescription.characterSize = .001f;
        textDescription.fontSize = TextFontSize;
        if (Direction == ChartDirection.Vertical)
        {
            textDescriptionObject.transform.Rotate(0, 0, 90);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateValue();
    }

    public void UpdateValue()
    {
        // standard cube is 1m x 1m x 1m

        Vector3 scale = new Vector3(Size.x, Size.y, Size.z);

        if (Direction == ChartDirection.Horizontal)
        {
            float factor = Size.x / (MaximumValue - MinimumValue);
            scale.x = Value * factor;
        }
        else
        {
            float factor = Size.y / (MaximumValue - MinimumValue);
            scale.y = Value * factor;
        }

        gameObject.transform.localScale = scale;

        TextMesh textMesh = textValueObject.GetComponent<TextMesh>();
        textMesh.text = string.Format("{0}{1}", Value.ToString(), Prefix);
        MeshRenderer textRenderer = textValueObject.GetComponent<MeshRenderer>();

        TextMesh textDescriptionMesh = textDescriptionObject.GetComponent<TextMesh>();
        textDescriptionMesh.text = Description;
        MeshRenderer textDescriptionRenderer = textDescriptionObject.GetComponent<MeshRenderer>();

        if (Direction == ChartDirection.Horizontal)
        {
            gameObject.transform.position = new Vector3(initialPosition.x + barRenderer.bounds.size.x / 2, initialPosition.y, initialPosition.z);
            textValueObject.transform.position = new Vector3(initialPosition.x + barRenderer.bounds.size.x + space, initialPosition.y + (textRenderer.bounds.size.y / 2), initialPosition.z);
            textDescriptionObject.transform.position = new Vector3(initialPosition.x, initialPosition.y + barRenderer.bounds.size.y + (textDescriptionRenderer.bounds.size.y), initialPosition.z);
        }
        else
        {
            gameObject.transform.position = new Vector3(initialPosition.x, initialPosition.y + barRenderer.bounds.size.y / 2, initialPosition.z);
            textValueObject.transform.position = new Vector3(initialPosition.x - textRenderer.bounds.size.x / 2f, initialPosition.y + barRenderer.bounds.size.y + textRenderer.bounds.size.y /2 + space, initialPosition.z);
            textDescriptionObject.transform.position = new Vector3(initialPosition.x - barRenderer.bounds.size.x - (textDescriptionRenderer.bounds.size.x / 2), initialPosition.y, initialPosition.z);
        }
    }
}