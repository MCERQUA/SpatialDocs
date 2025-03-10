# Session Preparation Guide

This guide provides step-by-step instructions for preparing and conducting a documentation session for Spatial SDK secondary components.

## Session Preparation Steps

### 1. Identify Component to Document

Check the root README.md for the next component to document:
- Look under "NEXT COMPONENT TO DOCUMENT" section
- Note the component name and category
- Note any special considerations or dependencies

### 2. Gather Reference Information

Use firecrawl to scrape documentation from the Spatial SDK website:

```
firecrawl_scrape(
    url="https://toolkit.spatial.io/reference/SpatialSys.UnitySDK.[ComponentName]",
    formats=["markdown"],
    onlyMainContent=true
)
```

If the component isn't documented directly, try searching for it:

```
firecrawl_map(
    url="https://toolkit.spatial.io/reference",
    search="[ComponentName]"
)
```

Look for additional information about the component, such as:
- Parent interfaces or services
- Related components
- Documentation in the source code

### 3. Check for Source Code

If available, inspect the source code implementation:

```
search_code(
    q="[ComponentName] extension:cs SpatialSys UnitySDK",
    repo="spatial-unity-sdk",
    owner="spatialsys"
)
```

If the source code is found, examine it:

```
get_file_contents(
    path="[FilePath]",
    repo="spatial-unity-sdk",
    owner="spatialsys"
)
```

### 4. Check for Example Usage

Look for examples of how the component is used:

```
search_code(
    q="[ComponentName] language:csharp",
    repo="spatial-unity-sdk",
    owner="spatialsys"
)
```

## Documentation Creation

### 1. Start with the Template

Use the secondary component template as a starting point:

```
get_file_contents(
    path="docs/SECONDARY_COMPONENT_TEMPLATE.md",
    repo="SpatialDocs",
    owner="MCERQUA"
)
```

### 2. Create Documentation File

Create a new Markdown file for the component:

```
create_or_update_file(
    path="research/reference/SpatialSys/UnitySDK/[ComponentName].md",
    repo="SpatialDocs",
    owner="MCERQUA",
    branch="main",
    content="[Template with filled information]",
    message="Add documentation for [ComponentName]"
)
```

### 3. Document Structure

Ensure your documentation includes:
- Component name and category
- Overview of purpose and functionality
- Properties/methods/events/constants as applicable
- Usage examples with code
- Best practices
- Common use cases
- Related components
- Any notes or special considerations

## Post-Documentation Steps

### 1. Present for Review and Upload Documentation

Present the completed documentation to the user for review before uploading:

```
// After user confirmation, create or update the documentation file
create_or_update_file(
    path="research/reference/SpatialSys/UnitySDK/[ComponentName].md",
    repo="SpatialDocs",
    owner="MCERQUA",
    branch="main",
    content="[Documentation content]",
    message="Add documentation for [ComponentName]"
)
```

Always verify the file was successfully uploaded to GitHub before proceeding.

### 2. Update Manifest

Update the manifest.json file to mark the component as documented:

```
// Get current manifest content
get_file_contents(
    path="docs/manifest.json",
    repo="SpatialDocs",
    owner="MCERQUA"
)

// Modify the manifest to mark component as completed

create_or_update_file(
    path="docs/manifest.json",
    repo="SpatialDocs",
    owner="MCERQUA",
    branch="main",
    content="[Updated manifest content]",
    message="Update manifest.json to mark [ComponentName] as completed"
)
```

### 3. Update README Files (Both of Them)

Update BOTH README.md files:

```
// Update root README.md
get_file_contents(
    path="README.md",
    repo="SpatialDocs",
    owner="MCERQUA"
)

// Modify the README to mark component as completed with a linked checkmark
// IMPORTANT: Make the checkmark visible and link to the documentation file:
// 1. Use - [x] syntax for each completed component
// 2. Add link to the documentation: - [x] [ComponentName](./research/reference/SpatialSys/UnitySDK/ComponentName.md)
// 3. Add emphasis text: - [x] [ComponentName](./path/to/file.md) - COMPLETED! (date)
// 4. Add category completion indicator: #### Category Name (COMPLETED ✅)

create_or_update_file(
    path="README.md",
    repo="SpatialDocs",
    owner="MCERQUA",
    branch="main",
    content="[Updated README content]",
    message="Update README.md to mark [ComponentName] as completed with documentation link"
)

// Update docs/README.md
get_file_contents(
    path="docs/README.md",
    repo="SpatialDocs",
    owner="MCERQUA"
)

// Modify the docs README to update progress metrics

create_or_update_file(
    path="docs/README.md",
    repo="SpatialDocs",
    owner="MCERQUA",
    branch="main",
    content="[Updated docs README content]",
    message="Update docs/README.md with progress for [ComponentName]"
)
```

### 4. Log Session

Update SESSIONS.md to add the new session:

```
get_file_contents(
    path="docs/SESSIONS.md",
    repo="SpatialDocs",
    owner="MCERQUA"
)

// Add a new session entry

create_or_update_file(
    path="docs/SESSIONS.md",
    repo="SpatialDocs",
    owner="MCERQUA",
    branch="main",
    content="[Updated SESSIONS.md content]",
    message="Add session for [ComponentName] documentation"
)
```

### 5. Identify Next Component

Determine the next component to document based on the priority list and update the README:

```
create_or_update_file(
    path="README.md",
    repo="SpatialDocs",
    owner="MCERQUA",
    branch="main",
    content="[Updated README with next component]",
    message="Update README.md with next component to document"
)
```

## Important Process Notes

### Repository Structure
- There are TWO README.md files in this repository:
  - `/README.md` (root): Contains the main component checklist
  - `/docs/README.md`: Contains project overview and progress statistics
- Both files must be updated when documenting components

### File Upload Process
1. Never skip pushing files to GitHub after creating them
2. Only upload files after user confirmation of content
3. Upload files in this sequence:
   - Component documentation first
   - manifest.json updates second  
   - README updates third (both files)
   - SESSIONS.md updates last
4. Always verify each file upload is successful

### Checkmark Visibility and Documentation Links
When updating the README.md checklist:
1. Use proper markdown checkmark syntax: `- [x]` 
2. Make checkmarks visually distinct with linked documentation:
   - Format: `- [x] [ComponentName](./research/reference/SpatialSys/UnitySDK/ComponentName.md)`
   - Add completion text: `- [x] [ComponentName](./path/to/file.md) - COMPLETED! (MM/DD/YYYY)`
   - Add category completion indicator: `#### Category Name (COMPLETED ✅)`
3. After updating, visually verify in GitHub's web interface that:
   - Checkmarks appear as green ✓ (not gray)
   - Component names link to their documentation files
   - Completed categories are clearly marked
   - The date of completion is visible

### Documentation Checklist
After uploading files, verify:
- [x] Component documentation file is in GitHub
- [x] manifest.json has been updated with the component
- [x] Root README has the component checked off with green visible checkmarks and documentation links
- [x] docs/README progress statistics are updated
- [x] SESSIONS.md contains the new session log

### Common Pitfalls
- Forgetting to upload files to GitHub
- Not updating both README files
- Using markdown that doesn't show green checkmarks in GitHub
- Missing documentation links in the checklist
- Incorrect relative paths in documentation links
- Incorrect progress metric calculations
- Missing relationships between components in documentation
- Not verifying GitHub uploads were successful

## Session Notes

- Record any challenges or insights from the documentation process
- Note any dependencies or connections between components
- Document any inconsistencies or gaps in the reference documentation
- Track time and token usage for cost management