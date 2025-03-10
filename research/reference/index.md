# C# Scripting API Reference

This documentation details the Spatial C# API for use with the Creator Toolkit inside Unity. If you are brand new to Spatial consider looking over the [Creator Toolkit manual](https://toolkit.spatial.io/docs) first, or to get started with the setup for C# inside the toolkit refer to the [C# overview guide](https://toolkit.spatial.io/docs/csharp).

This reference should be used in combination with [Unity's Scripting Reference](https://docs.unity3d.com/ScriptReference/) as well as [Microsoft's C# documentation](https://learn.microsoft.com/en-us/dotnet/csharp/).

## Spatial Bridge & Services

Interfacing with the Spatial API is done through the static `SpatialBridge` class. Contained in the class are various **services** that provide access to Spatial features.

```csharp
using SpatialSys.UnitySDK;

private void HelloWorld()
{
    SpatialBridge.coreGUIService.DisplayToastMessage("Hello World");
}
```

## Players, Actors, & Avatars

In Spatial, a single "player" or "user" is defined as an `Actor`. An Actor is like a seat at the table, it contains a users unique ID, username, and any avatar's they control.

See `IActor`

An `Avatar` is the 3D character that each player controls. In C# each remote Avatar is read-only, but the local Avatar can be modified and directly controlled with scripts like shown below.

See `IAvatar`

```csharp
using SpatialSys.UnitySDK;

private void LaunchPlayerAvatar()
{
    var localAvatar = SpatialBridge.actorService.localActor.avatar;
    localAvatar.AddForce(Vector3.up * 10);
}
```

## Async Operations

Many Spatial functions return a `SpatialAsyncOperation`. These are used to represent the status of an "async" or "latent" function. You can provide them a callback to execute code as soon as the operation is completed, or yield on them inside of a coroutine (see below).

```csharp
public void AsyncInline()
{
    SpatialBridge.userWorldDataStoreService.SetVariable("playerHighScore", 0).SetCompletedEvent((resp) => {
        //This code will execute once the SetVariable() AsyncOp completes
        Debug.Log("Game Saved!");
    });
}
```

```csharp
public IEnumerator AsyncCoroutine()
{
    var request = SpatialBridge.userWorldDataStoreService.SetVariable("playerHighScore", 0);
    yield return request;// wait for the request to complete
    Debug.Log("Game Saved!");
}
```

## Limitations

All C# used in a space must be inside a single assembly definition. Referenced or sub-assemblies are not supported.

Spatial supports a subset of C# and the UnityEngine codebase. This is critical to keep Spatial a secure platform. For more information view the [C# limitations page](https://toolkit.spatial.io/docs/csharp/limitations).

Spatial does not support additive scene loading or scene management of any kind. Each space package uploaded may only use a single scene.
