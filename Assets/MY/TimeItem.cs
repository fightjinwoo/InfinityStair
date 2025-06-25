using UnityEngine;

public class TimeItem : MonoBehaviour
{
    public float timeBonus = 5f; // 이 아이템을 먹으면 늘어나는 시간

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.AddTime(timeBonus);
                Destroy(gameObject);
            }
        }
    }
}
