using UnityEngine;
using System.Collections;

public class CharacterMover : MonoBehaviour {
	public enum ControlType
	{
		MYSELF,
		AUTORUN_HELP,
		AUTORUN_DISTURB
	}

	[SerializeField]
	private ControlType CharacterControlType;
	public ControlType CharaControlType
	{
		get { return CharaControlType; }
	}
	
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
		switch(CharacterControlType){
		case ControlType.MYSELF:
		case ControlType.AUTORUN_HELP:
			GetComponentInChildren<CharacterLookForward>().gameObject.SetActive(false);
			break;
		}
	}
	
	void Update () {
		switch(CharacterControlType){
		case ControlType.MYSELF:
			rig.velocity = new Vector3(SpeedScale*Input.GetAxis("Horizontal"), rig.velocity.y, 0f);
			if(Input.GetKeyDown(KeyCode.Space))
			{
				Jump();
			}
			break;
		case ControlType.AUTORUN_DISTURB:
			int rand = Random.Range(0, 200);
			if(rand == 0){
				GameObject bomb = Instantiate(Bomb, transform.position+Vector3.up, transform.rotation) as GameObject;
				bomb.GetComponent<Bomb>().SetVelocity(15f*(GoalPos.position-transform.position).normalized);
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
			break;
		case ControlType.AUTORUN_HELP:			
			rig.velocity = new Vector3(SpeedScale*0.5f, rig.velocity.y, 0f);
			break;
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
