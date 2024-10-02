using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public static class RigidbodyExtension
{
    /// <summary>
    /// 2D AddExplosionForce ����. ������ ��ġ�� �ݰ��� �������� ������Ʈ�� ���߷��� �����մϴ�.
    /// </summary>
    /// <param name="rb">���� ������ Rigidbody2D</param>
    /// <param name="explosionForce">������ ���߷��� ũ��</param>
    /// <param name="explosionPosition">���� ����</param>
    /// <param name="explosionRadius">���� �ݰ�</param>
    public static void AddExplosionForce(this Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, float explosionRadius)
    {
        // ������Ʈ�� ���� �������κ��� �󸶳� ������ �ִ��� ���
        var explosionDirection = (rb.position - explosionPosition);
        var explosionDistance = explosionDirection.magnitude;

        // �ݰ� ���� �ִ� ��쿡�� ���߷��� ����
        if (explosionDistance <= explosionRadius)
        {
            // ���� �������κ��� ������Ʈ�� ��ġ�� ���� �������� ����ȭ
            var explosionDirNormalized = (rb.transform.position - (Vector3)explosionPosition).normalized;

            // ���� �������κ��� �־������� �����ϴ� �� ��� (���߷� * �ݰ濡���� ����)
            var force = explosionForce * (1 - (explosionDistance / explosionRadius));

            // ���� �������� ������Ʈ�� ��ġ �������� �� ����
            rb.AddForce(explosionDirNormalized * force, ForceMode2D.Impulse);
        }
    }

    public static void AddExplosionMovePosition(this Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, float explosionRadius)
    {
        // ������Ʈ�� ���� �������κ��� �󸶳� ������ �ִ��� ���
        var explosionDirection = (rb.position - explosionPosition);
        var explosionDistance = explosionDirection.magnitude;

        // �ݰ� ���� �ִ� ��쿡�� ���߷��� ����
        if (explosionDistance <= explosionRadius)
        {
            // ���� �������κ��� ������Ʈ�� ��ġ�� ���� �������� ����ȭ
            var explosionDirNormalized = (rb.transform.position - (Vector3)explosionPosition).normalized;

            // ���� �������κ��� �־������� �����ϴ� �� ��� (���߷� * �ݰ濡���� ����)
            var force = explosionForce * (1 - (explosionDistance / explosionRadius));

            // ���� �������� ������Ʈ�� ��ġ �������� �� ����
            rb.AddForce(explosionDirNormalized * force, ForceMode2D.Impulse);
        }
    }
}
