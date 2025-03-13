using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MonoBehaviours
{
    public class ItemController : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        
        private const float BLINK_DURATION = 0.1f;
        private const int BLINK_COUNT = 3;
        private const float OFFSET = 0.1f;
        
        private Material _material;
        private Vector3 _startPosition;
        private bool _isAnimating;
        private CancellationTokenSource _animationCancellationTokenSource;

        private void Awake()
        {
            _material = _meshRenderer.material;
        }

        public Color Color => _material.color;

        public void SetMaterial(Material newMaterial)
        {
            _material = newMaterial;
            _meshRenderer.material = _material;
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            transform.position = Vector3.zero;
            gameObject.SetActive(false);
        }
        
        public Material GetMaterial()
        {
            return _material;
        }

        public async UniTaskVoid AnimateWrong(CancellationToken roundCancellationToken)
        {
            if (_isAnimating) return;

            try
            {
                _isAnimating = true;
                _startPosition = transform.position;
                _animationCancellationTokenSource?.Cancel();
                _animationCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(roundCancellationToken);

                await BlinkAsync(_animationCancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // Игнорируем отмену
            }
            finally
            {
                transform.position = _startPosition;
                _isAnimating = false;
            }
        }

        private async UniTask BlinkAsync(CancellationToken cancellationToken)
        {
            Vector3 startLocalPosition = transform.localPosition;
    
            try
            {
                for (var i = 0; i < BLINK_COUNT && !cancellationToken.IsCancellationRequested; i++)
                {
                    transform.localPosition = startLocalPosition + Vector3.left * OFFSET;
                    await UniTask.Delay((int)(BLINK_DURATION * 1000), cancellationToken: cancellationToken);

                    transform.localPosition = startLocalPosition + Vector3.right * OFFSET;
                    await UniTask.Delay((int)(BLINK_DURATION * 1000), cancellationToken: cancellationToken);
                }
            }
            finally
            {
                transform.localPosition = startLocalPosition;
            }
        }
    }
}