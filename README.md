# Ore Support

Shows supporting bounding boxes for mine rocks which makes collapsing them much easier.

Install on the client (modding [guide](https://youtu.be/L9ljm2eKLrk)).

## Instructions

Use `/bind [key code] ore_support` to quickly toggle the mod on/off with a keypress.

Mine rocks like copper or silver deposits consists of multiple parts. These parts are automatically destroyed when not supported by anything.

The game uses axis aligned bounding boxes for checking the support which makes it difficult to determine which parts are preventing the deposit from collapsing.

This mod visualizes those boxes:

- Red color indicates a part that is being supported by terrain or other object.
- Orange color indicates a part that collapses the deposit when destroyed.
- Green color indicates a part that is no longer supported but the game hasn't updated its state yet.
- Yellow color indicates a object that is supporting the deposit.

## Configuration

After first start up, the config file can be found in the \<GameDirectory\>\BepInEx\config\ folder:

- Critical supported color: Color of the last supported part.
- Enable: Whether this mod is enabled. Can be toggled with command ore_support.
- Line width: Width of the lines. Increase for more visibility, reduce for less clutter.
- Max boxes: The maximum amount of boxes to show. This is intended to improve performance and reduce clutter as initially most pieces support the mine rock.
- Max parts: The maximum amount of remaining parts to show. This is intended to improve performance and reduce clutter as initially most pieces support the mine rock.
- Min size: Minimum amount of total parts in the mine rock to show any boxes. This is intended to prevent boxes on smaller rocks.
- Refresh interval: How often the tool checks the support of each part. Lower values makes the boxes update faster but lowers performance.
- Support color: Color of the supporting objects.
- Supported color: Color of the supported parts.
- Supporting objects: If enabled, supporting objects are shown.
- Unsupported color: Color of the unsupported parts.

## Credits

Sources: [GitHub](https://github.com/JereKuusela/valheim-ore_support)

Donations: [Buy me a computer](https://www.buymeacoffee.com/jerekuusela)
 