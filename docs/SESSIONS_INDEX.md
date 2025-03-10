# Documentation Sessions Index

This file provides an index of all documentation sessions for the Spatial SDK Documentation project.

## New Session-Based Approach

Starting with Session #25 (March 10, 2025), we have switched to a session-based manifest system. Each session has:
- A corresponding manifest file in `docs/manifests/manifest-session-XXX.json`
- A session log file in `docs/sessions/`
- Components documented during that session

## Sessions

### Session #27 (2025-03-10)
- **Manifest**: [manifest-session-027.json](./manifests/manifest-session-027.json)
- **Session Log**: [2025-03-10-Core-Components-Documentation.md](./sessions/2025-03-10-Core-Components-Documentation.md)
- **Components**:
  - SpatialAsyncOperation (Core Components)
  - SpatialAsyncOperationExtensions (Core Components)
  - ReadOnlyAttribute (Attributes)
  - NetworkVariable (Networking Components)
- **Notes**: Documented four key components including networking and asynchronous operation support

### Session #26 (2025-03-10)
- **Manifest**: [manifest-session-026.json](./manifests/manifest-session-026.json)
- **Session Log**: [2025-03-10-ScriptableObjects-Documentation.md](./sessions/2025-03-10-ScriptableObjects-Documentation.md)
- **Components**:
  - SpatialScriptableObjectBase (Scriptable Objects)
  - SpatialMovementMaterial (Scriptable Objects)
  - SpatialSFX (Scriptable Objects)
- **Notes**: Completed the entire Scriptable Objects category

### Session #25 (2025-03-10)
- **Manifest**: [manifest-session-025.json](./manifests/manifest-session-025.json)
- **Session Log**: [2025-03-10-CoreComponents-SpatialMovementMaterialSurface.md](./sessions/2025-03-10-CoreComponents-SpatialMovementMaterialSurface.md)
- **Components**:
  - SpatialMovementMaterialSurface (Core Components)
- **Notes**: Implemented new session-based manifest system

### Previous Sessions (Legacy Format)

Sessions 1-24 used the previous manifest system with the following files:
- `docs/manifest.json` - Main tracker
- `docs/manifest-primary.json` - Primary interfaces
- `docs/manifest-completed.json` - Completed secondary components
- `docs/manifest-incomplete.json` - Incomplete components

These files are now preserved for historical reference but are no longer being updated.