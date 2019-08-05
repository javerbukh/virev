using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTimeSeries : Data {

    // Tuple<float, Vector3>[] time_3d_data_point;
    Dictionary<float, Vector3> time_3d_data_point;

    public Color color;
    public float scale;

    public void Add(float time, Vector3 position)
    {
        if (!this.time_3d_data_point.ContainsKey(time)){
            this.time_3d_data_point.Add(time, position);
        }
    }

}
