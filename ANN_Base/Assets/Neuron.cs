using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron 
{
	public int numInputs; //How many inputs are coming into the neuron
	public double bias; 
	public double output; //Output from the neuron
	public double errorGradient;
	public List<double> weights = new List<double>();
	public List<double> inputs = new List<double>();

	//Constructor: Takes the number of inputs, and adds weights and bias
	public Neuron(int nInputs)
	{
		bias = UnityEngine.Random.Range(-1.0f, 1.0f);
		numInputs = nInputs;
		for (int i = 0; i < nInputs; i++)
		{
			//For each input into the neuron, initialise a weight randomly
			weights.Add(UnityEngine.Random.Range(-1.0f, 1.0f));
		}
	}
	
}
