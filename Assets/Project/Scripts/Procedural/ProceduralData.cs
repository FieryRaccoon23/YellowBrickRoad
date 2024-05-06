using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BluMarble.Procedural
{
    [CreateAssetMenu(fileName = "ProceduralData", menuName = "BlueMarbleScriptableObjects/ProceduralScriptableObject")]
    public class ProceduralData : ScriptableObject
    {
        [SerializeField]
        [Tooltip("One of the images will be randomly selected to be displayed here. The images should be cohesive.")]
        private List<Image> m_Images;
    }
}
