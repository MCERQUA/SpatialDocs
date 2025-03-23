using UnityEngine;
using SpatialSys.UnitySDK;

public class DoorwayPortal : MonoBehaviour
{
    [Tooltip("The ID of the destination space to teleport to")]
    [SerializeField] private string destinationSpaceID = "YOUR_DESTINATION_SPACE_ID";
    
    [Tooltip("Whether to show a confirmation popup before teleporting")]
    [SerializeField] private bool showPopup = true;
    
    [Tooltip("Optional visual effect to play when teleport is triggered")]
    [SerializeField] private GameObject portalEffect;
    
    private void Start()
    {
        if (portalEffect != null)
        {
            portalEffect.SetActive(false);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // In Spatial, we don't use tags to identify avatars
        // Instead, we check if any collider enters our trigger and then teleport
        Debug.Log("Something entered the portal trigger zone");
        
        if (portalEffect != null)
        {
            portalEffect.SetActive(true);
        }
        
        // Teleport after a short delay to allow for visual effect to be seen
        Invoke("TeleportPlayer", 0.5f);
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (portalEffect != null)
        {
            portalEffect.SetActive(false);
        }
    }
    
    private void TeleportPlayer()
    {
        Debug.Log("Teleporting to space: " + destinationSpaceID);
        
        // Use the Spatial SDK's teleport method
        // This will teleport the local player to the specified space
        SpatialBridge.spaceService.TeleportToSpace(destinationSpaceID, showPopup);
    }
}