using UnityEngine;

namespace BluMarble.Camera
{
    public class CameraManager : BluMarble.Singleton.Singleton<CameraManager>
    {
        public Vector2 GetCameraViewHalf()
        {
            UnityEngine.Camera MainCamera = UnityEngine.Camera.main;

            float HalfHeight = MainCamera.orthographicSize;
            float HalfWidth = MainCamera.aspect * HalfHeight;
            return new Vector2(HalfWidth, HalfHeight);
        }

        public Vector2 GetCameraView()
        {
            return GetCameraViewHalf() * 2.0f;
        }
    }
}
