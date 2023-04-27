using UnityEngine;

public class Barrel : MonoBehaviour
{
    private new Rigidbody2D rigidbody;

    [SerializeField] private Transform clearBarrelColliderTransform;


    private bool onRightSide;

    public float speed = 1f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        onRightSide = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            rigidbody.AddForce(collision.transform.right * speed, ForceMode2D.Impulse);
        }
    }

    void Update() {

        if(rigidbody.velocity.x > 0f && onRightSide) {

            clearBarrelColliderTransform.localPosition = new Vector3(-clearBarrelColliderTransform.localPosition.x, clearBarrelColliderTransform.localPosition.y);

            onRightSide = false;

        } else if(rigidbody.velocity.x < 0f && !onRightSide) {

            clearBarrelColliderTransform.localPosition = new Vector3(-clearBarrelColliderTransform.localPosition.x, clearBarrelColliderTransform.localPosition.y);

            onRightSide = true;
        
        }

    }

    void OnTriggerEnter2D(Collider2D collider) {


        if(collider.gameObject.CompareTag("Player")) {

            GameObject.FindObjectOfType<SpeedRunAgentBARREL>().CallAddReward(0.075f);

        }

    }

}
