using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : Singleton<T> {
	static T instance = null;
	 public static T I
	 {
		get
		{
			if(instance != null)
				return instance;
			else
                Debug.LogError("Error");
            return instance;
		} 
	 }
	 
	 static void Initialize(T I)
    {
        if( instance == null )
        {
            instance = I;
 
            instance.OnInitialize();
        }
        else if( instance != I )
        {
            DestroyImmediate( instance.gameObject );
        }
    }
 
    static void Destroyed(T I)
    {
        if( instance == I )
        {
            instance.OnFinalize();
 
            instance = null;
        }
    }
 
    public virtual void OnInitialize() {}
    public virtual void OnFinalize() {}
 
    void Awake()
    {
        Initialize( this as T );
    }
 
    void OnDestroy()
    {
        Destroyed( this as T );
    }
 
    void OnApplicationQuit()
    {
        Destroyed( this as T );
    }
}
