# Ore Support

Shows supporting bounding boxes for mine rocks which makes collapsiong them much easier. By default only activates when only some parts remain.

# Manual Installation

1. Install the [BepInExPack Valheim](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim).
2. Download the latest zip
3. Extract it in the \<GameDirectory\>\BepInEx\plugins\ folder.
4. Optionally also install the [Configuration manager](https://github.com/BepInEx/BepInEx.ConfigurationManager/releases/tag/v16.4).

# Instructions

Mine rocks like copper or silver deposits consists of multiple parts. These parts are automatically destroyed when not supported by anything.

The game uses axis aligned bounding boxes for checking the support which makes it difficult to determine which parts are preventing the deposit from collapsing.

This mod will visualize those boxes after enough of the deposit has been destroyed.

- Red color indicates a part that is being supported by terrain or other object.
- Green color indicates a part that is no longer supported but the game hasn't updated its state yet (only updated when a part is destroyed).
- Yellow color indicates a object that is supporting the deposit.

# Configuration

After first start up, the config file can be found in the \<GameDirectory\>\BepInEx\config\ folder:

- Refresh interval: How often the tool checks the support of each part. Lower values makes the boxes update faster but lowers performance.
- Max boxes: The maximum amount of boxes to show. This is intended to improve performance and reduce clutter as initially most pieces support the mine rock.
- Min size: Minimum amount of total parts in the mine rock to show any boxes. This is intended to prevent boxes on smaller rocks.
- Max parts: The maximum amount of remaining parts to show. This is intended to improve performance and reduce clutter as initially most pieces support the mine rock.
- Line width: Width of the lines. Increase for more visibility, reduce for less clutter.
- Supported color: Color of the supported parts.
- Unsupported color: Color of the unsupported parts.
- Supporting objectsg: If enabled, supporting objects are shown.
- Support color: Color of the supporting objects.

# Changelog

- v1.2.0:
	- Added setting to show boxes only after only certain amount of parts exist.
	- Added setting to disable supporting objects.
	- Performance improvements.
- v1.1.0:
	- Added visual for objects that are supporting the mine rock.
	- Added automatic refresh without having to always destroy a part (collapsing still requires destroying a part).
	- Added different color for pieces that are no longer supported (but not yet updated by the game).
	- Added support for showing boxes when just hitting a deposit (instead of having to break a part).
	- Numeric settings now have min and max values to make configuring them easier.
- v1.0.0:
	- Initial release