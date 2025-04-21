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
        aRSession = GameObject.Find("AR Session Origin").GetComponent<ARSessionOrigin>();
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
        Debug.Log("FixedUpdate activo");
        if (isForceOK)
        {
            Debug.Log("Dardo disparado");
            dartFrontCollider.enabled = true;
            StartCoroutine(InitDartDestroyVFX());
            GetComponent<Rigidbody>().isKinematic = false;

            rg.AddForce(transform.forward * 6f, ForceMode.Impulse); // APLICA FUERZA AQUÍ
            
            isForceOK = false;
            isDartRotating = true;

        }
        //add force
        if (!isDartRotating)
        {
            rg.AddForce(dirObj.transform.forward * (12f + 6f) * Time.deltaTime, ForceMode.VelocityChange);
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


        IEnumerator InitDartDestroyVFX()
        {
            yield return new WaitForSeconds(5f);
            //if (!isDartHitOnBoard)
            //{
                Destroy(gameObject);
            //}
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("dart_board") && !isDartHitOnBoard)
        {
            Debug.Log("Dardo impactó");

            //Vibra
            Handheld.Vibrate();
            
            GetComponent<Rigidbody>().isKinematic = true;
            rg.velocity = Vector3.zero;
            rg.angularVelocity = Vector3.zero;
            
            isDartRotating = false;

            //dardo hit diana
            isDartHitOnBoard = true;

            // Lo hacemos hijo de la diana para que se mueva con ella
            transform.SetParent(other.transform);

        }
    }

}
