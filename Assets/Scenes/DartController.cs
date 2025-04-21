using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DartController : MonoBehaviour
{
    public GameObject DartPrefab;
    public Transform DartThrowPoint;
    ARSessionOrigin aRSession;
    GameObject ARCam;
    //Transform DartboardObj;
    private GameObject DartTemp;
    private Rigidbody rb;


    void Start()
    {
        aRSession = GameObject.FindWithTag("AR Session Origin").GetComponent<ARSessionOrigin>();
        ARCam = aRSession.transform.Find("AR Camera").gameObject;
    }

    void OnEnable()
    {
        PlaceObjectOnPlane.onPlacedObject += DartsInit;
    }

    void OnDisable()
    {
        PlaceObjectOnPlane.onPlacedObject -= DartsInit;
    }

    void Update()
    {
#if UNITY_EDITOR
        //bool tapped = Input.GetMouseButtonDown(0); // Detecta clic izquierdo del ratón
        //Vector2 touchPosition = Input.mousePosition;
        bool tapped = Input.GetMouseButtonDown(0);
#else
    //bool tapped = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began; // Para dispositivos táctiles
    //Vector2 touchPosition = Input.GetTouch(0).position;
    bool tapped = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
#endif

        if (tapped && DartTemp != null)
        {
            //if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            //{
            Debug.Log("Disparo");
            DartTemp.transform.parent = null; // Desvinculamos el dardo de la cámara
            Dart currentDartScript = DartTemp.GetComponent<Dart>();
            currentDartScript.isForceOK = true;

            // Preparamos nuevo dardo luego de un pequeño delay
            StartCoroutine(SpawnNextDart());

            //Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            //RaycastHit raycastHit;
            //if (Physics.Raycast(raycast, out raycastHit))
            //{
            //    if (raycastHit.collider.CompareTag("dart"))
            //    {
            //        //Disable back touch Collider from dart 
            //        raycastHit.collider.enabled = false;
            //        DartTemp.transform.parent = aRSession.transform;

            //        Dart currentDartScript = DartTemp.GetComponent<Dart>();
            //        currentDartScript.isForceOK = true;
            //    }
            //}
            //}
        }
        
    }


    void DartsInit()
    {

        StartCoroutine(WaitAndSpawnDart());
    }

    public IEnumerator WaitAndSpawnDart()
    {
        yield return new WaitForSeconds(0.1f);


        // Dirección hacia adelante, pero en plano horizontal
        Vector3 forward = ARCam.transform.forward;
        forward.y = 0; // Eliminar inclinación vertical
        forward.Normalize();

        // Rotación mirando al frente, manteniendo vertical (eje Y)
        Quaternion flatRotation = Quaternion.LookRotation(forward, Vector3.up) * Quaternion.Euler(10f, 0f, 0f); ;

        // Posición desplazada hacia adelante y abajo desde el punto de lanzamiento
        Vector3 spawnPosition = DartThrowPoint.position + forward * 0.6f + Vector3.down * 0.3f;

        // Instanciamos el dardo
        DartTemp = Instantiate(DartPrefab, spawnPosition, flatRotation);
        DartTemp.transform.parent = ARCam.transform;


        //ORIGINAL:
        //DartTemp = Instantiate(DartPrefab, DartThrowPoint.position, ARCam.transform.localRotation);
        //DartTemp.transform.parent = ARCam.transform;

        rb = DartTemp.GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    IEnumerator SpawnNextDart()
    {
        yield return new WaitForSeconds(1.5f); // Espera antes de crear el siguiente
        DartsInit(); // Reinstancia un nuevo dardo listo para disparar
    }
}

