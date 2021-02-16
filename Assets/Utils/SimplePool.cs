using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based off this: https://gist.github.com/quill18/5a7cfffae68892621267
public class SimplePool {

    const int DEFAULT_POOL_SIZE = 3;
        
    class Pool {
        int nextId = 1;

        Stack<GameObject> inactive;
        GameObject prefab;

        public Pool(GameObject prefab, int initialQuantity) {
            this.prefab = prefab;
            // TODO: Figure out if Stack uses linked lists so we can potentially dump this line:
            inactive = new Stack<GameObject>(initialQuantity);
        }

        public GameObject Spawn(Vector3 position, Quaternion rotation) {
            GameObject gameObject;

            if(inactive.Count == 0) {
                // No objects left in the stack so instantiate a new one.
                gameObject = (GameObject)GameObject.Instantiate(prefab, position, rotation);
                gameObject.name = $"{prefab.name}({nextId++})";
                // Add this custom component so we can link back to this pool.
                gameObject.AddComponent<PoolMember>().myPool = this;
            } else {
                gameObject = inactive.Pop();

                // Guard against possibility that the next inactive object has been destroyed.
                if(gameObject == null) {
                    return Spawn(position, rotation);
                }
            }

            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;
            gameObject.SetActive(true);
            return gameObject;
        }

        public void Despawn(GameObject gameObject) {
            gameObject.SetActive(false);
            inactive.Push(gameObject);
        }

    }

    class PoolMember: MonoBehaviour {
        public Pool myPool;
    }

    static Dictionary<GameObject, Pool> pools;

    static void Init(GameObject prefab = null, int quantity = DEFAULT_POOL_SIZE) {
        if(pools == null) {
            pools = new Dictionary<GameObject, Pool>();
        }
        if(prefab != null && pools.ContainsKey(prefab) == false) {
            pools[prefab] = new Pool(prefab, quantity);
        }
    }

    static public void Preload(GameObject prefab, int quantity = 1) {
        Init(prefab, quantity);

        GameObject[] objects = new GameObject[quantity];
        for(int i = 0; i < quantity; i++) {
            objects[i] = Spawn(prefab, Vector3.zero, Quaternion.identity);
        }
        for(int i = 0; i < quantity; i++) {
            Despawn(objects[i]);
        }
    }

    static public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation) {
        Init(prefab);
        return pools[prefab].Spawn(position, rotation);
    }

    static public void Despawn(GameObject gameObject) {
        PoolMember poolMember = gameObject.GetComponent<PoolMember>();
        if(poolMember == null) {
            Debug.Log($"Object {gameObject.name} wasn't spawned from a pool. Destroying it instead.");
            GameObject.Destroy(gameObject);
        } else {
            poolMember.myPool.Despawn(gameObject);
        }
    }
    
}