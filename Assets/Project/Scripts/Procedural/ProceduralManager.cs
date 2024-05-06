using System;
using System.Collections.Generic;
using UnityEngine;

namespace BluMarble.Procedural
{
    [Serializable]
    public class SerializedProceduralData
    {
        public List<ProceduralData> m_ProceduralData;
        public ProceduralRegionType m_ProceduralRegionType;
    }

    public class ProceduralManager : BluMarble.Singleton.Singleton<ProceduralManager>
    {
        [SerializeField]
        [Tooltip("List of all the road segments.")]
        private List<SerializedProceduralData> m_SerializedProceduralData;

        public override void PerformUpdate()
        {
        }
    }
}
