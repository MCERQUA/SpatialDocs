## Overview
The DocumentationCategoryAttribute is used for marking and organizing documentation categories within the Spatial SDK. This attribute helps classify classes, interfaces, and other code elements into logical categories, making the documentation more organized and easier to navigate.

## Properties
- **Category**: The category name that this element belongs to in the documentation

## Usage Example
```csharp
using SpatialSys.UnitySDK;

// Marking a class as part of the "Avatar" documentation category
[DocumentationCategory("Avatar")]
public class AvatarController
{
    // Class implementation
}

// Marking an interface as part of the "Services" documentation category
[DocumentationCategory("Services")]
public interface IExampleService
{
    // Interface definition
}

// Marking an enum as part of the "UI" documentation category
[DocumentationCategory("UI")]
public enum UIElementType
{
    Button,
    Panel,
    Label
}
```

## Best Practices
- Use consistent category names across related components
- Choose clear, descriptive category names that reflect the component's functionality
- Categories should be broad enough to group related components, but specific enough to be meaningful
- Follow the existing category naming conventions in the Spatial SDK documentation
- Use this attribute on public API elements that need to be part of the organized documentation
- Category names should be singular rather than plural (e.g., "Service" rather than "Services")

## Common Use Cases
- Organizing public SDK interfaces into logical groups
- Categorizing components by their functional area
- Creating hierarchical documentation structures
- Improving discoverability of related SDK components
- Setting up documentation navigation and table of contents
- Making the API reference more user-friendly by grouping similar elements

## Inherited Members
This attribute inherits all members from System.Attribute, including methods for retrieving and comparing attributes.

## Completed: March 10, 2025