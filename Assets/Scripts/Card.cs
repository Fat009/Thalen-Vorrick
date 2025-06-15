using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CardGame
{
    public enum CardEvent
    {
        Flipped,
        Matched,
        Mismatched
    }

    public class Card : MonoBehaviour, ISubject, IPointerClickHandler
    {
        public int id; 
        public bool isFaceUp = false; 
        public Sprite faceSprite; 
        public Sprite backSprite; 
        private Image cardImage;

        private List<IObserver> observers = new List<IObserver>();
        private Coroutine flipCoroutine;

        void Start()
        {
           
            cardImage = GetComponent<Image>();

           
            if (cardImage == null)
            {
                Debug.LogError("Card Image not assigned.");
                return;
            }

            cardImage.sprite = backSprite;
        }

        public void Attach(IObserver observer) => observers.Add(observer);
        public void Detach(IObserver observer) => observers.Remove(observer);

        public void Notify(Card card, CardEvent cardEvent)
        {
            foreach (var observer in observers)
            {
                observer.OnNotify(card, cardEvent);
            }
        }

        public void Flip()
        {
            if (flipCoroutine != null)
            {
                StopCoroutine(flipCoroutine); 
            }
            flipCoroutine = StartCoroutine(FlipCard());
        }

        public void FlipForLoad()
        {
            if (flipCoroutine != null)
            {
                StopCoroutine(flipCoroutine);
            }
            flipCoroutine = StartCoroutine(FlipCardForLoad());
        }
        private IEnumerator FlipCardForLoad()
        {

            float duration = 0.5f;
            float elapsedTime = 0f;
            Quaternion originalRotation = transform.localRotation;


            while (elapsedTime < duration / 2)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / (duration / 2);
                transform.localRotation = Quaternion.Slerp(originalRotation, originalRotation * Quaternion.Euler(0, 90, 0), t);
                yield return null;
            }


           // isFaceUp = !isFaceUp;
            cardImage.sprite = isFaceUp ? faceSprite : backSprite;


            transform.localRotation = originalRotation * Quaternion.Euler(0, 90, 0);

            elapsedTime = 0f;
            while (elapsedTime < duration / 2)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / (duration / 2);
                transform.localRotation = Quaternion.Slerp(originalRotation * Quaternion.Euler(0, 90, 0), originalRotation, t);
                yield return null;
            }


            transform.localRotation = originalRotation;
            Notify(this, CardEvent.Flipped);
        }
        private IEnumerator FlipCard()
        {
            
            float duration = 0.5f; 
            float elapsedTime = 0f;
            Quaternion originalRotation = transform.localRotation;

            
            while (elapsedTime < duration / 2)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / (duration / 2);
                transform.localRotation = Quaternion.Slerp(originalRotation, originalRotation * Quaternion.Euler(0, 90, 0), t);
                yield return null;
            }

            
            isFaceUp = !isFaceUp;
            cardImage.sprite = isFaceUp ? faceSprite : backSprite;

           
            transform.localRotation = originalRotation * Quaternion.Euler(0, 90, 0);

            elapsedTime = 0f;
            while (elapsedTime < duration / 2)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / (duration / 2);
                transform.localRotation = Quaternion.Slerp(originalRotation * Quaternion.Euler(0, 90, 0), originalRotation, t);
                yield return null;
            }

            
            transform.localRotation = originalRotation;
            Notify(this, CardEvent.Flipped);
        }

      
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isFaceUp) 
            {
                Flip();
                FindObjectOfType<GameManager>().CardClicked(this);
            }
        }
        public void InitializeCardSprite()
        {
            if (cardImage == null)
            {
                cardImage = GetComponent<Image>();
            }
            if(isFaceUp)
            {
                cardImage.sprite =faceSprite ;
            }
            else
            {
                cardImage.sprite = backSprite;
            }
           
        }
    }
}
