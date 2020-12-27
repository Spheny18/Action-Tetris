using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    ObjectPool platformPool;
    GameObject platform;
    public float timeBetween = 6f;
    public float minX = 8f;
    public float maxX = 20f;
    public float minY = 3f;
    public float maxY = 10f;
    public float range = 20f;
    public List<RangeCords> rangeCords;

    float counter;
    
    void Start()
    {
        rangeCords = new List<RangeCords>();
        counter = 0;
        platform = Resources.Load<GameObject>("Platform");
        platformPool = new ObjectPool(platform);
        platformPool.GetItem().transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        if(counter >= timeBetween){
            // Debug.Log(rangeCords);
            bool spawned = true;
            float scaleX = Random.Range(minX,maxX);
            float scaleY = Random.Range(minY,maxY);
            float posX = Random.Range(-range + scaleX/2, range - scaleX/2);
            foreach(RangeCords cords in rangeCords){
                if((posX + scaleX/2 < cords.maxX && posX + scaleX/2 > cords.minX) || (posX - scaleX/2 < cords.maxX && posX - scaleX/2 > cords.minX)){
                    spawned = false;
                }
            }
            if(spawned){
            GameObject obj = platformPool.GetItem();
            obj.transform.position = new Vector3(posX,20,0);
            obj.transform.localScale = new Vector3(scaleX,scaleY,0); 
            counter = 0;
            // Debug.Log((posX - scaleX/2) + " | " + (posX + scaleX/2));
            rangeCords.Add(new RangeCords(posX - scaleX/2,posX + scaleX/2,10));
            }
        }
        for(int i = 0; i < rangeCords.Count;i++){
            rangeCords[i].timer -= Time.deltaTime;
            if(rangeCords[i].timer <= 0){
                rangeCords.Remove(rangeCords[i]);
            }
        }
    }
}

public class RangeCords{
    public float minX { get; set; }
    public float maxX { get; set; }
    public float timer { get; set; }
    public RangeCords(float minX, float maxX, float timer){
        this.minX = minX;
        this.maxX = maxX;
        this.timer = timer;
    }
}