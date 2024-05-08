using UnityEngine;

namespace BluMarble.Pool
{
    public class PooledGameObject : MonoBehaviour
    {
        public BluMarble.Pool.ObjectPool m_ObjPool;

        private void Update()
        {
            SpriteRenderer SpriteRendererObj = GetComponent<SpriteRenderer>();

            float ScreenWidth = Screen.width / 2.0f;
            float ScreenWidthOffset = ScreenWidth * 0.20f;
            ScreenWidth += ScreenWidthOffset;

            // Use camera view and not screen width
            if(transform.localPosition.x < -ScreenWidthOffset)
            {
                m_ObjPool.ReleaseObject(gameObject);
            }

            // JUST FOR TESTING
            //transform.localPosition.Set(transform.localPosition.x - Time.deltaTime * 2.0f, transform.localPosition.y, transform.localPosition.z);
            transform.localPosition = new Vector3(transform.localPosition.x - Time.deltaTime * 5.0f, transform.localPosition.y, transform.localPosition.z);
        }
    }
}
