using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPointScript : MonoBehaviour
{

    public Dictionary<string, float> data = new Dictionary<string, float>();

    private void OnTriggerEnter(Collider other)
    {

        //if (!other.gameObject.GetComponent<DataPointScript>() && !other.gameObject.GetComponent<GraphScript>())
        //{
        //    print("!!!!!!!!!!!!Triggered!!!!!!!!!!!!!!!!!!!!");
        //    print(other);
        //}
        //else
        //{
        //    print("hit data point");
        //}

        //if (GetComponent<Collider>().GetType() == typeof(SphereCollider) && !other.gameObject.GetComponent<DataPointScript>())
        //{
        //    print("WORKKKSKKSKS");
        //    print(other);
        //}

        if (other.tag == "controller")
        {
            print("WOOOORKS");
        }
        else if (other.tag == "spherepoint")
        {
            print("spherepoint");
        }
        else
        {
            print("not controller: " + other);

        }
        //else if(other.tag != "datapoint")
        //{
        //    print("huuuuuuh?");
        //}

    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "controller")
        {
            print("data point trigger!!!!!!!!!!!!!!!!!!!!!!");

        }
        
    }

    public void AddDataRow(string column, float element)
    {
        if (data.ContainsKey(column))
        {
            data[column] = element;
        }
        else
        {
            data.Add(column, element);

        }
    }

}
