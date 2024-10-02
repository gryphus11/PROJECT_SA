using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionForce2DTest : MonoBehaviour
{
    public float explosionForce = 100f;         // 폭발력 크기
    public float explosionRadius = 5f;          // 폭발 반경

    private Vector2 _explosionPosition;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 폭발 지점 (마우스 클릭 위치)
            _explosionPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 반경 내에 있는 모든 오브젝트 찾기
            Collider2D[] colliders = Physics2D.OverlapCircleAll(_explosionPosition, explosionRadius);
            foreach (Collider2D collider in colliders)
            {
                var mc = collider.GetComponent<MonsterController>();

                if (mc == null)
                    continue;


                mc.OnDamaged(null, 0);
                return;
                // Rigidbody2D가 있는 경우에만 폭발력 적용
                Rigidbody2D rb = collider.attachedRigidbody;

                if (rb != null)
                {
                    // Rigidbody2D에 폭발력 추가
                    rb.AddExplosionForce(explosionForce, _explosionPosition, explosionRadius);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        // 폭발 반경을 시각적으로 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_explosionPosition, explosionRadius);
    }
}