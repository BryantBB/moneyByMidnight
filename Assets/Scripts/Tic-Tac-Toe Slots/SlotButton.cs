using UnityEngine;
using UnityEngine.InputSystem;

public class SlotButton : MonoBehaviour
{

    public SlotManager slotManager;
    
    public Sprite idleSprite;
    public Sprite pressedSprite;
    public Sprite disabledSprite;
    
    private bool disabled = false;

    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = idleSprite;
        mainCamera = Camera.main;
    }

    void Update()
    {        
        if (disabled) return;
        if (Keyboard.current.enterKey.wasPressedThisFrame
        || Keyboard.current.cKey.wasPressedThisFrame
        || Keyboard.current.jKey.wasPressedThisFrame) Activate();
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseScrPos = Mouse.current.position.ReadValue();
            Vector3 mouseWrldPos = mainCamera.ScreenToWorldPoint(mouseScrPos);
            mouseWrldPos.z = transform.position.z;
            if (Physics2D.OverlapPoint(mouseWrldPos)) Activate();
        }
    }

    void Activate()
    {
        spriteRenderer.sprite = pressedSprite;
        CancelInvoke(nameof(ResetSprite));
        Invoke(nameof(ResetSprite), 0.15f);
        slotManager.StopSlot();
    }

    void ResetSprite()
    {
        if (!disabled) spriteRenderer.sprite = idleSprite;
    }

    public void Enable()
    {
        disabled = false;
        ResetSprite();
    }

    public void Disable()
    {
        disabled = true;
        CancelInvoke(nameof(ResetSprite));
        spriteRenderer.sprite = disabledSprite;
    }

}