using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class movableBlocks : MonoBehaviour
{
    // animate the game object from min to max and back along y and x axis
    [Header("Movement toggle")]
    public bool yAxis = true; 
    public bool xAxis = false; 
    private bool xAndYAxis = false; 

    [Header("Movement away from current location")]
    public float minX = 0.0f;
    public float maxX =  0.0f;
    public float minY = -1.0f;
    public float maxY = 1.0f;
    public float changeSpeed = 0.5f;

    [Header("rotation")]
    public bool debug = false;
    public bool enableRotation = false;
    public float rotationSpeed = 5;
    private float angleToRotateTo = 0;

    private Vector3 addLocalPosition;

    // starting value for the Lerp
    private float tX = 0.0f;
    private float tY = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        addLocalPosition = transform.localPosition;

        if(yAxis && xAxis){
            xAndYAxis = true;
        }

        if(debug){
            Debug.Log("local position " + transform.localPosition);
            Debug.Log("position " + transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // animate the position of the game object...
        //Debug.Log("local position " + transform.localPosition); 
        //Debug.Log("position " + transform.position);
        
        if(xAndYAxis){
            transform.localPosition = new Vector3(Mathf.Lerp(minX, maxX, tX), Mathf.Lerp(minY, maxY, tY), 0) + addLocalPosition; // combo mombo
        }

        else if (yAxis){
            transform.localPosition = new Vector3(0, Mathf.Lerp(minY, maxY, tY), 0) + addLocalPosition; // inside a room so change its position (local) relative to the parent
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
        if (tY > 1.0f)
        {
            float temp = maxY;
            maxY = minY;
            minY = temp;
            tY = 0.0f;
        }

        if (tX > 1.0f)
        {
            float temp = maxX;
            maxX = minX;
            minX = temp;
            tX = 0.0f;
        }

        // rotation below
        if (enableRotation){
            transform.rotation = Quaternion.Euler(0, 0, angleToRotateTo );

            angleToRotateTo += rotationSpeed * Time.deltaTime;
        }

        
        


    }


}
