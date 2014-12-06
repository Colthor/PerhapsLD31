using UnityEngine;
using System.Collections;

public class ParticleSprite : MonoBehaviour {

	protected DrawParticles dp;
	public float maxParticlesPerSecond=250;
	protected float m_Health;
	
	private float m_Leftovers;
	private float m_ParticleCount;

	// Use this for initialization
 virtual protected void Start ()
	{
		m_Health = 100.0f;
		dp = GameObject.FindWithTag("_Globals").GetComponent<DrawParticles>();
		m_Leftovers = 0;
	}

	protected float GetFrameParticles()
	{
		m_ParticleCount = m_Leftovers + Time.deltaTime * maxParticlesPerSecond * (0.008f*m_Health + .2f) * dp.GetParticleDensity();
		return m_ParticleCount;
	}

	protected void SetLeftovers(int ParticlesRendered)
	{
		m_Leftovers = m_ParticleCount - (float)ParticlesRendered;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
