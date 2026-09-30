CENDRELITH — RELICS UI KIT — V1 GRAPHICS DRAFT

Five separate PNG assets derived from the approved gothic UI concept.
Original artwork and existing Unity scene have not been replaced.

globe_frame_v1.png — empty winged frame, transparent central opening.
health_liquid_v1.png — red circular fill artwork, no frame.
inventory_panel_v1.png — empty portrait panel background.
inventory_slot_v1.png — one empty reusable square slot.
button_normal_v1.png — blank normal-state button background.

All five are RGBA with alpha spanning 0–255. Frame center alpha is 0.
This verifies transparency exists, NOT that all edges are production-clean.
Panel, slot and liquid include slightly translucent pixels even internally.
Inspect against light and dark backgrounds before final use.
Padding differs between assets; frame and liquid are NOT pre-aligned layers.
The frame opening is below image center: do not simply stack full-size images.
Globe frame extremities approach the image boundary; review at final HUD size.

Not included: prefabs, automatic import settings, slicing borders, glass reflection,
hover/pressed artwork, item icons, XP decorations or functional UI scripts.
No labels or values are baked in. Add live text and icons as separate UI elements.
Existing scene/UI code was not changed by this art task.
Unity integration and visual testing remain pending; no GitHub push performed.

Generated with the built-in image-generation tool. Prompts are in PROMPTS.txt.
