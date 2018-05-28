using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ANNDrive : MonoBehaviour {

    ANN ann;
    public float visibleDistance = 200;
    public int epochs = 2000;
    public float speed = 50.0F;
    public float rotationSpeed = 100.0F;

    bool trainingDone = false;
    float trainingProgress = 0;
    double sse = 0;
    double lastSSE = 1; 

    public float translation;
    public float rotation;

    public bool loadFromFile = false;

	// Use this for initialization
	void Start () {
		ann = new ANN(5,2,1,10,0.05);
		if(loadFromFile)
        {
			LoadWeightsFromFile();
			trainingDone = true;
        }
        else
        	StartCoroutine(LoadTrainingSet());
	}

    //GUI code to print training values to screen
    void OnGUI()
    {
        GUI.Label (new Rect (25, 25, 250, 30), "SSE: " + lastSSE);
        GUI.Label (new Rect (25, 40, 250, 30), "Alpha: " + ann.alpha);
        GUI.Label (new Rect (25, 55, 250, 30), "Trained: " + trainingProgress);
    }

    // Method to perform the ANN training using data collected from the player.
    IEnumerator LoadTrainingSet()
    {

        string path = Application.dataPath + "/trainingData.txt";
        string line;
        if(File.Exists(path))
        {
            int lineCount = File.ReadAllLines(path).Length;
            StreamReader tdf = File.OpenText(path);

            List<double> calcOutputs = new List<double>();
            List<double> inputs = new List<double>();
            List<double> outputs = new List<double>();

            //Loop through the epochs
            for(int i = 0; i < epochs; i++)
            { 
                //set file pointer to beginning of file
                sse = 0;
                tdf.BaseStream.Position = 0;

                //Get the current weight comma separated string values from the ANN object
                string currentWeights = ann.PrintWeights();

                // Load the training data, line by line
                while((line = tdf.ReadLine()) != null)  
                {  
                    string[] data = line.Split(',');
                    //if nothing to be learned ignore this line
                    float thisError = 0;

                    //We are leaving out those data where we have training labels or y (translation and rotation values) with values zero to reduce the data set.
                    if(System.Convert.ToDouble(data[5]) != 0 && System.Convert.ToDouble(data[6]) != 0) //If translation and rotation outputs in training data are not zero
                    {
                        //Clear out lists from previous row
                        inputs.Clear();
                        outputs.Clear();

                        //Add training input data to the inputs to the ANN
                        inputs.Add(System.Convert.ToDouble(data[0]));
                        inputs.Add(System.Convert.ToDouble(data[1]));
                        inputs.Add(System.Convert.ToDouble(data[2]));
                        inputs.Add(System.Convert.ToDouble(data[3]));
                        inputs.Add(System.Convert.ToDouble(data[4]));

                        //Map labels to range (0,1) for efficient training
                        double o1 = Map(0, 1, -1, 1, System.Convert.ToSingle(data[5]));
                        outputs.Add(o1);
                        double o2 = Map(0, 1, -1, 1, System.Convert.ToSingle(data[6]));
                        outputs.Add(o2);

                        //Calculated output (y-hat)
                        calcOutputs = ann.Train(inputs,outputs);
                        //Sum squared Error value: for both labels 
                        thisError = ((Mathf.Pow((float)(outputs[0] - calcOutputs[0]),2) +
                            Mathf.Pow((float)(outputs[1] - calcOutputs[1]),2)))/2.0f;
                    }
                    //Add this to cumulative SSE for the epoch
                    sse += thisError;
                } 

                //Percentage training to display on screen
                trainingProgress = (float)i/(float)epochs;
                
                // Average SSE
                sse /= lineCount;
                
                //If sse isn't better then reload previous set of weights and decrease alpha. This adaptive training to let the ANN move out 
                // of local optima and hence find global optima.
                if(lastSSE < sse)
                {
                	ann.LoadWeights(currentWeights);
                	ann.alpha = Mathf.Clamp((float)ann.alpha - 0.001f,0.01f,0.9f);
                }
                else //increase alpha
                {
                	ann.alpha = Mathf.Clamp((float)ann.alpha + 0.001f,0.01f,0.9f);
                	lastSSE = sse;
                }

                yield return null; //Allow OnGUI some time to update on-screen values
            }
                
        }
        //Training done, save weights
        trainingDone = true;
        SaveWeightsToFile();
    }

    // Save training weights to files after training the ANN
    void SaveWeightsToFile()
    {
        string path = Application.dataPath + "/weights.txt";
        StreamWriter wf = File.CreateText(path);
        wf.WriteLine (ann.PrintWeights());
        wf.Close();
    }

    // Load weights from file: Used when we have a good trained model, the agent can use these weights to move the kart
    void LoadWeightsFromFile()
    {
    	string path = Application.dataPath + "/weights.txt";
    	StreamReader wf = File.OpenText(path);

        if(File.Exists(path))
        {
        	string line = wf.ReadLine();
        	ann.LoadWeights(line);
        }
    }

    //Map Helper function: converts value from range (origfro, origto) to (newfrom, newto)
    float Map (float newfrom, float newto, float origfrom,float origto, float value) 
    {
    	if (value <= origfrom)
        	return newfrom;
    	else if (value >= origto)
        	return newto;
    	return (newto - newfrom) * ((value - origfrom) / (origto - origfrom)) + newfrom;
	}

    //Round helper function
    float Round(float x) 
    {   
        return (float)System.Math.Round(x, System.MidpointRounding.AwayFromZero) / 2.0f;
    }

    //Called every frame
    void Update() 
    {
        //If the ANN has not been trained, return
        if(!trainingDone) return;

        //Create lists for calculated outputs, inputs and outputs list (placeholder requirement for our ANN implementation)
        List<double> calcOutputs = new List<double>();
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();

        //raycasts        
        float fDist = 0, rDist = 0, lDist = 0, r45Dist = 0, l45Dist = 0;
        Utils.PerformRayCasts(out fDist,out  rDist,out  lDist,out  r45Dist,out  l45Dist, this.transform, visibleDistance);        

        //Add the raycast hit distances returned to the list as ANN inputs
        inputs.Add(fDist);
        inputs.Add(rDist);
        inputs.Add(lDist);
        inputs.Add(r45Dist);
        inputs.Add(l45Dist);

        //Add zeros to output list; We are going to receive these values from the trained ANN. These are just placeholders.
        outputs.Add(0);
        outputs.Add(0);

        // Run the ANN and calculate output values for movement from the trained ANN
        calcOutputs = ann.CalcOutput(inputs,outputs);

        //Standard movement code, using ANN output values
        float translationInput = Map(-1,1,0,1,(float) calcOutputs[0]);
        float rotationInput = Map(-1,1,0,1,(float) calcOutputs[1]);
        translation = translationInput * speed * Time.deltaTime;
        rotation = rotationInput * rotationSpeed * Time.deltaTime;
        this.transform.Translate(0, 0, translation);
        this.transform.Rotate(0, rotation, 0);        

    }
}
