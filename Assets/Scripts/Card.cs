using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public bool isFaceUp = false;
    public Sprite faceSprite; 
    public Sprite backSprite;
    private Image cardImage;
    private Coroutine flipCoroutine;

    void Start()
    {
       
        cardImage = GetComponent<Image>();

      
        if (cardImage == null)
        {
            Debug.Log("Card Image is not found");
            return;
        }

        cardImage.sprite = backSprite; 
    }

    public void Flip()
    {
        if (flipCoroutine != null)
        {
            StopCoroutine(flipCoroutine); 
        }
        flipCoroutine = StartCoroutine(FlipCard());
    }

    private IEnumerator FlipCard()
    {
       
        float duration = 0.5f; 
        float elapsedTime = 0f;
        Quaternion originalRotation = transform.localRotation;

       
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            transform.localRotation = Quaternion.Slerp(originalRotation, originalRotation * Quaternion.Euler(0, 180, 0), t);
            yield return null;
        }

       
        isFaceUp = !isFaceUp;
        cardImage.sprite = isFaceUp ? faceSprite : backSprite;

        
        transform.localRotation = originalRotation * Quaternion.Euler(0, 180, 0);
    }

   
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isFaceUp) 
        {
            Flip();
        }
    }
}
