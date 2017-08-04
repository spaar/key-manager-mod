# Key Manager

Key Manager is a mod for Besiege that allows bulk key remapping and easier discoverability of machine controls.
It is purely an additional feature for machines: If a user of the machine has the mod installed, they will get all the benefits
but if they don't, the machine will still work just fine.
The Key Manager also supports adding multiple keybinds to a single block control.

## Installation

0. Install spaar's Mod Loader if you haven't already.
1. [Download KeyManager.zip](https://github.com/spaar/key-manager-mod/releases/latest).
2. Out of it, extract KeyManager.dll to Besiege_Data/Mods/.
   The .zip file also includes a copy of this README and the mod's license but these files are not required to run the mod.
   
## Usage

#### 1. Use a machine with Key Manager support

If you load a machine that contains Key Manager data while having the mod installed, a Key Manager window will appear.
It shows the machine controls as assigned by the machine creator. You can use it to figure out how to control the machine.
Additionally, you can hover over the assigned keys and press another key to remap any controls you want.

The Key Manager window can be manually openend and closed using its button in the Settings drop-down (where you toggle the god tools).
And lastly, you can of course modify the machine's Key Manager data as described in section 2.

#### 2. Create a machine with Key Manager support

*Note:* Currently the key manager is mostly intended to be used on finished machines to make them more player-friendly.
Different ways to assign controls are being considered to maybe make it more useful while building in the future.

Key Manager organizes machine controls in so-called Key Groups: A group has a name that is displayed to users as well as block controls assigned to it.
The Key Manager will then allow you and users of the machine to assign a new key to all of the assigned controls at once.
It also supports multiple keys for a single control.

A simple car might have two groups for example, "Forward" and "Reverse", each having the corresponding controls of the wheels assigned to it.

To configure Key Manager support, open the Key Manager window using its button in the Settings drop-down (where you toggle the god tools).
Start editing the Key Manager data by pressing the "Edit" button in the top right of the window. You exit edit mode with the same button.
Create a group by pressing the "Add" button.
Edit a group by pressing the "..." button next to its name. A window will open that allows you to change the group's name as well as assign controls to it.
To quickly edit the names of multiple groups, you can cycle through them with Tab while focusing the name text field.
Before assigning controls, you need to assign one or multiple keys to the group. To do so, exit edit mode, hover over the "Add new" field and press a key.
To assign controls go back into edit mode, open the detail dialog for the group and press the "(Un-)Assign blocks" button. All currently assigned blocks will be highlighted.
A button will appear that lets you assign all controls that currently have one or more of the keys of the group assigned to them.
For details on how controls are assigned to groups, see [How controls are assigned](#how-controls-are-assigned).
To unassign a previously assigned control, click the highlighted block. It will be removed from the group and the highlight will disappear.

There is also an auto-add feature. It will automatically attempt to group all controls of the same block type that currently are assigned to the same key.
It is mostly intended to serve as a quick starting point for configuring an existing machine for use with the Key Manager. You can then edit the groups
as normal from there.

## How controls are assigned

A group has one or multiple keys assigned to it. When you use the button to assign controls, all controls that have _at least_ one of the group's keys assigned are added to the group.
Assigning controls follows the following rules:
- Any keys assigned to the group but not the control are added to the control.
- Any keys assigned to the control but not the group are left unmodified on the control.
- When later remapping keys in the Key Manager interface, keys that were left unmodified according to the previous rule, are always left untouched.

Lastly, if you edit the keybindings on the control manually, the key manager will behave as follows whenever keys are remapped in its interface:
- The key manager stores at what "index", so at what position, the keys it was told to add to the group are in the case of multikeybinds.
- If you change a key at a position that was added to the group, it will be changed again by the key manager when mapping keys through it.
- If you change a key that was not added to the group, such as in the case of the second rule above, the key manager will leave it alone.
- If you add a new key to a control after assigning the control to a group, the key manager will also leave it alone.


## License

The mod is licensed under the MIT license, see the [LICENSE](./LICENSE) file for details.
All source code is available on [GitHub](https://github.com/spaar/key-manager-mod/).