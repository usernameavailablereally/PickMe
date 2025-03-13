using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Services.Factories.Pools;
using UnityEngine;

namespace MonoBehaviourComponents
{
    public class ItemComponent : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;

        private const float BLINK_DURATION = 0.1f;
        private const int BLINK_COUNT = 3;
        private const float OFFSET = 0.1f;

        private MaterialData _materialData;
        private bool _isAnimating;
        private CancellationTokenSource _animationCancellationTokenSource;

        public Color Color => _materialData.Material.color;

        public void SetMaterial(MaterialData newMaterialData)
        {
            _materialData = newMaterialData;
            _meshRenderer.material = _materialData.Material;
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public MaterialData GetMaterialAlias()
        {
            return _materialData;
        }

        public async UniTaskVoid AnimateWrong(CancellationToken roundCancellationToken)
        {
            if (_isAnimating) return;

            try
            {
                _isAnimating = true;
                _animationCancellationTokenSource?.Cancel();
                _animationCancellationTokenSource =
                    CancellationTokenSource.CreateLinkedTokenSource(roundCancellationToken);

                await BlinkAsync(_animationCancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("AnimateWrong operation was cancelled");
            }
            finally
            {
                if (this != null)
                {
                    _isAnimating = false;
                }
            }
        }

        private async UniTask BlinkAsync(CancellationToken cancellationToken)
        {
            Vector3 startLocalPosition = transform.localPosition;
            for (var i = 0; i < BLINK_COUNT && !cancellationToken.IsCancellationRequested; i++)
            {
                transform.localPosition = startLocalPosition + Vector3.left * OFFSET;
                await UniTask.Delay((int)(BLINK_DURATION * 1000), cancellationToken: cancellationToken);

                transform.localPosition = startLocalPosition + Vector3.right * OFFSET;
                await UniTask.Delay((int)(BLINK_DURATION * 1000), cancellationToken: cancellationToken);
                transform.localPosition = startLocalPosition;
            }
        }
    }
}