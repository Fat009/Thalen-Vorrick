using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace CardGame
{
    public class GameManager : MonoBehaviour, IObserver
    {
        private Card firstCard;
        private Card secondCard;
        private int matchesFound = 0;
        private int matchedPairs = 0;
        private bool isGameCompleted = false;

        public TMP_Text scoreText;
        public TMP_Text comboText;
        public GameObject comboObject;
        public GameObject gameCompletedPanel;
        public AudioClip cardFlippedAudio;
        public AudioClip cardMatchedAudio;
        public AudioClip cardMismatchedAudio;
        public AudioClip gameCompletedAudio;
        private AudioSource audioSource;

        private bool isCheckingMatch = false;
        private List<int> matchedCardIds = new List<int>();

        private int comboStreak = 0;
        private int comboMultiplier = 1;

        private void Start()
        {
            UpdateScoreText();
            audioSource = GetComponent<AudioSource>();
        }

        public void CardClicked(Card clickedCard)
        {
            if (isCheckingMatch) return;

            if (firstCard == null)
            {
                firstCard = clickedCard;
                firstCard.Flip();
                PlayAudio(cardFlippedAudio);
            }
            else if (secondCard == null && clickedCard != firstCard)
            {
                secondCard = clickedCard;
                secondCard.Flip();
                PlayAudio(cardFlippedAudio);
                isCheckingMatch = true;
                CheckForMatch();
            }
        }

        private void CheckForMatch()
        {
            try
            {
                if (firstCard.id == secondCard.id)
                {
                    comboStreak++;

                    if (comboStreak >= 2)
                    {
                        comboMultiplier = comboStreak; 
                        ShowComboText($"Combo x{comboMultiplier}!");
                    }
                    else
                    {
                        comboMultiplier = 1;
                        ShowComboText("");
                    }

                    matchesFound += comboMultiplier;
                    matchedPairs++; 
                    matchedCardIds.Add(firstCard.id);

                    PlayAudio(cardMatchedAudio);
                    UpdateScoreText();
                    CheckForGameCompletion();

                    firstCard.Notify(firstCard, CardEvent.Matched);
                    secondCard.Notify(secondCard, CardEvent.Matched);

                    ResetSelectedCards();
                }
                else
                {
                    comboStreak = 0;
                    comboMultiplier = 1;
                    ShowComboText("");

                    PlayAudio(cardMismatchedAudio);

                    firstCard.Notify(firstCard, CardEvent.Mismatched);
                    secondCard.Notify(secondCard, CardEvent.Mismatched);

                    Invoke("ResetCards", 1f);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error during CheckForMatch: " + e);
            }
        }

        private void ResetCards()
        {
            firstCard.Flip();
            secondCard.Flip();
            ResetSelectedCards();
        }

        private void ResetSelectedCards()
        {
            firstCard = null;
            secondCard = null;
            isCheckingMatch = false;
        }

        private void UpdateScoreText()
        {
            if (scoreText != null)
            {
                if (comboMultiplier > 1)
                    scoreText.text = $"Match Score: {matchesFound} (x{comboMultiplier} Combo!)";
                else
                    scoreText.text = $"Match Score: {matchesFound}";
            }
        }

        private void ShowComboText(string message)
        {
            if (comboText != null)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    comboObject.SetActive(true);
                    comboText.text = message;
                    CancelInvoke(nameof(ClearComboText));
                    Invoke(nameof(ClearComboText), 1.5f);
                }
                else
                {
                    comboObject.SetActive(false);
                }
            }
        }

        private void ClearComboText()
        {
            if (comboText != null)
            {
                comboText.text = "";
                comboObject.SetActive(false);
            }
        }

        private void CheckForGameCompletion()
        {
            int totalPairs = FindObjectsOfType<Card>().Length / 2;

            if (matchedPairs == totalPairs)
            {
                isGameCompleted = true;
                ShowGameCompletedPanel();
            }
        }

        private void ShowGameCompletedPanel()
        {
            if (gameCompletedPanel != null)
            {
                gameCompletedPanel.SetActive(true);
                PlayAudio(gameCompletedAudio);
            }
        }

        private void PlayAudio(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
            else
            {
                Debug.Log("AudioSource or AudioClip is missing");
            }
        }

        public void OnNotify(Card card, CardEvent cardEvent)
        {
            switch (cardEvent)
            {
                case CardEvent.Matched:
                    Debug.Log($"Card {card.id} matched!");
                    break;
                case CardEvent.Mismatched:
                    Debug.Log($"Card {card.id} mismatched!");
                    break;
            }
        }

        public int GetScore()
        {
            return matchesFound;
        }

        public bool IsGameCompleted()
        {
            return isGameCompleted;
        }

        public void RestoreGameState(SaveData data)
        {
            matchesFound = data.score;
            isGameCompleted = data.isGameCompleted;

            var uniqueMatchedIds = new HashSet<int>(GetMatchedCardIds());
            matchedPairs = uniqueMatchedIds.Count;

            UpdateScoreText();

            if (matchedPairs == FindObjectsOfType<Card>().Length / 2)
            {
                isGameCompleted = true;
                ShowGameCompletedPanel();
            }
        }

        public List<int> GetMatchedCardIds()
        {
            return matchedCardIds;
        }

        public void ResetGame()
        {
            matchesFound = 0;
            matchedPairs = 0;
            comboStreak = 0;
            comboMultiplier = 1;
            matchedCardIds.Clear();
            isGameCompleted = false;

            UpdateScoreText();
            ShowComboText("");
            if (gameCompletedPanel != null)
                gameCompletedPanel.SetActive(false);

            ResetSelectedCards();

            GridManager gridManager = FindObjectOfType<GridManager>();
            if (gridManager != null)
            {
                gridManager.RestartGame();
            }
        }
    }
}
