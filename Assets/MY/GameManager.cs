using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("계단")]
    public GameObject[] Stairs;
    public bool[] isTurn;

    private enum State { Start, Left, Right };
    private State state;
    private Vector3 oldPosition;

    [Header("UI")]
    public GameObject Ui_GameOver;
    public TextMeshProUGUI textMaxScore;
    public TextMeshProUGUI textNowScore;
    public TextMeshProUGUI textShowScore;
    private int maxScore = 0;
    private int nowScore = 0;

    [Header("사운드")]
    public AudioClip bgmSound;
    public AudioClip dieSound;
    public AudioClip jumpSound;
    public AudioClip turnSound;
    public AudioClip congratsSound;
    private AudioSource audioSource;
    private bool isGameOver = false;

    [Header("아이템 관련")]
    public GameObject itemPrefab;
    [Range(0, 100)]
    public int itemSpawnChance = 30;

    private Player player;

    void Start()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        Screen.SetResolution(720, 1280, false);
#endif
        Instance = this;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        player = FindObjectOfType<Player>();
        maxScore = PlayerPrefs.GetInt("MaxScore", 0);

        Init();
        InitStairs();
        PlayBgm();
    }

    void Update()
    {
        // 게임 오버 상태를 감지하지만 사운드는 나중에 처리
        if (!isGameOver && Ui_GameOver != null && Ui_GameOver.activeSelf)
        {
            isGameOver = true;
        }

        if (player != null && !isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                PlayJumpSound();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                PlayTurnSound();
            }
        }
    }

    public void Init()
    {
        state = State.Start;
        oldPosition = Vector3.zero;

        isTurn = new bool[Stairs.Length];

        for (int i = 0; i < Stairs.Length; i++)
        {
            Stairs[i].transform.position = Vector3.zero;
            isTurn[i] = false;
        }

        nowScore = 0;
        textShowScore.text = nowScore.ToString();

        Ui_GameOver.SetActive(false);
        isGameOver = false;
        PlayBgm();
    }

    public void InitStairs()
    {
        for (int i = 0; i < Stairs.Length; i++)
        {
            switch (state)
            {
                case State.Start:
                    Stairs[i].transform.position = new Vector3(0.75f, -0.1f, 0);
                    state = State.Right;
                    break;
                case State.Left:
                    Stairs[i].transform.position = oldPosition + new Vector3(-0.75f, 0.5f, 0);
                    isTurn[i] = true;
                    break;
                case State.Right:
                    Stairs[i].transform.position = oldPosition + new Vector3(0.75f, 0.5f, 0);
                    isTurn[i] = false;
                    break;
            }

            oldPosition = Stairs[i].transform.position;

            if (i != 0)
            {
                int ran = Random.Range(0, 5);
                if (ran < 2 && i < Stairs.Length - 1)
                {
                    state = state == State.Left ? State.Right : State.Left;
                }
            }

            TrySpawnItemOnStair(i);
        }
    }

    public void SpawnStair(int Cnt)
    {
        int ran = Random.Range(0, 5);
        if (ran < 2)
        {
            state = state == State.Left ? State.Right : State.Left;
        }

        switch (state)
        {
            case State.Left:
                Stairs[Cnt].transform.position = oldPosition + new Vector3(-0.75f, 0.5f, 0);
                isTurn[Cnt] = true;
                break;
            case State.Right:
                Stairs[Cnt].transform.position = oldPosition + new Vector3(0.75f, 0.5f, 0);
                isTurn[Cnt] = false;
                break;
        }

        oldPosition = Stairs[Cnt].transform.position;
        TrySpawnItemOnStair(Cnt);
    }

    private void TrySpawnItemOnStair(int stairIndex)
    {
        if (itemPrefab == null) return;

        int rand = Random.Range(0, 100);
        if (rand >= itemSpawnChance) return;

        Vector3 stairPos = Stairs[stairIndex].transform.position;
        Vector3 spawnPos = stairPos + new Vector3(0f, 0.3f, 0f);
        Instantiate(itemPrefab, spawnPos, Quaternion.identity);
    }

    public void GameOver()
    {
        StartCoroutine(ShowGameOver());
    }

    IEnumerator ShowGameOver()
    {
        yield return new WaitForSeconds(1f);

        Ui_GameOver.SetActive(true);

        if (nowScore > maxScore)
        {
            maxScore = nowScore;
            PlayerPrefs.SetInt("MaxScore", maxScore);
            PlayerPrefs.Save();
            PlayCongratsSound(); // 최고 점수일 경우 축하 사운드
        }
        else
        {
            PlayGameOverSound(); // 최고 점수가 아닐 경우 일반 사운드
        }

        textMaxScore.text = maxScore.ToString();
        textNowScore.text = nowScore.ToString();
    }

    public void AddScore()
    {
        nowScore++;
        textShowScore.text = nowScore.ToString();
    }

    public void ResetMaxScore()
    {
        maxScore = 0;
        PlayerPrefs.SetInt("MaxScore", maxScore);
        PlayerPrefs.Save();

        if (textMaxScore != null)
        {
            textMaxScore.text = maxScore.ToString();
        }
    }

    private void PlayBgm()
    {
        if (bgmSound != null)
        {
            audioSource.Stop();
            audioSource.clip = bgmSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void PlayGameOverSound()
    {
        if (dieSound != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(dieSound);
        }
    }

    private void PlayJumpSound()
    {
        if (jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

    private void PlayTurnSound()
    {
        if (turnSound != null)
        {
            audioSource.PlayOneShot(turnSound);
        }
    }

    private void PlayCongratsSound()
    {
        if (congratsSound != null)
        {
            audioSource.PlayOneShot(congratsSound);
        }
    }
}
