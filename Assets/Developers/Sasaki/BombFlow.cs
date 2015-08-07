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
		GameObject _ParentObject = new GameObject("DrawUnitParent");
		_ParentObject.layer = LayerMask.NameToLayer("DrawObject");
		Rigidbody rig = _ParentObject.AddComponent<Rigidbody>();
		rig.constraints = RigidbodyConstraints.FreezeRotationX| 
							  RigidbodyConstraints.FreezeRotationY| 
							  RigidbodyConstraints.FreezePositionZ;
		col.gameObject.transform.parent = _ParentObject.transform;
		col.gameObject.transform.parent.GetComponent<Rigidbody>().AddForce(0.001f*(col.gameObject.transform.position - transform.position), ForceMode.Impulse);
		
		Debug.Log("instantiate");
		Instantiate(effect, transform.parent.position, Quaternion.identity);
		
		Destroy(_ParentObject, 2.0f);
		Destroy(transform.parent.gameObject);
	}
}
