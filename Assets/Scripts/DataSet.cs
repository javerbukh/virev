using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class DataSet : MonoBehaviour {
    public Double limit_x_position_min;
    public Double limit_y_position_min;
    public Double limit_z_position_min;

    public Double limit_x_position_max;
    public Double limit_y_position_max;
    public Double limit_z_position_max;

    private Double file_x_position_min;
    private Double file_y_position_min;
    private Double file_z_position_min;

    private Double file_x_position_max;
    private Double file_y_position_max;
    private Double file_z_position_max;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CheckNewMax()
    {

    }
}
