using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//TODO remove the scenemanagement and add a real death to this game;
public class PlayerStateManager : MonoBehaviour
{
    State curState;
    Move move;
    Attack attack;
    // AttackManager attackMan;
    // bool attacked;
    void Start()
    {
        curState = State.Move;
        move = GetComponent<Move>();
        attack = GetComponent<Attack>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (curState)
        {
            case State.Move:

                break;
            
        }

        //MoveMent Switch
        switch (curState){
            case State.Move:
                curState = move.normalMove();   
                break;
            case State.Airborne:
                curState = move.airMove();
                break;
            case State.WallSlideL:
                curState = move.wallMove(1);
                break;
            case State.WallSlideR:
                curState = move.wallMove(-1);
                break;
            case State.Die:
                Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);                
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
    WallSlideL,
    WallSlideR,
    Die
}
