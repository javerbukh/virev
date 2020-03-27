using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PersonScript : MonoBehaviour
{

    public Camera faceCamera;
    private Vector3 positionCamera;
    
    // Create list of graphs so that the user can alternate between active graphs
    public GameObject GraphPrefab;
    public GameObject TimeSeriesGraphPrefab;
    public GameObject GraphPlaceholderPrefab;
    public GameObject FileReaderPrefab;
    public GameObject SpherePrefab;
    public Canvas canvas;

    public SteamVR_Action_Vector2 ThumbstickAction = null;
    public SteamVR_Action_Vector2 TrackpadAction = null;
    public SteamVR_Action_Boolean TriggerAction = null;
    public SteamVR_Action_Boolean ButtonAAction = null;
    public SteamVR_Action_Boolean ButtonBAction = null;

    private GameObject currentFileReader;
    private GameObject currentGraph;
    private GameObject currentTimeSeriesGraph;
    private GameObject tempGraph;

    static float DISTANCE = 5.5f;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    private bool move_through_space = false;
    private bool fall_through_space = false;
    private float time_of_last_click = 0.0f;
    private bool doubleClick = false;

    private bool pushed_down = false;
    private float release_begin = 0.0f;
    private float release_end = 0.6f;

    // Use this for initialization
    void Start()
    {
        faceCamera.transform.parent = this.transform;

    }

    // Update is called once per frame
    void Update()
    {
        /* Not working currently
        * Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        * Vector3 rayPos = -ray.direction.normalized;
        *
        *
        */
        Thumbstick();
        Trackpad();
        Trigger();
        ButtonA();

        if (Input.GetKeyDown("s"))
        {
            tempGraph = Instantiate(GraphPlaceholderPrefab, transform.position + transform.forward * DISTANCE, transform.rotation);


        }
        if (Input.GetKeyUp("s"))
        {
            Destroy(tempGraph);
            currentGraph = Instantiate(GraphPrefab, transform.position + transform.forward * DISTANCE, transform.rotation);
            currentFileReader = Instantiate(FileReaderPrefab, transform.position, transform.rotation);
        }

        if (Input.GetKeyDown("a"))
        {
            if (currentGraph)
            {
                print("Current graph exists");
                // currentGraph.GetComponent<GraphScript>().LoadData();
                string[] input_files = new[] { "Assets/Resources/gaia_data.txt" };
                // string[] input_files = new[] { "Assets/Resources/gaia_100lyr.txt" };
                // string[] input_files = new[] { "Assets/Resources/gaia_data.txt", "Assets/Resources/gaia_50lyr.txt", "Assets/Resources/gaia_100lyr.txt", "Assets/Resources/gaia_200lyr.txt", "Assets/Resources/gaia_500lyr.txt" };
                // string[] input_files = new[] { "Assets/Resources/gaia_data.txt", "Assets/Resources/gaia_50lyr.txt" };
                currentFileReader.GetComponent<FileReaderScript>().LoadDataFromFiles(currentGraph, input_files);
            }
        }

        if (Input.GetKeyDown("d"))
        {
            // currentGraph.transform.position += currentGraph.transform.forward * 2f;
            GameObject sphere = (GameObject)Instantiate(SpherePrefab, transform.position, Quaternion.Euler(0, 0, 0));

        }

        if (Input.GetKeyDown("t"))
        {
            currentTimeSeriesGraph = Instantiate(GraphPrefab, transform.position + transform.forward * DISTANCE, transform.rotation);
            currentFileReader = Instantiate(FileReaderPrefab, transform.position, transform.rotation);
            // Center the Graph to the camera
            // currentTimeSeriesGraph.transform.position.Set(transform.position.x + (currentTimeSeriesGraph.transform.position.x)/2, currentTimeSeriesGraph.transform.position.y, currentTimeSeriesGraph.transform.position.z);

            string[] input_files = new[] { "Assets/Resources/sat00.txt", "Assets/Resources/sat01.txt", "Assets/Resources/sat02.txt", "Assets/Resources/sat03.txt", "Assets/Resources/sat04.txt",
             "Assets/Resources/sat05.txt", "Assets/Resources/sat06.txt", "Assets/Resources/sat07.txt", "Assets/Resources/sat08.txt", "Assets/Resources/sat09.txt", "Assets/Resources/sat10.txt"};
            currentFileReader.GetComponent<FileReaderScript>().LoadDataFromFiles(currentTimeSeriesGraph, input_files);
        }
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.Translate(Vector3.left * Time.deltaTime);
            // Define a target position above and behind the target transform
            //Vector3 targetPosition = transform.Translate(Vector3.left);

            //// Smoothly move the camera towards that target position
            //transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.Translate(Vector3.right * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            
            this.transform.Translate(Vector3.forward * Time.deltaTime);

        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            this.transform.Translate(Vector3.back * Time.deltaTime);
        }
        if (Input.GetKey("z"))
        {
            this.transform.Translate(Vector3.up * Time.deltaTime);
        }
        if (Input.GetKey("c"))
        {
            this.transform.Translate(Vector3.down * Time.deltaTime);
        }
        if (Input.GetKeyDown("m"))
        {
            currentGraph.transform.localScale = currentGraph.transform.localScale * 1.1f;
            //currentGraph.GetComponent<GraphScript>().ChangeScale(1.1f);
            
        }
        if (Input.GetKeyDown("n"))
        {
            print(this.transform.position);
            GameObject rightController = GameObject.Find("Controller (right)");
            print(rightController.transform.position);
        }
        if (Input.GetKeyDown("o"))
        {
            //Vector3 set_position = new Vector3(78.0f + currentGraph.transform.position.x, 0.0f + currentGraph.transform.position.y, 6.0f + currentGraph.transform.position.z);
            //Vector3 set_position = new Vector3(currentGraph.GetComponent<GraphScript>().mins["x"] + currentGraph.transform.position.x, currentGraph.GetComponent<GraphScript>().mins["y"] + currentGraph.transform.position.y, currentGraph.GetComponent<GraphScript>().mins["z"] + currentGraph.transform.position.z);
            Vector3 set_position = new Vector3(currentGraph.GetComponent<GraphScript>().mins["x"], currentGraph.GetComponent<GraphScript>().mins["y"], currentGraph.GetComponent<GraphScript>().mins["z"]);

            //Vector3 set_position = new Vector3(78.0f, 0.0f, 6.0f);

            this.transform.position = set_position;
            move_through_space = false;
        }
        if (move_through_space)
        {
            //Vector3 target_position = new Vector3((16f / 80f) * currentGraph.GetComponent<GraphScript>().graph_size + currentGraph.transform.position.x, (63f / 80f) * currentGraph.GetComponent<GraphScript>().graph_size + currentGraph.transform.position.y, (70f / 80f) * currentGraph.GetComponent<GraphScript>().graph_size + currentGraph.transform.position.z);
            // Vector3 target_position = new Vector3(16.0f, 63.0f, 70.0f);
            Vector3 target_position = new Vector3(currentGraph.GetComponent<GraphScript>().maxes["x"] + currentGraph.transform.position.x, currentGraph.GetComponent<GraphScript>().maxes["y"] + currentGraph.transform.position.y, currentGraph.GetComponent<GraphScript>().maxes["z"] + currentGraph.transform.position.z);

            float speed = 0.5f;
            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, target_position, step);
        }
        if (Input.GetKeyDown("p"))
        {
            move_through_space = false;
        }
    }
    public void LoadGaiaData()
    {
        currentGraph = Instantiate(GraphPrefab, transform.position + transform.forward * DISTANCE, transform.rotation);
        currentFileReader = Instantiate(FileReaderPrefab, transform.position, transform.rotation);

        print("Current graph exists");
        // currentGraph.GetComponent<GraphScript>().LoadData();
        string[] input_files = new[] { "Assets/Resources/gaia_data.txt" };
        // string[] input_files = new[] { "Assets/Resources/gaia_100lyr.txt" };
        // string[] input_files = new[] { "Assets/Resources/gaia_data.txt", "Assets/Resources/gaia_50lyr.txt", "Assets/Resources/gaia_100lyr.txt", "Assets/Resources/gaia_200lyr.txt", "Assets/Resources/gaia_500lyr.txt" };
        // string[] input_files = new[] { "Assets/Resources/gaia_data.txt", "Assets/Resources/gaia_50lyr.txt" };


        currentFileReader.GetComponent<FileReaderScript>().LoadDataFromFiles(currentGraph, input_files);
    }

    public void OnDataPointClick()
    {
        print("WORKS");
    }
    void PositionGraph()
    {
        /*
        * Find a way to instance an object in front of the camera
        * 
        */
        //Debug.Log(transform.position == this.transform.position);
        //new Vector2(Mathf.Cos(cornerAngle) * radius, Mathf.Sin(cornerAngle)
        //positionX = transform.position.x + Mathf.Cos(angle) * radius;
        //Y:= originY + sin(angle) * radius;

        //Vector3 newPosition = new Vector3();
        Ray ray = new Ray(faceCamera.transform.position, faceCamera.transform.forward);
        Vector3 rayPos = ray.direction.normalized;

        //float positionX = transform.position.x + Mathf.Cos(angle) * radius;

        Debug.Log(rayPos);
        Debug.Log(transform.forward);

        GameObject newGraph = Instantiate(GraphPrefab, transform.position + transform.forward * DISTANCE, transform.rotation);

        // Find angle between transform.forward and cameraDirection
        // Instatiate graph in the direction camera is facing, DISTANCE away
        Vector3 targetDir = rayPos - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);
        float positionX = transform.position.x + Mathf.Cos(angle) * DISTANCE;
        float positionZ = transform.position.z + Mathf.Sin(angle) * DISTANCE;
        Debug.Log(positionX);
        Debug.Log(positionZ);


        newGraph.transform.position = new Vector3(positionX, transform.position.y + 1.5f, positionZ);
        Debug.Log(newGraph.transform.position);
        newGraph.transform.LookAt(faceCamera.transform);

        //Instantiate(creation, transform.position + transform.forward * distance, transform.rotation);
        //Vector3 playerPos = player.transform.position;
        //Vector3 playerDirection = player.transform.forward;
        //Quaternion playerRotation = player.transform.rotation;
        //float spawnDistance = 10;

        //Vector3 spawnPos = playerPos + playerDirection * spawnDistance;

        //Instantiate(Resources.Load(iconDragging.GetComponent<UISprite>().spriteName), spawnPos, playerRotation);
    }

    private void Thumbstick()
    {
        if (ThumbstickAction.axis == Vector2.zero)
        {
            return;
        }
        if (ThumbstickAction.axis.x < 0)
        {
            this.transform.Translate(Vector3.left * Time.deltaTime);
        }
        else if (ThumbstickAction.axis.x > 0)
        {
            this.transform.Translate(Vector3.right * Time.deltaTime);
        }
        if (ThumbstickAction.axis.y < 0)
        {
            this.transform.Translate(Vector3.back * Time.deltaTime);
        }
        else if (ThumbstickAction.axis.y > 0)
        {
            this.transform.Translate(Vector3.forward * Time.deltaTime);
        }
        print("Thumbstick: " + ThumbstickAction.axis);
    }

    private void Trackpad()
    {
        //if (TrackpadAction.axis == Vector2.zero)
        //{
        //    return;
        //}
        //if (TrackpadAction.axis.y < 0.5f)
        //{
        //    this.transform.Translate(Vector3.down * Time.deltaTime);
        //}
        //else if (TrackpadAction.axis.y > 0.5f)
        //{
        //    this.transform.Translate(Vector3.up * Time.deltaTime);
        //}
        // print("Trackpad: " + TrackpadAction.axis);
    }

    private void Trigger()
    {
        //first click
        //release
        //second click
        // if release time < 0.5f
        //bool pushed_down = false;
        //float release_begin = 0.0f;
        //float release_end = 0.6f;
        if (Time.time <= 1.5f)
        {
            return;
        }
        if (TriggerAction.stateUp)
        {
            if (pushed_down)
            {
                release_begin = Time.time;
                pushed_down = false;
            }
        }
        else if (TriggerAction.state)
        {
            if (!pushed_down)
            {
                release_end = Time.time;
            }
            if(release_end - release_begin <= 0.25f)
            {
                this.transform.Translate(Vector3.down * Time.deltaTime);
            }
            else
            {
                this.transform.Translate(Vector3.up * Time.deltaTime);
            }
            pushed_down = true;
        }
    }
    private void ButtonA()
    {
        //first click
        //release
        //second click
        // if release time < 0.5f
        //bool pushed_down = false;
        //float release_begin = 0.0f;
        //float release_end = 0.6f;

        if (ButtonAAction.stateDown)
        {
            //currentGraph = Instantiate(GraphPrefab, transform.position + transform.forward * DISTANCE, transform.rotation);
            //currentFileReader = Instantiate(FileReaderPrefab, transform.position, transform.rotation);

            //print("Current graph exists");
            //// currentGraph.GetComponent<GraphScript>().LoadData();
            //string[] input_files = new[] { "Assets/Resources/gaia_data.txt" };
            //// string[] input_files = new[] { "Assets/Resources/gaia_100lyr.txt" };
            //// string[] input_files = new[] { "Assets/Resources/gaia_data.txt", "Assets/Resources/gaia_50lyr.txt", "Assets/Resources/gaia_100lyr.txt", "Assets/Resources/gaia_200lyr.txt", "Assets/Resources/gaia_500lyr.txt" };
            //// string[] input_files = new[] { "Assets/Resources/gaia_data.txt", "Assets/Resources/gaia_50lyr.txt" };
            //currentFileReader.GetComponent<FileReaderScript>().LoadDataFromFiles(currentGraph, input_files);

            canvas.gameObject.SetActive(true);
        }

    }

    public void ChangeGraphScale()
    {
        print(GameObject.Find("Slider"));
    }
    
}
