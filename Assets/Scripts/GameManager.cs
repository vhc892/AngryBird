using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    [SerializeField] private float secondsToWaitBeforDeathCheck = 3f;
    [SerializeField] private GameObject restartScreenObject;
    [SerializeField] private SlingShotHandler slingShotHandler;
    [SerializeField] private Image nextLevelImage;
    public static GameManager instance;

    public int maxNumberOfShot = 3;
    private int usedNumberOfShots;
    private IconHandler iconHandler;
    private List<Enemy> enemylist = new List<Enemy>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        iconHandler = FindObjectOfType<IconHandler>();

        Enemy[] enemy = FindObjectsOfType<Enemy>();
        for(int i = 0; i < enemy.Length; i++)
        {
            enemylist.Add(enemy[i]);
        }
        nextLevelImage.enabled = false;
    }

    public void UseShot()
    {
        usedNumberOfShots++;
        iconHandler.UseShot(usedNumberOfShots);
        CheckForLastShot();
    }
    public bool HasEnoughShots()
    {
        if(usedNumberOfShots < maxNumberOfShot)
        {
            return true;
        }
        else { return false; }
    }
    public void CheckForLastShot()
    {
        if(usedNumberOfShots == maxNumberOfShot )
        {
            StartCoroutine(CheckAfterWaitTime());
        }
    }
    private IEnumerator CheckAfterWaitTime()
    {
        yield return new WaitForSeconds( secondsToWaitBeforDeathCheck );
        if(enemylist.Count == 0 )
        {
            //win
            WinGame();
        }
        else
        {
            //loss
            RestartGame();
        }
    }
    public void RemoveEnemy(Enemy enemy)
    {
        enemylist.Remove(enemy);
        CheckForAllDeathEnemy();
    }
    private void CheckForAllDeathEnemy()
    {
        if(enemylist.Count == 0)
        {
            //win
            WinGame();
        }
    }
    private void WinGame()
    {
        restartScreenObject.SetActive(true);
        slingShotHandler.enabled = false;
        // have more level ?
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int maxLevel = SceneManager.sceneCountInBuildSettings;
        if(currentSceneIndex + 1 < maxLevel)
        {
            nextLevelImage.enabled = true;
        }
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
