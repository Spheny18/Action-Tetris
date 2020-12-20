using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    State curState;
    Move move;
    // AttackManager attackMan;
    // bool attacked;
    void Start()
    {
        curState = State.Move;
        move = GetComponent<Move>();

        //tryState = State
    }

    // Update is called once per frame
    void Update()
    {
        // Vector2 attackInputs = getAttackInputs();
        // Debug.Log("pre switch: " + curState);
        switch (curState){
            case State.Move:
                curState = move.normalMove();
               
                break;
            
            case State.Airborne:
                curState = move.airMove();
                break;
            case State.WallSlide:

                break;
        }
    }
    
    Vector2 getMoveInputs(){
        Vector2 v = Vector2.zero;
        v.x = Input.GetAxisRaw("Horizontal");
        v.y = Input.GetAxisRaw("Vertical");
        return v;
    }

    // Vector2 getAttackInputs(){
    //     Vector2 v = Vector2.zero;
    //     v.x = Input.GetAxisRaw("FireHorizontal");
    //     v.y = Input.GetAxisRaw("FireVertical");
    //     return v;
    // }
}
public enum State{
    Move,
    Airborne,
    WallSlide
}
