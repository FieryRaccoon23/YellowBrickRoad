using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions;

namespace BluMarble.Procedural
{
    [Serializable]
    public class SerializedProceduralTypeMetaData
    {
        [Tooltip("Type of procedural object. It will be used as index.")]
        public ProceduralObjectType m_ProceduralObjectType;

        [Tooltip("How fast the images should move.")]
        public float m_Speed;

        [Tooltip("Direction where the objects will move.")]
        public Vector3 m_MovingDirection;

        [Tooltip("Z order where it will be placed")]
        public float m_ZOrder = 0.0f;

        [Tooltip("Y position")]
        public float m_YPosition = 0.0f;
    }

    public class ProceduralHelper : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("List of all the road segments.")]
        private List<SerializedProceduralTypeMetaData> m_SerializedProceduralTypeMetaData;

        public Vector3 GetMovingDirection(ProceduralObjectType ProceduralObjectTypeValue)
        {
            int Index = (int)(ProceduralObjectTypeValue);

#if UNITY_EDITOR
            if (Index < 0 || Index >= m_SerializedProceduralTypeMetaData.Count)
            {
                Assert.IsTrue(false, "ProceduralObjectType is not valid");
                return Vector3.zero;
            }
#endif

            return m_SerializedProceduralTypeMetaData[Index].m_MovingDirection;
        }

        public float GetYPos(ProceduralObjectType ProceduralObjectTypeValue)
        {
            int Index = (int)(ProceduralObjectTypeValue);

#if UNITY_EDITOR
            if (Index < 0 || Index >= m_SerializedProceduralTypeMetaData.Count)
            {
                Assert.IsTrue(false, "ProceduralObjectType is not valid");
                return 0.0f;
            }
#endif

            return m_SerializedProceduralTypeMetaData[Index].m_YPosition;
        }

        public float GetZOrder(ProceduralObjectType ProceduralObjectTypeValue)
        {
            int Index = (int)(ProceduralObjectTypeValue);

#if UNITY_EDITOR
            if (Index < 0 || Index >= m_SerializedProceduralTypeMetaData.Count)
            {
                Assert.IsTrue(false, "ProceduralObjectType is not valid");
                return 0.0f;
            }
#endif

            return m_SerializedProceduralTypeMetaData[Index].m_ZOrder;
        }

        public float GetSpeedOfType(ProceduralObjectType ProceduralObjectTypeValue)
        {
            int Index = (int)(ProceduralObjectTypeValue);

#if UNITY_EDITOR
            if (Index < 0 || Index >= m_SerializedProceduralTypeMetaData.Count)
            {
                Assert.IsTrue(false, "ProceduralObjectType is not valid");
                return 0.0f; 
            }
#endif

            float Speed = m_SerializedProceduralTypeMetaData[Index].m_Speed;

            if (Speed >= BluMarble.Singleton.GameSettings.Instance.MaxProceduralSpeed)
            {
                return BluMarble.Singleton.GameSettings.Instance.MaxProceduralSpeed;
            }
            else if (Speed <= BluMarble.Singleton.GameSettings.Instance.MinProceduralSpeed)
            {
                return BluMarble.Singleton.GameSettings.Instance.MinProceduralSpeed;
            }
            return Speed;
        }
    }
}
