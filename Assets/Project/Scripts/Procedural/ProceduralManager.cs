using BluMarble.Pool;
using Codice.Client.Common.GameUI;
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

    [Serializable]
    public class SerializedProceduralTransitionData
    {
        public List<SerializedProceduralObjectTypeData> m_SerializedProceduralObjectTypeData = new List<SerializedProceduralObjectTypeData>();

        public ProceduralRegionType m_ProceduralRegionTypeFrom;
        public ProceduralRegionType m_ProceduralRegionTypeTo;
    }

    public class ProceduralManager : BluMarble.Singleton.Singleton<ProceduralManager>
    {
        [SerializeField]
        [Tooltip("List of all the object segments.")]
        private List<SerializedProceduralData> m_SerializedProceduralData;

        [SerializeField]
        [Tooltip("List of all the transition segments.")]
        private List<SerializedProceduralTransitionData> m_SerializedProceduralTransitionData;

        [SerializeField]
        [Tooltip("List of all the road segments prefabs.")]
        private List<SerializedProceduralObjectTypePrefab> m_SerializedProceduralObjectTypePrefab;

        [SerializeField]
        [Tooltip("Parent under which all the prefabs will be stored.")]
        private GameObject m_PooledParentObject;

        private ProceduralRegionType m_CurrentProceduralRegionType = ProceduralRegionType.GrassLand;
        private ProceduralRegionType m_LastProceduralRegionType = ProceduralRegionType.GrassLand;
        private ProceduralHelper m_ProceduralHelper;
        private Dictionary<ProceduralObjectType, Queue<GameObject>> m_DictionaryOfObjectsQueue;
        private List<GameObject> m_LastObjectInQueue;
        private bool m_ApplyTransition = false;

        private const int m_PoolCapacity = 10;
        private const int m_PoolMaxValue = 20;
        private const int m_NumOfObjsOffset = 4;

        public override void PerformInit()
        {
            m_CurrentProceduralRegionType = ProceduralRegionType.GrassLand;
            m_LastProceduralRegionType = ProceduralRegionType.GrassLand;
            m_ProceduralHelper = GetComponent<ProceduralHelper>();

            m_DictionaryOfObjectsQueue = new Dictionary<ProceduralObjectType, Queue<GameObject>>();
            m_LastObjectInQueue = new List<GameObject>();

            for (int i = 0; i < (int)ProceduralObjectType.MaxNum; ++i)
            {
                Queue<GameObject> NewQueue = new Queue<GameObject>();
                m_DictionaryOfObjectsQueue.Add((ProceduralObjectType)i,NewQueue);

                m_LastObjectInQueue.Add(null);
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
            Vector2 CameraView = BluMarble.Camera.CameraManager.Instance.GetCameraView();

            foreach (var ProceduralPrefab in m_SerializedProceduralObjectTypePrefab)
            {
                GameObject FirstProceduralObj = AddObjectToQueue(ProceduralPrefab.m_ProceduralObjectType);

                SpriteRenderer CurrentProceduralObjSprite = FirstProceduralObj.GetComponent<SpriteRenderer>();

                float CurrentSpriteWidth = CurrentProceduralObjSprite.size.x;

                int NumOfObjs = (int)(CameraView.x / CurrentSpriteWidth);
                NumOfObjs += m_NumOfObjsOffset;

                for (int i = 1; i < NumOfObjs; ++i)
                {
                    AddObjectToQueue(ProceduralPrefab.m_ProceduralObjectType);
                }
            }

            // TODO: Add fog of war if screen width to big
        }

        public override void PerformUpdate()
        {
            foreach(SerializedProceduralObjectTypePrefab ProceduralPrefab in m_SerializedProceduralObjectTypePrefab)
            {
                ProceduralObjectType ObjType = ProceduralPrefab.m_ProceduralObjectType;

                // Check if queue size needs updating depending on camera view
                UpdateQueueSizeFromCameraView(ObjType);

                // Move current object
                MoveObjects(ObjType);

                // Check if end reached
                UpdateQueueData();

            }
        }

        private void UpdateQueueSizeFromCameraView(ProceduralObjectType ObjType)
        {
            GameObject FirstObj = m_DictionaryOfObjectsQueue[ObjType].Peek();
            SpriteRenderer CurrentProceduralObjSprite = FirstObj.GetComponent<SpriteRenderer>();

            Vector2 CameraView = BluMarble.Camera.CameraManager.Instance.GetCameraView();

            float CurrentSpriteWidth = CurrentProceduralObjSprite.size.x;
            int NumOfObjs = (int)(CameraView.x / CurrentSpriteWidth);
            NumOfObjs += m_NumOfObjsOffset;

            //Debug.Log("Sprite width:" + CurrentSpriteWidth.ToString() + " Camera view: " + CameraView.x.ToString() + " Num of Objs: " + NumOfObjs.ToString());

            if(NumOfObjs <= m_DictionaryOfObjectsQueue[ObjType].Count)
            {
                return;
            }

            int NewObsToAdd = NumOfObjs - m_DictionaryOfObjectsQueue[ObjType].Count;

            for (int i = 1; i < NewObsToAdd; ++i)
            {
                AddObjectToQueue(ObjType);
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

                if (m_DictionaryOfObjectsQueue[ObjType].Count <= 0)
                {
                    continue;
                }

                GameObject FirstInStack = m_DictionaryOfObjectsQueue[ObjType].Peek();

                if (FirstInStack.transform.localPosition.x < -BluMarble.Camera.CameraManager.Instance.GetCameraView().x)
                {
                    GameObject ObjToRelease = m_DictionaryOfObjectsQueue[ObjType].Dequeue();
                    m_SerializedProceduralObjectTypePrefab[(int)ObjType].ReleaseObject(ObjToRelease);

                    AddObjectToQueue(ObjType);
                }

                // for negative layers, spawn randomly
            }
        }

        private GameObject AddObjectToQueue(ProceduralObjectType ObjType) 
        {
            float ZOrder = m_ProceduralHelper.GetZOrder(ObjType);
            float YPos = m_ProceduralHelper.GetYPos(ObjType);

            GameObject Obj = m_SerializedProceduralObjectTypePrefab[(int)ObjType].GetObject();
            m_DictionaryOfObjectsQueue[ObjType].Enqueue(Obj);

            // Sprite width
            SpriteRenderer CurrentProceduralObjSprite = Obj.GetComponent<SpriteRenderer>();
            float CurrentSpriteWidth = CurrentProceduralObjSprite.size.x;

            // Set object location
            Vector3 LastPosition = new Vector3(-BluMarble.Camera.CameraManager.Instance.GetCameraView().x, YPos, ZOrder);
            if (m_LastObjectInQueue[(int)ObjType])
            {
                LastPosition = new Vector3(m_LastObjectInQueue[(int)ObjType].transform.localPosition.x + CurrentSpriteWidth, YPos, ZOrder);
            }

            Obj.transform.localPosition = LastPosition;

            m_LastObjectInQueue[(int)ObjType] = Obj;

            ApplyDataSprite(CurrentProceduralObjSprite, ObjType);

            return Obj;
        }

        private void ApplyDataSprite(SpriteRenderer SpriteRen, ProceduralObjectType ObjType)
        {
            ProceduralData DataToApply = m_SerializedProceduralData[(int)m_CurrentProceduralRegionType].m_SerializedProceduralObjectTypeData[(int)ObjType].m_ProceduralData;

            if (m_ApplyTransition)
            {
                m_ApplyTransition = false;

                if (m_CurrentProceduralRegionType != m_LastProceduralRegionType)
                {
                    foreach (var TransitionData in m_SerializedProceduralTransitionData)
                    {
                        if(TransitionData.m_ProceduralRegionTypeFrom == m_LastProceduralRegionType && TransitionData.m_ProceduralRegionTypeTo == m_CurrentProceduralRegionType)
                        {
                            DataToApply = TransitionData.m_SerializedProceduralObjectTypeData[(int)ObjType].m_ProceduralData;
                            break;
                        }
                    }
                }

            }

            
            List<Sprite> DataImages = DataToApply.DataImages;

            if(DataImages.Count <= 0)
            {
                return;
            }

            int RandomIndex = UnityEngine.Random.Range(0, DataImages.Count);

            SpriteRen.sprite = DataImages[RandomIndex];
        }
    }
}
