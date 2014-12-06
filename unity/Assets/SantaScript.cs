using UnityEngine;
using System.Collections;

public class SantaScript : ParticleSprite {

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
		int bodyParticles = (int)(particleCount * 0.5f);
		int headParticles = (int)(particleCount * 0.25f);
		int hatParticles = (int)(particleCount * 0.25f);

		SetLeftovers(bodyParticles + hatParticles + headParticles);
		
		int i = 0;
		for(i = 0; i < bodyParticles; i++)
		{
			dp.DrawParticleOnSphere(transform.position, BodyRad, Color.red);
		}

		Vector3 headPos = new Vector3(0f, 0f, -5.0f) + transform.position;
		for(i = 0; i < headParticles; i++)
		{
			dp.DrawParticleOnSphere(headPos, HeadRad, Color.gray);
		}

		Vector3 hatPos = new Vector3(0,0,-10) + transform.position;
		for(i = 0; i < hatParticles; i++)
		{
			dp.DrawParticleInArc(hatPos, 4.0f, 0.0f, 0.8f, DrawParticles.ConePlane.CONE_Y, Color.red);
		}
	}

	// Update is called once per frame
	void Update ()
	{
		SpawnParticles();
	}
}
