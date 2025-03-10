# Documentation Session Template

## Session Information
- **Component**: [Component Name]
- **Category**: [Component Category]
- **Session Date**: [Date]
- **Session Number**: [#]
- **Estimated Cost**: $[Amount]

## Pre-Session Checklist
- [ ] Check the latest session number in docs/manifests/ directory
- [ ] Confirm component priority from README.md
- [ ] Check for any existing partial documentation
- [ ] Prepare firecrawl scraping URL

## Session Steps

### 1. Content Gathering
- [ ] Scrape reference documentation
- [ ] Review existing implementation
- [ ] Note key components:
  - [ ] Properties
  - [ ] Methods
  - [ ] Events
  - [ ] Dependencies

### 2. Documentation Structure
- [ ] Overview section
  - [ ] Purpose
  - [ ] Key concepts
  - [ ] Common scenarios
- [ ] Properties section
  - [ ] Group by category
  - [ ] Add descriptions
  - [ ] Note default values
- [ ] Methods section
  - [ ] Parameters
  - [ ] Return values
  - [ ] Usage notes
- [ ] Events section
  - [ ] Event args
  - [ ] Timing details
  - [ ] Common patterns

### 3. Code Examples
- [ ] Basic usage example
- [ ] Common scenarios
- [ ] Best practices implementation
- [ ] Error handling patterns

### 4. Quality Review
- [ ] Check all sections present
- [ ] Verify code examples
- [ ] Confirm formatting
- [ ] Spell check
- [ ] Technical accuracy

### 5. GitHub File Management
- [ ] Present completed documentation to user for review
- [ ] After user confirmation, upload documentation file to GitHub:
  ```
  create_or_update_file(
      path="research/reference/SpatialSys/UnitySDK/[ComponentName].md",
      repo="SpatialDocs",
      owner="MCERQUA",
      branch="main",
      content="[Documentation content]",
      message="Add documentation for [ComponentName]"
  )
  ```
- [ ] Verify file was successfully uploaded to GitHub

### 6. Session-Based Tracking Updates
- [ ] Create a new session manifest file:
  ```
  create_or_update_file(
      path="docs/manifests/manifest-session-XXX.json",
      repo="SpatialDocs",
      owner="MCERQUA",
      branch="main",
      content="{
        \"session\": XXX,
        \"date\": \"YYYY-MM-DD\",
        \"components\": [
          {
            \"name\": \"ComponentName\",
            \"category\": \"Category Name\",
            \"completionDate\": \"YYYY-MM-DD\",
            \"hasAllSections\": true,
            \"sections\": {
              \"overview\": true,
              \"properties\": true,
              \"methods\": true,
              \"events\": true,
              \"examples\": true,
              \"bestPractices\": true,
              \"useCases\": true
            }
          }
        ],
        \"summary\": {
          \"componentsCompleted\": 1,
          \"categoriesWorkedOn\": [\"Category Name\"]
        }
      }",
      message="Create session manifest for [ComponentName]"
  )
  ```

- [ ] Create a session log file:
  ```
  create_or_update_file(
      path="docs/sessions/YYYY-MM-DD-Category-ComponentName.md",
      repo="SpatialDocs",
      owner="MCERQUA",
      branch="main",
      content="# Documentation Session: [ComponentName]\n\n...",
      message="Add session log for [ComponentName] documentation"
  )
  ```

- [ ] Update the session index file:
  ```
  create_or_update_file(
      path="docs/SESSIONS_INDEX.md",
      repo="SpatialDocs",
      owner="MCERQUA",
      branch="main",
      content="...",
      message="Update session index with [ComponentName] documentation session"
  )
  ```

## Post-Session Notes
- Challenges encountered:
- Solutions implemented:
- Areas for improvement:
- Dependencies identified:

## Cost Tracking
- Tokens used:
- Time spent:
- Cost incurred:

## Next Steps
- [ ] Note next component to document
- [ ] Note any follow-up tasks
- [ ] Update main documentation plan

## Important Process Notes

### IMPORTANT: README.md Updates
- Do NOT update README.md files - these will be updated manually by the repository owner
- Provide a report to the user about what should be updated in the README.md files
- For details on this process change, see docs/DOCUMENTATION_PROCESS.md

### New Session-Based Manifest System
- Each documentation session now creates a **new** manifest file in docs/manifests/ directory
- Do NOT update the legacy manifest files (manifest.json, manifest-completed.json, etc.)
- For each new session:
  1. Create a new numbered session manifest file (manifest-session-XXX.json)
  2. Create a session log in docs/sessions/
  3. Update the sessions index in docs/SESSIONS_INDEX.md

### File Upload Process
- Only upload documentation files to GitHub AFTER user confirmation
- Always verify GitHub uploads were successful
- Update files in this sequence:
  1. Component documentation file first
  2. Session manifest file second
  3. Session log file third
  4. SESSIONS_INDEX.md update last

### Documentation Validation
- After uploading, verify your changes are reflected in the GitHub repository
- Test all links between documentation files
- Ensure formatting appears correctly in GitHub's markdown renderer

### Troubleshooting
- If a component is not found in the toolkit.spatial.io reference, search for related components
- If GitHub uploads fail, check file paths and repository permissions
