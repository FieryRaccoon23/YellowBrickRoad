using UnityEngine;

namespace BluMarble.Pool
{
    public class PoolManager : BluMarble.Singleton.Singleton<PoolManager>
    {
        private int m_Count = 0;

        public GameObject SpawnObject(GameObject ObjToSpawn)
        {
            GameObject Result = Instantiate(ObjToSpawn, new Vector3(0, 0, 0), Quaternion.identity);
            Result.name = Result.name + m_Count.ToString();
            ++m_Count;
            return Result;
        }

        public void DestroyObject(GameObject ObjToDestroy)
        {
            Destroy(ObjToDestroy);
        }
    }
}
