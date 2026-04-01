using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class TokenDisplay : MonoBehaviour
{

    // private Camera mainCamera;

    private SpriteRenderer spriteRenderer;
    private Material material;
    private Color flashColor = Color.white;
    private float flashTime = 2f; // 0.16
    public bool flash = false;

    void Awake()
    {
        // mainCamera = Camera.main; 
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
    }

    void Update()
    {
        // if (!cheatMode) return;
        // if (Mouse.current.leftButton.wasPressedThisFrame)
        // {
        //     print("E");
        //     // Vector2 mouseScrPos = Mouse.current.position.ReadValue();
        //     // Vector3 mouseWrldPos = mainCamera.ScreenToWorldPoint(mouseScrPos);
        //     // mouseWrldPos.z = transform.position.z;
        //     // if (Physics2D.OverlapPoint(mouseWrldPos)) slot.UpdateCheatToken(this, slotCheater.getCurrentToken());
        // }
    }

    public void FlashSprite(Sprite flashSprite)
    {
        flash = true;
        StartCoroutine(SpriteFlashRoutine(flashSprite));
    }

    public void FlashWhite()
    {
        // StartCoroutine(WhiteFlashRoutine());
    }

    IEnumerator SpriteFlashRoutine(Sprite flashSprite)
    {
        Sprite tokenSprite = spriteRenderer.sprite;
        while (flash)
        {
            spriteRenderer.sprite = flashSprite;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.sprite = tokenSprite;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator WhiteFlashRoutine()
    {
        material.SetColor("FlashColor", flashColor);
        float currentFlashAmount = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;
            currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime / flashTime));
            material.SetFloat("FlashAmount", currentFlashAmount);
            yield return null;
        }        
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}