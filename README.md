# Godot ImGui Extensions

***
[WIP]

Some customized imgui widgets for Godot used in my personal projects.
Not very optimized, but maybe useful enough to do some daily debugging.

<img src="https://github.com/maidopi-usagi/godot_imgui_ext/assets/33864304/71c9bcf0-e627-4659-b5c9-855878af47fe" height="200">
<img src="https://github.com/maidopi-usagi/godot_imgui_ext/assets/33864304/0c0eec39-6208-4838-ac28-5d756ce84533" height="200">
<img src="https://github.com/maidopi-usagi/godot_imgui_ext/assets/33864304/ae02bc6c-0f9c-4caf-b6d0-8156fa49d5f7" height="200">

## Features

* Fully support to Godot's variant types
* Runtime SceneTree inspector
* Runtime GodotObject inspector, with oranized structure and supports Godot export hints
* Nested data types
* Fixes to make IME working

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

## Planned work(May be shipped in another package)
- [ ] Migrate and clean up some existing widgets from my personal project
  - [ ] Datasheet/Table editing toolset (Excel-like editor with Godot features)
  - [ ] Runtime logger with filtering and backtraces
  - [ ] Runtime console
  - [ ] Resource picker with fuzzy match and filtering

<img src="https://github.com/maidopi-usagi/godot_imgui_ext/assets/33864304/7c97fa79-e8d0-4e7e-adef-8edf3b23de3f" height="200">
<img src="https://github.com/maidopi-usagi/godot_imgui_ext/assets/33864304/13673043-794c-4e7d-bc8c-711f7d925663" height="200">


- [ ] Better profiling tools, with common builtin engine stastics and custom measurement sources
- [ ] UX improvements
- [ ] ImPlot branch
- [x] Hexa.NET.ImGui branch at https://github.com/maidopi-usagi/imgui-godot with ImGuizmo ImNodes

![image](https://github.com/user-attachments/assets/38bf3b7a-702f-4254-9162-e9070fabcccb)

New ideas are also welcomed~
