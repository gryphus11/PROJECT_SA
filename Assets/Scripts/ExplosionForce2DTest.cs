using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionForce2DTest : MonoBehaviour
{
    public float explosionForce = 100f;         // ���߷� ũ��
    public float explosionRadius = 5f;          // ���� �ݰ�

    private Vector2 _explosionPosition;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // ���� ���� (���콺 Ŭ�� ��ġ)
            _explosionPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // �ݰ� ���� �ִ� ��� ������Ʈ ã��
            Collider2D[] colliders = Physics2D.OverlapCircleAll(_explosionPosition, explosionRadius);
            foreach (Collider2D collider in colliders)
            {
                var mc = collider.GetComponent<MonsterController>();

                if (mc == null)
                    continue;


                mc.OnDamaged(null, 0);
                return;
            }
        }
    }

    private void OnDrawGizmos()
    {
        // ���� �ݰ��� �ð������� ǥ��
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_explosionPosition, explosionRadius);
    }
}