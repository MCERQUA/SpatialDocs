# Component Documentation Checklist

Use this checklist for every component documentation session to ensure thoroughness and consistency.

## Pre-Documentation Preparation

- [ ] Review root README.md for next component to document
- [ ] Verify component priority in manifest.json
- [ ] Check for existing documentation or references
- [ ] Gather information sources:
  - [ ] toolkit.spatial.io reference
  - [ ] Source code examples (if available)
  - [ ] Related component documentation
  - [ ] Documentation templates

## Information Gathering

- [ ] Scrape component reference from toolkit.spatial.io:
  ```javascript
  firecrawl_scrape({
    url: "https://toolkit.spatial.io/reference/SpatialSys.UnitySDK.[ComponentName]",
    formats: ["markdown"],
    onlyMainContent: true
  })
  ```
- [ ] Note component details:
  - [ ] Component type (class, struct, enum, interface)
  - [ ] Inheritance hierarchy
  - [ ] Properties and fields with descriptions
  - [ ] Methods with parameters
  - [ ] Events and delegates
  - [ ] Related components
  - [ ] Usage patterns

## Documentation Creation

- [ ] Create comprehensive content following the template structure:
  - [ ] Component name and category header
  - [ ] Type declaration and description
  - [ ] Detailed overview of purpose and functionality
  - [ ] Properties/fields table with descriptions (if applicable)
  - [ ] Methods table with parameters (if applicable)
  - [ ] Events section (if applicable)
  - [ ] Multiple practical code examples
  - [ ] Best practices (4-6 items)
  - [ ] Common use cases (4-6 scenarios)
  - [ ] Related components section

## Quality Control

- [ ] Verify content quality:
  - [ ] Documentation includes all required sections
  - [ ] Code examples are complete and functional
  - [ ] All properties, methods, and events are documented
  - [ ] Best practices provide useful guidance
  - [ ] Language is clear and technical details are accurate
  - [ ] Formatting follows project standards
  - [ ] Links to related components are included
  - [ ] No placeholders or TODOs remain

## User Review Process

- [ ] Present completed documentation to user for review
- [ ] Make any requested adjustments or improvements
- [ ] Get final confirmation before uploading to GitHub

## File Upload (Only After User Confirmation)

- [ ] Upload component documentation file to GitHub:
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
- [ ] Verify file upload was successful

## Tracking Updates

- [ ] Update manifest.json:
  - [ ] Move component from "incomplete" to "completed"
  - [ ] Update completion date and sections
  - [ ] Recalculate progress metrics
  - [ ] Push updated manifest to GitHub
  ```
  create_or_update_file(
    path="docs/manifest.json",
    repo="SpatialDocs",
    owner="MCERQUA",
    branch="main",
    content="[Updated manifest content]",
    message="Update manifest.json to mark [ComponentName] as completed"
  )
  ```

- [ ] Update root README.md:
  - [ ] Use proper markdown syntax for checkmarks: `- [x]` (not just `- [x]`)
  - [ ] Add file link to the component name: `- [x] [ComponentName](./research/reference/SpatialSys/UnitySDK/ComponentName.md) - COMPLETED! (date)`
  - [ ] Add visual emphasis: `- [x] [ComponentName](./research/reference/SpatialSys/UnitySDK/ComponentName.md) - COMPLETED! (date)`
  - [ ] Add category completion indicator: `#### Category Name (COMPLETED ✅)`
  - [ ] Update "CURRENT STATUS" section metrics
  - [ ] Update "NEXT COMPONENT TO DOCUMENT" section
  - [ ] Push updated README to GitHub
  ```
  create_or_update_file(
    path="README.md",
    repo="SpatialDocs",
    owner="MCERQUA",
    branch="main",
    content="[Updated README content]",
    message="Update README.md to mark [ComponentName] as completed with documentation link"
  )
  ```

- [ ] Update docs/README.md:
  - [ ] Update secondary components statistics
  - [ ] Update overall progress metrics
  - [ ] Push updated docs README to GitHub
  ```
  create_or_update_file(
    path="docs/README.md",
    repo="SpatialDocs",
    owner="MCERQUA",
    branch="main",
    content="[Updated docs README content]",
    message="Update docs/README.md progress for [ComponentName]"
  )
  ```

- [ ] Add session information to docs/SESSIONS.md:
  - [ ] Create new session entry with date and component details
  - [ ] Document completed work, progress updates, and next steps
  - [ ] Push updated SESSIONS.md to GitHub
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

## Final Verification

- [ ] Verify all GitHub uploads were successful:
  - [ ] Component documentation file is in the repository
  - [ ] manifest.json has been updated
  - [ ] Root README.md has been updated with visible green checkmarks and documentation links
  - [ ] docs/README.md has been updated
  - [ ] SESSIONS.md has been updated

## Critical Reminders

1. **Multiple README Files**: Remember that BOTH README.md files must be updated:
   - Root `/README.md`: Contains component checklists
   - `/docs/README.md`: Contains project statistics

2. **Upload Sequence**: Always upload files in this order:
   - Documentation file first
   - manifest.json second
   - README files third
   - SESSIONS.md last

3. **User Confirmation**: Only upload files after getting user confirmation of content

4. **Upload Verification**: Always verify each file was successfully uploaded

5. **Complete Documentation**: Never skip sections or leave placeholders in documentation

6. **Checkmark Visibility**: Ensure checklist items in README.md appear with green checkmarks:
   - Use `- [x]` syntax for markdown checkboxes
   - Visually verify that the checkmarks appear green in the GitHub interface
   - Add emphasis text like "COMPLETED!" or "✅" for additional visual clarity
   - For completed categories, add "(COMPLETED ✅)" to the section heading

7. **Documentation Links**: Always add direct links to documentation files:
   - Format: `- [x] [ComponentName](./research/reference/SpatialSys/UnitySDK/ComponentName.md)`
   - Verify links work correctly after pushing to GitHub
   - Use correct relative paths from the root README.md file