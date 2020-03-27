using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FileReaderScript : MonoBehaviour {

    public GameObject Data;
    public GameObject DataPoint;

    public float seconds = 40f;

    //public ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particlePoints;


    public void LoadDataFromFiles(GameObject graph, string[] input_files)
    {
        /*
         * This function will mostly be used for Time Series graphs that use
         * multiple files to simulate object (dark halos, galaxies, etc.) movement
         * over time
         */

        //particleSystem = GetComponent<ParticleSystem>();

        bool is_time_column_present = false;
        int highest_file_row_count = 0;
        int highest_file_col_count = 0;

        ParticleSystem ps = GetComponent<ParticleSystem>();

        float graph_size = graph.GetComponent<GraphScript>().graph_size;
        float object_scale = graph.GetComponent<GraphScript>().object_scale;

        graph.transform.localScale = new Vector3(graph_size, graph_size, graph_size);
        foreach (string file in input_files)
        {
            Color random_color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            GameObject newData = (GameObject)Instantiate(Data, graph.transform.position, Quaternion.Euler(0, 0, 0));
            newData.transform.parent = graph.transform;
            newData.GetComponent<DataScript>().SetInputFile(file);
            newData.GetComponent<DataScript>().color = random_color;
            graph.GetComponent<GraphScript>().AddData(newData);

            ReadFile(newData);
            newData.GetComponent<DataScript>().GetMaxesAndMins();
            newData.GetComponent<DataScript>().FindXYZIndices();

            if(newData.GetComponent<DataScript>().file_row_count > highest_file_row_count)
            {
                highest_file_row_count = newData.GetComponent<DataScript>().file_row_count;
            }

            if (newData.GetComponent<DataScript>().file_col_count > highest_file_col_count)
            {
                highest_file_col_count = newData.GetComponent<DataScript>().file_col_count;
            }
            /*
             * If "time" column is present in one of the data files, run it as a
             * Coroutine once all Data objects are added to graph. If not, visualize
             * them after this point
             */

            if (newData.GetComponent<DataScript>().DataContains("time")){
                is_time_column_present = true;
            }

            // VisualizeData(graph, newData, random_color);
            // StartCoroutine(CreateDataPointsOverTime(graph, newData, random_color));


        }
        if (is_time_column_present)
        {
            StartCoroutine(CreateDataPointsOverTime(graph, highest_file_row_count));
        }
        else
        {
            List<GameObject> dataCollection = graph.GetComponent<GraphScript>().dataCollection;

            // Find the largest maximums and smallest minimums in the graph so scaling
            // is even across all dataSets
            //float[] graph_mins = new float[highest_file_col_count];
            //float[] graph_maxes = new float[highest_file_col_count];
            //for (int i = 0; i < graph_mins.Length; i++)
            //{
            //    graph_mins[i] = float.MaxValue;
            //}

            //foreach (GameObject data_m in dataCollection)
            //{

            //    float[] mins = data_m.GetComponent<DataScript>().mins;
            //    float[] maxes = data_m.GetComponent<DataScript>().maxes;

            //    int x_col_index = data_m.GetComponent<DataScript>().x_col_index;
            //    int y_col_index = data_m.GetComponent<DataScript>().y_col_index;
            //    int z_col_index = data_m.GetComponent<DataScript>().z_col_index;

            //    for (int index = 0; index < highest_file_col_count; index++)
            //    {
            //        if (graph_mins[index] > mins[index])
            //        {
            //            graph_mins[index] = mins[index];
            //        }
            //        if (graph_maxes[index] < maxes[index])
            //        {
            //            graph_maxes[index] = maxes[index];
            //        }
            //    }

            //}

            Dictionary<string, float> graph_maxes = graph.GetComponent<GraphScript>().maxes;
            Dictionary<string, float> graph_mins = graph.GetComponent<GraphScript>().mins;

            foreach (GameObject data_m in dataCollection)
            {
                float[] mins = data_m.GetComponent<DataScript>().mins;
                float[] maxes = data_m.GetComponent<DataScript>().maxes;

                int x_col_index = data_m.GetComponent<DataScript>().x_col_index;
                int y_col_index = data_m.GetComponent<DataScript>().y_col_index;
                int z_col_index = data_m.GetComponent<DataScript>().z_col_index;


                int[] positions = new int[3] {x_col_index, y_col_index, z_col_index };
                string[] xyz = new string[3] { "x", "y", "z" };
                for(int index = 0; index < 3; index++)
                {
                    string letter = xyz[index];
                    int pos = positions[index];
                    if (graph_mins[letter] > mins[pos])
                    {
                        graph_mins[letter] = mins[pos];
                    }
                    if (graph_maxes[letter] < maxes[pos])
                    {
                        graph_maxes[letter] = maxes[pos];
                    }
                }
            }

            graph.GetComponent<GraphScript>().maxes = graph_maxes;
            graph.GetComponent<GraphScript>().mins = graph_mins;

            print("x_max: " + graph_maxes["x"] + "y_max: " + graph_maxes["y"] + "z_max: " + graph_maxes["z"]);
            print("x_min: " + graph_mins["x"] + "y_min: " + graph_mins["y"] + "z_min: " + graph_mins["z"]);

            // Once all maxes and mins are found for the graph, visualize
            // the data
            foreach (GameObject data in dataCollection)
            {
               
                Color sphere_color = data.GetComponent<DataScript>().color;
                // VisualizeDataAsParticles(ps, graph, data, sphere_color);
                VisualizeData(graph, data, sphere_color);
            }
        }
    }

    IEnumerator CreateDataPointsOverTime(GameObject graph, int highest_file_row_count)
    {
        List<GameObject> dataCollection = graph.GetComponent<GraphScript>().dataCollection;
        int num_data = dataCollection.Count;
        GameObject[] previous_data_points = new GameObject[num_data];

        for (int index = 0; index < highest_file_row_count; index++)
        {
            int counter = 0;
            foreach (GameObject data in dataCollection)
            {
                float graph_size = graph.GetComponent<GraphScript>().graph_size;
                float object_scale = graph.GetComponent<GraphScript>().object_scale;

                float[] mins = data.GetComponent<DataScript>().mins;
                float[] maxes = data.GetComponent<DataScript>().maxes;

                int file_row_count = data.GetComponent<DataScript>().file_row_count;
                int file_col_count = data.GetComponent<DataScript>().file_col_count;

                // If the file_row_count of this data object is smaller than the
                // current index, break out of this loop
                if (index > file_row_count)
                {
                    break;
                }

                int x_col_index = data.GetComponent<DataScript>().x_col_index;
                int y_col_index = data.GetComponent<DataScript>().y_col_index;
                int z_col_index = data.GetComponent<DataScript>().z_col_index;
                int time_col_index = data.GetComponent<DataScript>().time_col_index;
                int redshift_col_index = data.GetComponent<DataScript>().redshift_col_index;


                Color sphere_color = data.GetComponent<DataScript>().color;

                float[] scale_diff = new float[file_col_count];

                for (int ind = 0; ind < file_col_count; ind++)
                {
                    if (ind == x_col_index)
                    {
                        scale_diff[ind] = graph_size / (maxes[x_col_index] - mins[x_col_index]);
                    }
                    else if (ind == y_col_index)
                    {
                        scale_diff[ind] = graph_size / (maxes[y_col_index] - mins[y_col_index]);
                    }
                    else if (ind == z_col_index)
                    {
                        scale_diff[ind] = graph_size / (maxes[z_col_index] - mins[z_col_index]);
                    }
                    else
                    {
                        scale_diff[ind] = 1.0f;
                    }
                }

                // float[] scale_diff = { 1, 1, GRAPHSIZE / (maxes[x_col_index] - mins[x_col_index]), GRAPHSIZE / (maxes[y_col_index] - mins[y_col_index]), GRAPHSIZE / (maxes[z_col_index] - mins[z_col_index]) };


                Vector3 new_position = new Vector3(((data.GetComponent<DataScript>().data["x"][index]) - mins[x_col_index]) * scale_diff[x_col_index] + data.transform.position.x,
                    ((data.GetComponent<DataScript>().data["y"][index]) - mins[y_col_index]) * scale_diff[y_col_index] + data.transform.position.y,
                    ((data.GetComponent<DataScript>().data["z"][index]) - mins[z_col_index]) * scale_diff[z_col_index] + data.transform.position.z);

                GameObject dataPoint = (GameObject)Instantiate(DataPoint, new_position, Quaternion.Euler(0, 0, 0));
                dataPoint.transform.localScale = new Vector3(object_scale, object_scale, object_scale);
                dataPoint.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", sphere_color);
                dataPoint.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                dataPoint.transform.parent = data.transform;
                

                // Gizmos.color = sphere_color;
                if (previous_data_points[counter])
                {
                    GameObject myLine = new GameObject();
                    myLine.transform.position = previous_data_points[counter].transform.position;
                    myLine.AddComponent<LineRenderer>();
                    LineRenderer lr = myLine.GetComponent<LineRenderer>();
                    lr.material.SetColor("_EmissionColor", sphere_color);
                    lr.material.EnableKeyword("_EMISSION");
                    //lr.SetColors(color, color);
                    lr.SetWidth(0.01f, 0.01f);
                    lr.SetPosition(0, previous_data_points[counter].transform.position);
                    lr.SetPosition(1, dataPoint.transform.position);
                }
                

                previous_data_points[counter] = dataPoint;
                counter++;

            }
            yield return new WaitForSeconds(.2f);
            foreach(GameObject DP in previous_data_points)
            {
                Destroy(DP);
            }
            
        }

    }

    public void LoadDataFromFile(GameObject graph, string input_file)
    {
        /*
         * Loads Data from a single file and creates a Data object out of it
         * 
         */
        float graph_size = graph.GetComponent<GraphScript>().graph_size;
        float object_scale = graph.GetComponent<GraphScript>().object_scale;
        graph.transform.localScale = new Vector3(graph_size, graph_size, graph_size);

        Color random_color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        GameObject newData = (GameObject)Instantiate(Data, graph.transform.position, Quaternion.Euler(0, 0, 0));
        newData.transform.parent = graph.transform;
        newData.GetComponent<DataScript>().SetInputFile(input_file);

        ReadFile(newData);
        newData.GetComponent<DataScript>().GetMaxesAndMins();
        newData.GetComponent<DataScript>().FindXYZIndices();
        float[] maxes = newData.GetComponent<DataScript>().maxes;
        float[] mins = newData.GetComponent<DataScript>().mins;

        int x_col_index = newData.GetComponent<DataScript>().x_col_index;
        int y_col_index = newData.GetComponent<DataScript>().y_col_index;
        int z_col_index = newData.GetComponent<DataScript>().z_col_index;

        graph.GetComponent<GraphScript>().maxes = new Dictionary<string, float>()
        {
            { "x", maxes[x_col_index] },
            { "y", maxes[y_col_index] },
            { "z", maxes[z_col_index] }
        };
        graph.GetComponent<GraphScript>().mins = new Dictionary<string, float>()
        {
            { "x", mins[x_col_index] },
            { "y", mins[y_col_index] },
            { "z", mins[z_col_index] }
        };
        VisualizeData(graph, newData, random_color);

    }

    public void ReadFile(GameObject data)
    {
        /*
         * Loads data from a file and populates a Data object
         * 
         */
        int file_row_count = 0;
        int file_col_count = 0;
        string inputFile = data.GetComponent<DataScript>().input_file;

        List<string> column_names = new List<string>();

        Debug.Log(inputFile);
        try
        {

            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr = new StreamReader(inputFile.ToString()))
            {
                int count = 0;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] split_line = line.Split(' ');
                    if (line[0] == '#')
                    {
                        foreach(string element in split_line)
                        {
                            if (element.Length > 0 && element[0].ToString() == "#")
                            {
                                column_names.Add(element.Substring(1));
                            }
                            else if (!(element == " ") && !(element == ""))
                            {
                                column_names.Add(element);
                            }
                        }
                        for (int col = 0; col < column_names.Count; col++)
                        {
                            // Add column names to data
                            data.GetComponent<DataScript>().AddDataColumn(column_names[col]);
                        }
                    }
                    else
                    {

                        for (int column = 0; column < column_names.Count; column++)
                        {
                            // Add each element to its corresponding column
                            data.GetComponent<DataScript>().AddDataRow(column_names[column], float.Parse(split_line[column]));
                        }

                        // Add to count for every line after the "#" line
                        count++;
                    }
                    file_col_count = column_names.Count;
                }
                file_row_count = count;
            }
        }
        catch (Exception e)
        {
            // Let the user know what went wrong.
            Debug.Log("The file could not be read:");
            Debug.Log(e.Message);

        }

        data.GetComponent<DataScript>().file_col_count = file_col_count;
        data.GetComponent<DataScript>().file_row_count = file_row_count;

        data.GetComponent<DataScript>().PrintAllData();

    }

    public void VisualizeData(GameObject graph, GameObject data, Color sphere_color)
    {
        /*
        * Creates DataPoints for each element in a Data object
        */

        // Graph mins and maxes are used for scaling
        Dictionary<string, float> graph_mins = graph.GetComponent<GraphScript>().mins;
        Dictionary<string, float> graph_maxes = graph.GetComponent<GraphScript>().maxes;

        float graph_size = graph.GetComponent<GraphScript>().graph_size;
        float object_scale = graph.GetComponent<GraphScript>().object_scale;

        float[] mins = data.GetComponent<DataScript>().mins;
        float[] maxes = data.GetComponent<DataScript>().maxes;

        int file_row_count = data.GetComponent<DataScript>().file_row_count;
        int file_col_count = data.GetComponent<DataScript>().file_col_count;


        int x_col_index = data.GetComponent<DataScript>().x_col_index;
        int y_col_index = data.GetComponent<DataScript>().y_col_index;
        int z_col_index = data.GetComponent<DataScript>().z_col_index;
        int time_col_index = data.GetComponent<DataScript>().time_col_index;
        int redshift_col_index = data.GetComponent<DataScript>().redshift_col_index;
        int radius_val_index = data.GetComponent<DataScript>().radius_val_index;
        int lum_val_index = data.GetComponent<DataScript>().lum_val_index;
        // Cooler looking colors
        Dictionary<float, Color> temperature_colors = new Dictionary<float, Color>() {
            { 2700f, Color.red },
            { 2900f, new Color(1.0f, 0.32f, 0.0f)},
            { 3100f, new Color(1.0f, 0.64f, 0.0f) },
            { 3300f, new Color(1.0f, 0.88f, 0.0f)},
            { 3500f, Color.yellow },
            { 3700f, new Color(0.9f, 0.9f, 0.5f)},
            { 3900f, Color.white },
            { 4200f, Color.cyan },
            { 1000000f, Color.blue }
        };
        // Realistic colors
        //Dictionary<float, Color> temperature_colors = new Dictionary<float, Color>() {
        //    { 3000f, Color.red },
        //    { 4200f, new Color(1.0f, 0.64f, 0.0f) },
        //    { 5800f, Color.yellow },
        //    { 10000f, Color.white },
        //    { 13000f, Color.cyan },
        //    { 1000000f, Color.blue }
        //};

        float[] scale_diff = new float[file_col_count];

        for (int ind = 0; ind < file_col_count; ind++)
        {
            if (ind == x_col_index)
            {
                scale_diff[ind] = graph_size / (graph_maxes["x"] - graph_mins["x"]);
            }
            else if (ind == y_col_index)
            {
                scale_diff[ind] = graph_size / (graph_maxes["y"] - graph_mins["y"]);
            }
            else if (ind == z_col_index)
            {
                scale_diff[ind] = graph_size / (graph_maxes["z"] - graph_mins["z"]);
            }
            else if (ind == radius_val_index)
            {
                scale_diff[ind] = object_scale;
            }
            else
            {
                scale_diff[ind] = 1.0f;
            }
        }

        //float[] scale_diff = { 1, 1, GRAPHSIZE / (graph_maxes["x"] - graph_mins["x"]), GRAPHSIZE / (graph_maxes["y"] - graph_mins["y"]), GRAPHSIZE / (graph_maxes["z"] - graph_mins["z"]) };

        for (int index = 0; index < file_row_count; index++)
        {

            Vector3 new_position = new Vector3(((data.GetComponent<DataScript>().data["x"][index]) - graph_mins["x"]) * scale_diff[x_col_index] + data.transform.position.x,
                ((data.GetComponent<DataScript>().data["y"][index]) - graph_mins["y"]) * scale_diff[y_col_index] + data.transform.position.y,
                ((data.GetComponent<DataScript>().data["z"][index]) - graph_mins["z"]) * scale_diff[z_col_index] + data.transform.position.z);

            //GameObject dataPointGameObject = Instantiate(DataPoint);

            //Rigidbody dataPoint = (Rigidbody)dataPointGameObject.GetComponent<Rigidbody>();

            GameObject dataPoint = Instantiate(DataPoint);

            dataPoint.name = "datapoint_" + index.ToString();

            foreach(string column in data.GetComponent<DataScript>().data.Keys)
            {
                dataPoint.GetComponent<DataPointScript>().AddDataRow(column, data.GetComponent<DataScript>().data[column][index]);
            }

            dataPoint.transform.position = new_position;

            // This can be changed if "size" or "mass" columns are present
            if (data.GetComponent<DataScript>().DataContains("radius_val"))
            {
                float new_object_scale = ((data.GetComponent<DataScript>().data["radius_val"][index]/maxes[radius_val_index]) * scale_diff[radius_val_index]) + 0.01f;
                // print(new_object_scale);
                dataPoint.transform.localScale = new Vector3((float)new_object_scale, (float)new_object_scale, (float)new_object_scale);

                // SET COLLIDER RADIUS TO NEW SCALE!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // dataPoint.GetComponent<Collider>().radius = new_object_scale;
                // print(dataPoint.transform.localScale);
            }
            else
            {
                dataPoint.transform.localScale = new Vector3(object_scale, object_scale, object_scale);
            }

            // This can be set if luminosity and radius are present
            // this would require a seperate script, most likely
            // In the future, probably better to just check if the *_index is > -1
            if (data.GetComponent<DataScript>().DataContains("lum_val") && data.GetComponent<DataScript>().DataContains("radius_val"))
            {
                float stefan_boltzmann_constant = 0.00000005670373f;
                float luminosity = data.GetComponent<DataScript>().data["lum_val"][index];
                float radius = data.GetComponent<DataScript>().data["radius_val"][index];

                float denominator = (float)(4.0f * Math.PI * (Math.Pow(radius, 2.0f)) * stefan_boltzmann_constant);

                // *100 because otherwise it does not work
                float temperature = (float)(Math.Pow((luminosity/denominator), 1.0f / 4.0f)) * 100;
                // print(temperature);
                foreach (KeyValuePair<float, Color> item in temperature_colors)
                {
                    if (temperature < item.Key)
                    {
                        Color star_color = item.Value;
                        star_color.a = 0.3f;
                        dataPoint.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", star_color);
                        dataPoint.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                        if (item.Value == Color.blue)
                        {
                            // print(temperature);
                        }
                        break;
                    }
                }

                }
            else
            {
                // dataPoint.GetComponent<MeshRenderer>().material.color = sphere_color;
                dataPoint.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", sphere_color);
                dataPoint.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
            }

            // print(dataPoint.GetComponent<Collider>());
            dataPoint.transform.parent = data.transform;
        }

    }

    public void VisualizeDataAsParticles(ParticleSystem ps, GameObject graph, GameObject data, Color sphere_color)
    {
        /*
        * Creates particles for each element in a Data object
        */
        float object_scale = graph.GetComponent<GraphScript>().object_scale;

        float[] mins = data.GetComponent<DataScript>().mins;
        float[] maxes = data.GetComponent<DataScript>().maxes;

        int file_row_count = data.GetComponent<DataScript>().file_row_count;
        int file_col_count = data.GetComponent<DataScript>().file_col_count;


        int x_col_index = data.GetComponent<DataScript>().x_col_index;
        int y_col_index = data.GetComponent<DataScript>().y_col_index;
        int z_col_index = data.GetComponent<DataScript>().z_col_index;
        int time_col_index = data.GetComponent<DataScript>().time_col_index;
        int redshift_col_index = data.GetComponent<DataScript>().redshift_col_index;
        int radius_val_index = data.GetComponent<DataScript>().radius_val_index;
        int lum_val_index = data.GetComponent<DataScript>().lum_val_index;
        // Cooler looking colors
        Dictionary<float, Color> temperature_colors = new Dictionary<float, Color>() {
            { 2700f, Color.red },
            { 2900f, new Color(1.0f, 0.32f, 0.0f)},
            { 3100f, new Color(1.0f, 0.64f, 0.0f) },
            { 3300f, new Color(1.0f, 0.88f, 0.0f)},
            { 3500f, Color.yellow },
            { 3700f, new Color(0.9f, 0.9f, 0.5f)},
            { 3900f, Color.white },
            { 4200f, Color.cyan },
            { 1000000f, Color.blue }
        };
        // Realistic colors
        //Dictionary<float, Color> temperature_colors = new Dictionary<float, Color>() {
        //    { 3000f, Color.red },
        //    { 4200f, new Color(1.0f, 0.64f, 0.0f) },
        //    { 5800f, Color.yellow },
        //    { 10000f, Color.white },
        //    { 13000f, Color.cyan },
        //    { 1000000f, Color.blue }
        //};

        float[] scale_diff = new float[file_col_count];

        for (int ind = 0; ind < file_col_count; ind++)
        {
            if (ind == x_col_index)
            {
                scale_diff[ind] = graph.GetComponent<GraphScript>().graph_size / (maxes[x_col_index] - mins[x_col_index]);
            }
            else if (ind == y_col_index)
            {
                scale_diff[ind] = graph.GetComponent<GraphScript>().graph_size / (maxes[y_col_index] - mins[y_col_index]);
            }
            else if (ind == z_col_index)
            {
                scale_diff[ind] = graph.GetComponent<GraphScript>().graph_size / (maxes[z_col_index] - mins[z_col_index]);
            }
            else if (ind == radius_val_index)
            {
                scale_diff[ind] = object_scale;
            }
            else
            {
                scale_diff[ind] = 1.0f;
            }
        }

        particlePoints = new ParticleSystem.Particle[file_row_count];

        for (int index = 0; index < file_row_count; index++)
        {

            Vector3 new_position = new Vector3(((data.GetComponent<DataScript>().data["x"][index]) - mins[x_col_index]) * scale_diff[x_col_index] + data.transform.position.x,
                ((data.GetComponent<DataScript>().data["y"][index]) - mins[y_col_index]) * scale_diff[y_col_index] + data.transform.position.y,
                ((data.GetComponent<DataScript>().data["z"][index]) - mins[z_col_index]) * scale_diff[z_col_index] + data.transform.position.z);

            // GameObject dataPoint = (GameObject)Instantiate(DataPoint, new_position, Quaternion.Euler(0, 0, 0));
            
            particlePoints[index].position = new_position;

            // This can be changed if "size" or "mass" columns are present
            if (data.GetComponent<DataScript>().DataContains("radius_val"))
            {
                float new_object_scale = ((data.GetComponent<DataScript>().data["radius_val"][index] / maxes[radius_val_index]) * scale_diff[radius_val_index]) + 0.01f;
                print(new_object_scale);
                particlePoints[index].startSize = new_object_scale;
                //print(dataPoint.transform.localScale);
            }
            else
            {
                particlePoints[index].startSize = object_scale;
            }

            // This can be set if luminosity and radius are present
            // this would require a seperate script, most likely
            // In the future, probably better to just check if the *_index is > -1
            if (data.GetComponent<DataScript>().DataContains("lum_val") && data.GetComponent<DataScript>().DataContains("radius_val"))
            {
                float stefan_boltzmann_constant = 0.00000005670373f;
                float luminosity = data.GetComponent<DataScript>().data["lum_val"][index];
                float radius = data.GetComponent<DataScript>().data["radius_val"][index];

                float denominator = (float)(4.0f * Math.PI * (Math.Pow(radius, 2.0f)) * stefan_boltzmann_constant);

                // *100 because otherwise it does not work
                float temperature = (float)(Math.Pow((luminosity / denominator), 1.0f / 4.0f)) * 100;
                // print(temperature);
                foreach (KeyValuePair<float, Color> item in temperature_colors)
                {
                    if (temperature < item.Key)
                    {
                        Color star_color = item.Value;
                        star_color.a = 0.3f;
                        particlePoints[index].startColor = star_color;
                        //dataPoint.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", star_color);
                        //dataPoint.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                        if (item.Value == Color.blue)
                        {
                            print(temperature);
                        }
                        break;
                    }
                }

            }
            else
            {
                // dataPoint.GetComponent<MeshRenderer>().material.color = sphere_color;
                //dataPoint.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", sphere_color);
                //dataPoint.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                particlePoints[index].startColor = sphere_color;
            }


            // dataPoint.transform.parent = data.transform;
        }

        ps.SetParticles(particlePoints, particlePoints.Length);
    }
}
