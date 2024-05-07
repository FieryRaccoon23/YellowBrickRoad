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
    }

    public class ProceduralHelper : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("List of all the road segments.")]
        private List<SerializedProceduralTypeMetaData> m_SerializedProceduralTypeMetaData;

        public float GetSpeedOfType(ProceduralObjectType ProceduralObjectTypeValue)
        {
            int Index = (int)(ProceduralObjectTypeValue);

            if(Index < 0 || Index >= m_SerializedProceduralTypeMetaData.Count)
            {
                Assert.IsTrue(false, "ProceduralObjectType is not valid");
                return 0.0f; 
            }

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
