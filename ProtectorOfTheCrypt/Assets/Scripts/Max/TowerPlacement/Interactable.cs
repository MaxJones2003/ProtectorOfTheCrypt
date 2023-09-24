using UnityEngine;
using UnityEngine.EventSystems;

public class Interactable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,  
IPointerEnterHandler, IPointerExitHandler
{
    /* 
    IpointerDownHandler detects the down press of a mouse button
    IPointerUpHandler detects the release of a mouse button
    IPointerEnterHandler detects when the mouse is hovering over an object
    IPointerExitHandler detects when the mouse stops hovering over an object
    */
    [Header("Interactable Variables")]
    protected bool isCurrentlyInteractable = true;
    [Header("Dragging Variables")]
    protected bool isDraggable = false;
    protected Vector2 offset;
    protected bool isDragging = false;
    protected float dragTime;
    private float initialDragTime;
    protected float maxDragTime = 0.6f;


    
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("hi");
        if(!isCurrentlyInteractable)
            return;
        Interact(); //This is important!!! Make sure the interactable objects have isCurrentlyInteractable set to true on start
    }
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if(!isCurrentlyInteractable)
            return;
    }
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if(!isCurrentlyInteractable)
            return;
    }
    /// <summary>
    /// This may be used to highlight an object that the player is hovering over
    /// </summary>
    /// <param name="eventData"></param> <summary>
    /// 
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Entered");
        if(!isCurrentlyInteractable)
            return;
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if(!isCurrentlyInteractable)
            return;
    }
    public virtual void Interact()
    {
        Debug.Log("hi");
    }

}
