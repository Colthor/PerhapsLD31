using UnityEngine;
using System.Collections;

public class TreeScript : ParticleSprite {

	bool m_IsLit = false;

	// Use this for initialization
	new void Start ()
	{
		base.Start();
	}

	private void SpawnParticles()
	{
		float particleCount = GetFrameParticles();
		int bottomParticles = (int)(particleCount * 0.50f);
		int middleParticles = (int)(particleCount * 0.28f);
		int topParticles    = (int)(particleCount * 0.12f);
		int i = 0;
		
		SetLeftovers(bottomParticles + middleParticles + topParticles);
		
		for(i = 0; i < bottomParticles; i++)
		{
			dp.DrawParticleInArc(transform.position, 6.5f, 0.0f, 1.8f, DrawParticles.ConePlane.CONE_Y, Color.green);
		}

		Vector3 middlePos = new Vector3(0f, 0f, -4.5f) + transform.position;
		for(i = 0; i < middleParticles; i++)
		{
			dp.DrawParticleInArc(middlePos, 5.5f, 0.0f, 1.5f, DrawParticles.ConePlane.CONE_Y, Color.green);
		}

		Vector3 topPos = new Vector3(0f, 0f, -7.5f) + transform.position;
		for(i = 0; i < topParticles; i++)
		{
			dp.DrawParticleInArc(topPos, 3.5f, 0.0f, 1.25f, DrawParticles.ConePlane.CONE_Y, Color.green);
		}

		
		if(m_IsLit)
		{
			//Draw star and lights
		}
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		SpawnParticles();
	}
}
