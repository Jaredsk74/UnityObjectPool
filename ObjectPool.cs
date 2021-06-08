using System.Collections.Generic;
using UnityEngine;

public class ObjectPool {

	private GameObject obj;
	private Queue<GameObject> pool;
	private Queue<GameObject> objects; // This tracks all created objects in case the pool needs to be cleared.

	private bool dynamic;

	// Static pool for a single object
	public ObjectPool(GameObject obj) {
		this.obj = obj;
		pool = new Queue<GameObject>();
		objects = new Queue<GameObject>();
		dynamic = false;
	}

	public GameObject GetPooledObject() {
		if (!dynamic) {
			GameObject o;
			if (pool.Count > 0) {
				return pool.Dequeue();
			}

			o = GameObject.Instantiate(obj);
			o.SetActive(false);
			objects.Enqueue(o);
			return o;  
		}
		return null;
	}

	public GameObject GetPooledObject(Vector3 position, Quaternion rotation) {
		if (!dynamic) {
			GameObject o;
			if (pool.Count > 0) {
				o = pool.Dequeue();
				o.transform.position = position;
				o.transform.rotation = rotation;
				return o;
			}

			o = GameObject.Instantiate(obj, position, rotation);
			o.SetActive(false);
			objects.Enqueue(o);
			return o; 
		}
		return null;
	}

	// Place an object into the pool
	public void ReturnPooledObject(GameObject obj) {
		obj.SetActive(false);
		pool.Enqueue(obj);
	}

	public void Clear() {
		while(objects.Count > 0) {
			GameObject.Destroy(objects.Dequeue());
		}
	}

	// Dynamic pool that can use any objects
	public ObjectPool() {
		obj = null;
		pool = new Queue<GameObject>();
		objects = new Queue<GameObject>();
		dynamic = true;
	}

	public GameObject GetPooledObject(GameObject obj) {
		if (dynamic) {
			GameObject o;
			if (pool.Count > 0) {
				for (int i = 0; i < pool.Count; i++) {
					o = pool.Peek();
					if (o.name == obj.name + "(Clone)" && o.activeInHierarchy == false) {
						return pool.Dequeue();
					}
					pool.Enqueue(pool.Dequeue());
				}
			}

			o = GameObject.Instantiate(obj);
			o.SetActive(false);
			objects.Enqueue(o);
			return o;
		}
		return null;
	}

	public GameObject GetPooledObject(GameObject obj, Vector3 position, Quaternion rotation) {
		if (dynamic) {
			GameObject o;
			if (pool.Count > 0) {
				for (int i = 0; i < pool.Count; i++) {
					o = pool.Peek();
					if (o.name == obj.name + "(Clone)" && o.activeInHierarchy == false) {
						o = pool.Dequeue();
						o.transform.position = position;
						o.transform.rotation = rotation;
						return o;
					}
					pool.Enqueue(pool.Dequeue());
				}
			}

			o = GameObject.Instantiate(obj, position, rotation);
			o.SetActive(false);
			objects.Enqueue(o);
			return o; 
		}
		return null;
	}

	// Shared Methods
	public int Count {
		get {
			return pool.Count;
		}
	}

	public int TotalCount {
		get {
			return objects.Count;
		}
	}

	public bool IsDynamic {
		get {
			return dynamic;
		}
	}
}