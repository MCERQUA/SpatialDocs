# Updated Documentation Process

## IMPORTANT: README.md Update Process Change

As of March 2025, the README.md file has grown too large to be automatically updated by the documentation process. To address this issue, the documentation process has been modified as follows:

1. **Manual README Updates**: README.md files will now be updated manually by the repository owner
2. **Do NOT automatically update README.md files**
3. **Focus on component documentation only**

## Revised Documentation Workflow

### 1. Content Gathering and Documentation
- Use firecrawl_scrape to fetch documentation from toolkit.spatial.io
- Create comprehensive documentation following the established template
- Ensure all sections are complete (Overview, Properties, Methods, Events, Usage Examples, Best Practices, Common Use Cases)

### 2. GitHub File Management
- Present completed documentation to user for review
- After user confirmation, upload ONLY the documentation file to GitHub:
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
- Verify file was successfully uploaded to GitHub

### 3. Session-Based Tracking Updates
- Create a new session manifest file:
  ```
  create_or_update_file(
      path="docs/manifests/manifest-session-XXX.json",
      repo="SpatialDocs",
      owner="MCERQUA",
      branch="main",
      content="{\n\"session\": XXX,\n\"date\": \"YYYY-MM-DD\",\n...}",
      message="Create session manifest for [ComponentName]"
  )
  ```

- Create a session log file:
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

- Update ONLY docs/SESSIONS_INDEX.md with the new session:
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

## What NOT to Update

Do NOT update the following files automatically:
1. `/README.md` (root) - This file will be updated manually by repository owner
2. `/docs/README.md` - This file will be updated manually by repository owner

## File Upload Sequence

Update files in this sequence:
1. Component documentation file first
2. Session manifest file second
3. Session log file third
4. SESSIONS_INDEX.md update last

Notify the user that the documentation has been completed and the README.md files need to be manually updated by the repository owner.

## Validation and Quality Assurance

After uploading, verify your changes are reflected in the GitHub repository:
- Confirm documentation file is properly formatted and accessible
- Ensure session manifest is correctly structured
- Verify session log contains comprehensive information
- Confirm session index has been updated correctly

## Component Status Reporting

Instead of updating README.md files, provide a clear report to the user including:
1. Name of the completed component(s)
2. Category they belong to
3. Component count statistics (completed vs. remaining)
4. Suggestion for what README updates the user should make manually
