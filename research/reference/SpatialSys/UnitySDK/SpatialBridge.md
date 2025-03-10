# SpatialBridge

Service Class

The main interface that provides access to Spatial services.
This acts like a bridge between user code and Spatial core functionality.

```csharp
void Start()
{
    SpatialBridge.coreGUIService.DisplayToastMessage("Hello World");
}
```

## Properties

| Property | Description |
| --- | --- |
| [actorService](../IActorService.md) | Service for interacting with actors and users in the space. |
| [adService](../IAdService.md) | Service to handle ads integration |
| [audioService](../IAudioService.md) | Service for handling audio and sound effects. |
| [badgeService](../IBadgeService.md) | Service for handling badges. |
| [cameraService](../ICameraService.md) | Provides access to all camera related functionality: Main camera state, player camera settings, camera shake, and target overrides. |
| [coreGUIService](../ICoreGUIService.md) | Service for handling all UI related functionality. |
| [eventService](../IEventService.md) | A service with helper methods for subscribing to events. |
| [graphicsService](../IGraphicsService.md) | A service for handling Graphics settings. |
| [inputService](../IInputService.md) | Service for handling all input related functionality. |
| [inventoryService](../IInventoryService.md) | Service to handle inventory and currency. |
| [loggingService](../ILoggingService.md) | Service for logging errors and messages to the console. |
| [marketplaceService](../IMarketplaceService.md) | Service to handle item purchases on the store. |
| [networkingService](../INetworkingService.md) | This service provides access to all the networking functionality in Spatial: connectivity, server management, matchmaking, remove events (RPCs/Messaging), etc. |
| [questService](../IQuestService.md) | Service for managing Quests on the space. |
| [spaceContentService](../ISpaceContentService.md) | Service for interacting with the current scene's synced objects |
| [spaceService](../ISpaceService.md) | Service for interacting with the current space. |
| [userWorldDataStoreService](../IUserWorldDataStoreService.md) | This service provides access to the `Users` datastore for the current `world`. Spaces that belong to the same `world` share the same user world datastore. |
| [vfxService](../IVFXService.md) | A service for handling visual effects. |
