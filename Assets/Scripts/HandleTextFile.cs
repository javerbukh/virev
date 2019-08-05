using UnityEngine;
using UnityEditor;
using System.IO;
using System;

using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ParticleSystem))]

public class HandleTextFile : MonoBehaviour
{
    public GameObject GraphPrefab;

    public Material graphMaterial;

    private GameObject graphObject;

    ParticleSystem particle_system;

    ParticleSystem.Particle[] particle_points;

    private GameObject m_MainCamera;

    public string input_file;

    private int file_row_count = 0;
    private int file_col_count = 0;

    float[] mins;
    float[] maxes;

    private static int GRAPHSIZE = 5;
    private static float OBJECTSCALE = 0.01f;

    public static float graph_scale_multiplyer = 1.0f;
    public static float graph_x_position = 2.2544f;
    private static float graph_y_position = 1.9139f;
    private static float graph_z_position = -7.3031f;



    // Use this for initialization
    void Start()
    {
        m_MainCamera = GameObject.Find("Camera");

        //GameObject graphInstance = Instantiate(
        //    GraphPrefab,
        //    new Vector3(graph_x_position, graph_y_position, graph_z_position),
        //    Quaternion.identity);
        GameObject graphInstance = Instantiate(GraphPrefab, m_MainCamera.transform.position, Quaternion.identity);

        this.graphObject = graphInstance;

        //m_MainCamera.enabled = true;
        //Vector3 cameraRelative = cam.InverseTransformPoint(transform.position);
        Debug.Log(m_MainCamera.transform.position);

        // GraphPrefab clone = (GraphPrefab)Instantiate(projectile, transform.position, transform.rotation);
        // GraphPrefab graph_1 = new GraphPrefab();
        print(graphInstance.GetComponent<GraphScript>().units);
        graphInstance.GetComponent<MeshRenderer>().material = graphMaterial;


        HandleTextFile htf = new HandleTextFile();
        //this.particle_system = GetComponent<ParticleSystem>();

        string input_file_chopped = "Assets/Resources/sat0";
        string input_file_suffix = ".txt";
        for (int filenums = 0; filenums < 5; filenums++)
        {
            string combined_file_name = String.Concat(String.Concat(input_file_chopped, filenums.ToString()), input_file_suffix);
            Debug.Log(combined_file_name);
            htf.input_file = combined_file_name;
            Color random_color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

            ReadFile(htf);
            GetMaxesAndMins(htf);
            CreatePoints(htf, random_color, graphInstance);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w"))
        {
            this.graphObject.transform.Translate(Vector3.right);
            // graph_x_position++;
            print("up arrow key is held down");
        }
        if (Input.GetKeyUp("w"))
        {
            // Start();
            print("up arrow key is up");
        }
        if (Input.GetKeyDown("s"))
        {
            this.graphObject.transform.Rotate(new Vector3(30, 0, 0));
            print("down arrow key is held down");
        }
        if (Input.GetKey("a"))
        {
            graph_scale_multiplyer += 1.0f;
            // this.graphObject.transform.localScale.Set(GRAPHSIZE*graph_scale_multiplyer, GRAPHSIZE * graph_scale_multiplyer, GRAPHSIZE * graph_scale_multiplyer);
            this.graphObject.transform.localScale += new Vector3(10.0f, 10.0f, 10.0f);
            print("left arrow key is held down");
        }
        if (Input.GetKeyDown("d"))
        {
            this.graphObject.transform.Translate(Vector3.left);

            print("right arrow key is held down");
        }
    }

    [MenuItem("Tools/Read file")]
    static void ReadString()
    {
        string path = "Assets/Resources/test.txt";

        //Read the text from directly from the test.txt file
        // StreamReader reader = new StreamReader(path);
        try
        {
            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                // Read and display lines from the file until the end of 
                // the file is reached.
                while ((line = sr.ReadLine()) != null)
                {
                    //Console.WriteLine(line);
                    float[] position = Array.ConvertAll(line.Split(' '), new Converter<string, float>(float.Parse));
                    Vector3 new_position = new Vector3(position[0], position[1], position[2]);


                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);


                    GameObject Sphere = (GameObject)Instantiate(sphere, new_position, Quaternion.Euler(0, 0, 0));
                    Debug.Log(position[0]);

                }
            }
        }
        catch (Exception e)
        {
            // Let the user know what went wrong.
            Debug.Log("The file could not be read:");
            Debug.Log(e.Message);

            //Console.WriteLine("The file could not be read:");
            //Console.WriteLine(e.Message);
        }
        //Debug.Log(reader.ReadToEnd());
        //reader.Close();
    }
    static void ReadFile(HandleTextFile htf)
    {
        // string path = "Assets/Resources/sat00.txt";
        Debug.Log(htf.input_file);
        try
        {

            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr = new StreamReader(htf.input_file.ToString()))
            {
                int count = 0;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] split_line = line.Split(' ');
                    htf.file_col_count = split_line.Length;
                    count++;
                }
                htf.file_row_count = count;
            }
        }
        catch (Exception e)
        {
            // Let the user know what went wrong.
            Debug.Log("The file could not be read:");
            Debug.Log(e.Message);

        }
    }

    static void GetMaxesAndMins(HandleTextFile htf)
    {
        // string path = "Assets/Resources/sat00.txt";

        try
        {
            htf.mins = new float[htf.file_col_count];
            htf.maxes = new float[htf.file_col_count];

            for (int i=0; i<htf.mins.Length; i++)
            {
                htf.mins[i] = float.MaxValue;
            }


            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr = new StreamReader(htf.input_file))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line[0] == '#')
                    {
                        // Split line
                        // create dict with each split as a key
                    }
                    else
                    {
                        string[] split_line = line.Split(' ');
                        float[] position = new float[] { float.Parse(line.Split(' ')[0]), float.Parse(line.Split(' ')[1]), float.Parse(line.Split(' ')[2]), float.Parse(line.Split(' ')[3]), float.Parse(line.Split(' ')[4]) };

                        for (int index = 0; index < position.Length; index++)
                        {
                            if (position[index] < htf.mins[index])
                            {
                                htf.mins[index] = position[index];
                            }
                            if (position[index] > htf.maxes[index])
                            {
                                htf.maxes[index] = position[index];
                            }
                        }
                    }


                }
            }
            Debug.Log(htf.mins[0]);
            Debug.Log(htf.maxes[0]);
        }
        catch (Exception e)
        {
            // Let the user know what went wrong.
            Debug.Log("The file could not be read:");
            Debug.Log(e.Message);

        }
    }
    static void CreatePointsParticles(HandleTextFile htf, Color sphere_color, GameObject graph)
    {
        //string path = "Assets/Resources/sat00.txt";

        //Read the text from directly from the test.txt file
        try
        {

            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr = new StreamReader(htf.input_file))
            {
                Debug.Log("Step 1");
                string line;
                // Read and display lines from the file until the end of 
                // the file is reached.
                int count = 0;

                //htf.particle_system = GetComponent<ParticleSystem>();
                htf.particle_points = new ParticleSystem.Particle[htf.file_row_count];

                while ((line = sr.ReadLine()) != null)
                {
                    Debug.Log("Step 2");
                    Debug.Log(htf.particle_system);
                    //Debug.Log(line[0] == '#');
                    if (line[0] == '#')
                    {
                        // Split line
                        // create dict with each split as a key
                    }
                    else
                    {
                        // dict[key] += row[column number]
                        Debug.Log("Step 3");
                        //Debug.Log(line.Split(' ')[0]);
                        // Why is this code not working???????????
                        // float[] position = Array.ConvertAll(line.Split(' '), new Converter<string, float>(float.Parse));
                        float[] position = new float[] { float.Parse(line.Split(' ')[0]), float.Parse(line.Split(' ')[1]), float.Parse(line.Split(' ')[2]), float.Parse(line.Split(' ')[3]), float.Parse(line.Split(' ')[4]) };
                        Debug.Log("Step 4");

                        // Vector3 new_position = new Vector3(((htf.maxes[0]-position[0])/htf.maxes[0]) * GRAPHSIZE + graph_x_position, ((htf.maxes[1] - position[1]) / htf.maxes[1]) * GRAPHSIZE + graph_y_position, ((htf.maxes[2] - position[2]) / htf.maxes[2]) * GRAPHSIZE + graph_z_position);

                        /*
                         * Actual xyz
                         * 
                         * float[] scale_diff = { GRAPHSIZE / (htf.maxes[2] - htf.mins[2]), GRAPHSIZE / (htf.maxes[3] - htf.mins[3]), GRAPHSIZE / (htf.maxes[4] - htf.mins[4]) };
                         * Vector3 new_position = new Vector3(((position[2]) - htf.mins[2]) * scale_diff[2] + graph_x_position, ((position[3]) - htf.mins[3]) * scale_diff[3] + graph_y_position, ((position[4]) - htf.mins[4]) * scale_diff[4] + graph_z_position);
                        */
                        float[] scale_diff = { GRAPHSIZE / (htf.maxes[0] - htf.mins[0]), GRAPHSIZE / (htf.maxes[1] - htf.mins[1]), GRAPHSIZE / (htf.maxes[2] - htf.mins[2]) };

                        Vector3 new_position = new Vector3(((position[0]) - htf.mins[0]) * scale_diff[0] + graph_x_position, ((position[1]) - htf.mins[1]) * scale_diff[1] + graph_y_position, ((position[2]) - htf.mins[2]) * scale_diff[2] + graph_z_position);

                        htf.particle_points[count].position = new_position;
                        htf.particle_points[count].startColor = sphere_color;
                        htf.particle_points[count].startSize = OBJECTSCALE;


                        //Debug.Log(position[0]);
                        Debug.Log("Step 7");

                    }

                    count++;
                }
                Debug.Log("Step 8");


                htf.particle_system.SetParticles(htf.particle_points, htf.particle_points.Length);


                Debug.Log("Step 9");

            }
        }
        catch (Exception e)
        {
            // Let the user know what went wrong.
            Debug.Log("The points could not be created");
            Debug.Log(e.Message);

        }

    }
    static void CreatePoints(HandleTextFile htf, Color sphere_color, GameObject graph)
    {
        //string path = "Assets/Resources/sat00.txt";

        //Read the text from directly from the test.txt file
        try
        {

            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr = new StreamReader(htf.input_file))
            {
                Debug.Log("Step 1");
                string line;
                // Read and display lines from the file until the end of 
                // the file is reached.
                int count = 0;

                while ((line = sr.ReadLine()) != null)
                {
                    Debug.Log("Step 2");
                    //Debug.Log(line[0] == '#');
                    if (line[0] == '#')
                    {
                        // Split line
                        // create dict with each split as a key
                    }
                    else
                    {
                        // dict[key] += row[column number]
                        Debug.Log("Step 3");
                        //Debug.Log(line.Split(' ')[0]);
                        // Why is this code not working???????????
                        // float[] position = Array.ConvertAll(line.Split(' '), new Converter<string, float>(float.Parse));
                        float[] position = new float[] { float.Parse(line.Split(' ')[0]), float.Parse(line.Split(' ')[1]), float.Parse(line.Split(' ')[2]), float.Parse(line.Split(' ')[3]), float.Parse(line.Split(' ')[4]) };
                        Debug.Log("Step 4");

                        // Vector3 new_position = new Vector3(((htf.maxes[0]-position[0])/htf.maxes[0]) * GRAPHSIZE + graph_x_position, ((htf.maxes[1] - position[1]) / htf.maxes[1]) * GRAPHSIZE + graph_y_position, ((htf.maxes[2] - position[2]) / htf.maxes[2]) * GRAPHSIZE + graph_z_position);

                        /*
                         * Actual xyz
                         * 
                         * float[] scale_diff = { GRAPHSIZE / (htf.maxes[2] - htf.mins[2]), GRAPHSIZE / (htf.maxes[3] - htf.mins[3]), GRAPHSIZE / (htf.maxes[4] - htf.mins[4]) };
                         * Vector3 new_position = new Vector3(((position[2]) - htf.mins[2]) * scale_diff[2] + graph_x_position, ((position[3]) - htf.mins[3]) * scale_diff[3] + graph_y_position, ((position[4]) - htf.mins[4]) * scale_diff[4] + graph_z_position);
                        */
                        float[] scale_diff = {GRAPHSIZE/(htf.maxes[0]-htf.mins[0]), GRAPHSIZE / (htf.maxes[1] - htf.mins[1]), GRAPHSIZE / (htf.maxes[2] - htf.mins[2])};

                        Vector3 new_position = new Vector3(((position[0]) - htf.mins[0]) * scale_diff[0] + graph_x_position, ((position[1]) - htf.mins[1]) * scale_diff[1] + graph_y_position, ((position[2]) - htf.mins[2]) * scale_diff[2]  + graph_z_position);

                        // TODO: Replace with dataPoint prefab!!!!!!!!!!!!!!!!!!!!!!!!!!
                        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);


                        GameObject Sphere = (GameObject)Instantiate(sphere, new_position, Quaternion.Euler(0, 0, 0));
                        Sphere.transform.localScale = new Vector3(OBJECTSCALE, OBJECTSCALE, OBJECTSCALE);
                        Sphere.GetComponent<MeshRenderer>().material.color = sphere_color;

                        Sphere.transform.parent = graph.transform;

                        //Debug.Log(position[0]);
                        Debug.Log("Step 7");

                    }

                    count++;
                }

                
                Debug.Log("Step 8");
            }
        }
        catch (Exception e)
        {
            // Let the user know what went wrong.
            Debug.Log("The points could not be created");
            Debug.Log(e.Message);

        }

    }
}