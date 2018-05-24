using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour 
{
	ANN ann;
	double sumSquareError = 0; //How closely the model fits the data

	// Use this for initialization
	void Start () 
	{
		ann = new ANN(2, 1, 1, 2, 0.8); 

		//Stores the result of each line in the training set
		List<double> result;

		//Loop over the epochs
		for (int i = 0; i< 1000; i++)
		{
			sumSquareError = 0;

			//Training set for the XOR operation
			result = Train(1,1,0);
			sumSquareError += Mathf.Pow((float)result[0]-0, 2 ); //desired result is 0
			result = Train(1,0,1);
			sumSquareError += Mathf.Pow((float)result[0]-1, 2 ); //desired result is 1
			result = Train(0,1,1);
			sumSquareError += Mathf.Pow((float)result[0]-1, 2 );
			result = Train(0,0,0);
			sumSquareError += Mathf.Pow((float)result[0]-0, 2 );
		}
		//Print out the final sum of squared errors after training epochs
		Debug.Log("SSE: " + sumSquareError);

		//Run and test the neural network
		result = Train(1,1,0);
		Debug.Log(" 1 1 " + result[0]);
		result = Train(1,0,1);
		Debug.Log(" 1 0 " + result[0]);
		result = Train(0,1,1);
		Debug.Log(" 0 1 " + result[0]);
		result = Train(0,0,0);
		Debug.Log(" 0 0 " + result[0]);		
	}

	List<double> Train(double i1, double i2, double o)
	{
		List<double> inputs = new List<double>();
		List<double> outputs = new List<double>();
		inputs.Add(i1);
		inputs.Add(i2);
		outputs.Add(o);
		return (ann.Go(inputs, outputs));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
