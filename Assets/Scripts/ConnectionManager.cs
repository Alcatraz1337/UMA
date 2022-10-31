using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public GameObject[] stations; // Stores all the station GameObjects
    public GameObject[,] connectionLines; // Stores all the connection line GameObjects
    public Material mat;
    public float lineWidth = 0.3f;
    public Dictionary<string, int> statusMap;
    public HashSet<GameObject> onComSet;

    // Start is called before the first frame update
    void Start()
    {
        // statusMap = new Dictionary<string, int>();
        // 
        // onComSet = new HashSet<GameObject>(); // Store the connections set to 'Connected'
        // 
    }
    private void Awake() {
        statusMap = new Dictionary<string, int>();
        onComSet = new HashSet<GameObject>(); // Store the connections set to 'Connected'
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAllConnection();
    }
    public void SetStations(GameObject[] s){
        stations = new GameObject[s.Length];
        s.CopyTo(stations, 0);
        connectionLines = new GameObject[s.Length, s.Length];
        for(int i = 0; i < s.Length - 1; i++)
            for(int j = i + 1; j < s.Length; j++){
                // Initialize LineRenderer
                Vector3 startPos = stations[i].transform.position;
                Vector3 endPos = stations[j].transform.position;
                connectionLines[i, j] = new GameObject("comLine" + i.ToString() + j.ToString());
                connectionLines[i, j].transform.position = (startPos + endPos) / 2;
                connectionLines[i, j].transform.SetParent(this.transform.parent.transform);

                // Set lineRenderer for connection line
                LineRenderer line = connectionLines[i, j].AddComponent<LineRenderer>();
                line.material = mat;
                line.SetPosition(0, startPos);
                line.SetPosition(1,endPos);
                line.startWidth = lineWidth;
                line.endWidth = lineWidth;
                line.startColor = Color.gray;
                line.endColor = Color.gray;
                line.useWorldSpace = true;

                //Set BoxCollider for collider
                GameObject go = new GameObject("Collider" + i.ToString() + j.ToString());
                BoxCollider col = go.AddComponent<BoxCollider>();
                go.AddComponent<ComTrigger>(); //Attach the script
                col.isTrigger = true;
                col.transform.parent = line.transform;
                float lineLen = Vector3.Distance(startPos,endPos);
                col.size = new Vector3(lineLen, lineWidth, lineWidth);
                Vector3 midPoint = (startPos + endPos) / 2;
                col.transform.position = midPoint;
                //Calculate the angle bewtween start and end
                float angle = (Mathf.Abs(startPos.z - endPos.z) / Mathf.Abs(startPos.x - endPos.x));
                if(!(startPos.z < endPos.z && startPos.x > endPos.x) && !(endPos.z < startPos.z && endPos.x > startPos.x))
                    angle *= -1;
                angle = Mathf.Rad2Deg * Mathf.Atan(angle);
                col.transform.Rotate(0, angle, 0);
                
                // statusMap.Add(connectionLines[i, j].gameObject.name, 0); // Set the connection available
                statusMap.TryAdd(connectionLines[i, j].gameObject.name, 0);// Set the connection available               
            }
    }
    public void SetConnection(int isOn, int st1idx, int st2idx){
        // Get Connection component of each station
        Connections st1Connection = stations[st1idx].GetComponent<Connections>(), st2Connection = stations[st2idx].GetComponent<Connections>();
        if(isOn == 1){          
            if(st1Connection.isAvailable() && st2Connection.isAvailable()){
                statusMap[connectionLines[st1idx, st2idx].gameObject.name] = 1; //Set the connection on
                //Add connection status to station
                st1Connection.ConnectTo(stations[st2idx]);
                st2Connection.ConnectTo(stations[st1idx]);
                onComSet.Add(connectionLines[st1idx, st2idx]); //Add activated com
            }
            else{
                if(!st1Connection.isAvailable()){
                    Debug.Log("Station"+st1idx.ToString()+" not available!");
                    st1Connection.DisconnectFrom(stations[st2idx]);
                    st2Connection.DisconnectFrom(stations[st1idx]);
                    statusMap[connectionLines[st1idx, st2idx].gameObject.name] = 0;
                    onComSet.Remove(connectionLines[st1idx, st2idx]); // Remove com  
                }
                if(!st2Connection.isAvailable()){
                    Debug.Log("Station"+st2idx.ToString()+" not available!");
                    st1Connection.DisconnectFrom(stations[st2idx]);
                    st2Connection.DisconnectFrom(stations[st1idx]);
                    statusMap[connectionLines[st1idx, st2idx].gameObject.name] = 0;
                    onComSet.Remove(connectionLines[st1idx, st2idx]); // Remove com  
                }
            }
        }
        else{
            statusMap[connectionLines[st1idx,st2idx].gameObject.name] = 0;
            st1Connection.DisconnectFrom(stations[st2idx]);
            st2Connection.DisconnectFrom(stations[st1idx]);
            onComSet.Remove(connectionLines[st1idx, st2idx]); // Remove com  
        }
    }
    public bool isAllConnected(){
        Queue<GameObject> unsearched = new Queue<GameObject>();
        unsearched.Enqueue(stations[0]);
        ISet<string> searched = new HashSet<string>();
        while(unsearched.Count > 0){
            //Get the connection of current station from unsearched queue
            Connections currentConnection = unsearched.Peek().GetComponent<Connections>();
            foreach(var s in currentConnection.connectedStations)
                // If s hasn't been searched...
                if(!searched.Contains(s.gameObject.name))
                    unsearched.Enqueue(s); //Add current station connected stations to unsearched station
            searched.Add(unsearched.Dequeue().gameObject.name);
        }
        return searched.Count == stations.Length; // if all stations are searched, the network must be connected.
    }
    public void UpdateAllConnection(){
        //connectionMap
        for(int i = 0; i < stations.Length - 1; i++)
            for(int j = i + 1; j < stations.Length; j++){
                DrawConnection(statusMap[connectionLines[i, j].gameObject.name], i, j);
            }
    }
    public void DrawConnection(int status, int station1idx, int station2idx){
        // Available
        if(status == 0){
            LineRenderer line = connectionLines[station1idx, station2idx].GetComponent<LineRenderer>();
            line.startColor = Color.gray;
            line.endColor = Color.gray;
        }
        // Connected
        if(status == 1){
            LineRenderer line = connectionLines[station1idx, station2idx].GetComponent<LineRenderer>();
            line.startColor = Color.green;
            line.endColor = Color.green;
        }
        // Blocked
        if(status == 2){
            LineRenderer line = connectionLines[station1idx, station2idx].GetComponent<LineRenderer>();
            line.startColor = Color.red;
            line.endColor = Color.red;
        }
    }
    public void SetAllConnectionsByToggle(){
        ClearLog();
        //Test only
        for(int i = 0; i < stations.Length - 1; i++){
            for(int j = i + 1; j < stations.Length; j++){
                int stat = Random.Range(0, 2);
                SetConnection(stat, i, j);
            }
        }
        if(isAllConnected())
            Debug.Log("All stations are connected!");
    }
    private void ClearLog(){
        // For Debug use only
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
