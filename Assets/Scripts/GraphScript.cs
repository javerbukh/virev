using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphScript : MonoBehaviour {

    public float scale;
    public string units;

    private float DEFAULT_SCALE = 10f;
    private string DEFAULT_UNITS = "units";

    GraphScript()
    {
        this.scale = DEFAULT_SCALE;
        this.units = DEFAULT_UNITS;
    }

    public string get_units()
    {
        return this.units;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
