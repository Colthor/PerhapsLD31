using UnityEngine;
using System.Collections;

public class SantaScript : ParticleSprite {

	public float MoveSpeed = 15.0f;

	const float BodyRad 	= 3.0f;
	const float HeadRad 	= 1.5f;
	const float HatRad 		= 4.0f;
	const float HatAngle 	= 0.8f;
	// Use this for initialization
	new void Start ()
	{
		base.Start();
	}

	private void SpawnParticles()
	{
		
		float particleCount = GetFrameParticles();
		int bodyParticles = (int)(particleCount * 0.45f);
		int beltParticles = (int)(particleCount * 0.15f);
		int hatParticles = (int)(particleCount * 0.2f);
		int legParticles = (int)(particleCount * 0.2f);

		SetLeftovers(bodyParticles + hatParticles + beltParticles + legParticles);
		
		int i = 0;
		for(i = 0; i < bodyParticles; i++)
		{
			dp.DrawParticleOnSphere(transform.position, BodyRad, Color.red);
		}

		Vector3 beltPos = new Vector3(0f, -3f, -7.5f) + transform.position;
		for(i = 0; i < beltParticles; i++)
		{
			dp.DrawParticleInArc(beltPos, 6.5f, 0.0f, 1.0f, DrawParticles.ParticlePlane.PLANE_Y, Color.white, 1.0f, 0.9f);
		}


		Vector3 hatPos = new Vector3(0,0.0f,-9.0f) + transform.position;
		for(i = 0; i < hatParticles; i++)
		{
			switch(Random.Range(0, 6))
			{
			case 0:
				dp.DrawParticleOnSphere(hatPos, 0.5f, Color.white);
				break;
			case 1:
			case 2:
				dp.DrawParticleInArc(hatPos, 4.0f, 0.0f, 0.8f, DrawParticles.ParticlePlane.PLANE_Y, Color.white, 1.0f, 0.9f);
				break;
			default:
				dp.DrawParticleInArc(hatPos, 3.8f, 0.0f, 0.8f, DrawParticles.ParticlePlane.PLANE_Y, Color.red);
				break;
			}
		}

		Vector3 legPos = new Vector3(0f, 0f, 5.5f);// + transform.position;
		for(i = 0; i < legParticles / 2; i++)
		{
			legPos.x = Random.Range(-2.0f, -0.5f);
			dp.DrawParticleWithVel(legPos + transform.position, 2.5f, Mathf.PI, 0.0f, DrawParticles.ParticlePlane.PLANE_Y, Color.red);
			legPos.x = Random.Range(2.0f, 0.5f);
			dp.DrawParticleWithVel(legPos + transform.position, 2.5f, Mathf.PI, 0.0f, DrawParticles.ParticlePlane.PLANE_Y, Color.red);

		}
	}

	private void DoMovement()
	{
		Vector3 MoveDir = new Vector3(0,0,0);
		if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			MoveDir.x = -1.0f;
		}
		else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			MoveDir.x = 1.0f;
		}

		if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
		{
			MoveDir.y = 1.0f;
		}
		else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		{
			MoveDir.y = -1.0f;
		}

		MoveDir.Normalize();
		this.transform.position += MoveDir * MoveSpeed * Time.deltaTime;
	}


	// Update is called once per frame
	void Update ()
	{
		if(!IsDead())
		{
			DoMovement();
			SpawnParticles();
		}
	}
}
