# Spatial Templates Documentation Guide

This document provides guidance for documenting Spatial templates consistently throughout this project.

## Documentation Standard Format

Each template documentation should follow this structure:

```markdown
# Template Name

## Overview
Brief description of the template, its purpose, and what it offers.

## Features
- **Feature 1**: Description
- **Feature 2**: Description
- **Feature 3**: Description
- etc.

## Included Components

### 1. Component Name
Detailed description of component:
- Key aspect 1
- Key aspect 2
- etc.

### 2. Component Name
...

## Integration with SDK Components
Table listing SDK components used and how they're used:

| SDK Component | Usage in Template |
|---------------|-------------------|
| Component 1 | How it's used |
| Component 2 | How it's used |
| etc. | ... |

## When to Use
Bullet list of use cases and scenarios when this template is appropriate to use.

## Implementation Details

### Feature Implementation
Code example with explanation:

```csharp
// Code example
public class ExampleClass : MonoBehaviour
{
    // Implementation details
}
```

### Another Feature
Additional code examples as needed.

## Best Practices
- Practice 1
- Practice 2
- etc.

## Related Templates
- [Template 1](path/to/template1.md) - Brief description of relation
- [Template 2](path/to/template2.md) - Brief description of relation

## Additional Resources
- [GitHub Repository](link)
- [Live Demo](link) (if available)
- [Related Documentation](link)
```

## Documentation Process

Follow these steps when documenting a template:

1. **Research**: Gather information from the Spatial Creator Toolkit website and GitHub repository
2. **Structure**: Create the document following the standard format above
3. **Implementation**: Focus on practical implementation details and code examples
4. **References**: Link to related SDK components from the main documentation project
5. **Completion**: Update the README.md with completion status and date

## Template Categories and Documentation Priorities

Templates are grouped into categories with the following documentation priorities:

1. **Core Templates** (Highest Priority)
   - Spatial Starter Template
   - Camera Modes
   - Simple Car Controller
   - Obby (Obstacle Course) Game

2. **Game Templates** (High Priority)
   - Top Down Shooter
   - Gem Collection Game
   - Run Master Game
   - Basketball Game
   - Hyper Jump Game

3. **Technical Templates** (Medium Priority)
   - Avatar Input Control
   - Matchmaking (Lobby System)
   - Daily/Weekly Rewards
   - WebGL Instancer
   - GPU Particles
   - HTTP Request Demo
   - Addressables
   - Embedded Package

4. **Environment Templates** (Lower Priority)
   - Abstract
   - Agora
   - Auditorium
   - And other environment templates

## Special Documentation Considerations

### Code Examples
- Ensure code examples are complete and functional
- Include comments explaining key aspects
- Use proper C# formatting with syntax highlighting
- Focus on the integration with Spatial SDK

### Component Integration
- Clearly document how the template integrates with Spatial's components
- Explain any custom modifications to standard Spatial behavior
- Highlight networking and synchronization aspects

### Performance Notes
- Include performance considerations for WebGL deployment
- Note any optimizations used in the template
- Mention potential bottlenecks and how to avoid them

### Cross-Platform Compatibility
- Document any platform-specific considerations
- Note if certain features work differently on mobile vs desktop

## Using SDK Component References

When referencing SDK components that have been documented in the main project, use relative links like:

```markdown
[ICameraService](../../SpatialSys/UnitySDK/ICameraService.md)
```

This helps create a connected documentation network where users can easily navigate between template documentation and component documentation.

## Template Documentation Status Tracking

When a template is documented, update the README.md file with:

1. Mark the template as completed with the date:
```markdown
- [x] [Template Name](./path/to/file.md) - COMPLETED! (MM/DD/YYYY)
```

2. Update the overall completion statistics:
```markdown
- Templates identified: 35
- Templates documented: X
- Overall completion: X%
```

3. Add an entry to the "Documentation Completed" section with a brief summary of what was documented.

## Documentation Examples

For reference, see these completed template documentations:

1. [Spatial Starter Template](./Starter/SpatialStarterTemplate.md)
2. [Camera Modes](./Camera/CameraModes.md)
3. [Obby (Obstacle Course) Game](./Games/ObbyGame.md)
4. [Simple Car Controller](./Vehicles/SimpleCarController.md)

These examples demonstrate the expected depth, structure, and quality for all template documentation.
