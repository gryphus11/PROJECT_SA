using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public static class RigidbodyExtension
{
    /// <summary>
    /// 2D AddExplosionForce 구현. 지정된 위치와 반경을 기준으로 오브젝트에 폭발력을 적용합니다.
    /// </summary>
    /// <param name="rb">힘을 적용할 Rigidbody2D</param>
    /// <param name="explosionForce">적용할 폭발력의 크기</param>
    /// <param name="explosionPosition">폭발 지점</param>
    /// <param name="explosionRadius">폭발 반경</param>
    public static void AddExplosionForce(this Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, float explosionRadius)
    {
        // 오브젝트가 폭발 지점으로부터 얼마나 떨어져 있는지 계산
        var explosionDirection = (rb.position - explosionPosition);
        var explosionDistance = explosionDirection.magnitude;

        // 반경 내에 있는 경우에만 폭발력을 적용
        if (explosionDistance <= explosionRadius)
        {
            // 폭발 지점으로부터 오브젝트의 위치로 가는 방향으로 정규화
            var explosionDirNormalized = (rb.transform.position - (Vector3)explosionPosition).normalized;

            // 폭발 지점으로부터 멀어질수록 감소하는 힘 계산 (폭발력 * 반경에서의 비율)
            var force = explosionForce * (1 - (explosionDistance / explosionRadius));

            // 폭발 지점에서 오브젝트의 위치 방향으로 힘 적용
            rb.AddForce(explosionDirNormalized * force, ForceMode2D.Impulse);
        }
    }

    public static void AddExplosionMovePosition(this Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, float explosionRadius)
    {
        // 오브젝트가 폭발 지점으로부터 얼마나 떨어져 있는지 계산
        var explosionDirection = (rb.position - explosionPosition);
        var explosionDistance = explosionDirection.magnitude;

        // 반경 내에 있는 경우에만 폭발력을 적용
        if (explosionDistance <= explosionRadius)
        {
            // 폭발 지점으로부터 오브젝트의 위치로 가는 방향으로 정규화
            var explosionDirNormalized = (rb.transform.position - (Vector3)explosionPosition).normalized;

            // 폭발 지점으로부터 멀어질수록 감소하는 힘 계산 (폭발력 * 반경에서의 비율)
            var force = explosionForce * (1 - (explosionDistance / explosionRadius));

            // 폭발 지점에서 오브젝트의 위치 방향으로 힘 적용
            rb.AddForce(explosionDirNormalized * force, ForceMode2D.Impulse);
        }
    }
}
