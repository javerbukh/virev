using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerScript : MonoBehaviour
{
    public GameObject SpherePrefab;

    private GameObject currentSphere;
    private TextMesh textObject;

    private void Start()
    {
        currentSphere = Instantiate(SpherePrefab, transform.position, transform.rotation);
        textObject = GameObject.Find("TextPopup").GetComponent<TextMesh>();
    }
    private void Update()
    {
        if (currentSphere)
        {
            currentSphere.transform.position = transform.position;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        // print("triggered!!");
        // print(other);
        //TextMesh textObject = GameObject.Find("TextPopup").GetComponent<TextMesh>();
        //textObject.text = "in";

        //GameObject cubeObject = GameObject.Find("Cube");
        //Vector3 cubePosition = cubeObject.transform.position;

        //textObject.transform.position = cubePosition + cubeObject.transform.up;

        if(other.tag == "datapoint")
        {
            print("in controller" + other);
            //TextMesh textObject = GameObject.Find("TextPopup").GetComponent<TextMesh>();
            string data_meta = "";
            foreach (string column in other.GetComponent<DataPointScript>().data.Keys)
            {
                string temp = column + ": " + other.GetComponent<DataPointScript>().data[column] + "\n";

                data_meta += temp;
            }
            textObject.text = data_meta;

        }

    }

    void OnCollisionEnter(Collision collision)
    {
        // print("controller trigger!!!!!!!!!");

        if (collision.gameObject.tag == "datapoint")
        {
            print("WOOOORKS");
        }
    }
}
