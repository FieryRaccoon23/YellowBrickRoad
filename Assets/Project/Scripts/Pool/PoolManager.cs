using UnityEngine;

namespace BlueMarble.Pool
{
    public class PoolManager : BluMarble.Singleton.Singleton<PoolManager>
    {
        public GameObject SpawnObject(GameObject ObjToSpawn)
        {
            return Instantiate(ObjToSpawn, new Vector3(0, 0, 0), Quaternion.identity);
        }

        public void DestroyObject(GameObject ObjToDestroy)
        {
            Destroy(ObjToDestroy);
        }
    }
}
