# Godot ImGui Extensions

***
[WIP]

Some customized imgui widgets for Godot used in my personal projects.
Not very optimized, but maybe useful enough to do some daily debugging.

<img src="https://github.com/maidopi-usagi/godot_imgui_ext/assets/33864304/71c9bcf0-e627-4659-b5c9-855878af47fe" height="300">
<img src="https://github.com/maidopi-usagi/godot_imgui_ext/assets/33864304/0c0eec39-6208-4838-ac28-5d756ce84533" height="300">


## Features

* Fully support to Godot's variant types
* Runtime SceneTree inspector
* Runtime GodotObject inspector, with oranized structure and supports Godot export hints
* Nested data types
* Make IME working
* (opt) Method execution time profiling tools with a barely simple plot

## Demo and some usecases

### 01_multi_mesh_instance

https://github.com/maidopi-usagi/godot_imgui_ext/assets/33864304/8c42f859-0c31-4285-9a73-3ab30eab92e4

### 02_json_editor

https://github.com/maidopi-usagi/godot_imgui_ext/assets/33864304/82036697-7662-47db-886c-db668fc9c1b1

## Dependencies
* Godot 4.2+
* .NET 8.0
* imgui-godot: https://github.com/pkdawson/imgui-godot
  * Install the plugin to res://addons/imgui-godot/
* Fody(optional, for simple profiling)

## Usage

## Planned work(May be shipped in an optional package)
- [ ] Migrate and clean up some existing widgets from my personal project
   - [ ] Datasheet editing toolset (Excel-like editor with Godot features)
   - [ ] Runtime logger with filtering and backtraces
   - [ ] Runtime console
   - [ ] Resource picker with fuzzy match and filtering
- [ ] Better profiling tools, with common builtin engine stastics and custom measurement sources
- [ ] UX improvements
New ideas are also welcomed~
