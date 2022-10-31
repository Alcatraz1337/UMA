using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComTrigger : MonoBehaviour
{
    public ConnectionManager manager;
    GameObject parentGO;
    HashSet<Collider> obstacleList;
    int stat;
    // bool m_started;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ConnectionManager>();
        parentGO = this.transform.parent.gameObject;
        obstacleList = new HashSet<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        //CollisionDetect();
    }
    // void CollisionDetect(){
    //     Collider[] otherCollider = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, 0);
    //     if(manager.connectionMap[parentGO] == 1 && otherCollider.Length > 0)
    //         manager.connectionMap[parentGO] = 2;
    //     if(manager.connectionMap[parentGO] == 2 && otherCollider.Length == 0)
    //         manager.connectionMap[parentGO] = 1;
    // }
    // void OnDrawGizmos(){
    //     Gizmos.color = Color.yellow;
    //     if(m_started)
    //         Gizmos.DrawWireCube(transform.position, transform.localScale);
    // }
    
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject != null){
            if((other.gameObject.tag == "Obstacle" || 
                other.gameObject.tag == "Station") && 
                manager.statusMap[parentGO.name] == 1){
                    obstacleList.Add(other);
                    manager.statusMap[parentGO.name] = 2; // Set the connection stat to 'Blocked'
            }
        }
    }
    private void OnTriggerStay(Collider other) {
        if(other.gameObject != null){
            if((other.gameObject.tag == "Obstacle" || 
                other.gameObject.tag == "Station") &&
                manager.statusMap[parentGO.name] == 1){
                    if(!obstacleList.Contains(other))
                        obstacleList.Add(other);
                    manager.statusMap[parentGO.name] = 2;
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.gameObject != null){
            obstacleList.Remove(other);
            if((other.gameObject.tag.Equals((string)"Obstacle") || 
                other.gameObject.tag.Equals((string)"Station")) &&
                manager.statusMap[parentGO.name] == 2 && obstacleList.Count == 0)
                    manager.statusMap[parentGO.name] = 1;//Set the connection back to 'connected'
        }
    }
}
