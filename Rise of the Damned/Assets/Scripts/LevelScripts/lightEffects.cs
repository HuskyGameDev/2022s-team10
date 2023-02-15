using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal; // add to ref light2D

public class lightEffects : MonoBehaviour
{
    // animate the game object from min to max and back along y and x axis
    [Header("Movement toggle")]
    public bool yAxis = true; 
    public bool xAxis = false; 
    private bool xAndYAxis = false; 

    [Header("Movement away from current location")]
    public float minX = 0;
    public float maxX =  0;
    public float minY = -1.0f;
    public float maxY = 1.0f;
    public float changeSpeed = 0.5f;

    [Header("Flicker")]
    public bool flicker = true; 
    public float flickerIntensity = 3;
    public float lowestIntensity = 0;
    public float flickerSpeed = 1;
    public float delayTime = 0;
    private float t = 0.0f; //increment value for mathf.pingpong

    private Vector3 addLocalPosition;

    private Light2D candleLight;

    // starting value for the Lerp
    private float tX = 0.0f;
    private float tY = 0.0f;

    void Start(){
        addLocalPosition = transform.localPosition;

        candleLight = GetComponent<Light2D>();

        if(yAxis && xAxis){
            xAndYAxis = true;
        }

        t -= delayTime;

        flickerIntensity -= lowestIntensity; // account for the lowest intensity being added
    }

    void Update(){

        // animate the position of the game object...
        //Debug.Log("local position " + transform.localPosition);
        //Debug.Log("position " + transform.position);

        if(xAndYAxis){
            transform.localPosition = new Vector3(Mathf.Lerp(minX, maxX, tX), Mathf.Lerp(minY, maxY, tY), 0) + addLocalPosition; // combo mombo
        }

        else if (yAxis){
            transform.localPosition = new Vector3(0, Mathf.Lerp(minY, maxY, tY), 0) + addLocalPosition; // local position 
        }

        else if (xAxis){
            transform.localPosition = new Vector3(Mathf.Lerp(minX, maxX, tX), 0, 0) + addLocalPosition; // local position 
        }

        // .. and increase the t interpolater
        tY += changeSpeed * Time.deltaTime;
        tX += changeSpeed * Time.deltaTime;

        // now check if the interpolator has reached 1.0
        // and swap maximum and minimum so game object moves
        // in the opposite direction.
        if (tY > 1.0f){
            float temp = maxY;
            maxY = minY;
            minY = temp;
            tY = 0.0f;
        }

        if (tX > 1.0f){
            float temp = maxX;
            maxX = minX;
            minX = temp;
            tX = 0.0f;
        }

        // flicker candle light
        if (flicker) {   
            flickerLight();
        }

        

    }

    public void flickerLight(){

        if (t < 0){
            t += flickerSpeed * Time.deltaTime;
            return;
        }

        candleLight.intensity = Mathf.PingPong( t, flickerIntensity); // 0 to intensity 
        candleLight.intensity += lowestIntensity;

        t += flickerSpeed * Time.deltaTime;
    }

   
    


}
