using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
  [SerializeField] private List<Mole> moles;

  [Header("UI objects")]
  [SerializeField] private GameObject playButton;
  [SerializeField] private GameObject gameUI;
  [SerializeField] private GameObject outOfTimeText;
  [SerializeField] private GameObject bombText;
  [SerializeField] private TMPro.TextMeshProUGUI timeText;
  [SerializeField] private TMPro.TextMeshProUGUI scoreText1;
  [SerializeField] private TMPro.TextMeshProUGUI scoreText2;

  // Hardcoded variables you may want to tune.
  private float startingTime = 30f;

  // Global variables
  private float timeRemaining;
  private HashSet<Mole> currentMoles = new HashSet<Mole>();
  private int scoreP1;
  private int scoreP2;
  public bool playing = false;

  // This is public so the play button can see it.
  public void StartGame() {
    // Hide/show the UI elements we don't/do want to see.
    playButton.SetActive(false);
    outOfTimeText.SetActive(false);
    bombText.SetActive(false);
    gameUI.SetActive(true);
    // Hide all the visible moles.

    for (int i = 0; i < moles.Count; i++) {
      moles[i].Hide();
      moles[i].SetIndex(i);
    }

    moles[0].SetKey(new KeyCode[]{KeyCode.Q, KeyCode.I});
    moles[1].SetKey(new KeyCode[]{KeyCode.W, KeyCode.O});
    moles[2].SetKey(new KeyCode[]{KeyCode.E, KeyCode.P});
    moles[3].SetKey(new KeyCode[]{KeyCode.A, KeyCode.K});
    moles[4].SetKey(new KeyCode[]{KeyCode.S, KeyCode.L});
    moles[5].SetKey(new KeyCode[]{KeyCode.D, KeyCode.Semicolon});
    moles[6].SetKey(new KeyCode[]{KeyCode.Z, KeyCode.Less});
    moles[7].SetKey(new KeyCode[]{KeyCode.X, KeyCode.Greater});
    moles[8].SetKey(new KeyCode[]{KeyCode.C, KeyCode.Question});

    // Remove any old game state.
    currentMoles.Clear();
    // Start with 30 seconds.
    timeRemaining = startingTime;
    scoreP1 = 0;
    scoreP2 = 0;
    scoreText1.text = "0";
    scoreText2.text = "0";
    playing = true;
  }

  public void GameOver(int type) {
    // Show the message.
    if (type == 0) {
      outOfTimeText.SetActive(true);
    } else {
      bombText.SetActive(true);
    }
    // Hide all moles.
    foreach (Mole mole in moles) {
      mole.StopGame();
    }
    // Stop the game and show the start UI.
    playing = false;
    playButton.SetActive(true);
  }

  // Update is called once per frame
  void Update() {
    if (playing) {
      // Update time.
      timeRemaining -= Time.deltaTime;
      if (timeRemaining <= 0) {
        timeRemaining = 0;
        GameOver(0);
      }
      timeText.text = $"{(int)timeRemaining / 60}:{(int)timeRemaining % 60:D2}";
      // Check if we need to start any more moles.
      if (currentMoles.Count <= (scoreP1+scoreP2 / 10)) {
        // Choose a random mole.
        int index = Random.Range(0, moles.Count);
        // Doesn't matter if it's already doing something, we'll just try again next frame.
        if (!currentMoles.Contains(moles[index])) {
          currentMoles.Add(moles[index]);
          moles[index].Activate(scoreP1+scoreP2 / 10);
        }
      }
    }
  }

  public void AddScore(int player, int moleIndex) {
    // Add and update score.
    if (player == 1) {
      scoreP1 += 1;
      scoreText1.text = $"{scoreP1}";
    }
    else {
      scoreP2 += 1;
      scoreText2.text = $"{scoreP2}";
    }
    // Increase time by a little bit.
    timeRemaining += 1;
    // Remove from active moles.
    currentMoles.Remove(moles[moleIndex]);
  }

  public void Missed(int moleIndex, bool isMole) {
    if (isMole) {
      // Decrease time by a little bit.
      timeRemaining -= 2;
    }
    // Remove from active moles.
    currentMoles.Remove(moles[moleIndex]);
  }
}
