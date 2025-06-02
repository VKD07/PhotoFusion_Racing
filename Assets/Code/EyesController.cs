using Code;
using Fusion;
using UnityEngine;

public class EyesController : NetworkBehaviour
{
    [SerializeField] private Transform _pupilParent;
    [SerializeField] private MeshRenderer [] eyeVisuals; 
    [Header("Movement Settings")] [SerializeField]
    private float maxOffsetY = 0.056f;

    [SerializeField] private float verticalLookRange = 80f;

    private Vector3 _initialLocalPosition;

    public override void Spawned()
    {
        _initialLocalPosition = _pupilParent.localPosition;
        
        if (HasInputAuthority)
        {
            SetActiveEyes(false);
        }
        else
        {
            SetActiveEyes(true);
        }
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

    private void SetActiveEyes(bool enable)
    {
        for (int i = 0; i < eyeVisuals.Length; i++)
        {
            eyeVisuals[i].enabled = enable;
        }
    }
}