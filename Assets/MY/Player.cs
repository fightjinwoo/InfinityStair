using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Vector3 startPosition;
    private Vector3 oldPosition;
    private bool isTurn = false;

    private int moveCut = 0;
    private int turnCnt = 0;
    private int spawnCnt = 0;

    private bool isDie = false;

    private AudioSource sound;

    [Header("? 타이머 설정")]
    public float maxPlayTime = 30f;
    private float currentTime;
    private bool isTimerActive = true;

    [Header("? 타이머 UI")]
    public TextMeshProUGUI timerText;

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        sound = GetComponent<AudioSource>();

        startPosition = transform.position;
        Init();
    }

    void Update()
    {
        if (isDie) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CharTurn();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CharMove();
        }

        if (isTimerActive)
        {
            currentTime -= Time.deltaTime;

            if (timerText != null)
                timerText.text = $"Time: {Mathf.CeilToInt(currentTime)}";

            if (currentTime <= 0f)
            {
                currentTime = 0f;
                isTimerActive = false;
                CharDie();
            }
        }
    }

    private void Init()
    {
        anim.SetBool("Die", false);
        transform.position = startPosition;
        oldPosition = startPosition;
        moveCut = 0;
        spawnCnt = 0;
        turnCnt = 0;
        isTurn = false;
        spriteRenderer.flipX = isTurn;
        isDie = false;

        currentTime = maxPlayTime;
        isTimerActive = true;

        if (timerText != null)
            timerText.text = $"Time: {Mathf.CeilToInt(currentTime)}";
    }

    public void CharTurn()
    {
        isTurn = !isTurn;
        spriteRenderer.flipX = isTurn;
    }

    public void CharMove()
    {
        if (isDie)
            return;

        sound.Play();

        moveCut++;
        MoveDirection();

        if (isFailTurn())
        {
            CharDie();
            return;
        }

        if (moveCut > 5)
        {
            RespawnStair();
        }

        GameManager.Instance.AddScore();
    }

    private void MoveDirection()
    {
        if (isTurn)
        {
            oldPosition = new Vector3(-0.75f, 0.5f, 0);
        }
        else
        {
            oldPosition = new Vector3(0.75f, 0.5f, 0);
        }

        transform.position += oldPosition;
        anim.SetTrigger("Move");
    }

    private bool isFailTurn()
    {
        bool result = false;

        if (GameManager.Instance.isTurn[turnCnt] != isTurn)
        {
            result = true;
        }

        turnCnt++;

        if (turnCnt > GameManager.Instance.Stairs.Length - 1)
        {
            turnCnt = 0;
        }

        return result;
    }

    private void RespawnStair()
    {
        GameManager.Instance.SpawnStair(spawnCnt);
        spawnCnt++;

        if (spawnCnt > GameManager.Instance.Stairs.Length - 1)
        {
            spawnCnt = 0;
        }
    }

    private void CharDie()
    {
        GameManager.Instance.GameOver();
        anim.SetBool("Die", true);
        isDie = true;
        isTimerActive = false;

        if (timerText != null)
            timerText.text = "Time: 0";
    }

    public void ButtonRestart()
    {
        Init();
        GameManager.Instance.Init();
        GameManager.Instance.InitStairs();
    }

    public void ButtonResetScore()
    {
        GameManager.Instance.ResetMaxScore(); // ? 최고 점수 초기화
    }

    public void AddTime(float amount)
    {
        if (isDie || !isTimerActive)
            return;

        currentTime += amount;
        if (currentTime > maxPlayTime)
            currentTime = maxPlayTime;

        if (timerText != null)
            timerText.text = $"Time: {Mathf.CeilToInt(currentTime)}";
    }
}
