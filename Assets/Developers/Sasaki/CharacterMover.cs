using UnityEngine;
using System.Collections;

public class CharacterMover : MonoBehaviour {

	[SerializeField]
	private float JumpPower = 2f;
	[SerializeField]
	private float SpeedScale = 1f;
	[SerializeField]
	private Transform GoalPos;

	private bool isJump;
	private Rigidbody rig;
	private Animator anim;
	
	void Start () {
		isJump = false;
		rig = GetComponent<Rigidbody>();
	}
	
	void Update () {
		float goalDir = GoalPos.position.x - transform.position.x;
		if(Mathf.Abs(goalDir) > 0.1f){
			if(isJump == false)
				rig.velocity = new Vector3(Mathf.Sign(goalDir)*SpeedScale, rig.velocity.y, 0f);
		}else{
			rig.velocity = new Vector3(0f, rig.velocity.y, 0f);
			Jump();
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
