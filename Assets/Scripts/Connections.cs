using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connections : MonoBehaviour
{
    public bool isConnected = false;
    public int connectionsLimit = 2;
    public List<GameObject> connectedStations; //All stations connected to
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ConnectTo(GameObject station){
        isConnected = true;
        connectedStations.Add(station);
    }
    public void DisconnectFrom(GameObject station){
        connectedStations.Remove(station);
        if(connectedStations.Count == 0)
            isConnected = false;
    }
    public void RemoveConnection(GameObject station){
        connectedStations.Remove(station);
        if(connectedStations.Count == 0)
            isConnected = false;
    }
    public bool isAvailable(){
        return connectedStations.Count < connectionsLimit;
    }
}
