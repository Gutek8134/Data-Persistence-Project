using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    [SerializeField] Text HiScore;
    private string playerName;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    private int hiScore;
    private string hiScoreHolder;
    
    private bool m_GameOver = false;

    
    // Start is called before the first frame update
    void Start()
    {
        playerName = MenuUIManager.Instance.playerName;
        LoadData();
        HiScore.text = $"Best Score: {hiScoreHolder}: {hiScore}";
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
        if (m_Points > hiScore)
        {
            HiScore.text = $"Best Score: {playerName}: {m_Points}";
            hiScore = m_Points;
            hiScoreHolder = playerName;
        }
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        SaveData();
    }

    [System.Serializable]
    public class SavedData{
        public int highScore;
        public string highScoreHolder;
    }

    public void SaveData() 
    {
        SavedData data = new SavedData();
        data.highScore = hiScore;
        data.highScoreHolder = hiScoreHolder;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SavedData data = JsonUtility.FromJson<SavedData>(json);

            hiScore = data.highScore;
            hiScoreHolder = data.highScoreHolder;
        }
        else {
            HiScore.text = "Best Score: None yet. Be the first one!";
        }
    }

}
