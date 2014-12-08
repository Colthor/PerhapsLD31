using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour {

	public float TreeLightDistance = 7.0f;
	public float BurnDistance = 20.0f;
	public float BurnPerSecond = 20.0f;
	public float HealPerSecond = 33.0f;
	public float WindMoveRadius = 100.0f;
	public float WindMoveRadsPerSec = 0.5f;
	private GameObject Santa;
	private GameObject[] Trees;

	// Use this for initialization
	void Start () {
		Santa = GameObject.FindGameObjectWithTag("Santa");
		Trees = GameObject.FindGameObjectsWithTag("Tree");
	}
	
	// Update is called once per frame
	void Update () {
		PlayGame();
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

	private void PlayGame()
	{
		UpdateWindPos();

		SantaScript santaScr = Santa.GetComponent<SantaScript>();
		GameObject[] Snowmen = GameObject.FindGameObjectsWithTag("Snowman");
		float BurnDistSquared = BurnDistance * BurnDistance;

		foreach(GameObject treeGO in Trees)
		{
			TreeScript tree = treeGO.GetComponent<TreeScript>();
			float distToSantaSquared =  (Santa.transform.position - treeGO.transform.position).sqrMagnitude;

			if( distToSantaSquared < TreeLightDistance*TreeLightDistance) //Santa lights trees if he gets close
			{
				tree.SetLit(true);
			}

			if(tree.IsLit() && distToSantaSquared < BurnDistSquared) //Trees heal Santa if he's nearby
			{
				//these are all nonsense... Want to scale from 0 at distance to max at close, but this isn't how to do that!
				santaScr.AdjustHealth( HealPerSecond * DistanceScale( distToSantaSquared, BurnDistSquared) ); //Nonlinear, but who cares.
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
		}
	}
}
