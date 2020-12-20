using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    // Start is called before the first frame update
    private BoxCollider2D boxCollider;
    Vector2 velocity;
    public float speed = 9;
    public float walkAcceleration = 75;
    public float groundDeceleration = 70;
    public float jumpHeight = 25;
    public float fallingGravity = -125;
    public float jumpingGravity = -50;
    LayerMask ignore;
    bool grounded;
    void Start()
    {
        velocity = Vector2.zero;
        boxCollider = GetComponent<BoxCollider2D>();
        ignore =~ LayerMask.GetMask("Ignore Raycast");
        // stats = GetComponent<Stats>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public State normalMove(){
        float moveInput = Input.GetAxisRaw("Horizontal");

        velocity.y = 0;

        if (Input.GetButtonDown("Jump"))
        {
                // Calculate the velocity required to achieve the target jump height.
            velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(jumpingGravity));
        }

        velocity.x = moveInput * speed;
        velocity.y += fallingGravity * Time.deltaTime;
        //BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance = Mathf.Infinity, int layerMask = Physics2D.AllLayers, float minDepth = -Mathf.Infinity, float maxDepth = Mathf.Infinity);
        RaycastHit2D hit = Physics2D.BoxCast(transform.position,transform.lossyScale,0,velocity,(velocity*Time.deltaTime).magnitude,ignore);
        if(hit){
            Vector3 move = new Vector3(hit.centroid.x,hit.centroid.y,0) - transform.position;
            transform.Translate(move);
        } else {
            transform.Translate(velocity * Time.deltaTime);
        }

        grounded = Collisions();

        if(grounded){
            return State.Move;
        } else {
            return State.Airborne;
        }
    }

    public State airMove(){

        float gravity = fallingGravity;
        float moveInput = Input.GetAxisRaw("Horizontal");

        if(Input.GetButtonUp("Jump") && velocity.y > 0){

                velocity.y = 0;
        } else if(Input.GetButton("Jump") && velocity.y > 0){
            gravity = jumpingGravity;
        }

        velocity.x = moveInput * speed;
        velocity.y += gravity * Time.deltaTime;
        //BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance = Mathf.Infinity, int layerMask = Physics2D.AllLayers, float minDepth = -Mathf.Infinity, float maxDepth = Mathf.Infinity);
        RaycastHit2D hit = Physics2D.BoxCast(transform.position,transform.lossyScale,0,velocity,(velocity*Time.deltaTime).magnitude,ignore);
        if(hit){
            Vector3 move = new Vector3(hit.centroid.x,hit.centroid.y,0) - transform.position;
            transform.Translate(move);
        } else {
            transform.Translate(velocity * Time.deltaTime);
        }

        grounded = Collisions();

        if(grounded){
            return State.Move;
        } else {
            return State.Airborne;
        }
    }

    bool Collisions(){
        
        
        grounded = false;

        // Retrieve all colliders we have intersected after velocity has been applied.
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);

        foreach (Collider2D hit in hits)
        {
            // Ignore our own collider.
            if (hit == boxCollider)
                continue;

            ColliderDistance2D colliderDistance = hit.Distance(boxCollider);

            // Ensure that we are still overlapping this collider.
            // The overlap may no longer exist due to another intersected collider
            // pushing us out of this one.
            if (colliderDistance.isOverlapped)
            {
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);

                Debug.Log( hit.gameObject.name + ": " + Vector2.Angle(colliderDistance.normal, Vector2.up));

                // If we intersect an object beneath us, set grounded to true. 
                if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90 && velocity.y < 0)
                {
                    grounded = true;
                }
            }
        }

        return grounded;
    }
}
