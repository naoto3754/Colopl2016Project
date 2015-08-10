using UnityEngine;
using System.Collections;

public class BombFlow : MonoBehaviour {
	[SerializeField]
	private GameObject effect;
	
	private Collider thisColl;
	void Start () {
		thisColl = GetComponent<Collider>();
		SetActive(false);
	}
	
	public void SetActive(bool b)
	{
		thisColl.enabled = b;
	}
	
	void OnTriggerEnter(Collider col){
		if(col.gameObject.layer == LayerMask.NameToLayer("DrawObject")){
			GameObject _ParentObject = new GameObject("DrawUnitParent");
			_ParentObject.layer = LayerMask.NameToLayer("DrawObject");
			Rigidbody rig = _ParentObject.AddComponent<Rigidbody>();
			rig.constraints = RigidbodyConstraints.FreezeRotationX| 
								  RigidbodyConstraints.FreezeRotationY| 
								  RigidbodyConstraints.FreezePositionZ;
			col.gameObject.transform.parent = _ParentObject.transform;
			col.gameObject.transform.parent.GetComponent<Rigidbody>().AddForce(0.001f*(col.gameObject.transform.position - transform.position), ForceMode.Impulse);
			Destroy(_ParentObject, 2.0f);
		}
		
		Instantiate(effect, transform.parent.position, Quaternion.identity);
		Destroy(transform.parent.gameObject);
	}
}
