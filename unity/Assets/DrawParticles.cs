using UnityEngine;
using System.Collections;

public class DrawParticles : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < 5; i++)
		{
			DrawParticleOnSphere(new Vector3(0,0,0), 3.0f, Color.red);
		}
	}


	public void DrawParticleOnSphere(Vector3 pos, float radius, Color colour)
	{

		float theta = Random.Range (0f, 6.2831853f);
		float phi = Mathf.Acos( Random.Range (-1.0f, 1.0f));
		Vector3 localPos;
		localPos.x = Mathf.Sin(phi) * Mathf.Cos(theta) * radius;
		localPos.y = Mathf.Sin (phi) * Mathf.Sin (theta) * radius;
		localPos.z = Mathf.Cos(phi) * radius;
		
		particleSystem.Emit(pos + localPos, new Vector3(0,0,0), 1.0f, 3.0f, colour);

	}
}
