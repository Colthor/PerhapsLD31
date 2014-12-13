using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour {

	public TreeScript TreePrefab;
	public SnowmanScript SnowmanPrefab;

	public float TreeLightDistance = 7.0f;
	public float BurnDistance = 20.0f;
	public float BurnPerSecond = 20.0f;
	public float HealPerSecond = 33.0f;
	public float WindMoveRadius = 100.0f;
	public float WindMoveRadsPerSec = 0.5f;
	public float TreeCircleRadius = 80.0f;
	public float SnowmanCircleRadius = 100.0f;
	public int   SnowmanCount = 7;

	bool WonGame = false;
	float WonTime = 0.0f;
	float CameraWinMoveTime = 2.0f;
	Vector3 WonCameraStart;
	Vector3 WonCameraEnd = new Vector3(0f, -40f, -40f);
	Color StartLight = Color.black;
	Color StartCameraBG = Color.black;
	Color EndLight = new Color(1.0f, 1.0f, 0.9f);
	bool EndingFinished = false;

	private GameObject Santa;
	private GameObject MainCam;

	// Use this for initialization
	void Start () {
		MainCam = GameObject.FindGameObjectWithTag("MainCamera");
		Santa = GameObject.FindGameObjectWithTag("Santa");

		SpawnTrees();
		SpawnSnowmen();
	}

	//Setup functions

	void SpawnTrees()
	{
		Instantiate(TreePrefab, new Vector3(0,0,0), Quaternion.identity);

		for(float i = 0.25f; i <= 2.25f; i+=0.4f)
		{
			Vector3 pos = new Vector3(Mathf.Cos(Mathf.PI*i)*TreeCircleRadius, Mathf.Sin(Mathf.PI*i)*TreeCircleRadius, 0.0f);
			Instantiate(TreePrefab, pos, Quaternion.identity);
		}
	}

	void SpawnSnowmen()
	{
		float minSantaDistSquared = 33f*33f;
		Vector3 santaPos = Santa.transform.position;
		Vector3 snowmanPos;
		for(int i = 0; i < SnowmanCount; i++)
		{
			do
			{
				snowmanPos = Random.insideUnitCircle * SnowmanCircleRadius;
			} while( (snowmanPos - santaPos).sqrMagnitude < minSantaDistSquared );
			Instantiate(SnowmanPrefab, snowmanPos, Quaternion.identity);
		}
	}

	void WinGame()
	{
		WonGame = true;
		WonTime = Time.time;
		WonCameraStart = MainCam.transform.position;
		GameObject.FindGameObjectWithTag("Snow").particleSystem.enableEmission = false;
		StartLight = RenderSettings.ambientLight;
		StartCameraBG = MainCam.camera.backgroundColor;
	}

	
	// Update is called once per frame
	void Update () {
		UpdateCameraPos();
		if(!WonGame)
		{
			PlayGame();
		}
		else if (!EndingFinished)
		{
			PlayEnding();
		}
	}
	private void UpdateCameraPos()
	{
		if(WonGame)
		{
			float camPosScale = Mathf.Min (1.0f, (Time.time - WonTime)/CameraWinMoveTime);
			MainCam.transform.position = WonCameraStart + (WonCameraEnd - WonCameraStart)*camPosScale;
		}
		else
		{
			Vector3 newPos = Santa.transform.position;
			newPos.y -= 40.0f;
			newPos.z -= 40.0f;
			MainCam.transform.position = newPos;
		}

	}

	private void UpdateWindPos()
	{
		Vector3 pos = new Vector3(Mathf.Sin(WindMoveRadsPerSec * Time.realtimeSinceStartup), Mathf.Cos(WindMoveRadsPerSec * Time.realtimeSinceStartup), 0.0f);
		transform.position = pos * WindMoveRadius;
	}

	private float DistanceScale(float distance, float maxDistance)
	{
		return (1.0f - Mathf.Min(1.0f, distance/maxDistance)) * Time.deltaTime;
	}

	//Main game loop
	private void PlayGame()
	{
		UpdateWindPos();

		SantaScript santaScr = Santa.GetComponent<SantaScript>();
		GameObject[] Snowmen = GameObject.FindGameObjectsWithTag("Snowman");
		GameObject[] Trees = GameObject.FindGameObjectsWithTag("Tree");
		float BurnDistSquared = BurnDistance * BurnDistance;
		int unlitTrees = 0;

		foreach(GameObject treeGO in Trees)
		{
			TreeScript tree = treeGO.GetComponent<TreeScript>();
			float distToSantaSquared =  (Santa.transform.position - treeGO.transform.position).sqrMagnitude;
			unlitTrees++;

			if( distToSantaSquared < TreeLightDistance*TreeLightDistance) //Santa lights trees if he gets close
			{
				tree.SetLit(true);
			}

			if(tree.IsLit())
			{
				unlitTrees--;
				if(distToSantaSquared < BurnDistSquared) //Trees heal Santa if he's nearby
				{
					//these are all nonsense... Want to scale from 0 at distance to max at close, but this isn't how to do that!
					santaScr.AdjustHealth( HealPerSecond * DistanceScale( distToSantaSquared, BurnDistSquared) ); //Nonlinear, but who cares.
				}
			}

			foreach(GameObject smGO in Snowmen)
			{
				SnowmanScript snowman = smGO.GetComponent<SnowmanScript>();
				float smDistToSantaSquared =  (Santa.transform.position - smGO.transform.position).sqrMagnitude;
				float smDistToTreeSquared =  (smGO.transform.position - treeGO.transform.position).sqrMagnitude;

				if(smDistToSantaSquared < BurnDistSquared) //Snowmen burn Santa with cold
				{
					santaScr.AdjustHealth( -BurnPerSecond * DistanceScale( smDistToSantaSquared, BurnDistSquared) ); //Nonlinear, but who cares.
				}

				if(tree.IsLit() && smDistToTreeSquared < BurnDistSquared) //Trees melt snowmen if they're lit
				{
					snowman.AdjustHealth( -BurnPerSecond * DistanceScale( smDistToTreeSquared, BurnDistSquared) ); //Nonlinear, but who cares.
				}

			}
		}//foreach

		if(!santaScr.IsDead() && unlitTrees == 0)
		{
			WinGame();
		}
	}

	private void DestroyObsInRadius(float radius)
	{
		GameObject[] Trees = GameObject.FindGameObjectsWithTag("Tree");
		if(Santa != null && Santa.transform.position.magnitude < radius)
		{
			Destroy(Santa);
			Santa = null;
		}
		foreach(GameObject treeGO in Trees)
		{
			if(treeGO.transform.position.magnitude < radius) Destroy(treeGO);
		}
		GameObject[] Snowmen = GameObject.FindGameObjectsWithTag("Snowman");
		foreach(GameObject smGO in Snowmen)
		{
			if(smGO.transform.position.magnitude < radius) Destroy(smGO);
		}
	}

	//Play ending effects
	private void PlayEnding()
	{
		float EndingStart = WonTime + 1.5f;
		float ExplosionLength = 7.0f;
		float ExplosionTime = EndingStart + ExplosionLength;
		float ExplosionRadius = 100.0f;
		Color GrassColour=new Color(0.25f, 0.9f, 0.25f);
		if(Time.time > EndingStart)
		{
			DrawParticles dp = this.GetComponent<DrawParticles>();
			if(Time.time < ExplosionTime)
			{
				float scale = (Time.time - EndingStart)/ExplosionLength;
				float curRad = scale * ExplosionRadius;
				DestroyObsInRadius(curRad);

				RenderSettings.ambientLight = StartLight + (EndLight - StartLight)*scale;
				//MainCam.camera.backgroundColor = StartCameraBG + (GrassColour - StartCameraBG)*scale;

				for(int i = 0; i < (50+500*scale*scale)*dp.GetParticleDensity(); i++)
				{
					dp.DrawParticleOnSphere(Vector3.zero, curRad, new Color(1.0f, 1.0f, 0.9f), 3.0f, true);
				}
			}
			else /*if( Time.time < ExplosionTime + 1000.0f )*/
			{
				if(Time.time < ExplosionTime + ExplosionLength)
				{
					float scale = (Time.time - ExplosionTime)/ExplosionLength;
					MainCam.camera.backgroundColor = StartCameraBG + (GrassColour - StartCameraBG)*scale;
				}

				GameObject.FindGameObjectWithTag("Ground").particleSystem.startColor = GrassColour;
				this.transform.position = new Vector3(500f, 500f, 500f);
				if(Random.Range(0, 100) < 2)
				{
					dp.DrawPermanentParticleInRadius(new Vector3(0f, 0f, -1.0f), 100.0f, dp.RandomColour());
					//dp.DrawParticleOnSphere(new Vector3(0f, 0f, -1.0f), 100.0f, dp.RandomColour(), 3.0f, true);
				}
			}
			/*else
			{
				EndingFinished = true;
			}*/

		}
	}
}
