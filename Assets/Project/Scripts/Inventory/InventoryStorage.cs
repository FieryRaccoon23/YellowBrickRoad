using UnityEngine;

namespace BluMarble.Inventory
{
    public class InventoryStorage : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_LeftWall;

        [SerializeField]
        private GameObject m_RightWall;

        [SerializeField]
        private GameObject m_FrontWall;

        [SerializeField]
        private GameObject m_BackWall;

        private SpriteRenderer m_LeftWallSprite;
        private SpriteRenderer m_RightWallSprite;
        private SpriteRenderer m_FrontWallSprite;
        private SpriteRenderer m_BackWallSprite;

        public void PerformInit()
        {
            SetupWalls();
        }

        private void SetupWalls()
        {
            m_LeftWall = Instantiate(m_LeftWall);
            m_LeftWallSprite = m_LeftWall.GetComponent<SpriteRenderer>();
            m_LeftWall.transform.SetParent(gameObject.transform);
            m_LeftWall.SetActive(false);

            //m_RightWallSprite = m_RightWall.GetComponent<SpriteRenderer>();
            //m_FrontWallSprite = m_FrontWall.GetComponent<SpriteRenderer>();
            //m_BackWallSprite = m_BackWall.GetComponent<SpriteRenderer>();
        }

    }
}
