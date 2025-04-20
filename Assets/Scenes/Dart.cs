using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class Dart : MonoBehaviour
{
    private Rigidbody rg;
    private GameObject dirObj;
    public bool isForceOK = false;
    bool isDartRotating = false;
    bool isDartReadyToShoot = true;
    bool isDartHitOnBoard = false;

    ARSessionOrigin aRSession;
    GameObject ARcam;

    public Collider dartFrontCollider;

    void Start()
    {
        aRSession = GameObject.Find("AR Session").GetComponent<ARSessionOrigin>();
        ARcam = aRSession.transform.Find("AR Camera").gameObject;


        dirObj = GameObject.Find("DartThrowPoint");
    }
    void Awake()
    {
        rg = GetComponent<Rigidbody>();
    }

    //---------------------------------------------------------------------------------------------------------
    private void FixedUpdate()
    {
        if (rg == null)
        {
            Debug.LogError("El Rigidbody 'rg' es null");
            return;
        }
        if (isForceOK)
        {
            Debug.Log("Dardo disparado");
            dartFrontCollider.enabled = true;
            StartCoroutine(InitDartDestroyVFX());
            GetComponent<Rigidbody>().isKinematic = false;
            //rg.AddForce(dirObj.transform.forward * 18f, ForceMode.Impulse); // APLICA FUERZA AQUÍ
            Debug.DrawRay(transform.position, Camera.main.transform.forward * 5, Color.red, 2f); //dibuja el recorrido
            rg.AddForce(Camera.main.transform.forward * 18f, ForceMode.Impulse);

            isForceOK = false;
            isDartRotating = true;

        }
        //add force
        
        if (!isDartRotating && dirObj != null)
        {
            rg.AddForce(dirObj.transform.forward * 18f * Time.deltaTime, ForceMode.VelocityChange);
        }

        //Dart ready
        if (isDartReadyToShoot)
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * 20f);
        }

        //Dart ready
        if (isDartRotating)
        {
            isDartReadyToShoot = false;
            transform.Rotate(Vector3.forward * Time.deltaTime * 400f);
        }

        StartCoroutine(InitDartDestroyVFX());


    }
    private IEnumerator InitDartDestroyVFX()
    {
        yield return new WaitForSeconds(5f);
        if (!isDartHitOnBoard)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("dart_board"))
        {
            //Vibra
            Handheld.Vibrate();
            GetComponent<Rigidbody>().isKinematic = true;
            isDartRotating = false;

            //dardo hit diana
            isDartHitOnBoard = true;

        }
    }

}
