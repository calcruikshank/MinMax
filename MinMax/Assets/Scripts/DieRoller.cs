using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class DieRoller : MonoBehaviour
{
    public static DieRoller singleton;
    public GameObject diePrefab, botPrefab, timerPanel, dieValuePanel, joinText, mainCanvas;
    public GameObject[] addBotButtons;
    public SpriteRenderer jefferyLogo;
    public Transform placeDiceHere, placePlayersHere;
    [HideInInspector] public DieScript currentDie;
    public TMP_Text valueText, timerText;
    public Button rollButton, resetButton, optionsButton, xButton, playButton;
    public MenuStatScript[] playerPanels;
    public bool useBots = true, useSameStats = true;
    public Toggle botsToggle, sameStatsToggle;
    public List<string> sameStats = new List<string>();
    public List<string> statStrings = new List<string>();
    bool isRemovingPlayer = false;
    bool isAddingPlayer = false;
    bool isAnimating = false;
    public float dieSpawnSize = 0.75f;

    void Awake()
    {
        if (singleton is null)
        {
            singleton = this;
        }
    }

    void Start()
    {
        Button_RollSameStats();
        Toggle_UseBots(true);
        botsToggle.isOn = useBots;
        sameStatsToggle.isOn = useSameStats;
        isAnimating = true;
        mainCanvas.SetActive(false);
        jefferyLogo.transform.localScale = Vector3.zero;
        jefferyLogo.transform.DOScale(Vector3.one, 1.0f).OnComplete(() => {
            mainCanvas.SetActive(true);
            isAnimating = false;
        });
        jefferyLogo.DOFade(SoundManager.singleton.pcs.Count > 0 ? 0 : 0.55f, 0.35f);
        
    }

    public void Button_RollDie()
    {
        if (isRemovingPlayer || isAddingPlayer || isAnimating) return;
        if (PlayersAreReady()) return;
        if (currentDie != null && !EveryoneHasUsedCurrentDie()) return;
        timerPanel.transform.DOScale(0, 0.2f).OnComplete(() => {
            timerPanel.SetActive(false);
        });
        dieValuePanel.transform.DOScale(0, 0.2f).OnComplete(() => {
            dieValuePanel.SetActive(false);
        });
        currentDie = null;
        rollButton.interactable = false;
        GameObject rolledDie = Instantiate(diePrefab, placeDiceHere.position, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        rolledDie.transform.localScale = new Vector3(dieSpawnSize, dieSpawnSize, dieSpawnSize);
        rolledDie.GetComponent<DieScript>().thisRB.AddForce(Vector3.right * Random.Range(200, 1000));
        rolledDie.GetComponent<DieScript>().thisRB.AddTorque(Vector3.right * Random.Range(-200000, 200000));
        rolledDie.GetComponent<DieScript>().thisDR = this;
        foreach(PlayerCursor pc in SoundManager.singleton.pcs)
        {
            if (!pc.gameObject.activeSelf) continue;
            pc.usedCurrentDie = false;
            pc.thisMSS.readyPanel.SetActive(false);
            pc.thisMSS.addBotButton.gameObject.SetActive(false);
        }
        time = 6;
    }

    public void TogglePlayerJoining(bool on)
    {
        if (on)
        {
            GetComponent<PlayerInputManager>().EnableJoining();
        }
        else
        {
            GetComponent<PlayerInputManager>().DisableJoining();
        }
    }

    bool isStartingGame = false;

    public void Button_LoadGamesScene()
    {
        if (isAnimating) return;
        isStartingGame = true;
        GetComponent<PlayerInputManager>().DisableJoining();

        foreach(PlayerCursor pc in SoundManager.singleton.pcs)
        {
            pc.SetUnusedStatsToThree();
            pc.transform.SetParent(SoundManager.singleton.transform);
        }

        SceneManager.LoadScene("SampleScene");
    }

    public void Button_RollSameStats()
    {
        if (isAnimating) return;
        List<string> statsCopy = new List<string>(statStrings);
        for(int i = 0; i < 6; i++)
        {
            int randStat = Mathf.FloorToInt(Random.Range(0, statsCopy.Count));
            string newStat = statsCopy[randStat];
            sameStats.Add(newStat);
            statsCopy.RemoveAt(randStat);
        }
    }

    public bool EveryoneHasUsedCurrentDie()
    {
        if (SoundManager.singleton.pcs.Count < 1) return false;
        foreach(PlayerCursor pc in SoundManager.singleton.pcs)
        {
            if (!pc.gameObject.activeSelf) continue;
            if (!pc.usedCurrentDie) return false;
        }
        timerPanel.transform.DOScale(0, 0.2f).OnComplete(() => {
            timerPanel.SetActive(false);
        });
        dieValuePanel.transform.DOScale(0, 0.2f).OnComplete(() => {
            dieValuePanel.SetActive(false);
        });
        return true;
    }

    public bool PlayersAreRollingStats()
    {
        if (SoundManager.singleton.pcs.Count < 1) return false;
        foreach(PlayerCursor pc in SoundManager.singleton.pcs)
        {
            if (pc is null) continue;
            if (!pc.gameObject.activeSelf) continue;
            if (pc.thisMSS.waitPanel.activeSelf) return true;
        }
        return false;
    }

    public void CheckForDieUsage()
    {
        if (currentDie is null || !EveryoneHasUsedCurrentDie() || isAnimating) return;
        
        Destroy(currentDie.gameObject);
        currentDie = null;
        valueText.text = "-";
        rollButton.interactable = !PlayersAreReady();
    }

    public void OnPlayerJoined(PlayerInput pi)
    {
        if (isRemovingPlayer || isStartingGame || isAnimating) return;
        isAddingPlayer = true;
        pi.transform.SetParent(placePlayersHere);
        PlayerCursor joinedPlayer = pi.GetComponent<PlayerCursor>();
        joinedPlayer.Initialize(FirstUnusedMenuStatScript(), false);
        SoundManager.singleton.pcs.Add(joinedPlayer);
        joinText.SetActive(SoundManager.singleton.pcs.Count < 4);
        rollButton.gameObject.SetActive(SoundManager.singleton.pcs.Count > 0);
        rollButton.interactable = !PlayersAreRollingStats();
        resetButton.interactable = SoundManager.singleton.pcs.Count > 0;
        ResetAllStats();
        Invoke("SetRemovingPlayerToFalse", 0.1f);
        jefferyLogo.DOFade(SoundManager.singleton.pcs.Count > 0 ? 0 : 0.55f, 0.35f);
    }

    public void OnPlayerLeft(PlayerInput pi)
    {
        if (isAddingPlayer) return;
        if (pi.currentControlScheme == "Keyboard&Mouse") Cursor.visible = true;
        isRemovingPlayer = true;
        PlayerCursor leftPlayer = pi.GetComponent<PlayerCursor>();
        leftPlayer.thisMSS.backgroundPanel.gameObject.SetActive(false);
        joinText.SetActive(SoundManager.singleton.pcs.Count < 4);
        rollButton.gameObject.SetActive(SoundManager.singleton.pcs.Count > 0);
        rollButton.interactable = !PlayersAreRollingStats() && !PlayersAreReady();
        resetButton.interactable = SoundManager.singleton.pcs.Count > 0;
        Invoke("SetRemovingPlayerToFalse", 0.1f);
        jefferyLogo.DOFade(SoundManager.singleton.pcs.Count > 0 ? 0 : 0.55f, 0.35f);
    }

    void SetRemovingPlayerToFalse()
    {
        isRemovingPlayer = false;
        isAddingPlayer = false;
    }

    public MenuStatScript FirstUnusedMenuStatScript()
    {
        foreach(MenuStatScript mss in playerPanels)
        {
            if (mss.thisPC is null)
            {
                return mss;
            }
        }
        return null;
    }

    public void ResetAllStats()
    {
        foreach(MenuStatScript mss in playerPanels)
        {
            if (!mss.gameObject.activeSelf) return;
            mss.ResetTexts();
        }
        
        if (currentDie != null)
        {
            Destroy(currentDie.gameObject);
            currentDie = null;
        }
        valueText.text = "-";
        timerText.text = "";
        rollButton.interactable = true;

        foreach(PlayerCursor pc in SoundManager.singleton.pcs)
        {
            pc.usedCurrentDie = false;
        }

        StopCoroutine(Timer());

        if (currentDie != null)
        {
            Destroy(currentDie.gameObject);
        }

        currentDie = null;
        joinText.SetActive(SoundManager.singleton.pcs.Count < 4);
    }

    public void RerollAllStats()
    {
        foreach(PlayerCursor pc in SoundManager.singleton.pcs)
        {
            StartCoroutine(pc.thisMSS.RollStats());
        }
    }
    
    public int time = 6;

    public IEnumerator Timer()
    {
        timerText.text = ":0" + time.ToString();

        while (time > 0)
        {
            yield return new WaitForSeconds(1.35f);
            time--;
            timerText.text = ":0" + time.ToString();

            foreach(PlayerCursor pc in SoundManager.singleton.pcs)
            {
                if (pc.thisMSS.isBot && !pc.usedCurrentDie)
                {
                    pc.ChooseRandomStat(currentDie.CurrentValue());
                }
            }
        }

        timerText.text = "";

        foreach(PlayerCursor pc in SoundManager.singleton.pcs)
        {
            if (!pc.usedCurrentDie)
            {
                pc.ChooseRandomStat(currentDie.CurrentValue());
            }
            pc.usedCurrentDie = false;
        }

        if (currentDie != null)
        {
            Destroy(currentDie.gameObject);
        }

        currentDie = null;
        valueText.text = "-";
        Button_RollDie();
    }

    public void Toggle_SameStats(bool s)
    {
        if (PlayersAreRollingStats()) return;
        useSameStats = s;
    }

    public void Toggle_UseBots(bool b)
    {
        useBots = b;
        for(int i = SoundManager.singleton.pcs.Count - 1; i >= 0; i--)
        {
            if (SoundManager.singleton.pcs[i].thisMSS.isBot)
            {
                isRemovingPlayer = true;
                PlayerCursor leftPlayer = SoundManager.singleton.pcs[i];
                SoundManager.singleton.pcs[i].thisMSS.backgroundPanel.gameObject.SetActive(false);
                SoundManager.singleton.pcs[i].thisMSS.addBotButton.gameObject.SetActive(false);
                SoundManager.singleton.pcs.Remove(leftPlayer);
                joinText.SetActive(SoundManager.singleton.pcs.Count < 4);
                rollButton.gameObject.SetActive(SoundManager.singleton.pcs.Count > 0);
                rollButton.interactable = !PlayersAreRollingStats() && !PlayersAreReady();
                resetButton.interactable = SoundManager.singleton.pcs.Count > 0;
                Destroy(leftPlayer.gameObject);
                Invoke("SetRemovingPlayerToFalse", 0.1f);
            }
        }
        foreach(GameObject go in addBotButtons)
        {
            go.SetActive(b);
        }
        jefferyLogo.DOFade(SoundManager.singleton.pcs.Count > 0 ? 0 : 0.55f, 0.35f);
    }

    public void AddBot()
    {
        if (!useBots || currentDie != null || isStartingGame || isAnimating) return;
        isAddingPlayer = true;
        GameObject botCursor = Instantiate(botPrefab);
        botCursor.transform.SetParent(placePlayersHere);
        PlayerCursor joinedPlayer = botCursor.GetComponent<PlayerCursor>();
        MenuStatScript first = FirstUnusedMenuStatScript();
        int mssIndex = System.Array.IndexOf(playerPanels, first);
        joinedPlayer.Initialize(first, true);
        SoundManager.singleton.pcs.Add(joinedPlayer);
        joinText.SetActive(SoundManager.singleton.pcs.Count < 4);
        rollButton.gameObject.SetActive(SoundManager.singleton.pcs.Count > 0);
        rollButton.interactable = !PlayersAreRollingStats() && !PlayersAreReady();
        resetButton.interactable = SoundManager.singleton.pcs.Count > 0;
        ResetAllStats();
        Invoke("SetRemovingPlayerToFalse", 0.1f);
        jefferyLogo.DOFade(SoundManager.singleton.pcs.Count > 0 ? 0 : 0.55f, 0.35f);
    }

    public void ResetAllThings(){
        foreach(var mss in playerPanels){
            RemovePlayer(mss);
        }
        playButton.gameObject.SetActive(false);
        ResetAllStats();
    }

    public bool PlayersAreReady()
    {
        if (SoundManager.singleton.pcs.Count < 1) return false;
        foreach(PlayerCursor pc in SoundManager.singleton.pcs)
        {
            if (!pc.thisMSS.IsFull()) return false;
        }
        playButton.gameObject.SetActive(true);
        playButton.transform.localScale = Vector3.zero;
        playButton.transform.DOScale(1, 0.2f);
        return true;
    }

    public void RemovePlayer(MenuStatScript mss)
    {
        if (isAddingPlayer) return;
        isRemovingPlayer = true;
        PlayerCursor leftPlayer = mss.thisPC;
        leftPlayer.isAnimating = true;
        if (leftPlayer.playerInput != null && leftPlayer.playerInput.currentControlScheme == "Keyboard&Mouse") Cursor.visible = true;
        SoundManager.singleton.pcs.Remove(leftPlayer);
        int mssIndex = System.Array.IndexOf(playerPanels, leftPlayer.thisMSS);
        leftPlayer.thisMSS.backgroundPanel.GetComponent<RectTransform>().DOAnchorPosY(-230, 0.2f).OnComplete( ()=> {
            leftPlayer.isAnimating = false;
            leftPlayer.thisMSS.backgroundPanel.gameObject.SetActive(false);
            leftPlayer.thisMSS.addBotButton.gameObject.SetActive(true);
        });
        joinText.SetActive(SoundManager.singleton.pcs.Count < 4);
        rollButton.gameObject.SetActive(SoundManager.singleton.pcs.Count > 0);
        rollButton.interactable = !PlayersAreRollingStats() && !PlayersAreReady();
        resetButton.interactable = SoundManager.singleton.pcs.Count > 0;
        
        Destroy(leftPlayer.gameObject);
        Invoke("SetRemovingPlayerToFalse", 0.1f);

        jefferyLogo.DOFade(SoundManager.singleton.pcs.Count > 0 ? 0 : 0.55f, 0.35f);
    }
}
