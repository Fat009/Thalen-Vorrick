using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CardGame
{
  

    public class GameManager : MonoBehaviour, IObserver
    {
        private Card firstCard;
        private Card secondCard;
        private int matchesFound = 0;
        private bool isGameCompleted = false;
        private bool isInGameplay = false; 

        public TMP_Text scoreText;
        public GameObject gameCompletedPanel; 
        public AudioClip cardFlippedAudio;
        public AudioClip cardMatchedAudio;
        public AudioClip cardMismatchedAudio;
        public AudioClip gameCompletedAudio;
        private AudioSource audioSource;

        private GridManager gridManager;
        private bool isCheckingMatch = false;
        private void Start()
        {
            UpdateScoreText();
            audioSource = GetComponent<AudioSource>();
            gridManager = FindObjectOfType<GridManager>();
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
                    matchesFound++;
                    PlayAudio(cardMatchedAudio);
                    UpdateScoreText();
                    CheckForGameCompletion();

                    firstCard.Notify(firstCard, CardEvent.Matched);
                    secondCard.Notify(secondCard, CardEvent.Matched);

                    ResetSelectedCards();
                }
                else
                {
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
                scoreText.text = "Match Score: " + matchesFound;
            }
        }

        private void CheckForGameCompletion()
        {
          
            if (matchesFound == (FindObjectsOfType<Card>().Length / 2))
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

        public void SetInGameplay(bool isGameplay)
        {
            isInGameplay = isGameplay;  
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
    }
}
