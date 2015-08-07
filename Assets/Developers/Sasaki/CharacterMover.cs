using UnityEngine;
using System.Collections;

public class CharacterMover : MonoBehaviour {

	[SerializeField]
	private float JumpPower = 2f;
	[SerializeField]
	private float SpeedScale = 1f;
	[SerializeField]
	private Transform GoalPos;
	[SerializeField]
	private GameObject Bomb;

	private bool isJump;
	private Rigidbody rig;
	private Animator anim;
	
	void Start () {
		isJump = false;
		rig = GetComponent<Rigidbody>();
	}
	
	void Update () {
		int rand = Random.Range(0, 200);
		if(rand == 0){
			GameObject bomb = Instantiate(Bomb, transform.position, transform.rotation) as GameObject;
			bomb.GetComponent<Bomb>().SetVelocity(new Vector3(10f,5f,0f));
		}
		
		float goalDir = GoalPos.localPosition.x - transform.localPosition.x;
		if(Mathf.Abs(goalDir) > 0.1f){
			if(isJump == false)
				rig.velocity = new Vector3(Mathf.Sign(goalDir)*SpeedScale, rig.velocity.y, 0f);
		}else if(GoalPos.localPosition.y - transform.localPosition.y > 0f){
			rig.velocity = new Vector3(0f, rig.velocity.y, 0f);
			Jump();
		}else{
			rig.velocity = new Vector3(0f, rig.velocity.y, 0f);
		}
	}
	
	public void Jump()
	{
		if(isJump == false){
			isJump = true;
			rig.velocity = new Vector3(rig.velocity.x, JumpPower, rig.velocity.z);
		}
	}
	
	void OnCollisionEnter(Collision col)
	{
		isJump = false;
	}
}
