using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearBarrelDetector : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider) {

        if(collider.gameObject.CompareTag("Player"))
            GameObject.FindObjectOfType<SpeedRunAgentBARREL>().CallAddReward(0.15f);

    }

}
