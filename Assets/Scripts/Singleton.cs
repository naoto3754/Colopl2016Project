using UnityEngine;
using System.Collections;

public class Singlton<T> : MonoBehaviour where T : Singlton<T> {
	static T instance = null;
	 public static T I
	 {
		get
		{
			if(instance != null)
				return instance;
				
			System.Type type = typeof(T);
 
            T I = GameObject.FindObjectOfType(type) as T;
 
            if( I == null )
            {
                string typeName = type.ToString();
 
                GameObject gameObject = new GameObject( typeName, type );
                I = gameObject.GetComponent<T>();
 
                if( I == null )
                {
                    Debug.LogError("Problem during the creation of " + typeName,gameObject );
                }
            }
            else
            {
                Initialize(I);
            }
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
