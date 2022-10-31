using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetect : MonoBehaviour
{
    public ConnectionManager manager;
    public int layerMask;
    bool m_started;
    GameObject parentGO;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ConnectionManager>();
        m_started = true;
        layerMask = LayerMask.NameToLayer("Default");
    }

    // Update is called once per frame
    void Update()
    {
        CollisionDetect();
    }
    void CollisionDetect(){
        Collider[] otherCollider = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, layerMask);
        for(int i = 0; i < otherCollider.Length; i++)
            Debug.Log("ColliderBox: " + otherCollider[i].name + i);
        if(manager.statusMap[this.gameObject.name] == 1 && otherCollider.Length > 0)
            manager.statusMap[this.gameObject.name] = 2;
        if(manager.statusMap[this.gameObject.name] == 2 && otherCollider.Length == 0)
            manager.statusMap[this.gameObject.name] = 1;
    }
    void OnDrawGizmos(){
        Gizmos.color = Color.yellow;
        if(m_started)
            Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
