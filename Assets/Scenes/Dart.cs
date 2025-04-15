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


        rg = gameObject.GetComponent < Rigidbody>();
        dirObj = GameObject.Find("DartThrowPoint");
    }
    //---------------------------------------------------------------------------------------------------------
    private void FixedUpdate()
    {
        if (isForceOK)
        {
            dartFrontCollider.enabled = true;
            StartCoroutine(InitDartDestroyVFX());
            GetComponent<Rigidbody>().isKinematic = false;
            isForceOK = false;
            isDartRotating = true;

        }
        //add force
        rg.AddForce(dirObj.transform.forward * (12f + 6f) * Time.deltaTime, ForceMode.VelocityChange);

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
            if (!isDartHitOnBoard)
            {
                Destroy(gameObject);
            }
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
