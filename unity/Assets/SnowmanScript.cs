using UnityEngine;
using System.Collections;


public class SnowmanScript : ParticleSprite {
	
	const float BottomRad  = 3.0f;
	const float MiddleRad  = 2.0f;
	const float TopRad     = 1.0f;
	const float NoseLength = 1.0f;
	const float NoseAngle  = 0.25f;

	public float SnowmanSpeed = 20.0f;
	public float SnowmanTurnRate = 1.0f;
	public float SightDistance = 25.0f;
	public float SightAngle = 0.3f;

	float m_Angle;
	Vector3 m_TargetPos;
	bool m_HasSeenSanta = false;

	// Use this for initialization
	new void Start ()
	{
		base.Start();
		m_Angle = 0.0f;
		PickNewTarget();
	}

	private void SpawnParticles()
	{
		//m_Angle = (int)Time.realtimeSinceStartup;

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

		Vector3 nosePos = new Vector3(Mathf.Cos(m_Angle) * (.75f*TopRad + NoseLength), Mathf.Sin(m_Angle) * (.75f*TopRad + NoseLength), 0f);
		for(i = 0; i < noseParticles; i++)
		{
			dp.DrawParticleInArc(topPos + nosePos, NoseLength, -m_Angle, NoseAngle, DrawParticles.ParticlePlane.PLANE_Z, new Color(1.0f, 0.5f, 0.0f));
		}



	}

	void PickNewTarget()
	{
		
		m_TargetPos = Random.insideUnitCircle * 150;
	}

	void TrackSanta()
	{
		GameObject santa = GameObject.FindGameObjectWithTag("Santa");
		if(null != santa)
		{
			if(!m_HasSeenSanta)
			{
				Vector3 toSanta = santa.transform.position - transform.position;

				float angleToSanta = Mathf.Atan2(toSanta.y, toSanta.x);
				float angleDiff = (m_Angle - angleToSanta + Mathf.PI*2.0f)%(Mathf.PI*2.0f);
				//Debug.Log(angleDiff);
				if (angleDiff < SightAngle || angleDiff > Mathf.PI*2.0f - SightAngle)
				{
					Debug.DrawLine(transform.position, santa.transform.position, Color.blue);
					if (toSanta.magnitude < SightDistance)
					{
						m_HasSeenSanta = true;
						m_TargetPos = santa.transform.position;

					}
				}
			}
			else
			{
				m_TargetPos = santa.transform.position;
			}

		}
	}

	void DoSnowmanBrain()
	{
		Vector3 toTarget = m_TargetPos - transform.position;
		float angleToTarget = Mathf.Atan2(toTarget.y,toTarget.x);
		float angleDiff = (m_Angle - angleToTarget + Mathf.PI*2.0f)%(Mathf.PI*2.0f);
		float frameTurn = SnowmanTurnRate * Time.deltaTime;
		float frameMove = SnowmanSpeed * Time.deltaTime;
		Vector3 facingVector = new Vector3(Mathf.Cos(m_Angle), Mathf.Sin(m_Angle), 0.0f);

		if(angleDiff < frameTurn)
		{
			m_Angle = angleToTarget;
		}
		else
		{
			if(angleDiff > Mathf.PI)
			{
				m_Angle += frameTurn;
			}
			else
			{
				m_Angle -= frameTurn;
			}
		}

		if(angleDiff < 0.2f)
		{
			transform.position += frameMove * facingVector;
		}

		if(!m_HasSeenSanta && toTarget.magnitude < 2.0f*frameMove) PickNewTarget();

		Debug.DrawLine(transform.position, m_TargetPos, Color.cyan);
		Debug.DrawLine(transform.position, transform.position + facingVector*SightDistance, Color.magenta);
		Debug.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(m_Angle + SightAngle), Mathf.Sin(m_Angle + SightAngle), 0.0f)*SightDistance, Color.magenta);
		Debug.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(m_Angle - SightAngle), Mathf.Sin(m_Angle - SightAngle), 0.0f)*SightDistance, Color.magenta);
		Debug.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(angleToTarget), Mathf.Sin(angleToTarget), 0.0f)*SightDistance, Color.yellow);



	}

	// Update is called once per frame
	void Update () {
	
		if(!IsDead())
		{
			TrackSanta();
			DoSnowmanBrain();
			SpawnParticles();
		}
	}
}
