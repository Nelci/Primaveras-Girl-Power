using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathOnObjectColide : GenericFixedBehaviour
{
    public GameManagerFase1 gm;
    private int deathBool;                       // Animator variable related to whether or not the player is Death.
    // Start is called before the first frame update
    void Start()
    {
        deathBool = Animator.StringToHash("Dead");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Objects"))
        {
            behaviourManager.GetAnim.SetBool(deathBool, true);
            gm.setVictimDead(true);
            // Destroy(this.gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
