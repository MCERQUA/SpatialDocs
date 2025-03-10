# Spatial SDK Documentation Project
 
## Overview
This repository contains comprehensive documentation for the Spatial Unity SDK interfaces and components. The documentation is organized to provide clear, practical, and thorough coverage of each interface and its related components.
 
## Project Structure
 
```
SpatialDocs/
├── docs/
│   ├── PLAN.md                 # Project plan and progress tracking
│   ├── manifest.json           # Main documentation status tracker (now split)
│   ├── manifest-primary.json   # Primary interfaces tracking
│   ├── manifest-completed.json # Completed secondary components tracking
│   ├── manifest-incomplete.json # Incomplete components tracking
│   ├── SESSION_TEMPLATE.md     # Template for documentation sessions
│   ├── SESSIONS_INDEX.md       # Index of all documentation sessions
│   ├── sessions/               # Individual session documentation files
│   │   └── YYYY-MM-DD-Category-Name.md  # Session files by date and category
│   └── README.md               # Project overview
└── research/
    └── reference/              # Interface documentation
        └── SpatialSys/
            └── UnitySDK/       # Interface implementations
```
 
## CURRENT STATUS AND NEXT TASK
 
### Current Status (Updated 2025-03-10)
- Primary interfaces (24): 100% complete ✅
- Secondary components (120): 72/120 completed (60%) ✅
- Overall completion: ~66.7%
 
### Important Update: Documentation Structure Improvements
- **New Session-Based Manifest System**: The project now uses a split manifest structure for better management
- **New Sessions Organization**: Individual session files are now stored in the `docs/sessions/` directory with an index in `SESSIONS_INDEX.md`
 
### NEXT CATEGORY TO DOCUMENT
**Core Components** category is the next to document, continuing with remaining Core Components
 
To document components:
1. Use firecrawl_scrape to fetch documentation from toolkit.spatial.io
2. Create Markdown file in research/reference/SpatialSys/UnitySDK/
3. Follow documentation template (see Example section below)
4. Create new session manifest in docs/manifests/
5. Create session log file in docs/sessions/
6. Mark as completed in README.md
 
## Documentation Process
Each component is documented in a dedicated session following these steps:
1. Content gathering from reference
2. Documentation structure creation
3. Code examples implementation
4. Quality review
5. Progress tracking update
6. GitHub file upload (after user confirmation)
7. Update of all tracking files
 
For a detailed process, see:
- [docs/SESSION_TEMPLATE.md](docs/SESSION_TEMPLATE.md) for the documentation template
- [docs/COMPONENT_DOCUMENTATION_CHECKLIST.md](docs/COMPONENT_DOCUMENTATION_CHECKLIST.md) for a comprehensive checklist
 
## Documentation Example
 
Here's a simplified example of how a secondary component should be documented:
 
```markdown
# ComponentName
 
Category: Service Category
 
Interface/Class/Enum: Type
 
Brief description of the component's purpose and functionality.
 
## Properties/Fields (if applicable)
 
| Property | Description |
| --- | --- |
| propertyName | Description of the property |
 
## Methods (if applicable)
 
| Method | Description |
| --- | --- |
| methodName(params) | Description of the method |
 
## Usage Examples
 
```csharp
// Example code showing how to use the component
var example = new ComponentName();
example.DoSomething();
```
 
## Best Practices
 
1. Practice 1
2. Practice 2
 
## Common Use Cases
 
1. Use case 1
2. Use case 2
```
 
## Documentation Status
 
### Primary Interfaces (Main Services)
- **Total Primary Interfaces**: 24
- **Completed**: 24 (100%)
- **Remaining**: 0 (0%)
 
### Secondary Components
- **Total Secondary Components**: 120
- **Completed**: 72 (60%)
- **Remaining**: 48 (40%)
 
### Overall Progress
- **Total Components**: 144
- **Overall Completion**: 66.7%
 
See [docs/manifest.json](docs/manifest.json) and its related split files for detailed tracking of components and [Component Checklist](#spatial-sdk-component-documentation-checklist) below for a comprehensive list of all components.
 
## Documentation Standards
Each component documentation must include:
- Component Overview
- Properties/Fields with descriptions (where applicable)
- Methods with parameters (where applicable)
- Events and callbacks (where applicable)
- Usage examples
- Best practices (where applicable)
- Common use cases (where applicable)
 
## Scraping Instructions
 
To get documentation for a component from the Spatial SDK website, use the Firecrawl MCP tool:
 
```javascript
// Use firecrawl_scrape to fetch documentation
firecrawl_scrape({
    url: "https://toolkit.spatial.io/reference/SpatialSys.UnitySDK.[ComponentName]",
    formats: ["markdown"],
    onlyMainContent: true
})
```
 
If the component is not available at the main URL, search for related components:
 
```javascript
// Search for component references
firecrawl_map({
    url: "https://toolkit.spatial.io/reference",
    search: "[ComponentName]"
})
```
 
## Session Continuation Instructions
 
When starting a new documentation session:
 
1. Check the "NEXT CATEGORY TO DOCUMENT" section in this README
2. Review [docs/manifest-incomplete.json](docs/manifest-incomplete.json) for priority and categorization
3. Fetch the component documentation using firecrawl
4. Create/update the appropriate markdown file
5. Present the completed documentation for user review
6. After user confirmation, upload the file to GitHub
7. Update tracking files after successful upload:
   - Create new session manifest file in docs/manifests/
   - Create session log file in docs/sessions/
   - Update SESSIONS_INDEX.md
   - Update BOTH README.md files (this one and docs/README.md)
 
## Important Process Notes
 
### Repository Structure
- This repository contains TWO README.md files:
  - This main README.md in the root directory
  - A secondary docs/README.md with project summary information
- BOTH files must be updated when a component is completed
 
### File Upload Process
1. Always upload files to GitHub after user confirmation
2. Upload files in this sequence:
   - Component documentation first
   - Session manifest file second
   - Session log file third
   - README updates (both files) fourth
   - SESSIONS_INDEX.md update last
3. Always verify uploads are successful
 
### Checkmark and Documentation Links
- Use proper markdown syntax: `- [x]` for checkmarks
- Add links to documentation files: `- [x] [ComponentName](./path/to/file.md)`
- Add completion date: `- [x] [ComponentName](./path/to/file.md) - COMPLETED! (MM/DD/YYYY)`
- Mark completed categories: `#### Category Name (COMPLETED ✅)`
- Always verify checkmarks appear green and links work correctly
 
## Access Information
 
- GitHub Repository: https://github.com/MCERQUA/SpatialDocs
- Reference Website: https://toolkit.spatial.io/reference
- Documentation Structure: Follow existing files in research/reference/SpatialSys/UnitySDK/
 
## Troubleshooting
 
- If you can't find the component directly, it may be documented as part of another component
- For event args classes, look for the corresponding event in the parent service
- If you need to see the implementation, check the EditorSimulation folder in the SDK source
- When creating examples, prefer complete, practical code over abstract snippets
 
## Remaining Work Estimation
 
- Time per component: ~30-60 minutes
- Average cost per component: $3-5
- Total remaining work: ~24-48 hours
- Estimated total cost: $144-240
 
## Spatial SDK Component Documentation Checklist
 
### Main Services (Primary Interfaces)
 
| Component | On toolkit.spatial.io | In GitHub Repo | Status |
|-----------|----------------------|----------------|--------| 
| [SpatialBridge](./research/reference/SpatialSys/UnitySDK/SpatialBridge.md) | ✅ | ✅ | Documented |
| [IActorService](./research/reference/SpatialSys/UnitySDK/IActorService.md) | ✅ | ✅ | Documented |
| [IActor](./research/reference/SpatialSys/UnitySDK/IActor.md) | ✅ | ✅ | Documented |
| [IAvatar](./research/reference/SpatialSys/UnitySDK/IAvatar.md) | ✅ | ✅ | Documented |
| [IAdService](./research/reference/SpatialSys/UnitySDK/IAdService.md) | ✅ | ✅ | Documented |
| [IAudioService](./research/reference/SpatialSys/UnitySDK/IAudioService.md) | ✅ | ✅ | Documented |
| [IBadgeService](./research/reference/SpatialSys/UnitySDK/IBadgeService.md) | ✅ | ✅ | Documented |
| [ICameraService](./research/reference/SpatialSys/UnitySDK/ICameraService.md) | ✅ | ✅ | Documented |
| [ICoreGUIService](./research/reference/SpatialSys/UnitySDK/ICoreGUIService.md) | ✅ | ✅ | Documented |
| [ICoreGUIShopService](./research/reference/SpatialSys/UnitySDK/ICoreGUIShopService.md) | ✅ | ✅ | Documented |
| [IEventService](./research/reference/SpatialSys/UnitySDK/IEventService.md) | ✅ | ✅ | Documented |
| [IGraphicsService](./research/reference/SpatialSys/UnitySDK/IGraphicsService.md) | ✅ | ✅ | Documented |
| [IInputService](./research/reference/SpatialSys/UnitySDK/IInputService.md) | ✅ | ✅ | Documented |
| [IInventoryService](./research/reference/SpatialSys/UnitySDK/IInventoryService.md) | ✅ | ✅ | Documented |
| [ILoggingService](./research/reference/SpatialSys/UnitySDK/ILoggingService.md) | ✅ | ✅ | Documented |
| [IMarketplaceService](./research/reference/SpatialSys/UnitySDK/IMarketplaceService.md) | ✅ | ✅ | Documented |
| [INetworkingService](./research/reference/SpatialSys/UnitySDK/INetworkingService.md) | ✅ | ✅ | Documented |
| [INetworkingRemoteEventsService](./research/reference/SpatialSys/UnitySDK/INetworkingRemoteEventsService.md) | ✅ | ✅ | Documented |
| [IQuestService](./research/reference/SpatialSys/UnitySDK/IQuestService.md) | ✅ | ✅ | Documented |
| [ISpaceContentService](./research/reference/SpatialSys/UnitySDK/ISpaceContentService.md) | ✅ | ✅ | Documented |
| [ISpaceObject](./research/reference/SpatialSys/UnitySDK/ISpaceObject.md) | ✅ | ✅ | Documented |
| [ISpaceService](./research/reference/SpatialSys/UnitySDK/ISpaceService.md) | ✅ | ✅ | Documented |
| [IUserWorldDataStoreService](./research/reference/SpatialSys/UnitySDK/IUserWorldDataStoreService.md) | ✅ | ✅ | Documented |
| [IVFXService](./research/reference/SpatialSys/UnitySDK/IVFXService.md) | ✅ | ✅ | Documented |
 
### Secondary Components (To Be Documented)
 
#### Actor Service Related (COMPLETED ✅)
- [x] [ActorCustomPropertiesChangedEventArgs](./research/reference/SpatialSys/UnitySDK/ActorCustomPropertiesChangedEventArgs.md) - COMPLETED! (3/9/2025)
- [x] [ActorJoinedEventArgs](./research/reference/SpatialSys/UnitySDK/ActorJoinedEventArgs.md) - COMPLETED! (3/9/2025)
- [x] [ActorLeftEventArgs](./research/reference/SpatialSys/UnitySDK/ActorLeftEventArgs.md) - COMPLETED! (3/9/2025)
- [x] [ActorProfilePictureRequest](./research/reference/SpatialSys/UnitySDK/ActorProfilePictureRequest.md) - COMPLETED! (3/9/2025)
- [x] [AvatarRespawnEventArgs](./research/reference/SpatialSys/UnitySDK/AvatarRespawnEventArgs.md) - COMPLETED! (3/9/2025)
- [x] [EquipAttachmentRequest](./research/reference/SpatialSys/UnitySDK/EquipAttachmentRequest.md) - COMPLETED! (3/9/2025)
- [x] [ILocalActor](./research/reference/SpatialSys/UnitySDK/ILocalActor.md) - COMPLETED! (3/9/2025)
- [x] [IReadOnlyAvatar](./research/reference/SpatialSys/UnitySDK/IReadOnlyAvatar.md) - COMPLETED! (3/9/2025)
 
#### Ad Service Related (COMPLETED ✅)
- [x] [AdRequest](./research/reference/SpatialSys/UnitySDK/AdRequest.md) - COMPLETED! (3/8/2025)
- [x] [SpatialAdType](./research/reference/SpatialSys/UnitySDK/SpatialAdType.md) - COMPLETED! (3/8/2025)
 
#### Camera Service Related (COMPLETED ✅)
- [x] [SpatialCameraMode](./research/reference/SpatialSys/UnitySDK/SpatialCameraMode.md) - COMPLETED! (3/8/2025)
- [x] [SpatialCameraRotationMode](./research/reference/SpatialSys/UnitySDK/SpatialCameraRotationMode.md) - COMPLETED! (3/8/2025)
- [x] [XRCameraMode](./research/reference/SpatialSys/UnitySDK/XRCameraMode.md) - COMPLETED! (3/8/2025)
 
#### Core GUI Service Related (COMPLETED ✅)
- [x] [CoreGUIUtility](./research/reference/SpatialSys/UnitySDK/CoreGUIUtility.md) - COMPLETED! (3/9/2025)
- [x] [SpatialCoreGUIState](./research/reference/SpatialSys/UnitySDK/SpatialCoreGUIState.md) - COMPLETED! (3/9/2025)
- [x] [SpatialCoreGUIType](./research/reference/SpatialSys/UnitySDK/SpatialCoreGUIType.md) - COMPLETED! (3/9/2025)
- [x] [SpatialCoreGUITypeFlags](./research/reference/SpatialSys/UnitySDK/SpatialCoreGUITypeFlags.md) - COMPLETED! (3/9/2025)
- [x] [SpatialMobileControlsGUITypeFlags](./research/reference/SpatialSys/UnitySDK/SpatialMobileControlsGUITypeFlags.md) - COMPLETED! (3/9/2025)
- [x] [SpatialSystemGUIType](./research/reference/SpatialSys/UnitySDK/SpatialSystemGUIType.md) - COMPLETED! (3/9/2025)
 
#### Input Service Related (COMPLETED ✅)
- [x] [IAvatarInputActionsListener](./research/reference/SpatialSys/UnitySDK/IAvatarInputActionsListener.md) - COMPLETED! (3/9/2025)
- [x] [IInputActionsListener](./research/reference/SpatialSys/UnitySDK/IInputActionsListener.md) - COMPLETED! (3/9/2025)
- [x] [InputCaptureType](./research/reference/SpatialSys/UnitySDK/InputCaptureType.md) - COMPLETED! (3/9/2025)
- [x] [InputPhase](./research/reference/SpatialSys/UnitySDK/InputPhase.md) - COMPLETED! (3/9/2025)
- [x] [IVehicleInputActionsListener](./research/reference/SpatialSys/UnitySDK/IVehicleInputActionsListener.md) - COMPLETED! (3/9/2025)
- [x] [VehicleInputFlags](./research/reference/SpatialSys/UnitySDK/VehicleInputFlags.md) - COMPLETED! (3/9/2025)
 
#### Inventory Service Related (COMPLETED ✅)
- [x] [AddInventoryItemRequest](./research/reference/SpatialSys/UnitySDK/AddInventoryItemRequest.md) - COMPLETED! (3/9/2025)
- [x] [AwardWorldCurrencyRequest](./research/reference/SpatialSys/UnitySDK/AwardWorldCurrencyRequest.md) - COMPLETED! (3/9/2025)
- [x] [DeleteInventoryItemRequest](./research/reference/SpatialSys/UnitySDK/DeleteInventoryItemRequest.md) - COMPLETED! (3/9/2025)
- [x] [GetInventoryItemRequest](./research/reference/SpatialSys/UnitySDK/GetInventoryItemRequest.md) - COMPLETED! (3/9/2025)
- [x] [IInventoryItem](./research/reference/SpatialSys/UnitySDK/IInventoryItem.md) - COMPLETED! (3/9/2025)
- [x] [ItemType](./research/reference/SpatialSys/UnitySDK/ItemType.md) - COMPLETED! (3/9/2025)
- [x] [UseInventoryItemRequest](./research/reference/SpatialSys/UnitySDK/UseInventoryItemRequest.md) - COMPLETED! (3/9/2025)
 
#### Quest Service Related (COMPLETED ✅)
- [x] [IQuest](./research/reference/SpatialSys/UnitySDK/IQuest.md) - COMPLETED! (3/9/2025)
- [x] [IQuestTask](./research/reference/SpatialSys/UnitySDK/IQuestTask.md) - COMPLETED! (3/9/2025)
- [x] [IReward](./research/reference/SpatialSys/UnitySDK/IReward.md) - COMPLETED! (3/9/2025)
- [x] [QuestStatus](./research/reference/SpatialSys/UnitySDK/QuestStatus.md) - COMPLETED! (3/9/2025)
- [x] [QuestTaskType](./research/reference/SpatialSys/UnitySDK/QuestTaskType.md) - COMPLETED! (3/9/2025)
- [x] [RewardType](./research/reference/SpatialSys/UnitySDK/RewardType.md) - COMPLETED! (3/9/2025)
 
#### Space Content Service Related (COMPLETED ✅)
- [x] [DestroySpaceObjectRequest](./research/reference/SpatialSys/UnitySDK/DestroySpaceObjectRequest.md) - COMPLETED! (3/9/2025)
- [x] [SpaceObjectOwnerChangedEventArgs](./research/reference/SpatialSys/UnitySDK/SpaceObjectOwnerChangedEventArgs.md) - COMPLETED! (3/9/2025)
- [x] [SpaceObjectType](./research/reference/SpatialSys/UnitySDK/SpaceObjectType.md) - COMPLETED! (3/9/2025)
- [x] [SpaceObjectVariablesChangedEventArgs](./research/reference/SpatialSys/UnitySDK/SpaceObjectVariablesChangedEventArgs.md) - COMPLETED! (3/9/2025)
- [x] [IPrefabObject](./research/reference/SpatialSys/UnitySDK/IPrefabObject.md) - COMPLETED! (3/9/2025)
- [x] [IReadOnlyPrefabObject](./research/reference/SpatialSys/UnitySDK/IReadOnlyPrefabObject.md) - COMPLETED! (3/9/2025)
- [x] [IReadOnlySpaceObject](./research/reference/SpatialSys/UnitySDK/IReadOnlySpaceObject.md) - COMPLETED! (3/9/2025)
- [x] [IReadOnlySpaceObjectComponent](./research/reference/SpatialSys/UnitySDK/IReadOnlySpaceObjectComponent.md) - COMPLETED! (3/9/2025)
- [x] [ISpaceObjectComponent](./research/reference/SpatialSys/UnitySDK/ISpaceObjectComponent.md) - COMPLETED! (3/9/2025)
- [x] [SpaceObjectOwnershipTransferRequest](./research/reference/SpatialSys/UnitySDK/SpaceObjectOwnershipTransferRequest.md) - COMPLETED! (3/9/2025)
- [x] [SpawnAvatarRequest](./research/reference/SpatialSys/UnitySDK/SpawnAvatarRequest.md) - COMPLETED! (3/9/2025)
- [x] [SpawnNetworkObjectRequest](./research/reference/SpatialSys/UnitySDK/SpawnNetworkObjectRequest.md) - COMPLETED! (3/9/2025)
- [x] [SpawnPrefabObjectRequest](./research/reference/SpatialSys/UnitySDK/SpawnPrefabObjectRequest.md) - COMPLETED! (3/9/2025)
- [x] [SpawnSpaceObjectRequest](./research/reference/SpatialSys/UnitySDK/SpawnSpaceObjectRequest.md) - COMPLETED! (3/9/2025)
 
#### Marketplace Service Related (COMPLETED ✅)
- [x] [ItemPurchasedEventArgs](./research/reference/SpatialSys/UnitySDK/ItemPurchasedEventArgs.md) - COMPLETED! (3/9/2025)
- [x] [PurchaseItemRequest](./research/reference/SpatialSys/UnitySDK/PurchaseItemRequest.md) - COMPLETED! (3/9/2025)
 
#### Networking Service Related (COMPLETED ✅)
- [x] [NetworkingRemoteEventArgs](./research/reference/SpatialSys/UnitySDK/NetworkingRemoteEventArgs.md) - COMPLETED! (3/9/2025)
- [x] [ServerConnectionStatus](./research/reference/SpatialSys/UnitySDK/ServerConnectionStatus.md) - COMPLETED! (3/9/2025)
 
#### User World Data Store Service Related (COMPLETED ✅)
- [x] [DataStoreDumpVariablesRequest](./research/reference/SpatialSys/UnitySDK/DataStoreDumpVariablesRequest.md) - COMPLETED! (3/9/2025)
- [x] [DataStoreGetVariableRequest](./research/reference/SpatialSys/UnitySDK/DataStoreGetVariableRequest.md) - COMPLETED! (3/9/2025)
- [x] [DataStoreHasAnyVariableRequest](./research/reference/SpatialSys/UnitySDK/DataStoreHasAnyVariableRequest.md) - COMPLETED! (3/9/2025)
- [x] [DataStoreHasVariableRequest](./research/reference/SpatialSys/UnitySDK/DataStoreHasVariableRequest.md) - COMPLETED! (3/9/2025)
- [x] [DataStoreOperationRequest](./research/reference/SpatialSys/UnitySDK/DataStoreOperationRequest.md) - COMPLETED! (3/9/2025)
- [x] [DataStoreResponseCode](./research/reference/SpatialSys/UnitySDK/DataStoreResponseCode.md) - COMPLETED! (3/9/2025)
 
#### VFX Service Related (COMPLETED ✅)
- [x] [FloatingTextAnimStyle](./research/reference/SpatialSys/UnitySDK/FloatingTextAnimStyle.md) - COMPLETED! (3/9/2025)
 
#### Interfaces (COMPLETED ✅)
- [x] [IOwnershipChanged](./research/reference/SpatialSys/UnitySDK/IOwnershipChanged.md) - COMPLETED! (3/9/2025)
- [x] [IVariablesChanged](./research/reference/SpatialSys/UnitySDK/IVariablesChanged.md) - COMPLETED! (3/9/2025)
 
### Core Components (COMPLETED ✅)
- [x] [AvatarAnimationClipType](./research/reference/SpatialSys/UnitySDK/AvatarAnimationClipType.md) - COMPLETED! (3/9/2025)
- [x] [SpatialAttachmentAvatarAnimSettings](./research/reference/SpatialSys/UnitySDK/SpatialAttachmentAvatarAnimSettings.md) - COMPLETED! (3/9/2025)
- [x] [SpatialAvatar](./research/reference/SpatialSys/UnitySDK/SpatialAvatar.md) - COMPLETED! (3/9/2025)
- [x] [SpatialAvatarAnimation](./research/reference/SpatialSys/UnitySDK/SpatialAvatarAnimation.md) - COMPLETED! (3/9/2025)
- [x] [SpatialAvatarAnimOverrides](./research/reference/SpatialSys/UnitySDK/SpatialAvatarAnimOverrides.md) - COMPLETED! (3/9/2025)
- [x] [SpatialAvatarAttachment](./research/reference/SpatialSys/UnitySDK/SpatialAvatarAttachment.md) - COMPLETED! (3/9/2025)
- [x] [SpatialAvatarAttachment.AttachmentAnimatorType](./research/reference/SpatialSys/UnitySDK/SpatialAvatarAttachment.AttachmentAnimatorType.md) - COMPLETED! (3/9/2025)
- [x] [SpatialAvatarAttachment.Category](./research/reference/SpatialSys/UnitySDK/SpatialAvatarAttachment.Category.md) - COMPLETED! (3/9/2025)
- [x] [SpatialAvatarAttachment.Slot](./research/reference/SpatialSys/UnitySDK/SpatialAvatarAttachment.Slot.md) - COMPLETED! (3/9/2025)
- [x] [SpatialAvatarAttachment.SlotMask](./research/reference/SpatialSys/UnitySDK/SpatialAvatarAttachment.SlotMask.md) - COMPLETED! (3/9/2025)
- [x] [SpatialAvatarAttachmentSlotMaskExtensions](./research/reference/SpatialSys/UnitySDK/SpatialAvatarAttachmentSlotMaskExtensions.md) - COMPLETED! (3/9/2025)
- [x] [SpatialAvatarDefaultAnimSetType](./research/reference/SpatialSys/UnitySDK/SpatialAvatarDefaultAnimSetType.md) - COMPLETED! (3/9/2025)
- [x] [SpatialAvatarTeleporter](./research/reference/SpatialSys/UnitySDK/SpatialAvatarTeleporter.md) - COMPLETED! (3/9/2025)
- [x] [SpatialCameraPassthrough](./research/reference/SpatialSys/UnitySDK/SpatialCameraPassthrough.md) - COMPLETED! (3/9/2025)
- [x] [SpatialClimbable](./research/reference/SpatialSys/UnitySDK/SpatialClimbable.md) - COMPLETED! (3/9/2025)
- [x] [SpatialEmptyFrame](./research/reference/SpatialSys/UnitySDK/SpatialEmptyFrame.md) - COMPLETED! (3/9/2025)
- [x] [SpatialEntrancePoint](./research/reference/SpatialSys/UnitySDK/SpatialEntrancePoint.md) - COMPLETED! (3/9/2025)
- [x] [SpatialEnvironmentSettingsOverrides](./research/reference/SpatialSys/UnitySDK/SpatialEnvironmentSettingsOverrides.md) - COMPLETED! (3/9/2025)
- [x] [SpatialInteractable](./research/reference/SpatialSys/UnitySDK/SpatialInteractable.md) - COMPLETED! (3/9/2025)
- [x] [SpatialMovementMaterialSurface](./research/reference/SpatialSys/UnitySDK/SpatialMovementMaterialSurface.md) - COMPLETED! (3/10/2025)
- [x] [SpatialNetworkObject](./research/reference/SpatialSys/UnitySDK/SpatialNetworkObject.md) - COMPLETED! (3/10/2025)
- [x] [SpatialNetworkVariables](./research/reference/SpatialSys/UnitySDK/SpatialNetworkVariables.md) - COMPLETED! (3/10/2025)
- [x] [SpatialNetworkVariables.Data](./research/reference/SpatialSys/UnitySDK/SpatialNetworkVariables.Data.md) - COMPLETED! (3/10/2025)
- [x] [SpatialPointOfInterest](./research/reference/SpatialSys/UnitySDK/SpatialPointOfInterest.md) - COMPLETED! (3/10/2025)
- [x] [SpatialPrefabObject](./research/reference/SpatialSys/UnitySDK/SpatialPrefabObject.md) - COMPLETED! (3/10/2025)
- [x] [SpatialProjectorSurface](./research/reference/SpatialSys/UnitySDK/SpatialProjectorSurface.md) - COMPLETED! (3/10/2025)
- [x] [SpatialQuest](./research/reference/SpatialSys/UnitySDK/SpatialQuest.md) - COMPLETED! (3/10/2025)
- [x] [SpatialQuest.Reward](./research/reference/SpatialSys/UnitySDK/SpatialQuest.Reward.md) - COMPLETED! (3/10/2025)
- [x] [SpatialQuest.Task](./research/reference/SpatialSys/UnitySDK/SpatialQuest.Task.md) - COMPLETED! (3/10/2025)
- [x] [SpatialRenderPipelineSettingsOverrides](./research/reference/SpatialSys/UnitySDK/SpatialRenderPipelineSettingsOverrides.md) - COMPLETED! (3/10/2025)
- [x] [SpatialSeatHotspot](./research/reference/SpatialSys/UnitySDK/SpatialSeatHotspot.md) - COMPLETED! (3/10/2025)
- [x] [SpatialSyncedAnimator](./research/reference/SpatialSys/UnitySDK/SpatialSyncedAnimator.md) - COMPLETED! (3/10/2025)
- [x] [SpatialSyncedObject](./research/reference/SpatialSys/UnitySDK/SpatialSyncedObject.md) - COMPLETED! (3/10/2025)
- [x] [SpatialThumbnailCamera](./research/reference/SpatialSys/UnitySDK/SpatialThumbnailCamera.md) - COMPLETED! (3/10/2025)
- [x] [SpatialTriggerEvent](./research/reference/SpatialSys/UnitySDK/SpatialTriggerEvent.md) - COMPLETED! (3/10/2025)
- [x] [SpatialVirtualCamera](./research/reference/SpatialSys/UnitySDK/SpatialVirtualCamera.md) - COMPLETED! (3/10/2025)

### Scriptable Objects (COMPLETED ✅)
- [x] [SpatialMovementMaterial](./research/reference/SpatialSys/UnitySDK/SpatialMovementMaterial.md) - COMPLETED! (3/10/2025)
- [x] [SpatialScriptableObjectBase](./research/reference/SpatialSys/UnitySDK/SpatialScriptableObjectBase.md) - COMPLETED! (3/10/2025)
- [x] [SpatialSFX](./research/reference/SpatialSys/UnitySDK/SpatialSFX.md) - COMPLETED! (3/10/2025)

### Other Classes (COMPLETED ✅)
- [x] [APINotAuthorizedException](./research/reference/SpatialSys/UnitySDK/APINotAuthorizedException.md) - COMPLETED! (3/10/2025)
- [x] [AttachmentAnimationClips](./research/reference/SpatialSys/UnitySDK/AttachmentAnimationClips.md) - COMPLETED! (3/10/2025)
- [x] [AttachmentAvatarAnimConfig](./research/reference/SpatialSys/UnitySDK/AttachmentAvatarAnimConfig.md) - COMPLETED! (3/10/2025)
- [x] [ClampAttribute](./research/reference/SpatialSys/UnitySDK/ClampAttribute.md) - COMPLETED! (3/10/2025)
- [x] [DocumentationCategoryAttribute](./research/reference/SpatialSys/UnitySDK/DocumentationCategoryAttribute.md) - COMPLETED! (3/10/2025)
- [x] [MinMaxAttribute](./research/reference/SpatialSys/UnitySDK/MinMaxAttribute.md) - COMPLETED! (3/10/2025)
- [x] [NetworkVariable<T>](./research/reference/SpatialSys/UnitySDK/NetworkVariable.md) - COMPLETED! (3/10/2025)
- [x] [ReadOnlyAttribute](./research/reference/SpatialSys/UnitySDK/ReadOnlyAttribute.md) - COMPLETED! (3/10/2025)
- [x] [SpatialAsyncOperation](./research/reference/SpatialSys/UnitySDK/SpatialAsyncOperation.md) - COMPLETED! (3/10/2025)
- [x] [SpatialAsyncOperationExtensions](./research/reference/SpatialSys/UnitySDK/SpatialAsyncOperationExtensions.md) - COMPLETED! (3/10/2025)
- [x] [SpatialNetworkBehaviour](./research/reference/SpatialSys/UnitySDK/SpatialNetworkBehaviour.md) - COMPLETED! (3/10/2025)
- [x] [ToggleWithEnum](./research/reference/SpatialSys/UnitySDK/ToggleWithEnum.md) - COMPLETED! (3/10/2025)
- [x] [VisualScriptingUtility](./research/reference/SpatialSys/UnitySDK/VisualScriptingUtility.md) - COMPLETED! (3/10/2025)

### Enums (COMPLETED ✅)
- [x] [AssetType](./research/reference/SpatialSys/UnitySDK/AssetType.md) - COMPLETED! (3/10/2025)
- [x] [PackageType](./research/reference/SpatialSys/UnitySDK/PackageType.md) - COMPLETED! (3/10/2025)
- [x] [SpaceObjectFlags](./research/reference/SpatialSys/UnitySDK/SpaceObjectFlags.md) - COMPLETED! (3/10/2025)
- [x] [SpatialInteractable.IconType](./research/reference/SpatialSys/UnitySDK/SpatialInteractable.IconType.md) - COMPLETED! (3/10/2025)
- [x] [SpatialPlatform](./research/reference/SpatialSys/UnitySDK/SpatialPlatform.md) - COMPLETED! (3/10/2025)
- [x] [SpatialTriggerEvent.ListenFor](./research/reference/SpatialSys/UnitySDK/SpatialTriggerEvent.ListenFor.md) - COMPLETED! (3/10/2025)

### Structs (COMPLETED ✅)
- [x] [NetworkObjectOwnershipChangedEventArgs](./research/reference/SpatialSys/UnitySDK/NetworkObjectOwnershipChangedEventArgs.md) - COMPLETED! (3/10/2025)
- [x] [NetworkObjectVariablesChangedEventArgs](./research/reference/SpatialSys/UnitySDK/NetworkObjectVariablesChangedEventArgs.md) - COMPLETED! (3/10/2025)
