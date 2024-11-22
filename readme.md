# 🖱️ DoubleClickFix

A lightweight solution for mitigating double-click issues caused by malfunctioning mice.  

This tool ensures smoother operation by filtering unintended double-click events, allowing you to define the minimal delay between valid clicks directly from an intuitive user interface.

![logo](DoubleClickFix/app.ico)

---

## ✨ Features
- **Customizable Delay**: Adjust the minimal delay between two clicks via a user-friendly interface.
- **Windows Tray Integration**: Double-click the tray icon to open the settings UI.
- **Startup Option**: Easily register the application to launch with Windows.

---

## 🚀 Installation

### Quick Setup
1. **Download**: Grab the latest release from the [Releases page](https://github.com/nenning/DoubleClickFix/releases).
2. **Unzip & Run**: Extract the files and execute the `.exe`.  
   > Note: You might need to install the [.NET Runtime](https://dotnet.microsoft.com/en-us/download/dotnet) first.

### Advanced Setup
- **Build from Source**: Clone the repository and compile the application yourself using Visual Studio or your preferred .NET toolchain.

---

## ⚙️ Configuration

### User Interface
- Most settings can be adjusted directly through the graphical UI.
- Configuration changes are saved in `DoubleClickFix.dll.config`.

### Manual Edits
- You can edit `DoubleClickFix.dll.config` manually, but changes require restarting the application to take effect.

### Handling Touch Devices
- Double-clicks from touchpads or touchscreens are generally allowed by default. 
- If your device is not recognized (e.g., it has a different device ID !=0), adjust the `ignoredDevice` value in the configuration file based on the application logs.
- Alternatively, enable the **Allow 0ms Double-Click Duration** option in the UI.

> **Note**: When upgrading to a new release, reconfigure your settings as there is no automatic migration of previous configurations.

---

## 💡 Tips
- Check logs for detailed information on device IDs and other runtime details.
- Experiment with different delay settings to optimize for your specific hardware issues.

---

## 🤝 Contributions
Contributions are welcome! Feel free to open issues, submit pull requests, or suggest improvements via the [Issues tab](https://github.com/nenning/DoubleClickFix/issues).

---

## 📜 License
This project is distributed under the [MIT License](LICENSE).
