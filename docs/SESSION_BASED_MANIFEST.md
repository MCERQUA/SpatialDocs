# Session-Based Manifest System

## Overview
To improve performance and manageability, the Spatial SDK Documentation project now uses a session-based manifest system. Instead of maintaining a single large manifest file or even split manifests that grow in size over time, we now create a new manifest file for each documentation session.

## New Process

### 1. Starting a New Documentation Session

When starting a new documentation session:

1. First, identify which component(s) to document based on the README.md file
2. Determine the next sequential session number by checking the latest in the `docs/manifests/` directory
3. Create a new session manifest file with the naming pattern: `manifest-session-XXX.json` (where XXX is the session number)

### 2. Creating the Session Manifest

For each new session, create a manifest with this structure:

```json
{
  "session": 25,
  "date": "2025-03-10",
  "components": [
    {
      "name": "ComponentName",
      "category": "Category Name",
      "completionDate": "2025-03-10",
      "hasAllSections": true,
      "sections": {
        "overview": true,
        "properties": true,
        "methods": true,
        "events": true,
        "examples": true,
        "bestPractices": true,
        "useCases": true
      }
    }
  ],
  "summary": {
    "componentsCompleted": 1,
    "categoriesWorkedOn": ["Category Name"]
  }
}
```

### 3. Updating Project Files

After documenting components and creating the session manifest, update these files:

1. **README.md** (root directory):
   - Update the list of completed components with checkmarks
   - Update the current status (completion percentages)
   - Update the next component to document

2. **docs/README.md**:
   - Update the status summary
   - Update completed categories list
   - Update the next documentation target

3. **docs/SESSIONS_INDEX.md**:
   - Add a new entry for the current session

### 4. Creating Session Log

Create a session log file in the `docs/sessions/` directory:
- Filename pattern: `YYYY-MM-DD-Category-ComponentName.md`
- Include a summary of the work done, any challenges, and decisions made

## Example Workflow

1. Identify next component: "SpatialMovementMaterialSurface"
2. Check that the latest session is #24, so this will be session #25
3. Create component documentation in `research/reference/SpatialSys/UnitySDK/SpatialMovementMaterialSurface.md`
4. Create `docs/manifests/manifest-session-025.json` with details of this session
5. Update README.md to mark SpatialMovementMaterialSurface as completed
6. Update docs/README.md with new statistics
7. Add session #25 to docs/SESSIONS_INDEX.md
8. Create `docs/sessions/2025-03-10-CoreComponents-SpatialMovementMaterialSurface.md`

## Benefits

This session-based approach:
- Prevents manifest files from becoming too large
- Eliminates the need to update previous files
- Creates a clear historical record of documentation progress
- Makes it easier to track work over time
- Improves system performance by working with smaller files

## Implementation

Starting with Session #25 (March 10, 2025), all documentation sessions will follow this new manifest approach. Previous components will remain tracked in the original manifest files, but no new components will be added to them.
