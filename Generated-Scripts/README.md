# Generated Scripts

This directory contains validated C# scripts for use in Spatial Unity SDK projects. These scripts have been tested and verified to work correctly with the Spatial platform.

## Available Scripts

### PeriodicCurrencyRewarder.cs
A script that awards world currency to players periodically. It awards currency when the scene initializes and then continues to award currency at regular intervals.

**Features:**
- Configurable currency amount
- Adjustable time interval between rewards
- Can be enabled/disabled during runtime
- Uses proper Spatial SDK components

**Equivalent to Visual Scripting:**
This script implements the equivalent functionality of a Visual Scripting graph with the following nodes:
- "Spatial System On Scene Initialized Event" as the starting trigger
- "Spatial Award World Currency" node with Amount parameter
- "While Loop" with a Boolean condition
- "Wait For Seconds" with a Delay of 120 seconds (2 minutes)

**Documentation:** Full documentation is available in our Pinecone database with ID: `spatial-sdk-example-periodic-currency-rewarder`

### DoorwayPortal.cs
A script that creates a custom portal trigger for teleporting between Spatial spaces. When a player enters the trigger area, they are teleported to another space.

**Features:**
- Configurable destination space ID
- Optional confirmation popup toggle
- Support for visual effects when triggered
- Uses the latest TeleportToSpace functionality from Creator Toolkit v1.58+

**Implementation Details:**
- Uses trigger colliders to detect when a player enters the portal area
- Adds a slight delay to allow visual effects to display before teleporting
- Utilizes `SpatialBridge.spaceService.TeleportToSpace()` for the actual teleportation

**Documentation:** Full documentation is available in our Pinecone database with ID: `spatial-sdk-example-doorway-portal`

## Usage Guidelines

1. Copy the desired script into your Unity project
2. Attach the script to an appropriate GameObject in your scene
3. Configure the script parameters in the Unity Inspector
4. Test in Play mode before building your Spatial package

## Contribution

These scripts are generated from validated implementations and stored in our Pinecone database. If you have suggestions for improvements or find issues, please contact the documentation team.

**Last Updated:** March 23, 2025