using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{

	static float Round(float x) 
    {  	
     	return (float)System.Math.Round(x, System.MidpointRounding.AwayFromZero) / 2.0f;
 	}

	public static string PerformRayCasts(out float f,out float r,out float l, out float r45,out float l45, Transform t, float visibleDistance)
	{
        //Draw debug rays
		Debug.DrawRay(t.position, t.forward * visibleDistance, Color.red);
        Debug.DrawRay(t.position, t.right * visibleDistance, Color.red);        
        Debug.DrawRay(t.position, Quaternion.AngleAxis(-45, Vector3.up) * t.right * visibleDistance, Color.green);        
        Debug.DrawRay(t.position, Quaternion.AngleAxis(45, Vector3.up) * -t.right * visibleDistance, Color.green);

		RaycastHit hit;
		f = r = l = r45 = l45 = 0; //Initialize raycast hit distances to zero

		//forward raycast, then normalize, round and assign the hit distance 
        if (Physics.Raycast(t.position, t.forward, out hit, visibleDistance))
        {
            f = 1-Round(hit.distance/visibleDistance);
        }

        //right
        if (Physics.Raycast(t.position, t.right, out hit, visibleDistance))
        {
            r = 1-Round(hit.distance/visibleDistance);
        }

        //left
        if (Physics.Raycast(t.position, -t.right, out hit, visibleDistance))
        {
            l = 1-Round(hit.distance/visibleDistance);
        }

        //right 45
        if (Physics.Raycast(t.position, 
                            Quaternion.AngleAxis(-45, Vector3.up) * t.right, out hit, visibleDistance))
        {
            r45 = 1-Round(hit.distance/visibleDistance);
        }

        //left 45
        if (Physics.Raycast(t.position, 
                            Quaternion.AngleAxis(45, Vector3.up) * -t.right, out hit, visibleDistance))
        {
            l45 = 1-Round(hit.distance/visibleDistance);
        }

        //Return the comma separated string of training data
        return (f + "," + r + "," + l + "," + 
        	          r45 + "," + l45);
	}
}
