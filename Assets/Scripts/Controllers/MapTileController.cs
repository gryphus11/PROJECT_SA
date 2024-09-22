using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTileController : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        Camera camera = collision.GetComponent<Camera>();
        if (camera == null)
            return;

        Vector3 direction = camera.transform.position - transform.position;

        float dirX = direction.x < 0 ? -1 : 1;
        float dirY = direction.y < 0 ? -1 : 1;

        // 맵을 좌우로 이동
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        { 
            transform.Translate(Vector3.right * dirX * 200.0f);
        }
        else // 맵을 상하로 이동
        {
            transform.Translate(Vector3.up * dirY * 200.0f);
        }
    }
}
