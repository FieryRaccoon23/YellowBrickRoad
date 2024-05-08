using UnityEngine;
using UnityEngine.Pool;

namespace BluMarble.Pool
{
    public class ObjectPool 
    {
        private GameObject m_PoolPrefab = null;
        private GameObject m_Parent = null;
        public bool CollectionChecks = true;
        private IObjectPool<GameObject> m_Pool;

        public void Init(GameObject PoolPrefabValue, int Capacity, int MaxSize, GameObject ParentValue = null)
        {
            m_PoolPrefab = PoolPrefabValue;
            m_Parent = ParentValue;
            m_Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, CollectionChecks, Capacity, MaxSize);
        }

        public GameObject GetObject()
        {
            return m_Pool.Get();
        }

        public void ReleaseObject(GameObject Obj) 
        {
            m_Pool.Release(Obj);
        }

        private GameObject CreatePooledItem()
        {
            GameObject Result = PoolManager.Instance.SpawnObject(m_PoolPrefab);

            if (m_Parent)
            {
                Result.transform.SetParent(m_Parent.transform);
            }

            Result.SetActive(false);

            return Result;
        }

        private void OnTakeFromPool(GameObject Obj)
        {
            Obj.SetActive(true);
        }

        void OnReturnedToPool(GameObject Obj)
        {
            Obj.SetActive(false);
        }

        void OnDestroyPoolObject(GameObject Obj)
        {
            PoolManager.Instance.DestroyObject(Obj);
        }
    }
}
