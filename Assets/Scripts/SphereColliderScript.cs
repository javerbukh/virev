using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereColliderScript : MonoBehaviour
{

    //private void OnTriggerEnter(Collider other)
    //{
    //    print("sphere prefab collide");
    //}

    //void OnCollisionEnter(Collision collision)
    //{
    //    // print("controller trigger!!!!!!!!!");

    //    if (collision.gameObject.tag == "datapoint")
    //    {
    //        print("WOOOORKS");
    //    }
    //}
    private TextMesh textObject;


    private void Awake()
    {
        textObject = GameObject.Find("TextPopup").GetComponent<TextMesh>();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "datapoint")
        {
            print("in sphere: " + other);
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

}
