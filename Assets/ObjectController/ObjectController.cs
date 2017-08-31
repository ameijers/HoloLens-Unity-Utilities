using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public enum ControllerState
{
    Idle = 0,
    Rotate = 1,
    Resize = 2,
    Positioning = 3
}

public class ObjectController : MonoBehaviour
{
    [Header("Object")]
    public GameObject ObjectToControl = null;

    [Header("Lines")]
    public float LineWidth = 0.001f;

    public Color LineColor = Color.white;

    [Header("Controllers")]
    public Vector3 ControllerSize = new Vector3(.1f, .1f, .1f);

    public Shader lineShader = null;

    public int ControllerLayer = 16;

    public float OutboundOfObject = .1f;

    [Header("Rotation")]
    public float RotationSensitivity = 10.0f;

    [Header("Sizing")]
    public float ResizeSensitivity = .5f;

    public Vector3 MinimalSize = new Vector3(.1f, .1f, .1f);

    public Vector3 MaxiumSize = new Vector3(1f, 1f, 1f);

    [Header("Positioning")]
    public int PositionLayer = 0;

    public float PositionHitdistance = .25f;

    public float PositionDistance = 2.5f;

    /// <summary>
    /// Top-left-back
    /// Top-right-back
    /// Top-right-front
    /// Top-left-front
    /// Bottom-left-back
    /// Bottom-right-back
    /// Bottom-right-front
    /// Bottom-left-front
    /// Middle-left-back
    /// Middle-right-back
    /// Middle-right-front
    /// Middle-left-front
    /// </summary>
    private GameObject[] controllers = new GameObject[12];
    private string[] controllerActions = new string[12];

    private LineRenderer[] lines = new LineRenderer[6];

    private GameObject positioner = null;

    private Renderer objectRenderer = null;

    private GameObject focusedController = null;

    private GestureRecognizer navigationRecognizer = null;
    private Vector3 NavigationPosition = Vector3.zero;

    private ControllerState state = ControllerState.Idle;

    private Vector3 previousObjectPosition = Vector3.zero;
    private Quaternion previousObjectQuaterion = new Quaternion();
    private Vector3 originalObjectSize = Vector3.zero;

    // Use this for initialization
    void Start ()
    {
        objectRenderer = ObjectToControl.GetComponent<Renderer>();

        originalObjectSize = ObjectToControl.transform.localScale;

        for (int index = 0; index < controllers.Length; index++)
        {
            controllers[index] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            controllers[index].transform.SetParent(gameObject.transform);
            controllers[index].transform.localScale = ControllerSize;
            // after localScale is set than you can adjust the collider
            float scaleFactor = 10f;
            BoxCollider collider = controllers[index].GetComponent<BoxCollider>();
            collider.size = new Vector3(ControllerSize.x * 2f * scaleFactor, ControllerSize.y * 2f * scaleFactor, ControllerSize.z * 2f * scaleFactor);
            controllers[index].layer = ControllerLayer;

            if (index < 8)
            {
                controllerActions[index] = "resize";            }
            else
            {
                controllerActions[index] = "rotate";
            }
        }

        for (int index = 0; index < lines.Length; index++)
        {
            GameObject line = new GameObject();
            line.transform.SetParent(gameObject.transform);
            lines[index] = line.AddComponent<LineRenderer>();
        }

        positioner = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        positioner.transform.localScale = ControllerSize;
        positioner.transform.SetParent(gameObject.transform);
        positioner.layer = ControllerLayer;

        EnableNavigationGestures();

    }

    // Update is called once per frame
    void Update ()
    {
        switch (state)
        {
            case ControllerState.Idle:
                var headPosition = Camera.main.transform.position;
                var gazeDirection = Camera.main.transform.forward;

                RaycastHit hitInfo;
                if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, 1 << ControllerLayer, QueryTriggerInteraction.Collide))
                {
                    focusedController = hitInfo.collider.gameObject;

                    focusedController.transform.localScale = new Vector3(.15f, .15f, .15f);
                }
                else
                {
                    if (focusedController != null)
                    {
                        focusedController.transform.localScale = ControllerSize;
                        focusedController = null;
                    }
                }
                break;

            case ControllerState.Rotate:
                PerformRotation();
                break;

            case ControllerState.Resize:
                PerformResize();
                break;

            case ControllerState.Positioning:
                PerformPosition();
                break;
        }

        if (ObjectToControl != null && objectRenderer != null)
        {
            if (previousObjectPosition != ObjectToControl.transform.position)
            {
                previousObjectPosition = ObjectToControl.transform.position;

                UpdateControllers();
                UpdatePositioner();
               
                UpdateLines();
            }

            if (previousObjectQuaterion != ObjectToControl.transform.rotation)
            {
                previousObjectQuaterion = ObjectToControl.transform.rotation;

                // when the parent has rotation, the controllers already move with the parent
                // the lines do not move so they need to be redrawn
                UpdateLines();
            }
        }
    }

 

    private void ResetControllers()
    {
        foreach(GameObject controller in controllers)
        {
            controller.transform.localScale = ControllerSize;
        }
    }

    private void UpdatePositioner()
    {
        Vector3 size = objectRenderer.bounds.size;
        Vector3 position = objectRenderer.bounds.center; // the absolute center of the object

        positioner.transform.position = new Vector3(position.x, position.y + (size.y / 2) + OutboundOfObject * 2f, position.z);
    }

    private void UpdateControllers()
    {
        Vector3 size = objectRenderer.bounds.size;
        Vector3 position = objectRenderer.bounds.center; // the absolute center of the object

        float divX = (size.x / 2f) + OutboundOfObject;
        float divY = (size.y / 2f) + OutboundOfObject;
        float divZ = (size.z / 2f) + OutboundOfObject;

        controllers[0].transform.position = new Vector3(position.x - divX, position.y + divY, position.z + divZ);
        controllers[1].transform.position = new Vector3(position.x + divX, position.y + divY, position.z + divZ);
        controllers[2].transform.position = new Vector3(position.x + divX, position.y + divY, position.z - divZ);
        controllers[3].transform.position = new Vector3(position.x - divX, position.y + divY, position.z - divZ);

        controllers[4].transform.position = new Vector3(position.x - divX, position.y - divY, position.z + divZ);
        controllers[5].transform.position = new Vector3(position.x + divX, position.y - divY, position.z + divZ);
        controllers[6].transform.position = new Vector3(position.x + divX, position.y - divY, position.z - divZ);
        controllers[7].transform.position = new Vector3(position.x - divX, position.y - divY, position.z - divZ);

        controllers[8].transform.position = new Vector3(position.x - divX, position.y, position.z + divZ);
        controllers[9].transform.position = new Vector3(position.x + divX, position.y, position.z + divZ);
        controllers[10].transform.position = new Vector3(position.x + divX, position.y, position.z - divZ);
        controllers[11].transform.position = new Vector3(position.x - divX, position.y, position.z - divZ);

    }

    private void UpdateLines()
    {
        Vector3[] lineTop = new Vector3[5] {
                controllers[0].transform.position,
                controllers[1].transform.position,
                controllers[2].transform.position,
                controllers[3].transform.position,
                controllers[0].transform.position
            };

        Vector3[] lineBottom = new Vector3[5] {
                controllers[4].transform.position,
                controllers[5].transform.position,
                controllers[6].transform.position,
                controllers[7].transform.position,
                controllers[4].transform.position
            };

        Vector3[] lineLeftBack = new Vector3[2] {
                controllers[0].transform.position,
                controllers[4].transform.position
               };

        Vector3[] lineRightBack = new Vector3[2] {
                controllers[1].transform.position,
                controllers[5].transform.position
               };

        Vector3[] lineLeftFront = new Vector3[2] {
                controllers[2].transform.position,
                controllers[6].transform.position
               };

        Vector3[] lineRightFront = new Vector3[2] {
                controllers[3].transform.position,
                controllers[7].transform.position
               };

        foreach (LineRenderer line in lines)
        {
            line.startWidth = LineWidth;
            line.endWidth = LineWidth;
            line.startColor = LineColor;
            line.endColor = LineColor;

            // The shader is needed to change to the color specified
            if (lineShader != null)
            {
                line.material = new Material(lineShader);
            }
        }

        lines[0].positionCount = 5;
        lines[0].SetPositions(lineTop);
        lines[1].positionCount = 5;
        lines[1].SetPositions(lineBottom);
        lines[2].positionCount = 2;
        lines[2].SetPositions(lineLeftBack);
        lines[3].positionCount = 2;
        lines[3].SetPositions(lineRightBack);
        lines[4].positionCount = 2;
        lines[4].SetPositions(lineLeftFront);
        lines[5].positionCount = 2;
        lines[5].SetPositions(lineRightFront);

    }

    private string GetControllerAction(GameObject controller)
    {
        string action = "";

        if (controller == positioner)
        {
            return "position";
        }

        for(int index = 0; index < controllers.Length; index++)
        {
            if (controllers[index] == controller)
            {
                action = controllerActions[index];
                break;
            }
        }

        return action;
    }

    private void EnableNavigationGestures()
    {
        // Set up a GestureRecognizer to detect Select gestures.
        navigationRecognizer = new GestureRecognizer();
        navigationRecognizer.NavigationStartedEvent += (source, normalizedOffset, headRay) =>
        {
            if (state == ControllerState.Idle)
            {
                if (focusedController != null)
                {
                    string action = GetControllerAction(focusedController);

                    if (action == "rotate")
                    {
                        state = ControllerState.Rotate;
                    }
                    else if (action == "resize")
                    {
                        state = ControllerState.Resize;
                    }

                    ResetControllers();
                }

                NavigationPosition = normalizedOffset;
            }

        };
        navigationRecognizer.NavigationUpdatedEvent += (source, normalizedOffset, headRay) =>
        {
            NavigationPosition = normalizedOffset;
        };
        navigationRecognizer.NavigationCompletedEvent += (source, normalizedOffset, headRay) =>
        {
            state = ControllerState.Idle;
        };
        navigationRecognizer.TappedEvent += (source, tapCount, ray) =>
        {
            switch(state)
            {
                case ControllerState.Idle:
                    if (focusedController == positioner)
                    {
                        state = ControllerState.Positioning;
                    }
                    break;

                case ControllerState.Positioning:
                    state = ControllerState.Idle;
                    break;
            }

        };

        navigationRecognizer.StartCapturingGestures();
    }

    public void PerformRotation()
    {
        float rotationFactor = NavigationPosition.x * RotationSensitivity;

        ObjectToControl.transform.Rotate(new Vector3(0, -1 * rotationFactor, 0));

    }

    public void PerformPosition()
    {
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;

        if (PositionLayer == 0)
        {
            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f))
            {
                ObjectToControl.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z - PositionHitdistance);
            }
            else
            {
                ObjectToControl.transform.position = headPosition + (gazeDirection.normalized * PositionDistance);
            }
        }
        else
        {
            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, 1 << PositionLayer, QueryTriggerInteraction.Collide))
            {
                ObjectToControl.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z - PositionHitdistance);
            }
            else
            {
                ObjectToControl.transform.position = headPosition + (gazeDirection.normalized * PositionDistance);
            }
        }
    }

    public void PerformResize()
    {
        float resizeFactor = NavigationPosition.x * ResizeSensitivity;

        ObjectToControl.transform.localScale = originalObjectSize + new Vector3(-1 * resizeFactor, -1 * resizeFactor, -1 * resizeFactor);

        if (ObjectToControl.transform.localScale.x < MinimalSize.x || ObjectToControl.transform.localScale.y < MinimalSize.y || ObjectToControl.transform.localScale.z < MinimalSize.z)
        {
            ObjectToControl.transform.localScale = MinimalSize;
        }

        if (ObjectToControl.transform.localScale.x > MaxiumSize.x || ObjectToControl.transform.localScale.y > MaxiumSize.y || ObjectToControl.transform.localScale.z > MaxiumSize.z)
        {
            ObjectToControl.transform.localScale = MaxiumSize;
        }

        UpdateLines();
        
    }
}
