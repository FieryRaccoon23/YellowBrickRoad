using UnityEngine;

namespace BluMarble.Camera
{
    public class CameraManager : BluMarble.Singleton.Singleton<CameraManager>
    {
        public Vector2 GetCameraViewHalf()
        {
            UnityEngine.Camera MainCamera = UnityEngine.Camera.main;

            float halfHeight = MainCamera.orthographicSize;
            float halfWidth = MainCamera.aspect * halfHeight;

            //return new Vector2(halfWidth, halfHeight);
            return new Vector2(25.0f, 25.0f);
        }

        public Vector2 GetCameraView()
        {
            return GetCameraViewHalf() * 2.0f;
        }
    }
}
