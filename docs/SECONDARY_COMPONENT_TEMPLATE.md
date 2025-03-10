# Secondary Component Documentation Template

## Session Information
- **Component**: [Component Name]
- **Category**: [Service Category]
- **Type**: [Class/Interface/Struct/Enum]
- **Session Date**: [Date]
- **Session Number**: [#]
- **Estimated Cost**: $[Amount]

## Pre-Session Checklist
- [ ] Load and verify manifest.json
- [ ] Confirm component priority
- [ ] Check for any related documentation
- [ ] Prepare firecrawl scraping URL

## Session Steps

### 1. Content Gathering
- [ ] Scrape reference documentation using:
  ```javascript
  firecrawl_scrape({
    url: "https://toolkit.spatial.io/reference/SpatialSys.UnitySDK.[ComponentName]",
    formats: ["markdown"],
    onlyMainContent: true
  })
  ```
- [ ] Note key component elements:
  - [ ] Properties/Fields
  - [ ] Methods
  - [ ] Events
  - [ ] Related components

### 2. Documentation Structure
- [ ] Create markdown file at `research/reference/SpatialSys/UnitySDK/[ComponentName].md`
- [ ] Include header information:
  ```markdown
  # [ComponentName]
  
  Category: [Service Category]
  
  [Type]: [Brief description]
  
  [Detailed description of the component's purpose and functionality]
  ```
- [ ] Properties/Fields section (if applicable):
  ```markdown
  ## Properties/Fields
  
  | Property | Description |
  | --- | --- |
  | [propertyName] | [Description] |
  ```
- [ ] Methods section (if applicable):
  ```markdown
  ## Methods
  
  | Method | Description |
  | --- | --- |
  | [methodName(params)] | [Description] |
  ```
- [ ] Events section (if applicable)
- [ ] Inheritance section (if applicable)

### 3. Code Examples
- [ ] Create practical usage examples:
  ```markdown
  ## Usage Examples
  
  ```csharp
  // Example code showing typical usage
  ```
  ```
- [ ] Include multiple examples showing different use cases
- [ ] Ensure examples are complete and functional
- [ ] Add comments explaining key concepts

### 4. Best Practices Section
- [ ] List 4-6 best practices for using this component
- [ ] Focus on practical tips and common pitfalls
- [ ] Include performance considerations

### 5. Common Use Cases Section
- [ ] List 4-6 common use cases for this component
- [ ] Provide brief descriptions of each use case
- [ ] Connect to real-world scenarios

### 6. Related Components Section
- [ ] List related components with links to their documentation
- [ ] Explain relationships between components

### 7. Quality Review
- [ ] Check all sections present
- [ ] Verify code examples
- [ ] Confirm formatting
- [ ] Spell check
- [ ] Technical accuracy

### 8. GitHub File Upload
- [ ] Present completed documentation to user for review
- [ ] After user confirmation, upload documentation file:
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

### 9. Update Tracking Files
- [ ] Update manifest.json:
  - [ ] Move component from "incomplete" to "completed"
  - [ ] Update completion date
  - [ ] Update progress metrics
  ```
  create_or_update_file(
    path="docs/manifest.json",
    repo="SpatialDocs",
    owner="MCERQUA",
    branch="main",
    content="[Updated manifest content]",
    message="Update manifest.json for [ComponentName]"
  )
  ```
- [ ] Update BOTH README files:
  - [ ] Root README.md: Check off component in checklist with visible green checkmark and documentation link:
    ```
    // In the README.md content, use this format for checkmarks with documentation links:
    // - [x] [ComponentName](./research/reference/SpatialSys/UnitySDK/ComponentName.md) - COMPLETED! (MM/DD/YYYY)
    
    // For completed categories, use:
    // #### Category Name (COMPLETED ✅)
    
    create_or_update_file(
      path="README.md",
      repo="SpatialDocs",
      owner="MCERQUA",
      branch="main",
      content="[Updated README content]",
      message="Update README.md to mark [ComponentName] as completed with documentation link"
    )
    ```
  - [ ] docs/README.md: Update progress statistics
    ```
    create_or_update_file(
      path="docs/README.md",
      repo="SpatialDocs",
      owner="MCERQUA",
      branch="main",
      content="[Updated docs README content]",
      message="Update docs/README.md with updated progress for [ComponentName]"
    )
    ```
- [ ] Add session information to docs/SESSIONS.md
  ```
  create_or_update_file(
    path="docs/SESSIONS.md",
    repo="SpatialDocs",
    owner="MCERQUA",
    branch="main",
    content="[Updated SESSIONS.md content]",
    message="Add session log for [ComponentName] documentation"
  )
  ```

## Post-Session Notes
- Challenges encountered:
- Solutions implemented:
- Areas for improvement:
- Related components identified:

## Cost Tracking
- Tokens used:
- Time spent:
- Cost incurred:

## Next Component
- [ ] Identify next component in priority order
- [ ] Note any prerequisites or dependencies

## Important Reminders

### Repository Structure
- This repository contains TWO README.md files, both must be updated:
  - `/README.md` (root): Contains component checklists and main project info
  - `/docs/README.md`: Contains summary statistics and project overview

### File Upload Sequence
1. Always upload files to GitHub in this order:
   - Component documentation first
   - manifest.json updates second
   - README updates third (both README files)
   - SESSIONS.md updates last
2. Always verify uploads are successful before proceeding to the next file

### Checkmark Visibility with Documentation Links
When updating the README.md checklist:
1. Use the correct markdown syntax: `- [x]` (not just using a plain `-` with text)
2. Add documentation links and visual indicators:
   - Link format: `- [x] [ComponentName](./research/reference/SpatialSys/UnitySDK/ComponentName.md)`
   - Add completion text: `- [x] [ComponentName](./path/to/file.md) - COMPLETED! (MM/DD/YYYY)`
   - Add category completion indicator: `#### Category Name (COMPLETED ✅)`
   - Use emoji (✅) to make completion clear at a glance
3. Always validate the GitHub web interface:
   - Checkmarks appear as green ✓ (not gray)
   - Component names are clickable links to their documentation files
   - Links work correctly when clicked
4. If checkmarks appear gray instead of green, try:
   - Using different formatting with emphasis markers
   - Adding extra spacing before the checkbox
   - Using a section heading with the completion status indicator

### Documentation Validation
- After creating documentation, verify all sections are complete
- Ensure code examples will compile and work as expected
- Check all links to related components
- Format documentation consistently with existing files

### Common Pitfalls to Avoid
- Not updating both README files
- Forgetting to push files to GitHub after preparing them
- Using markdown that displays as gray checkboxes instead of green checkmarks
- Missing or incorrect documentation links in the checklist
- Relative path errors in documentation links
- Missing related components in documentation
- Incorrect progress metrics calculations