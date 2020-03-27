using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSeriesGraphScript : MonoBehaviour {

    private static int GRAPHSIZE = 5;
    private static float OBJECTSCALE = 0.1f;

    // A list of Data objects
    public List<GameObject> dataCollection;

    // Use this for initialization
    void Start () {
        dataCollection = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddData(GameObject data)
    {
        dataCollection.Add(data);
    }
}
