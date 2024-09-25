using UnityEngine;

public class DynamicSortingOrder : MonoBehaviour
{
    private SpriteRenderer[] spriteRenderers;   // 다중 SpriteRenderer를 처리하기 위해 배열 사용
    private Vector3 previousPosition;           // 이전 위치를 저장하여 이동 감지
    private const int sortingFactor = 1000;     // 정밀도를 높이기 위한 상수 값
    private const int minSortingOrder = 0;      // 최소 sortingOrder 값 설정 (음수 방지)

    void Start()
    {
        // 오브젝트 내 모든 SpriteRenderer를 가져옴 (다중 처리 가능)
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        previousPosition = transform.position;
    }

    void Update()
    {
        // 오브젝트가 실제로 이동했을 때만 sortingOrder를 업데이트
        if (transform.position != previousPosition)
        {
            UpdateSortingOrder();
            previousPosition = transform.position;  // 위치 업데이트
        }
    }

    private void UpdateSortingOrder()
    {
        // y 좌표 기반으로 모든 SpriteRenderer의 sortingOrder 업데이트
        int sortingOrder = Mathf.Max(minSortingOrder, Mathf.RoundToInt(-transform.position.y * sortingFactor));

        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.sortingOrder = sortingOrder;
        }
    }
}
