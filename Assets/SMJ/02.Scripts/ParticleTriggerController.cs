using UnityEngine;

public class ParticleTriggerController : MonoBehaviour
{
    [SerializeField] private ParticleSystem effectParticle;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float radius = 5f;
    [SerializeField] private LayerMask targetLayer;

    private void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, targetLayer);
        bool playerFound = false;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(playerTag))
            {
                playerFound = true;
                break;
            }
        }

        if (playerFound)
        {
            if (!effectParticle.isPlaying)
            {
                effectParticle.Play();
            }
        }
        else
        {
            if (effectParticle.isPlaying)
            {
                effectParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }

    // 파티클을 즉시 중지하고 모든 입자를 제거하는 메서드
    public void StopParticleImmediate()
    {
        if (effectParticle != null)
        {
            print("중지!");
            effectParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            effectParticle.Clear();
        }
    }

    // Update 체크를 비활성화하는 메서드
    public void DisableChecking()
    {
        StopParticleImmediate();
        enabled = false;
    }

    public void EnableChecking()
    {
        enabled = true;
    }
}