using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Drive : MonoBehaviour {

	public float speed = 50.0F;
    public float rotationSpeed = 100.0F;
    public float visibleDistance = 200.0f;
    List<string> collectedTrainingData = new List<string>();
    StreamWriter tdf;

    void Start()
    {
        //Create file to hold training data
    	string path = Application.dataPath + "/trainingData.txt";
    	tdf = File.CreateText(path);
    }

    // When the application stops playing, we write training data to file
    void OnApplicationQuit()
    {
    	foreach(string td in collectedTrainingData)
        {
        	tdf.WriteLine(td);
        }
        tdf.Close();
    }

    // Helper function to round off values to the nearest .5
    float Round(float x) 
    {  	
     	return (float)System.Math.Round(x, System.MidpointRounding.AwayFromZero) / 2.0f;
 	}

    //Called every frame
    void Update() 
    {
        //Standard rotation and translation using input axis values for movement of the kart
        float translationInput = Input.GetAxis("Vertical");
        float rotationInput = Input.GetAxis("Horizontal");
        float translation = Time.deltaTime * speed * translationInput;
        float rotation = Time.deltaTime * rotationSpeed * rotationInput;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
        

		//Raycasts        
        // forward, right, left, right 45 deg, left 45 deg distances for raycasting
        float fDist = 0, rDist = 0, lDist = 0, r45Dist = 0, l45Dist = 0; 
        
        //Perform raycasts using these distances as out parameters
        Utils.PerformRayCasts(out fDist, out rDist, out lDist, out r45Dist, out l45Dist, this.transform, visibleDistance);
        
        //Use the returned distances to create the string of training data row
        string td = fDist + "," + rDist + "," + lDist + "," + 
                      r45Dist + "," + l45Dist + "," + 
                      Round(translationInput) + "," + Round(rotationInput); 

        //Add training data row to the training data list while avoiding duplicate values
        if(!collectedTrainingData.Contains(td))
        {
            collectedTrainingData.Add(td);
        }
        
    }
}
