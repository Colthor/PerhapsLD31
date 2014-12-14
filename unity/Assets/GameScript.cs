using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour {

	public TreeScript TreePrefab;
	public SnowmanScript SnowmanPrefab;
	public GameObject SantaPrefab;

	public float TreeLightDistance = 7.0f;
	public float BurnDistance = 20.0f;
	public float BurnPerSecond = 20.0f;
	public float MeltPerSecond = 50.0f;
	public float HealPerSecond = 5.0f;
	public float LifeDrainPerSecond = 5.0f;
	public float WindMoveRadius = 100.0f;
	public float WindMoveRadsPerSec = 0.5f;
	public float TreeCircleRadius = 80.0f;
	public float SnowmanCircleRadius = 100.0f;
	public int   SnowmanCount = 7;
	public float CameraSpeed = 50.0f;

	bool WonGame = false;
	float WonTime = 0.0f;
	float CameraWinMoveTime = 2.0f;
	Vector3 WonCameraStart;
	Vector3 WonCameraEnd = new Vector3(0f, -40f, -40f);
	Color StartLight = Color.black;
	Color StartCameraBG = Color.black;
	Color EndLight = new Color(1.0f, 1.0f, 0.9f);
	bool EndingFinished = false;

	bool LostGame = false;
	float GameLostTime = 0.0f;
	float GameStartTime = 0.0f;

	private GameObject Santa;
	private GameObject MainCam;
	private Material FadeMat;

	// Use this for initialization
	void Start () {
		MainCam = GameObject.FindGameObjectWithTag("MainCamera");
		FadeMat = GameObject.FindGameObjectWithTag("FadeOut").renderer.material;

		StartGame();
	}

	//Setup functions

	void StartGame()
	{
		DestroyObsInRadius(100000.0f);
		WonGame = false;
		LostGame = false;
		GameStartTime = Time.time;
		FadeMat.color = new Color(0,0,0, 1.0f);
		Santa = (GameObject)Instantiate(SantaPrefab, new Vector3(-75f, -75f, 0f), Quaternion.identity);
		SpawnTrees();
		SpawnSnowmen();
	}

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
		FadeMat.color = new Color(0,0,0, 0.0f);
		GameObject.FindGameObjectWithTag("Snow").particleSystem.enableEmission = false;
		StartLight = RenderSettings.ambientLight;
		StartCameraBG = MainCam.camera.backgroundColor;
	}

	void LoseGame()
	{
		LostGame = true;
		GameLostTime = Time.time;
	}

	
	// Update is called once per frame
	void Update ()
	{
		if(LostGame)
		{
			PlayLoseGame();
		}
		else
		{
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
	}


	private void UpdateCameraPos()
	{
		if(WonGame)
		{
			float camPosScale = Mathf.Min (1.0f, (Time.time - WonTime)/CameraWinMoveTime);
			MainCam.transform.position = WonCameraStart + (WonCameraEnd - WonCameraStart)*camPosScale;
		}
		else if (null != Santa)
		{
			Vector3 newPos = Santa.transform.position;
			newPos.y -= 40.0f;
			newPos.z -= 40.0f;
			Vector3 diff = MainCam.transform.position - newPos;
			float frameMove = CameraSpeed * Time.deltaTime;
			float dist = diff.magnitude;
			if( dist > frameMove)
			{
				diff = (diff/dist)*frameMove;
			}

			MainCam.transform.position -= diff;// = newPos;
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

		
		FadeMat.color = new Color(0,0,0, 1.0f - Mathf.Min(1.0f, (Time.time - 0.5f - GameStartTime)/2.0f));

		SantaScript santaScr = Santa.GetComponent<SantaScript>();
		GameObject[] Snowmen = GameObject.FindGameObjectsWithTag("Snowman");
		GameObject[] Trees = GameObject.FindGameObjectsWithTag("Tree");
		//float BurnDistSquared = BurnDistance * BurnDistance;
		int unlitTrees = 0;
		bool freezing = true;

		foreach(GameObject treeGO in Trees)
		{
			TreeScript tree = treeGO.GetComponent<TreeScript>();
			float distToSanta =  (Santa.transform.position - treeGO.transform.position).magnitude;
			unlitTrees++;

			if( distToSanta < TreeLightDistance) //Santa lights trees if he gets close
			{
				tree.SetLit(true);
			}

			if(tree.IsLit())
			{
				unlitTrees--;
				if(distToSanta < BurnDistance) //Trees heal Santa if he's nearby
				{
					//these are all nonsense... Want to scale from 0 at distance to max at close, but this isn't how to do that!
					santaScr.AdjustHealth( HealPerSecond * DistanceScale( distToSanta, BurnDistance) ); //Nonlinear, but who cares.
					freezing = false;
				}
			}

			foreach(GameObject smGO in Snowmen)
			{
				SnowmanScript snowman = smGO.GetComponent<SnowmanScript>();
				//float smDistToSanta =  (Santa.transform.position - smGO.transform.position).magnitude;
				float smDistToTree =  (smGO.transform.position - treeGO.transform.position).magnitude;

				/*if(!snowman.IsDead() && smDistToSanta < BurnDistance) //Snowmen burn Santa with cold
				{
					santaScr.AdjustHealth( -BurnPerSecond * DistanceScale( smDistToSanta, BurnDistance) ); //Nonlinear, but who cares.
				}*/

				if(tree.IsLit() && smDistToTree < BurnDistance) //Trees melt snowmen if they're lit
				{
					snowman.AdjustHealth( -MeltPerSecond * DistanceScale( smDistToTree, BurnDistance) ); //Nonlinear, but who cares.
				}

			}
		}//foreach

		foreach(GameObject smGO in Snowmen)
		{
			SnowmanScript snowman = smGO.GetComponent<SnowmanScript>();
			float smDistToSanta =  (Santa.transform.position - smGO.transform.position).magnitude;
			
			if(!snowman.IsDead() && smDistToSanta < BurnDistance) //Snowmen burn Santa with cold
			{
				santaScr.AdjustHealth( -BurnPerSecond * DistanceScale( smDistToSanta, BurnDistance) );
			}
			
		}

		if(freezing)
		{
			santaScr.AdjustHealth( - LifeDrainPerSecond * Time.deltaTime);
		}

		if(santaScr.IsDead())
		{
			LoseGame();
		}

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
		float ExplosionRadius = 130.0f;
		Color GrassColour=new Color(0.25f, 0.9f, 0.25f);
		Color SpringBGColour = new Color(.059f, .59f, .059f);
		if(Time.time > EndingStart)
		{
			DrawParticles dp = this.GetComponent<DrawParticles>();
			if(Time.time < ExplosionTime)
			{
				float scale = (Time.time - EndingStart)/ExplosionLength;
				float curRad = scale * ExplosionRadius;
				DestroyObsInRadius(curRad-5.0f);

				RenderSettings.ambientLight = StartLight + (EndLight - StartLight)*scale;
				//MainCam.camera.backgroundColor = StartCameraBG + (GrassColour - StartCameraBG)*scale;

				for(int i = 0; i < (50+1000*scale*scale)*dp.GetParticleDensity(); i++)
				{
					dp.DrawParticleOnSphere(Vector3.zero, curRad, new Color(1.0f, 1.0f, 0.9f), 3.5f, true);
				}
			}
			else /*if( Time.time < ExplosionTime + 1000.0f )*/
			{
				DestroyObsInRadius(100000.0f);
				if(Time.time < ExplosionTime + ExplosionLength)
				{
					float scale = (Time.time - ExplosionTime)/ExplosionLength;
					MainCam.camera.backgroundColor = StartCameraBG + (SpringBGColour - StartCameraBG)*scale;
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

	//Fade out and restart
	private void PlayLoseGame()
	{
		FadeMat.color = new Color(0,0,0, Mathf.Max(0.0f, Mathf.Min(1.0f, (Time.time - 0.5f - GameLostTime)/2.0f)));

		if(Time.time > GameLostTime + 3.0f) StartGame();
	}
}
