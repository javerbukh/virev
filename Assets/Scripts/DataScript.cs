using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataScript : MonoBehaviour {

    public float[] mins;
    public float[] maxes;

    public int file_row_count;
    public int file_col_count;

    public string input_file;

    public Dictionary<string, List<float>> data = new Dictionary<string, List<float>>();

    public bool contains_time = false;

    public int x_col_index = 0;
    public int y_col_index = 1;
    public int z_col_index = 2;
    public int time_col_index = -1;
    public int redshift_col_index = -1;
    public int radius_val_index = -1;
    public int lum_val_index = -1;

    public Color color;

    public void SetInputFile(string file)
    {
        input_file = file;
    }

    public void AddDataColumn(string column)
    {
        print(column);
        if (!(data.ContainsKey(column)))
        {
            List<float> data_row = new List<float>();
            data.Add(column, data_row);
            if (column == "time")
            {
                contains_time = true;
            }
        }
    }

    public void FindXYZIndices()
    {
        int index = 0;
        foreach (string column in data.Keys)
        {
            switch (column.ToLower())
            {
                case "x":
                    x_col_index = index;
                    break;
                case "y":
                    y_col_index = index;
                    break;
                case "z":
                    z_col_index = index;
                    break;
                case "time":
                    time_col_index = index;
                    break;
                case "redshift":
                    redshift_col_index = index;
                    break;
                case "radius_val":
                    radius_val_index = index;
                    break;
                case "lum_val":
                    lum_val_index = index;
                    break;
            }
            index++;
        }
    }

    public void AddDataRow(string column, float element)
    {
        if (data.ContainsKey(column))
        {
            data[column].Add(element);
        }
    }

    public void PrintAllData()
    {
        print("in PrintAllData");
        foreach (KeyValuePair<string, List<float>> item in data)
        {
            // Console.WriteLine("Key: {0}, Value: {1}", item.Key, item.Value);
            print(item);
            int counter = 0;
            foreach(float value in item.Value)
            {
                counter++;
            }
            print(counter);
        }
    }

    public void GetMaxesAndMins()
    {
        mins = new float[file_col_count];
        maxes = new float[file_col_count];

        for (int i = 0; i < mins.Length; i++)
        {
            mins[i] = float.MaxValue;
        }

        int col_counter = 0;
        foreach (string column in data.Keys)
        {
            for(int index = 0; index < file_row_count; index++)
            {
                if (data[column][index] < mins[col_counter])
                {
                    mins[col_counter] = data[column][index];
                }
                if (data[column][index] > maxes[col_counter])
                {
                    maxes[col_counter] = data[column][index];
                }
            }
            col_counter++;
        }
    }

    public bool DataContains(string element)
    {
        foreach(string col_name in data.Keys)
        {
            if (col_name.ToLower().Equals(element.ToLower()))
            {
                return true;
            }
        }
        return false;
    }
}
