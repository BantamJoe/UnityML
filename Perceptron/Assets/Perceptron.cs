using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrainingSet
{
	public double[] input;
	public double output;
}

public class Perceptron : MonoBehaviour 
{
	public TrainingSet[] ts;
	double[] weights = {0,0}; //Each weight has to be assigned to an input
	double bias = 0;
	double totalError = 0; //Will keep track of each epoch's errors during training

	//Dot Product
	double DotProductBias(double[] v1, double[] v2)
	{
		if (v1 == null || v2 == null)
			return -1;
		if (v1.Length != v2.Length) return -1;

		double d = 0;
		for (int x = 0; x < v1.Length; x++)
		{
			d += v1[x] * v2[x];
		}
		d += bias;

		return d;
	}

	//Activation Function: f(x) = 1 if w.x + b > 0, f(x) = 0 otherwise
	//int i is the line from the training set that we want to run through
	double CalcOutput(int i)
	{
		double dp = DotProductBias(weights, ts[i].input);
		if (dp > 0) return (1);
		return (0);
	}

	//Initialize weights
	void InitialiseWeights()
	{
		for (int i = 0; i < weights.Length; i++)
		{
			weights[i] = Random.Range(-1.0f, 1.0f);
		}
		bias = Random.Range(-1.0f, 1.0f);
	}

	//Takes the number of the line in the training set and works on it
	void UpdateWeights(int j)
	{
		//Calculate error: difference between the output and the calculated error, will be used to update the weights
		double error = ts[j].output - CalcOutput(j);
		//Add up the total errors to see every epoch what the total error is
		totalError += Mathf.Abs((float)error);

		//Update the weight values
		for (int i = 0; i < weights.Length; i++)
		{
			// old value plus error multiplied by that particular input
			weights[i] = weights[i] +  error * ts[j].input[i];
		}
		// Bias does not have an input, so it is updated with the error
		bias += error;
	}

	//This is us asking the perceptron for an OR operation output after it has been trained
	double CalcOutput(double i1, double i2)
	{
		double[] inp = new double[] {i1, i2};
		double dp = DotProductBias(weights, inp);
		if (dp > 0) return (1);
		return (0);
	}

	//Train our perceptron
	void Train(int epochs)
	{
		InitialiseWeights();

		for (int e = 0; e < epochs; e++)
		{
			totalError = 0;
			for (int t=0; t < ts.Length; t++)
			{
				UpdateWeights(t);
				Debug.Log("W1: " + weights[0] + " W2: " + weights[1] + " B: " + bias);
			}
			Debug.Log("Total Error: " + totalError);
		}		
	}

	// Use this for initialization
	void Start () 
	{
		Train(8);

		//After training, we can test our perceptron
		//Here we test each of the cases of the OR operation
		Debug.Log("Test 0 0: " + CalcOutput(0, 0));
		Debug.Log("Test 0 1: " + CalcOutput(0, 1));
		Debug.Log("Test 1 0: " + CalcOutput(1, 0));
		Debug.Log("Test 1 1: " + CalcOutput(1, 1));
		
	}
	
	
}
