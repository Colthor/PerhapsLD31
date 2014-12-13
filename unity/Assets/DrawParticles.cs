using UnityEngine;
using System.Collections;

public class DrawParticles : MonoBehaviour {

	public float ParticleDensity	= 1.0f;
	public float ParticleLifespan 	= 1.0f;
	public float ParticleSize 		= 0.9f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		/*for(int i = 0; i < 10; i++)
		{
			DrawParticleOnSphere(new Vector3(0,0,0), 3.0f, Color.yellow);
			DrawParticleInArc(new Vector3(0,0,0), 10.0f, (int)(Time.realtimeSinceStartup * 0.95f), .3f, ConePlane.CONE_Z, Color.blue);
			
			DrawParticleOnSphere(new Vector3(10,0,0), 3.0f, Color.magenta);
			DrawParticleInArc(new Vector3(10,0,0), 10.0f, (int)(Time.realtimeSinceStartup * 0.95f), .3f, ConePlane.CONE_Y, Color.green);

			DrawParticleOnSphere(new Vector3(-10,0,0), 3.0f, Color.cyan);
			DrawParticleInArc(new Vector3(-10,0,0), 10.0f, (int)(Time.realtimeSinceStartup * 0.95f), .3f, ConePlane.CONE_X, Color.red);
		}*/
	}

	public enum ParticlePlane
	{
		PLANE_X,
		PLANE_Y,
		PLANE_Z
	};

	public float GetParticleDensity()
	{
		return ParticleDensity;
	}

	public Color RandomColour()
	{
		Color rv = new Color(0,0,0);
		bool unset = true;
		if(Random.Range(0, 2) < 1)
		{
			rv.r = 1.0f;
			unset = false;
		}
		if(Random.Range(0, 2) < 1)
		{
			rv.g = 1.0f;
			unset = false;
		}
		if(Random.Range(0, 2) < 1)
		{
			rv.b = 1.0f;
			unset = false;
		}
		if(unset) rv.r = rv.b = rv.g = 1.0f;
		
		return rv;
	}

	public void DrawParticleWithVel(Vector3 pos, float radius, float angle, float velocity, ParticlePlane plane, Color colour)
	{
		float dist = Random.Range(0, radius);

		Vector3 localPos, Vel;
		float x = Mathf.Sin(angle);
		float y =  Mathf.Cos(angle);

		switch(plane)
		{
		case ParticlePlane.PLANE_X:
			localPos.x = 0;
			localPos.y = x;
			localPos.z = y;
			break;
		case ParticlePlane.PLANE_Y:
			localPos.x = x;
			localPos.y = 0;
			localPos.z = y;
			break;
		default:
			localPos.x = x;
			localPos.y = -y;
			localPos.z = 0;
			break;
		}

		Vel = localPos * velocity;
		localPos *= dist;


		particleSystem.Emit(pos + localPos, Vel, ParticleSize, ParticleLifespan, colour);
	}

	public void DrawPermanentParticleInRadius(Vector3 pos, float radius, Color colour)
	{
		Vector3 localPos = Random.insideUnitCircle * radius;

		particleSystem.Emit(pos + localPos, Vector3.zero, ParticleSize*3.0f, 200.0f, colour);
	}

	public void DrawParticleInArc(Vector3 pos, float radius, float angle, float arcWidthAngle, ParticlePlane plane, Color colour, float emphasise = 1.0f, float MinRangeSqrted = 0.0f)
	{
		float actualAngle = angle + Random.Range(-0.5f*arcWidthAngle, 0.5f*arcWidthAngle);
		float dist = Random.Range(0.0f, 1f - MinRangeSqrted);
		dist = (1.0f - dist*dist) * radius; // Note: probably not perfectly uniform, but there aren't any sqrt()s
		
		Vector3 localPos;
		float x =dist * Mathf.Sin(actualAngle);
		float y =dist * Mathf.Cos(actualAngle);
		
		switch(plane)
		{
		case ParticlePlane.PLANE_X:
			localPos.x = 0;
			localPos.y = x;
			localPos.z = y;
			break;
		case ParticlePlane.PLANE_Y:
			localPos.x = x;
			localPos.y = 0;
			localPos.z = y;
			break;
		default:
			localPos.x = x;
			localPos.y = -y;
			localPos.z = 0;
			break;
		}
		
		
		particleSystem.Emit(pos + localPos, new Vector3(0,0,0), ParticleSize * emphasise, ParticleLifespan * emphasise, colour);
	}


	public void DrawParticleOnSphere(Vector3 pos, float radius, Color colour, float emphasise = 1.0f, bool negativeZOnly = false)
	{

		float theta = Random.Range (0f, 6.2831853f);
		float phi = Mathf.Acos( Random.Range (-1.0f, 1.0f)); // for uniform distribution on sphere.
		Vector3 localPos;
		localPos.x = Mathf.Sin(phi) * Mathf.Cos(theta) * radius;
		localPos.y = Mathf.Sin (phi) * Mathf.Sin (theta) * radius;
		localPos.z = Mathf.Cos(phi) * radius;
		if(negativeZOnly && localPos.z > 0) localPos.z *=-1.0f;
		
		particleSystem.Emit(pos + localPos, new Vector3(0,0,0), ParticleSize*emphasise, ParticleLifespan*emphasise, colour);

	}
}
