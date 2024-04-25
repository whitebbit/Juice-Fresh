using UnityEngine;

namespace JuiceFresh.Scripts
{
    public class AdjustOrthographicSize : MonoBehaviour
    {
        private Camera camera;

        // Начальное соотношение сторон, которое вы хотите сохранить
        [SerializeField] private Vector2 referenceAspectRatio;
        private float _targetAspectRatio;

        // Ортографический размер, который вы хотите поддерживать при целевом соотношении сторон
        [SerializeField] private float baseOrthographicSize;

        void Start()
        {
            camera = GetComponent<Camera>();
            AdjustCameraSize();
        }

        void AdjustCameraSize()
        {
            _targetAspectRatio = referenceAspectRatio.x / referenceAspectRatio.y;

            var currentAspectRatio = (float)Screen.width / Screen.height;

            // Определяем, какой фактор больше: горизонтальное или вертикальное масштабирование
            var scaleFactor = currentAspectRatio / _targetAspectRatio;


            if (LevelManager.THIS == null)
            {
                if (scaleFactor < 1f)
                {
                    camera.orthographicSize = baseOrthographicSize / scaleFactor;
                }
                else
                {
                    camera.orthographicSize = baseOrthographicSize;
                }

                return;
            }

            switch (LevelManager.THIS.gameStatus)
            {
                case GameState.Map:
                case GameState.Playing when scaleFactor < 1f:
                    camera.orthographicSize = baseOrthographicSize / scaleFactor;
                    break;
                case GameState.Playing:
                    camera.orthographicSize = baseOrthographicSize;
                    break;
            }
        }

        void Update()
        {
            // Если экран меняет размер (например, при повороте или изменении разрешения), адаптируем камеру
            AdjustCameraSize();
        }
    }
}