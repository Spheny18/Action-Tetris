using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO update the map to use some constants that can be defined somewhere else
public class Move : MonoBehaviour
{
    // Start is called before the first frame update
    private BoxCollider2D boxCollider;
    Vector2 velocity;
    public float speed = 9;
    public float walkAcceleration = 75;
    public float groundDeceleration = 70;
    public float jumpHeight = 25;
    public float fallingGravity = -75;
    public float jumpingGravity = -30;
    public float wallGravity = -10;
    public int recursiveCheck = 2;
    public float wallJumpHeight = 3;
    public float wallJumpKick = 2;
    LayerMask ignore;
    void Start()
    {
        velocity = Vector2.zero;
        boxCollider = GetComponent<BoxCollider2D>();
        ignore =~ LayerMask.GetMask("Ignore Raycast");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public State normalMove(){
        float moveInput = Input.GetAxisRaw("Horizontal");

        velocity.y = 0;
        bool jumping = false;
        if (Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(jumpingGravity));
            jumping = true;
        }

        velocity.y += fallingGravity * Time.deltaTime;

        if (moveInput != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput, walkAcceleration * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, groundDeceleration * Time.deltaTime);
        }

        MoveRecursive(velocity*Time.deltaTime,recursiveCheck,jumping);
        var collisions = Collisions();

        if(collisions["grounded"] && collisions["topCollision"]){
            return State.Die;
        } else if(collisions["grounded"]){
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

        velocity.y += gravity * Time.deltaTime;
        
        if (moveInput != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput, walkAcceleration * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, groundDeceleration * Time.deltaTime);
        }

        MoveRecursive(velocity*Time.deltaTime,recursiveCheck,false);
        
        var collisions = Collisions();

        if(collisions["grounded"] && collisions["topCollision"]){
            return State.Die;
        } else if(collisions["grounded"]){
            return State.Move;
        } else {
            if(collisions["wallSlide"]){
                return State.WallSlide;
            }
            return State.Airborne;
        }
    }

    public State wallMove(){

        bool jumping = false;
        float gravity = wallGravity;
        float moveInput = Input.GetAxisRaw("Horizontal");

        if(Input.GetButtonUp("Jump") && velocity.y > 0){
            velocity.y = 0;
        } else if(Input.GetButton("Jump") && velocity.y > 0){
            gravity = jumpingGravity;
        }

        velocity.x = moveInput * speed;

        if(Input.GetButtonDown("Jump")){
            velocity.y = Mathf.Sqrt(2 * wallJumpHeight * Mathf.Abs(jumpingGravity));
            velocity.x = Mathf.Sqrt(2 * wallJumpKick * Mathf.Abs(jumpingGravity));

            jumping = true;
        }

        
        velocity.y += gravity * Time.deltaTime;

        MoveRecursive(velocity*Time.deltaTime,recursiveCheck,jumping);

        var collisions = Collisions();

        if(collisions["grounded"] && collisions["topCollision"]){
            return State.Die;
        } else if(collisions["grounded"]){
            return State.Move;
        } else {
            if(collisions["wallSlide"]){
                return State.WallSlide;
            }
            return State.Airborne;
        }
    }

    Dictionary<string,bool> Collisions(){
        bool wallSlide = false;
        bool grounded = false;
        bool topCollision = false;
        var map = new Dictionary<string,bool>();

        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0,ignore);

        foreach (Collider2D hit in hits)
        {
            ColliderDistance2D colliderDistance = hit.Distance(boxCollider);

            if (colliderDistance.isOverlapped)
            {
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
                // Debug.Log(Vector2.Angle(colliderDistance.normal, Vector2.up));
                if(Vector2.Angle(colliderDistance.normal, Vector2.up) == 180){
                    topCollision = true;
                }
                if(Vector2.Angle(colliderDistance.normal, Vector2.up) == 90){
                    wallSlide = true;
                }
                if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90 && velocity.y < 0)
                {
                    grounded = true;
                }
            }
        }
        map.Add("grounded",grounded);
        map.Add("wallSlide",wallSlide);
        map.Add("topCollision",topCollision);
        return map;
    }

    void MoveRecursive(Vector2 velocity, int iterations, bool jumping){
        RaycastHit2D hit = Physics2D.BoxCast(transform.position,transform.lossyScale,0,velocity,(velocity*Time.deltaTime).magnitude,ignore);
        if(hit){
            Vector3 centroid = new Vector3(hit.centroid.x,hit.centroid.y,0) - transform.position;
            Vector2 tmp = new Vector2(centroid.x*hit.normal.x, centroid.y * hit.normal.y);
            Vector2 move = velocity * Time.deltaTime;

            if(tmp.x != 0){
                move.x = tmp.x;
            } 
            if(tmp.y != 0 && !jumping){
                move.y = tmp.y;
            }
            
            if(iterations > 0){
                hit.collider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                MoveRecursive(move, iterations-1, jumping);   
                hit.collider.gameObject.layer = LayerMask.NameToLayer("Default");
            } else {
                transform.Translate(move);
            }
        } else {
            transform.Translate(velocity);
        }
    }
}
