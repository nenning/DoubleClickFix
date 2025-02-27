# 🖱️ DoubleClickFix

A lightweight solution for mitigating double-click issues caused by malfunctioning mice.  

This tool ensures smoother operation by filtering unintended double-click events, allowing you to define the minimal delay between valid clicks directly from an intuitive user interface.

**Download from [Microsoft Store](https://apps.microsoft.com/detail/9PDGM7NL2FF2?hl=en-us&gl=CH&ocid=pdpshare)!**

![logo](DoubleClickFix/app.ico)

---

## ✨ Features
- **Customizable Delay**: Adjust the minimal delay between two clicks via a user-friendly interface.
- **Windows Tray Integration**: Double-click the tray icon to open the settings UI.
- **Startup Option**: Easily register the application to launch with Windows.

---

## 🚀 Installation

The following options are supported for installing and running the application:

### Install from Microsoft Store (recommended)
1. Go to the [Store page](https://apps.microsoft.com/detail/9PDGM7NL2FF2?hl=en-us&gl=CH&ocid=pdpshare) and install it.

### Manual Setup
1. **Download**: Grab the latest release from the [Releases page](https://github.com/nenning/DoubleClickFix/releases).
2. **Unzip & Run**: Extract the files and execute the `.exe`.  
   > Note: You might need to install the [.NET Runtime](https://dotnet.microsoft.com/en-us/download/dotnet) first.

### Advanced Setup
- **Build from Source**: Clone the repository and compile the application yourself using Visual Studio or your preferred .NET toolchain.

---

## ⚙️ Configuration

### Settings
- Most settings can be adjusted directly through the graphical user interface.
- Configuration changes are saved in the Windows registry.

### Handling Touch Devices
- Double-clicks from touchpads or touchscreens are generally allowed by default. 
- If your device is not recognized (e.g., it has a different device ID !=0), adjust the `ignoredDevice` value in the configuration file based on the application logs. But this ID is probably not stable, so this might not work. At the moment, it's better to enable the **Allow 0ms Double-Click Duration** option in the UI.

---

## ℹ️ Valve Anti-Cheat (VAC) Compatibility

### What **DoubleClickFix** Does
This application uses a **low-level mouse hook** to intercept and process mouse input events. It adjusts the behavior of specific clicks based on a user-defined delay, ensuring that only intentional clicks are registered. The tool does not interact with or manipulate other processes.

### What VAC Scans For
Valve Anti-Cheat is designed to detect:
- **Known cheating software**: Programs that directly modify game memory, inject code, or manipulate game data.
- **Unauthorized third-party processes**: Applications that interfere with or manipulate the game in unintended ways.

### Why This Shouldn't Be an Issue
DoubleClickFix operates independently of any game and does not interact with game files, memory, or processes. It only modifies mouse input at the system level to address hardware issues, which is outside the scope of VAC detection.  

**Disclaimer**: While DoubleClickFix is unlikely to trigger VAC, always use third-party tools responsibly and at your own discretion. For official information, refer to Valve's [VAC documentation](https://help.steampowered.com/en/faqs/view/571A-97DA-70E9-FF74).

---

## 💡 Tips
- Check logs for detailed information on device IDs and other runtime details.
- Experiment with different delay settings to optimize for your specific hardware issues.

---

## 🤝 Contributions
Contributions are welcome! Feel free to open issues, submit pull requests, or suggest improvements via the [Issues tab](https://github.com/nenning/DoubleClickFix/issues).

---

## 📜 License
This project is distributed under the [MIT License](LICENSE.txt).
