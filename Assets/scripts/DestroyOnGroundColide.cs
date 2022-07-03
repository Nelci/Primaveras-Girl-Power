using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnGroundColide : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
 
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground")){
            Destroy(this.gameObject);
        }
    }
}
