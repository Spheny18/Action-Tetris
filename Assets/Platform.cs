using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    LayerMask ground;
    bool collided;
    // Start is called before the first frame update
    void Start()
    {   
        ground = LayerMask.GetMask("Ground");
        collided = false;
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * 2 * Time.deltaTime);
        Collisions();
        if(collided){
            int layer = LayerMask.NameToLayer("Ground");
            gameObject.layer = layer;
            this.enabled = false;
        }
    }


    void Collisions(){
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, transform.lossyScale, 0, ground);

        foreach (Collider2D hit in hits)
        {
            if (hit == boxCollider)
                continue;

            collided = true;
            ColliderDistance2D colliderDistance = hit.Distance(boxCollider);

            if (colliderDistance.isOverlapped)
            {
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
                
            }
        }

    }
}
