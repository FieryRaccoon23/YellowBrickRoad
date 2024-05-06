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

        [SerializeField]
        [Tooltip("How fast the images should move.")]
        private float m_Speed = 0.0f;
        public float Speed
        {
            get 
            {
                if (m_Speed >= BluMarble.Singleton.GameSettings.Instance.MaxProceduralSpeed)
                {
                    return BluMarble.Singleton.GameSettings.Instance.MaxProceduralSpeed;
                }
                else if(m_Speed <= BluMarble.Singleton.GameSettings.Instance.MinProceduralSpeed)
                {
                    return BluMarble.Singleton.GameSettings.Instance.MinProceduralSpeed;
                }
                return m_Speed;
            }
        }
    }
}
