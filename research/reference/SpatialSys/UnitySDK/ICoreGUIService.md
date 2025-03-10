# ICoreGUIService

Core GUI Service Interface

Service for handling all UI related functionality.

## Properties

| Property | Description |
| --- | --- |
| shop | Interface for shop-specific functionality |

## Methods

| Method | Description |
| --- | --- |
| CloseAllCoreGUI() | Closes or minimizes all core GUIs. This simply hides the GUIs from the user, but does not disable them. |
| DisplayToastMessage(string, float) | Display a toast message to the user. This is a basic text-based notification. This can be called every 500ms. If called more frequently, the messages will be ignored. |
| GetCoreGUIState(SpatialCoreGUIType) | Returns the current state of a core GUI. |
| SetCoreGUIEnabled(SpatialCoreGUITypeFlags, bool) | Enables or Disables a core GUI. If enabled, this allows the user or script to open the GUI. But if it was not open before, it will not be opened. If disabled, this will not only close or minimize or hide the given GUI, but also prevent it from being opened by the user until it is re-enabled. When re-enabled, the GUI will be restored to its previous open state, either minimized or open/maximized. |
| SetCoreGUIOpen(SpatialCoreGUITypeFlags, bool) | Opens/Maximize or Close/Minimize a core GUI. When closed, this simply hides the GUI from the user, but does not disable it, which means the GUI can still be opened by the user via hotkeys. If the GUI is currently disabled and an attempt is made to open the UI, it will not be opened, nor will it be opened if it is eventually enabled. However, closing the GUI even when it is disabled will still mark it as closed. |
| SetMobileControlsGUIEnabled(SpatialMobileControlsGUITypeFlags, bool) | Enables or Disables mobile controls GUI. If disabled, this will hide the default controls GUI on mobile devices. |

## Events

| Event | Description |
| --- | --- |
| onCoreGUIEnabledStateChanged | Triggered when a core GUI is enabled or disabled. |
| onCoreGUIOpenStateChanged | Triggered when a core GUI is opened or closed. |
