using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Constructs all of the layers and neurons in the neural network and also trains it.
 */
public class ANN 
{
	public int numInputs; //Number of inputs into the NN
	public int numOutputs; //Number of outputs of the NN
	public int numHidden; //Number of hidden layers
	public int numNPerHidden; //Number of neurons per hidden layer
	public double alpha; //Learning rate
	List<Layer> layers = new List<Layer>();

	public ANN(int nI, int nO, int nH, int nPH, double a)
	{
		numInputs = nI;
		numOutputs = nO;
		numHidden = nH;
		numNPerHidden = nPH;
		alpha = a;

		if (numHidden > 0) //If we have hidden layers
		{
			//Add our input layer
			layers.Add(new Layer(numNPerHidden, numInputs));

			//Create our hidden layers (2 less than NH, as one will be input layer and the other the output layer)
			for (int i = 0; i < numHidden - 1; i++)
			{
				layers.Add(new Layer(numNPerHidden, numNPerHidden)); //Number of neurons and number of neurons coming in from the previous layer
			}

			//Create our output layer
			layers.Add(new Layer(numOutputs, numNPerHidden)); //Number of outputs, number of inputs = number of neurons in previous layer
		}
		else
		{
			layers.Add(new Layer(numInputs, numOutputs)); //Just a single layer if no hidden layers are specified.
		}
	}

	//Method to run; returns a list of outputs.
	//Parameter inputValues: List of inputs
	//Paramater desiredOutput: List of "Labels" used in training
	public List<double> Go(List<double> inputValues, List<double> desiredOutput)
	{
		//Inputs and outputs that we will keep track of for each neuron
		List<double> inputs = new List<double>();
		List<double> outputs = new List<double>();

		//Test if we have the correct number of input values
		if (inputValues.Count != numInputs)
		{
			Debug.Log(" Error: Number of inputs must be " + numInputs);
			return outputs;
		}

		inputs = new List<double>(inputValues);
		//Loop through each of the layers
		for (int i = 0; i < numHidden + 1; i++)
		{
			if (i > 0) //If this is not the first layer
			{
				//Set the inputs to the layer to the outputs of the previous layer
				inputs = new List<double>(outputs);
			}
			//Clear the outputs
			outputs.Clear();

			//Loop through the number of neurons of the current (ith) layer
			for (int j = 0; j < layers[i].numNeurons; j++)
			{
				double N = 0; //Product of weights and inputs
				//Clear the inputs to this particular neuron
				layers[i].neurons[j].inputs.Clear();

				//Loop through the current neuron's inputs
				for (int k = 0; k < layers[i].neurons[j].numInputs; k++)
				{
					//Add the input (if hidden layer, then inputs we initialised on Line 68)
					layers[i].neurons[j].inputs.Add(inputs[k]);

					//Multiply the weights and inputs for the particular input number k
					//Essentially the dot product
					N += layers[i].neurons[j].weights[k] * inputs[k];
				}

				//Add the negative bias
				N -= layers[i].neurons[j].bias;

				//Set the ouput for the neuron using activation function
				layers[i].neurons[j].output = ActivationFunction(N);
				//Add the output of this neuron to the list of outputs
				outputs.Add(layers[i].neurons[j].output);
			}
		}
		//We have run our network and calculated error, so we now update weights
		UpdateWeights(outputs, desiredOutput);
		return outputs;
	}

	//Again loop through and update weights for better fit
	void UpdateWeights(List<double> outputs, List<double> desiredOutput)
	{
		double error;
		//Loop through the layers
		for (int i = numHidden; i >= 0; i--) //Backpropagate through the layers
		{
			//Loop through the neurons
			for (int j=0; j < layers[i].numNeurons; j++)
			{
				if (i == numHidden) //If we are at the end or the output layer
				{
					error = desiredOutput[j] - outputs[j];
					//Calculate error gradient for the neuron using the delta rule. It assigns the amount of error contributed by the neuron 
					layers[i].neurons[j].errorGradient = outputs[j] * (1 - outputs[j]) * error; 
				}
				else
				{
					layers[i].neurons[j].errorGradient = layers[i].neurons[j].output * ( 1 - layers[i].neurons[j].output);
					double errorGradSum = 0; //error in the layer above this particular layer

					//Loop through the neurons in the layer after it and also add up those error gradients, and add it to the error gradient sum of current neuron
					for (int p = 0; p < layers[i +1].numNeurons; p++ )
					{
						errorGradSum += layers[i+1].neurons[p].errorGradient * layers[i+1].neurons[p].weights[j]; 
					}
					layers[i].neurons[j].errorGradient *= errorGradSum;
				}

				//Loop through the inputs for the particular neuron
				for (int k = 0; k < layers[i].neurons[j].numInputs; k++)
				{
					if (i == numHidden)
					{
						error = desiredOutput[j] - outputs[j];
						//update the weight using the error value
						layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * error;
					}
					else
					{
						//Update the weight using the error gradient
						layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * layers[i].neurons[j].errorGradient;
					}
				}
				//Update the bias
				layers[i].neurons[j].bias += alpha * -1 * layers[i].neurons[j].errorGradient;
			}
		}
	}

	//Our activation function
	double ActivationFunction(double value)
	{
		return Sigmoid(value);
	}

	//The step function
	double Step(double value)
	{
		if (value <0) return 0;
		else return 1;
	}

	//Sigmoid function
	double Sigmoid(double value)
	{
		double k = (double) System.Math.Exp(value);
		return k / (1.0f + k);
	}
}
