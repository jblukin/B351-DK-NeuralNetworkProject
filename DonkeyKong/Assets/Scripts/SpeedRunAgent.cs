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

    private Vector3 priorFramePosition;

    private int currentPlatform;

    private void Start() {

        playerRef = FindObjectOfType<PlayerAI>();

        currentPlatform = CheckCurrentPlatform();

    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {

        //base.OnActionReceived(actions);

        int decision = actions.DiscreteActions[0];

        if (decision == 0) {

            //run right
            if(!playerRef.climbing) {

                playerRef.direction.x = playerRef.moveSpeed;

            }

        } else if (decision == 1) {

            //run left
            if(!playerRef.climbing) {

                playerRef.direction.x = -playerRef.moveSpeed;

            }

        } else if (decision == 2) {

            //AddReward(-50f);

            //jump right
            if(playerRef.grounded) {

                playerRef.direction = new Vector2(playerRef.moveSpeed, playerRef.jumpStrength/1.5f);

            }

        } else if (decision == 3) {

            //AddReward(-50f);

            //jump left
            if(playerRef.grounded) {

                playerRef.direction = new Vector2(-playerRef.moveSpeed, playerRef.jumpStrength/1.5f);

            }

        }
        
         else if (decision == 4) {

            if(playerRef.climbing) {

                AddReward(+0.5f);

                playerRef.direction = new Vector2(0.0f, playerRef.moveSpeed);

            }

        }

        StartCoroutine("SavePriorFrameYPosition");

        //Debug.Log(priorFramePosition.y >= transform.position.y);


        if(priorFramePosition.y >= transform.position.y) { //Loss of height penalty
        
            AddReward(-0.05f);

        }

        if((currentPlatform = CheckCurrentPlatform()) == 1)
            /*0.05 Max Added Reward*/ AddReward((1 - Mathf.Abs(ladderTransforms[0].position.x - transform.position.x) / Mathf.Abs(ladderTransforms[0].position.x - -5.25f))/20); //Normalized x distance vector between startPos and Ladder1
        else if((currentPlatform = CheckCurrentPlatform()) == 2)
            /*0.05 Max Added Reward*/ AddReward((1 - Mathf.Abs(ladderTransforms[1].position.x - transform.position.x) / Mathf.Abs(ladderTransforms[1].position.x - ladderTransforms[0].position.x))/20); //Normalized x distance vector between Ladder1 and Ladder2
        else if((currentPlatform = CheckCurrentPlatform()) == 3)
            /*0.05 Max Added Reward*/ AddReward((1 - Mathf.Abs(ladderTransforms[2].position.x - transform.position.x) / Mathf.Abs(ladderTransforms[2].position.x - ladderTransforms[1].position.x))/20); //Normalized x distance vector between Ladder2 and Ladder3
        else if((currentPlatform = CheckCurrentPlatform()) == 4)
            /*0.05 Max Added Reward*/ AddReward((1 - Mathf.Abs(ladderTransforms[3].position.x - transform.position.x) / Mathf.Abs(ladderTransforms[3].position.x - ladderTransforms[2].position.x))/20); //Normalized x distance vector between Ladder3 and Ladder4
        else if((currentPlatform = CheckCurrentPlatform()) == 5)
            /*0.05 Max Added Reward*/ AddReward((1 - Mathf.Abs(ladderTransforms[4].position.x - transform.position.x) / Mathf.Abs(ladderTransforms[4].position.x - ladderTransforms[3].position.x))/20); //Normalized x distance vector between Ladder5 and Ladder6
        else if((currentPlatform = CheckCurrentPlatform()) == 6)
            /*0.05 Max Added Reward*/ AddReward((1 - Mathf.Abs(ladderTransforms[5].position.x - transform.position.x) / Mathf.Abs(ladderTransforms[5].position.x - ladderTransforms[4].position.x))/20); //Normalized x distance vector between Ladder1 and Ladder2

        /*0.025 Max Added Reward*/ AddReward((1 - (Mathf.Abs(goalTransform.position.y - transform.position.y) / Mathf.Abs(goalTransform.position.y - -5.25f)))/40); //The normal of the distance (in Y terms only - height) from the player to the goal

        if(goalTransform.position.y <= transform.position.y || CheckCurrentPlatform() == 7) {

            SetReward(+10f);
            EndEpisode();

        }

        AddReward(-0.05f); //Speed penalty

        if(this.StepCount == this.MaxStep) {
            
            
            EndEpisode();

        }

    }

    public override void CollectObservations(VectorSensor sensor)
    {

        //base.CollectObservations(sensor);
        sensor.AddObservation(transform.position);

        sensor.AddObservation(goalTransform.position);

        //sensor.AddObservation(GetClosestUsableLadder().position);

        sensor.AddObservation(currentPlatform);

    }

    // public override void Heuristic(in ActionBuffers actionsOut)
    // {

    //     base.Heuristic(actionsOut);

    // }

    public override void OnEpisodeBegin()
    {
        
        transform.position = new Vector2(-5.25f, -5.25f);

    }

    private int CheckCurrentPlatform() {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y-0.5f), 0.15f);

        foreach (Collider2D collider in colliders) {

            if(collider.name.Equals("Platform1"))
                return 1;
            else if(collider.name.Equals("Platform2"))
                return 2;
            else if(collider.name.Equals("Platform3"))
                return 3;
            else if(collider.name.Equals("Platform4"))
                return 4;
            else if(collider.name.Equals("Platform5"))
                return 5;
            else if(collider.name.Equals("Platform6"))
                return 6;
            else if(collider.name.Equals("Platform7"))
                return 7;

        }

        return currentPlatform;

    }

    private Transform GetClosestUsableLadder() { //Deprecated
        
        Transform closestUsableLadder = null;

        foreach (Transform ladder in ladderTransforms) {

            if((ladder.transform.position.y - (ladder.GetComponent<Collider2D>().bounds.size.y/2)) - transform.position.y < 0.4f) { //checks if player is within height to reach ladder

            /*(ladder.transform.position.y - (ladder.GetComponent<Collider2D>().bounds.size.y/2)) provides the lowest extent of the ladder collider, thus the lowest reachable point for player*/
                if(closestUsableLadder is null) {

                    closestUsableLadder = ladder;

                }else if(Vector2.Distance(ladder.transform.position, transform.position) < Vector2.Distance(closestUsableLadder.position, transform.position)) {

                    closestUsableLadder = ladder;

                }

            }

        }

        Debug.Log(closestUsableLadder.name);

        return closestUsableLadder;

    }

    private void OnCollisionEnter2D(Collision2D collision) {

        if(collision.transform.gameObject.layer == LayerMask.NameToLayer("Ladder")) {

            AddReward(+10f);

        }

        if(collision.transform.tag == "Barrier") {

            AddReward(-200f);

        }


    }

    IEnumerator SavePriorFrameYPosition() 
    {

        yield return new WaitForEndOfFrame();
        priorFramePosition = transform.position;

    }

}
