using UnityEngine;
public class TowerInteraction : Interactable
{
    public override void Interact()
    {
        base.Interact();
        // Open the Upgrade menu
        // Make sure to check that the menu isn't currently open
        // Pass in a reference to the tower in question to a upgrade menu script
        Debug.Log("Interacted with Tower");
    }
}
