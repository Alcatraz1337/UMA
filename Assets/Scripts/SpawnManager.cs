using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] stationSpawnArea = new GameObject[5];
    public GameObject station;
    public GameObject[] obstacleSpawnArea = new GameObject[2];
    public GameObject obstacle;
    public ConnectionManager cm;

    public int stationCount = 5;
    public int obstacleCount = 2;
    public int obstaclMinScale = 2;
    public int obstacleMaxScale = 5;
    public float obstacleMinVelocity = 1.0f;
    public float obstacleMaxVelocity = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Set up stations
        for(int i = 0; i < stationCount; i++){
            GameObject stationTemp;
            Vector3 center = stationSpawnArea[i].transform.position; // Get the center of the spawn area.
            Vector3 range = stationSpawnArea[i].transform.localScale / 2.0f;
            Vector3 randomRange = new Vector3(Random.Range(-range.x, range.x), 0, Random.Range(-range.z, range.z));
            stationTemp = Instantiate(station, center + randomRange, Quaternion.identity, this.transform.parent.transform);
            stationTemp.name = "station" + i.ToString();
        }
        // Set up obstacles
        for(int i = 0; i < obstacleCount; i++){
            Vector3 center = obstacleSpawnArea[i].transform.position;
            Vector3 range = obstacleSpawnArea[i].transform.localScale / 2.0f;
            Vector3 randomRange = new Vector3(Random.Range(-range.x, range.x), 0, Random.Range(-range.z, range.z));
            GameObject go = Instantiate(obstacle, center + randomRange, Quaternion.identity, this.transform.parent.transform);
            go.name = "Obstacle" + i.ToString();
            go.transform.localScale = new Vector3(Random.Range(obstaclMinScale, obstacleMaxScale), 1, Random.Range(obstaclMinScale, obstacleMaxScale));
            Rigidbody rb = go.GetComponent<Rigidbody>();
            // Add velocity by add force
            // float force = 300f;
            // Vector3 direction = Random.insideUnitSphere.normalized;
            // rb.AddForce(direction * force);
            
            //Add velocity by add velocity
            int minus = Random.Range(0, 2) * 2 - 1;
            Vector3 v = new Vector3(minus * Random.Range(obstacleMinVelocity, obstacleMaxVelocity), 0, minus * Random.Range(obstacleMinVelocity, obstacleMaxVelocity));
            rb.velocity = v;
        }
        cm = GetComponent<ConnectionManager>();
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Station");
        cm.SetStations(temp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
