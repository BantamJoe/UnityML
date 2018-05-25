using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour 
{
	Vector3 ballStartPosition;
	Rigidbody2D rb;
	float speed = 400f;
	public AudioSource blip;
	public AudioSource blop;

	// Use this for initialization
	void Start () 
	{
		rb = this.GetComponent<Rigidbody2D>();
		ballStartPosition = this.transform.position;
		ResetBall(); //Put the ball in the starting position and push it away		
	}

	//On collision enter
	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "backwall")
			blop.Play(); //Play when it hits back wall
		else
			blip.Play(); //Play this when it hits any other game object
	}

	//Reset the ball
	public void ResetBall()
	{
		this.transform.position = ballStartPosition;
		rb.velocity = Vector3.zero;
		//Give the ball a random direction in the 2D plane
		Vector3 dir = new Vector3(Random.Range(100,300), Random.Range(-100,100), 0).normalized;
		//Add force in the new direction
		rb.AddForce(dir * speed);
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Reset ball when spacebar is pressed
		if (Input.GetKeyDown("space"))
		{
			ResetBall();
		}
		
	}
}
