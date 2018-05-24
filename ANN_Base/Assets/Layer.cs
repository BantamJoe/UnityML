using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer 
{
	public int numNeurons; //The number of neurons in this layer
	public List<Neuron> neurons = new List<Neuron>(); //The list of neurons added to this layer
	
	public Layer(int nNeurons, int numNeuronInputs) //numNeuronInputs equals the number of neurons in the previous layer
	{
		numNeurons = nNeurons;
		//Add numNeurons neurons to our neurons list
		for (int i = 0; i < nNeurons; i++)
		{
			neurons.Add(new Neuron(numNeuronInputs));
		}
	}
}
