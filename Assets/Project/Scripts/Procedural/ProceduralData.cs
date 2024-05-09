using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BluMarble.Procedural
{
    [CreateAssetMenu(fileName = "ProceduralData", menuName = "BluMarbleScriptableObjects/ProceduralScriptableObject")]
    public class ProceduralData : ScriptableObject
    {
        [SerializeField]
        [Tooltip("One of the images will be randomly selected to be displayed here. The images should be cohesive.")]
        private List<Sprite> m_Images;
        public List<Sprite> DataImages
        {
            get
            {
                return m_Images;
            }
        }
    }
}
