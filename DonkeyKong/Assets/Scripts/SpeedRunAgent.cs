using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class SpeedRunAgent : Agent
{

    [SerializeField] private Transform goalTransform;

    [SerializeField] private Transform[] ladderTransforms;

    private PlayerAI playerRef;

    private void Start() {

        playerRef = FindObjectOfType<PlayerAI>();

    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {

        //base.OnActionReceived(actions);

        int decision = actions.DiscreteActions[0];

        if (decision == 0) {

            //run right
            if(!playerRef.climbing)
                playerRef.direction.x = playerRef.moveSpeed;

        } else if (decision == 1) {

            //run left
            if(!playerRef.climbing)
                playerRef.direction.x = -playerRef.moveSpeed;

        } else if (decision == 2) {

            //jump right
            if(playerRef.grounded) {

                playerRef.direction = new Vector2(playerRef.moveSpeed, playerRef.jumpStrength/1.5f);

            }

        } else if (decision == 3) {

            //jump left
            if(playerRef.grounded) {

                playerRef.direction = new Vector2(-playerRef.moveSpeed, playerRef.jumpStrength/1.5f);

            }

        }
        
         else if (decision == 4) {

            if(playerRef.climbing) {

                playerRef.direction = new Vector2(0.0f, playerRef.moveSpeed);
                AddReward(+1f);

            }

        }

        //AddReward((-1/Vector2.Distance(new Vector2(-5.25f, -5.25f), transform.position)));

        AddReward((goalTransform.position.y - Mathf.Abs(transform.position.y))/(goalTransform.position.y-(5.25f)));

        // if(playerRef.rigidbody.velocity.y < 0.0f)
        //     AddReward(-3f);

        // else if (decision == 5) {

        //     //stand still

        // }

        if(this.StepCount == this.MaxStep) {
            
            

            EndEpisode();

        }

    }

    public override void CollectObservations(VectorSensor sensor)
    {

        //base.CollectObservations(sensor);
        sensor.AddObservation(transform.position);

        sensor.AddObservation(goalTransform.position);

        sensor.AddObservation(GetClosestUsableLadder().position);

    }

    // public override void Heuristic(in ActionBuffers actionsOut)
    // {

    //     base.Heuristic(actionsOut);

    // }

    public override void OnEpisodeBegin()
    {
        
        transform.position = new Vector2(-5.25f, -5.25f);

    }

    private Transform GetClosestUsableLadder() {
        
        Transform closestUsableLadder = null;

        foreach (Transform ladder in ladderTransforms) {

            if((ladder.transform.position.y - (ladder.GetComponent<Collider2D>().bounds.size.y/2)) - transform.position.y < 0.15f) { //checks if player is within height to reach ladder

            /*(ladder.transform.position.y - (ladder.GetComponent<Collider2D>().bounds.size.y/2)) provides the lowest extent of the ladder collider, thus the lowest reachable point for player*/
                if(closestUsableLadder is null) {

                    closestUsableLadder = ladder;

                }else if(Vector2.Distance(ladder.transform.position, transform.position) < Vector2.Distance(closestUsableLadder.position, transform.position)) {

                    closestUsableLadder = ladder;

                }

            }

        }

        return closestUsableLadder;

    }

    private void OnCollisionEnter2D(Collision2D collision) {

        if(collision.transform.gameObject.layer == LayerMask.NameToLayer("Ladder")) {

            AddReward(+5f);

        }

        if(collision.transform.tag == "Barrier") {

            AddReward(-20f);

        }


    }

}
