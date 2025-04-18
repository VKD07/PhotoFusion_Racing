using Code;
using Fusion;
using UnityEngine;

public class PupilController : NetworkBehaviour
{
    [SerializeField] private Transform _pupilParent;

    [Header("Movement Settings")] [SerializeField]
    private float maxOffsetY = 0.056f;

    [SerializeField] private float verticalLookRange = 80f;

    private Vector3 _initialLocalPosition;

    public override void Spawned()
    {
        _initialLocalPosition = _pupilParent.localPosition;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData input))
        {
            float t = Mathf.InverseLerp(-verticalLookRange, verticalLookRange, -input.pupilVerticalPitch);

            float yOffset = Mathf.Lerp(-maxOffsetY, maxOffsetY, t);

            Vector3 newLocalPos = _initialLocalPosition;
            newLocalPos.y += yOffset;
            _pupilParent.localPosition = newLocalPos;
        }
    }
}