using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphScript : MonoBehaviour {

    public float graph_size = 20.0f;
    public float object_scale = 0.1f;

    //public float[] mins;
    //public float[] maxes;

    public Dictionary<string, float> maxes = new Dictionary<string, float>()
    {
        { "x", 0.0f },
        { "y", 0.0f },
        { "z", 0.0f }
    };

    public Dictionary<string, float> mins = new Dictionary<string, float>()
    {
        { "x", float.MaxValue },
        { "y", float.MaxValue },
        { "z", float.MaxValue }
    };

    // A list of Data objects

    public int num_data = 0;
    public List<GameObject> dataCollection;

    // Use this for initialization
    void Start()
    {
        dataCollection = new List<GameObject>();
    }

    public void AddData(GameObject data)
    {
        dataCollection.Add(data);
        num_data++;
    }

    public void ChangeScale(float scale)
    {
        float pre_scaling_size = object_scale / scale;
        foreach (GameObject data in dataCollection)
        {
            data.transform.localScale *= scale;
        }
    }
}
