using UnityEngine;
using System.Collections;

public class TreeScript : ParticleSprite {

	public float LightEmphasis = 2.0f;
	bool m_IsLit = false;

	public void SetLit(bool alight)
	{
		m_IsLit = alight;
		this.GetComponentInChildren<Light>().enabled = m_IsLit;
	}

	public bool IsLit()
	{
		return m_IsLit;
	}

	// Use this for initialization
	new void Start ()
	{
		base.Start();
		SetLit(m_IsLit);
	}


	private void SpawnParticles()
	{
		float particleCount = GetFrameParticles();
		int bottomParticles = (int)(particleCount * 0.42f);
		int middleParticles = (int)(particleCount * 0.26f);
		int topParticles    = (int)(particleCount * 0.12f);
		int starParticles   = (int)(particleCount * 0.15f);
		int lightParticles  = (int)(particleCount * 0.05f);
		int i = 0;
		Color DullGreen = new Color(0, 0.75f, 0);
		
		SetLeftovers(bottomParticles + middleParticles + topParticles + starParticles + lightParticles);
		
		for(i = 0; i < bottomParticles; i++)
		{
			dp.DrawParticleInArc(transform.position, 6.5f, 0.0f, 1.8f, DrawParticles.ParticlePlane.PLANE_Y, DullGreen);
		}

		Vector3 middlePos = new Vector3(0f, 0f, -4.5f) + transform.position;
		for(i = 0; i < middleParticles; i++)
		{
			dp.DrawParticleInArc(middlePos, 5.5f, 0.0f, 1.5f, DrawParticles.ParticlePlane.PLANE_Y, DullGreen);
		}

		Vector3 topPos = new Vector3(0f, 0f, -7.5f) + transform.position;
		for(i = 0; i < topParticles; i++)
		{
			dp.DrawParticleInArc(topPos, 3.5f, 0.0f, 1.25f, DrawParticles.ParticlePlane.PLANE_Y, DullGreen);
		}

		
		if(m_IsLit)
		{
			//Draw star and lights
			
			for(i = 0; i < lightParticles; i++)
			{
				switch(Random.Range(0, 6))
				{
				case 0:
				case 1:
				case 2:
					dp.DrawParticleInArc(transform.position, 6.5f, 0.0f, 1.8f, DrawParticles.ParticlePlane.PLANE_Y, dp.RandomColour(), LightEmphasis);
					break;
				case 3:
				case 4:
					dp.DrawParticleInArc(middlePos, 5.5f, 0.0f, 1.5f, DrawParticles.ParticlePlane.PLANE_Y, dp.RandomColour(), LightEmphasis);
					break;
				case 5:
					dp.DrawParticleInArc(topPos, 3.5f, 0.0f, 1.25f, DrawParticles.ParticlePlane.PLANE_Y, dp.RandomColour(), LightEmphasis);
					break;
				}

				
			}

			float starPointAngle = Mathf.PI/2.5f;
			Vector3 starPos = new Vector3(0f, -0.2f, -0.2f) + topPos;

			for(i = 0; i < starParticles; i++)
			{
				dp.DrawParticleWithVel(starPos, 0.25f, Mathf.PI + i*starPointAngle, Random.Range(1.8f, 2.3f), DrawParticles.ParticlePlane.PLANE_Y, new Color(1.0f, 1.0f, 0.6f));
			}
		}
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!IsDead()) SpawnParticles();
	}
}
