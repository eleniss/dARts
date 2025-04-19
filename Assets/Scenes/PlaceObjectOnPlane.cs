using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceObjectOnPlane : MonoBehaviour
{
    public GameObject objectToPlace;

    public GameObject placementIndicator;
    private Pose placementPose;
    private Transform placementTransform;
    private bool placementPoseIsValid = false;
    private bool isObjectPlaced = false;
    private TrackableId placedPlaneId = TrackableId.invalidId;

    ARRaycastManager m_RaycastManager;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    public static event Action onPlacedObject;
    void Awake()
    {
        if(TryGetComponent(out ARRaycastManager aRRaycast))
        m_RaycastManager = aRRaycast;
    }

    void Update()
    {
        if (!isObjectPlaced)
        {
            UpdatePlacementPosistion();
            UpdatePlacementIndicator();
#if UNITY_EDITOR
            bool tapped = Input.GetMouseButtonDown(0);
            Vector2 touchPosition = Input.mousePosition;
#else
    bool tapped = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
    Vector2 touchPosition = Input.touchCount > 0 ? Input.GetTouch(0).position : default;
#endif

            if (placementPoseIsValid && tapped)
            {
                Debug.Log("Colocando objeto desde " + (Application.isEditor ? "Editor" : "Móvil"));
                PlaceObject();
            }

            //if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            //{
            //    PlaceObject();
            //}
        }
    }

    private void UpdatePlacementPosistion()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f)); //no poner camera.current
        if (m_RaycastManager.Raycast(screenCenter, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            placementPoseIsValid = s_Hits.Count > 0;
            if (placementPoseIsValid)
            {
                placementPose = s_Hits[0].pose;
                placedPlaneId = s_Hits[0].trackableId;

                var planeManager = GetComponent<ARPlaneManager>();
                ARPlane arPlane = planeManager.GetPlane(placedPlaneId);
                placementTransform = arPlane.transform;
            }
        }
    } //end of UpdatePlacementIndicator

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementTransform.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void PlaceObject()
    {
        Debug.Log("Colocando diana en: " + placementPose.position);
        Instantiate(objectToPlace, placementPose.position, placementTransform.rotation);
        onPlacedObject?.Invoke();
        isObjectPlaced = true;
        placementIndicator.SetActive(false);
    }
}
