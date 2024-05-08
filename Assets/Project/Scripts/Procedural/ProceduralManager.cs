using BluMarble.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace BluMarble.Procedural
{
    [Serializable]
    public class SerializedProceduralObjectTypePrefab
    {
        public GameObject m_ProceduralPrefab;

        public ProceduralObjectType m_ProceduralObjectType;

        private BluMarble.Pool.ObjectPool m_ObjectPool;

        public void Init(GameObject ParentObject, int Capacity, int MaxSize)
        {
            m_ObjectPool = new ObjectPool();
            m_ObjectPool.Init(m_ProceduralPrefab, Capacity, MaxSize, ParentObject);
        }

        public GameObject GetObject()
        {
            return m_ObjectPool.GetObject();
        }

        public void ReleaseObject(GameObject Obj)
        {
            m_ObjectPool.ReleaseObject(Obj);
        }
    }

    [Serializable]
    public class SerializedProceduralObjectTypeData
    {
        public ProceduralData m_ProceduralData;

        public ProceduralObjectType m_ProceduralObjectType;
    }

    [Serializable]
    public class SerializedProceduralData
    {
        public List<SerializedProceduralObjectTypeData> m_SerializedProceduralObjectTypeData = new List<SerializedProceduralObjectTypeData>();

        public ProceduralRegionType m_ProceduralRegionType;
    }

    public class ProceduralManager : BluMarble.Singleton.Singleton<ProceduralManager>
    {
        [SerializeField]
        [Tooltip("List of all the road segments.")]
        private List<SerializedProceduralData> m_SerializedProceduralData;

        [SerializeField]
        [Tooltip("List of all the road segments prefabs.")]
        private List<SerializedProceduralObjectTypePrefab> m_SerializedProceduralObjectTypePrefab;

        [SerializeField]
        [Tooltip("Parent under which all the prefabs will be stored.")]
        private GameObject m_PooledParentObject;

        private ProceduralRegionType m_CurrentProceduralRegionType = ProceduralRegionType.GrassLand;
        private ProceduralHelper m_ProceduralHelper;
        private Dictionary<ProceduralObjectType, Queue<GameObject>> m_DictionaryOfObjectsQueue;
        private Vector3 m_EndPosition = Vector3.zero;
        private Vector3 m_StartPosition = Vector3.zero;

        private const int m_PoolCapacity = 10;
        private const int m_PoolMaxValue = 20;
        private const int m_NumOfObjsOffset = 4;

        public override void PerformInit()
        {
            m_CurrentProceduralRegionType = ProceduralRegionType.GrassLand;
            m_ProceduralHelper = GetComponent<ProceduralHelper>();

            m_DictionaryOfObjectsQueue = new Dictionary<ProceduralObjectType, Queue<GameObject>>();
            for(int i = 0; i < (int)ProceduralObjectType.MaxNum; ++i)
            {
                Queue<GameObject> NewQueue = new Queue<GameObject>();
                m_DictionaryOfObjectsQueue.Add((ProceduralObjectType)i,NewQueue);
            }

#if UNITY_EDITOR
            Validate();
#endif
            // Init pool before creating any world data
            InitPool();

            // Init the world
            InitWorld();
        }

        private bool Validate()
        {
            bool Result = true;
            if (m_ProceduralHelper == null)
            {
                Assert.IsTrue(false, "ProceduralHelper not found in Procedural manager!");
                Result = false;
            }

            return Result;
        }

        private void InitPool()
        {
            foreach(var ProceduralPrefab in m_SerializedProceduralObjectTypePrefab)
            {
                ProceduralPrefab.Init(m_PooledParentObject, m_PoolCapacity, m_PoolMaxValue);
                
                // This will load all the objects initially and none will be generated during runtime
                List<GameObject> ObjInScene = new List<GameObject>();
                for(int i = 0; i < m_PoolCapacity; ++i)
                {
                    GameObject TempObj = ProceduralPrefab.GetObject();
                    ObjInScene.Add(TempObj);
                }
                for (int i = 0; i < m_PoolCapacity; ++i)
                {
                    ProceduralPrefab.ReleaseObject(ObjInScene[i]);
                }
            }
        }


        private void InitWorld()
        {
            // Get screen width to get an idea of how far the objects will be displayed

            // If the screen is too wide, make Fog of War to hide the area

            // Spawn objects from Layer0 to ProceduralObjectType.MaxBackLayers

            // For objects from ProceduralObjectType.MaxBackLayers onwards, they need to be randomly generated
            // Use camera view and not screen width
            float ScreenWidth = Screen.width;
            foreach (var ProceduralPrefab in m_SerializedProceduralObjectTypePrefab)
            {
                GameObject FirstProceduralObj = AddObjectToQueue(ProceduralPrefab.m_ProceduralObjectType, m_StartPosition);

                SpriteRenderer CurrentProceduralObjSprite = FirstProceduralObj.GetComponent<SpriteRenderer>();

                float CurrentSpriteWidth = CurrentProceduralObjSprite.size.x;

                int NumOfObjs = (int)(ScreenWidth / CurrentSpriteWidth);
                NumOfObjs += m_NumOfObjsOffset;

                // first in queue has to be closer to the end position
                FirstProceduralObj.transform.localPosition += (new Vector3(-CurrentSpriteWidth * (5 - 1), 0.0f, 0.0f));

                for (int i = 1; i < /*NumOfObjs*/5; ++i) 
                {
                    GameObject CurrentProceduralObj = AddObjectToQueue(ProceduralPrefab.m_ProceduralObjectType, new Vector3(-CurrentSpriteWidth * (5 - 1 - i), 0.0f, 0.0f));
                    //CurrentProceduralObj.transform.localPosition += (new Vector3(-CurrentSpriteWidth * i, 0.0f, 0.0f));
                }
            }
        }

        public override void PerformUpdate()
        {
            int Index = (int)m_CurrentProceduralRegionType;

            foreach (SerializedProceduralObjectTypeData CurrentSerializedProceduralObjectTypeData in m_SerializedProceduralData[Index].m_SerializedProceduralObjectTypeData)
            {
                ProceduralObjectType ObjType = CurrentSerializedProceduralObjectTypeData.m_ProceduralObjectType;

                // Move current object
                MoveObjects(ObjType);

                // Check if end reached
                UpdateQueueData();

            }
        }

        private void MoveObjects(ProceduralObjectType ObjType)
        {
            float TranslationSpeed = m_ProceduralHelper.GetSpeedOfType(ObjType);
            Vector3 TranslationDir = m_ProceduralHelper.GetMovingDirection(ObjType);

            foreach (GameObject ObjectInQueue in m_DictionaryOfObjectsQueue[ObjType])
            {
                ObjectInQueue.transform.Translate(TranslationDir * TranslationSpeed * Time.deltaTime);
            }
        }

        private void UpdateQueueData()
        {
            for (int i = 0; i < (int)ProceduralObjectType.MaxNum; ++i)
            {
                ProceduralObjectType ObjType = (ProceduralObjectType)i;

                if(m_DictionaryOfObjectsQueue[ObjType].Count <= 0)
                {
                    continue;
                }

                GameObject FirstInStack = m_DictionaryOfObjectsQueue[ObjType].Peek();

                if (FirstInStack.transform.localPosition.x < -50.0f) // m_EndPosition.x
                {
                    GameObject ObjToRelease = m_DictionaryOfObjectsQueue[ObjType].Dequeue();
                    m_SerializedProceduralObjectTypePrefab[(int)ObjType].ReleaseObject(ObjToRelease);

                    AddObjectToQueue(ObjType, m_StartPosition );
                }
            }
        }

        private GameObject AddObjectToQueue(ProceduralObjectType ObjType, Vector3 Pos) 
        {
            // check region type to set data

            GameObject Obj = m_SerializedProceduralObjectTypePrefab[(int)ObjType].GetObject();
            m_DictionaryOfObjectsQueue[ObjType].Enqueue(Obj);

            // Set object location
            Obj.transform.localPosition = Pos;

            return Obj;
        }
    }
}
