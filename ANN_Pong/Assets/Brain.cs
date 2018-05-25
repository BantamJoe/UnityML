using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour 
{
	public GameObject paddle;
	public GameObject ball;
	Rigidbody2D brb; //Ball's rigid body, so that the brain can figure out its position and movement direction
	float yvel;      // The Y-velocity of the paddle - the value we want the ANN to output
	float paddleMinY = 8.8f;
	float paddleMaxY = 17.4f;
	float paddleMaxSpeed = 15;
	public float numSaved = 0;  // Number of balls that we hit using the paddle
	public float numMissed = 0; //Number of balls that hit the back wall

	/*
	Inputs into the ANN: BallX, BallY, BallVelocityX, BallVelocityY, PaddleX, PaddleY
	Output of ANN: PaddleVelocityY
	*/
	ANN ann;

	// Use this for initialization
	void Start () 
	{
		// 6 inputs, 1 output, 1 hidden layer, 4 neurons per hidden layer, learning rate = 0.11
		ann = new ANN(6,1,1,4,0.11);
		brb = ball.GetComponent<Rigidbody2D>();		
	}

	//Run the ANN, that can train or calculate output based on a boolean
	List<double> Run(double bx, double by, double bvx, double bvy, double px, double py, double pv, bool train)
	{
		List<double> inputs = new List<double>();
		List<double> outputs = new List<double>();

		//Add the inputs to the inputs list
		inputs.Add(bx);
		inputs.Add(by);
		inputs.Add(bvx);
		inputs.Add(bvy);
		inputs.Add(px);
		inputs.Add(py);
		//Ad the expected output to outputs list
		outputs.Add(pv);

		//Call function according to boolean flag
		if (train)
		{
			return (ann.Train(inputs, outputs));
		}
		else
		{
			return (ann.CalcOutput(inputs, outputs));
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Position where we want to move the paddle to. Here yvel is what we are calculating from the ANN.
		//Since the ANN is using tanh activation, it will give us a value between -1 and 1, with the sign indicating whether to move up or down.
		float posy = Mathf.Clamp(paddle.transform.position.y + (yvel * Time.deltaTime * paddleMaxSpeed), paddleMinY, paddleMaxY);
		paddle.transform.position = new Vector3(paddle.transform.position.x, posy, paddle.transform.position.z);

		//List for output values that we can feed to the ANN and receive what it spits out
		List<double> output = new List<double>();

		//Raycasting is going to tell us exactly where the ball is going to hit. The ANN needs to know this when we train it, it should be told where
		// it should have been at the time. The difference in positions is used to calculate the error.
		int layerMask = 1 << 9; //backwall is on layer 9

		//Raycast in the direction of ball velocity
		RaycastHit2D hit = Physics2D.Raycast(ball.transform.position, brb.velocity, 1000, layerMask);

		//If we hit the top or the bottom, we will work out the reflection vector of the ball velocity
		if (hit.collider != null)
		{
			if (hit.collider.gameObject.tag == "tops")
			{
				Vector3 reflection = Vector3.Reflect(brb.velocity, hit.normal);
				hit = Physics2D.Raycast(hit.point, reflection, 1000, layerMask);
			}		

			if (hit.collider != null && hit.collider.gameObject.tag == "backwall") // If the raycast hits backwall
			{
				// delta y between hit point on back wall and paddle
				float dy = (hit.point.y - paddle.transform.position.y);

				// Train the ANN and get the output
				output = Run(ball.transform.position.x, ball.transform.position.y, brb.velocity.x, brb.velocity.y,
							paddle.transform.position.x, paddle.transform.position.y, dy, true); 
				
				//Set the yvel to the output
				yvel = (float) output[0];
			}
		}
		else
			yvel = 0; // The paddle is not going to move in this case		
	}


}
