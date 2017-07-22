# Key Manager

Key Manager is a mod for Besiege that allows bulk key remapping and easier discoverability of machine controls.
Is is purely an additional feature for machines: If a user of the machine has the mod installed, they will get all the benefits
but if they don't, the machine will still work just fine.
The Key Manager also supports adding multiple keybinds to a single block control.

## Installation

0. Install spaar's Mod Loader if you haven't already.
1. [Download KeyManager.zip](https://github.com/spaar/key-manager-mod/releases/latest).
2. Out of it, extract KeyManager.dll and the Resources/ folder to Besiege_Data/Mods/.
   As a result, KeyManager.dll should be in Mods/ and a Mods/Resources/KeyManager/ folder should exist.
   If that is not the case, you did not extract the files to the correct location.
   
## Usage

#### 1. Use a machine with Key Manager support

If you load a machine that contains Key Manager data while having the mod installed, a Key Manager window will appear.
It shows the machine controls as assigned by the machine creator. You can use it to figure out how to control the machine.
Additionally, you can hover over the assigned keys and press another key to remap any controls you want.

The Key Manager window can be manually openend and closed using its button in the Settings drop-down (where you toggle the god tools).
And lastly, you can of course modify the machine's Key Manager data as described in section 2.

#### 2. Create a machine with Key Manager support

Key Manager organizes machine controls in so-called Key Groups: A group has a name that is displayed to users as well as block controls assigned to it.
The Key Manager will then allow you and users of the machine to assign a new key to all of the assigned controls at once.

A simple car might have two groups for example, "Forward" and "Reverse", each having the corresponding controls of the wheels assigned to it.

To configure Key Manager support, open the Key Manager window using its button in the Settings drop-down (where you toggle the god tools).
Start editing the Key Manager data by pressing the "Edit" button in the top right of the window. You exit edit mode with the same button.
Create a group by pressing the "Add" button.
Edit a group by pressing the "..." button next to its name. A window will open that allows you to change the group's name as well as assign controls to it.
Assign controls by pressing the "(Un-)Assign blocks" button. All currently assigned blocks will be highlighted.
To assign a new control, click a non-highlighted block (that has key controls) and choose which control you want to map in the window that appears.
To unassign a previously assigned control, click the highlighted block. It will be removed from the group and the highlight will disappear.

There is also an auto-add feature. It will automatically attempt to group all controls of the same block type that currently are assigned to the same key.
It is mostly intended to serve as a quick starting point for configuring an existing machine for use with the Key Manager. You can then edit the groups
as normal from there.

## License

The mod is licensed under the MIT license, see the [LICENSE](./LICENSE) file for details.
All source code is available on [GitHub](https://github.com/spaar/key-manager-mod/).