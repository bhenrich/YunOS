<img src="https://github.com/bhenrich/YunOS/blob/main/MEDIA/yunos%20logo%20png.png" alt="YunOS Logo" width="200">
A simulated Linux-Style Subsystem for Windows.

---
## Table of Contents
- [Features](#features)
- [Install](#install)
  * [1. Downloading the Release.](#1-downloading-the-release)
  * [2. Adding yunos.exe to PATH.](#2-adding-yunosexe-to-path)
  * [3. Restart your shell.](#3-restart-your-shell)
  * [4. First-Time Setup.](#4-first-time-setup)
- [Changelog & Roadmap](#changelog--roadmap)
  * [Planned for the next Release](#planned-for-the-next-release)
  * [03/23/2023 - v0.1](#03232023---v01)
---
Heads up. This project is made for me to learn more about C#. It's not meant for actual, practical use. See it as some sort of toy.
This project has been made by Benjamin "YuNii" Henrich, with numerous contributions from [GeoBaer24](https://github.com/geobaer24).

## Features
- Full Linux-Style Commandline
- Optional support for Python and NodeJS
- Comes with a primitive scripting language called YunScript
- Small disk space and memory footprint

---
## Install
### 1. Downloading the Release.
- Go to the 'Release' tab and download the latest version of YunOS.
- Save the .exe file in your Windows folder ('C:\Windows\System32') or any folder you can remember, like Documents.

### 2. Adding yunos.exe to PATH.
- Edit your PATH Enviroment Variable.
- Add a new path. Make sure you enter the full path including the yunos.exe (e.g: 'C:\Windows\System32\yunos.exe').

### 3. Restart your shell.
- Restart your command prompt or windows powershell window. You should be able to run YunOS by typing the 'yunos' command.

### 4. First-Time Setup.
- YunOS comes with optional support for Python, NodeJS and the CLI-Based Text Editor Nano. YunOS will ask you if you want to install those during the first time setup. I recommend installing them, since it doesn't take long, doesn't take up a lot of space and for now you cannot install them later without first re-installing YunOS.
- YunOS will create it's enviroment in the "C:\" root of your Windows OS Drive.

### Should you run into any problems during the install process, or while using YunOS, write an [E-Mail to our support](mailto://support@yuniiworks.de) or [create an Issue in the Repository](https://github.com/bhenrich/YunOS/issues/new/choose). We would be happy to help!

---
## Changelog & Roadmap

*NOTICE: YunOS is a hobby project and will not receive a steady flow of updates. Updates to YunOS's development can be found on my [Twitter Page](https://twitter.com/yuniiworks).*

### Planned for the next Release
- ~~Support for Debian-based Linux Distributions as a host system~~
- Support for the winget/apt package manager to install any cli-software to YunOS

### 03/28/2023 - v0.1.1
- Added Fail Safes for the 'newuser' and 'remuser' commands
- Updated the first time setup completion message to improve readability

### 03/23/2023 - v0.1
- First release of YunOS.
