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

    public string Prefix = "%";

    [Header("Description")]

    public string Description = "";

    public Color TextColor = Color.white;

    private float space = 0.01f;

    private Renderer barRenderer = null;

    private Vector3 initialPosition;

    private GameObject textObject;

    // Use this for initialization
    void Start()
    {
        barRenderer = gameObject.GetComponent<Renderer>();
        initialPosition = gameObject.transform.position;

        barRenderer.material.color = BarColor;

        textObject = new GameObject();
        //textObject.transform.SetParent(gameObject.transform);
        TextMesh textMesh = textObject.AddComponent<TextMesh>();
        textObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        textMesh.color = ValueColor;
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

        TextMesh textMesh = textObject.GetComponent<TextMesh>();
        textMesh.text = string.Format("{0}{1}", Value.ToString(), Prefix);
        MeshRenderer textRenderer = textObject.GetComponent<MeshRenderer>();

        if (Direction == ChartDirection.Horizontal)
        {
            gameObject.transform.position = new Vector3(initialPosition.x + barRenderer.bounds.size.x / 2, initialPosition.y, initialPosition.z);
            textObject.transform.position = new Vector3(initialPosition.x + barRenderer.bounds.size.x + space, initialPosition.y + (textRenderer.bounds.size.y / 2), initialPosition.z);
        }
        else
        {
            gameObject.transform.position = new Vector3(initialPosition.x, initialPosition.y + barRenderer.bounds.size.y / 2, initialPosition.z);
            textObject.transform.position = new Vector3(initialPosition.x + (textRenderer.bounds.size.x / 2), initialPosition.y + barRenderer.bounds.size.y + space, initialPosition.z);
        }
    }
}