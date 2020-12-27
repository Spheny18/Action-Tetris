using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    List<GameObject> inactive;
    List<GameObject> active;
    GameObject item;
    public ObjectPool(GameObject item)
    {
        this.item = item;
        inactive = new List<GameObject>();
        active = new List<GameObject>();
        for(int i = 0 ;i < 10; i++){
            inactive.Add(GameObject.Instantiate(item));
            inactive[i].SetActive(false);
        }
    }

    // Update is called once per frame
    public GameObject GetItem(){
        if(inactive.Count == 0){
            inactive.Add(GameObject.Instantiate(item));
        }
        GameObject obj = inactive[0];
        obj.SetActive(true);
        inactive.Remove(obj);
        active.Add(obj);
        return obj;
    }

    public void RemoveItem(GameObject obj){
        if(active.Contains(obj)){
            active.Remove(obj);
            inactive.Add(obj);
            obj.SetActive(false);
        }
        else{
            Debug.LogError("This object does not exist! | " + obj.name);
        }
    }
}
