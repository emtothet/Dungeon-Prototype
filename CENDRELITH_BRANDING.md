# Cendrelith

Game title: **Cendrelith**. Display logo: **CENDRELITH**. No subtitle.
In the story, Cendrelith is the ancient name of the magicians' complex.

The shared title lives in DungeonMenuController.GameTitle and is used by the home screen, credits and editor setup.
Run Tools > Dungeon > Set Up Menus after applying the v2 package to update Unity Product Name.

The local repository's productName, metroPackageName, metroApplicationDescription and projectName fields have also been renamed. The installation ZIP deliberately does not replace the full ProjectSettings.asset from another computer: setup applies the desktop product name directly. Existing Unity Cloud identifiers and application identifiers stay intact.

Internal scene paths (MainMenu, Dungeon_PrototypeUnityAI), component class names and PlayerPrefs keys remain stable so references keep working. The GitHub repository URL remains emtothet/Dungeon-Prototype; no remote repository or cloud project rename was performed.

The updated visual concept is Art/Cendrelith/Concepts/main_menu_cendrelith_v1.png. The runtime menu uses live text on the clean background, not the concept image's baked text.

Use cendrelith_menus_rpg_v2.zip instead of the older menus/RPG packages. It includes their current scripts and the Cendrelith title update. Existing historical ZIPs and generated mockups retain their original content as archived versions.

Renaming Unity Product Name can change the location Unity uses for local preferences/save data. No migration of previous local preferences or asset-pack saves is included; gameplay saves are not yet integrated with the new menu.
