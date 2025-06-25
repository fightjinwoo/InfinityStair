using UnityEngine;

public class TimeItem : MonoBehaviour
{
    public float timeBonus = 5f; // �� �������� ������ �þ�� �ð�

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
