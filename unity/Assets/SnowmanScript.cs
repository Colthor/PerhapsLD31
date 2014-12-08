using UnityEngine;
using System.Collections;


public class SnowmanScript : ParticleSprite {
	
	const float BottomRad  = 3.0f;
	const float MiddleRad  = 2.0f;
	const float TopRad     = 1.0f;
	const float NoseLength = 1.0f;
	const float NoseAngle  = 0.25f;

	float m_Angle;

	// Use this for initialization
	new void Start ()
	{
		base.Start();
		m_Angle = 0.0f;
	}

	private void SpawnParticles()
	{
		m_Angle = (int)Time.realtimeSinceStartup;

		float particleCount = GetFrameParticles();
		int bottomParticles = (int)(particleCount * 0.50f);
		int middleParticles = (int)(particleCount * 0.28f);
		int topParticles    = (int)(particleCount * 0.12f);
		int noseParticles   = (int)(particleCount * 0.10f);
		int i = 0;

		SetLeftovers(bottomParticles + middleParticles + topParticles + noseParticles);

		for(i = 0; i < bottomParticles; i++)
		{
			dp.DrawParticleOnSphere(transform.position, BottomRad, Color.white);
		}
		Vector3 middlePos = new Vector3(0f, 0f, -4.5f) + transform.position;
		for(i = 0; i < middleParticles; i++)
		{
			dp.DrawParticleOnSphere(middlePos, MiddleRad, Color.white);
		}
		Vector3 topPos = new Vector3(0f, 0f, -7.5f) + transform.position;
		for(i = 0; i < topParticles; i++)
		{
			dp.DrawParticleOnSphere(topPos, TopRad, Color.white);
		}

		Vector3 nosePos = new Vector3(Mathf.Sin(m_Angle) * (TopRad + NoseLength), Mathf.Cos(m_Angle) * (TopRad + NoseLength), 0f);
		for(i = 0; i < noseParticles; i++)
		{
			dp.DrawParticleInArc(topPos + nosePos, NoseLength, -m_Angle, NoseAngle, DrawParticles.ParticlePlane.PLANE_Z, new Color(1.0f, 0.5f, 0.0f));
		}



	}

	// Update is called once per frame
	void Update () {
	
		if(!IsDead()) SpawnParticles();
	}
}
