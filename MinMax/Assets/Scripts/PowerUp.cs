using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private enum PowerT{
        projectiles = 0
    }

    PowerT type;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            var playerHit = other.GetComponent<PlayerController>();
            var stats = playerHit.stats;
            switch(type){
                case PowerT.projectiles:
                    if(stats.NumberOfProjectiles<7){
                        stats.NumberOfProjectiles++;
                    }
                    break;
            }
            Destroy(this.gameObject);
        }
    }
}
