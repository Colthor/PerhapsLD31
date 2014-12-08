using UnityEngine;
using System.Collections;

public class ParticleSprite : MonoBehaviour {

	protected DrawParticles dp;
	public float maxParticlesPerSecond=2500;
	protected float m_Health;
	private bool m_IsDead;
	
	private float m_Leftovers;
	private float m_ParticleCount;

	public bool IsDead()
	{
		return m_IsDead;
	}

	public void AdjustHealth(float adjustment)
	{
		if(!m_IsDead)
		{
			m_Health += adjustment;
			if(m_Health > 100.0f) m_Health = 100.0f;
			if(m_Health <= 0.0f)
			{
				m_Health = 0.0f;
				m_IsDead = true;
			}
		}
	}

	// Use this for initialization
 virtual protected void Start ()
	{
		m_Health = 100.0f;
		m_IsDead = false;
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
