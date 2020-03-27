using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasPointer : MonoBehaviour
{
    public float defaultLength = 3.0f;

    public EventSystem eventSystem = null;
    public StandaloneInputModule inputModule = null;
    public GameObject SpherePrefab;

    private LineRenderer lineRenderer = null;

    private GameObject currentSphere;
    private TextMesh textObject;


    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        currentSphere = Instantiate(SpherePrefab, transform.position, transform.rotation);
        // textObject = GameObject.Find("TextPopup").GetComponent<TextMesh>();
    }

    private void Update()
    {
            UpdateLength();
    }

    private void UpdateLength()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, GetEnd());
    }

    private Vector3 GetEnd()
    {
        float distance = GetCanvasDistance();
        Vector3 endPosition = CalculateEnd(defaultLength);

        if(distance != 0.0f)
        {
            endPosition = CalculateEnd(distance);
        }

        if (currentSphere)
        {
            currentSphere.transform.position = endPosition;
        }

        return endPosition;
    }

    private float GetCanvasDistance()
    {
        // Get data
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = inputModule.inputOverride.mousePosition;

        // Raycast using data
        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(eventData, results);

        // Get closest
        RaycastResult closestResult = FindFirstRaycast(results);
        float distance = closestResult.distance;

        // Clamp
        distance = Mathf.Clamp(distance, 0.0f, defaultLength);
        return distance;
    }

    private RaycastResult FindFirstRaycast(List<RaycastResult> results)
    {
        foreach(RaycastResult result in results)
        {
            if (!result.gameObject)
            {
                continue;
            }
            return result;
        }

        return new RaycastResult();
    }

    private Vector3 CalculateEnd(float length)
    {
        return transform.position + (transform.forward * length);
    }

}
