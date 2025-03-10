## Overview
The PackageType enum categorizes the different types of packages that can exist within the Spatial platform. It defines the classification of various package assets based on their intended use and functionality in the Spatial ecosystem.

## Properties
- **Space**: Represents a complete spatial environment or room.
- **SpaceTemplate**: Represents a template that can be used as a starting point for creating spaces.
- **Avatar**: Represents a user avatar package.
- **AvatarAttachment**: Represents an attachment that can be added to avatars.
- **AvatarAnimation**: Represents an animation that can be applied to avatars.
- **PrefabObject**: Represents a prefabricated object that can be placed in a space.

## Usage Example
```csharp
// Example of filtering packages by type
public List<SpatialPackage> GetAvatarPackages(List<SpatialPackage> allPackages)
{
    List<SpatialPackage> avatarPackages = new List<SpatialPackage>();
    
    foreach (SpatialPackage package in allPackages)
    {
        if (package.packageType == PackageType.Avatar)
        {
            avatarPackages.Add(package);
        }
    }
    
    return avatarPackages;
}

// Example of checking package type
public void HandlePackage(SpatialPackage package)
{
    switch (package.packageType)
    {
        case PackageType.Space:
            LoadSpace(package);
            break;
        case PackageType.Avatar:
            ApplyAvatar(package);
            break;
        case PackageType.AvatarAttachment:
            AttachToAvatar(package);
            break;
        default:
            Debug.Log("Unsupported package type: " + package.packageType);
            break;
    }
}
```

## Best Practices
- Use the appropriate PackageType when creating new packages to ensure they're handled correctly by the Spatial system
- When developing interfaces for package management, group or filter packages by type for better organization
- Consider the package type when importing or exporting packages to ensure compatibility
- Use SpaceTemplate for creating reusable space designs rather than duplicating complete spaces

## Common Use Cases
- Categorizing packages in a library or marketplace
- Filtering packages when displaying them to users
- Determining the appropriate loading or instantiation method based on package type
- Creating systems that manage specific types of packages (e.g., avatar customization systems)
- Building content creation tools that generate properly typed packages

## Completed: March 10, 2025
