# 🖱️ DoubleClickFix

A lightweight solution for mitigating double-click issues caused by malfunctioning mice.  

This tool ensures smoother operation by filtering unintended double-click events, allowing you to define the minimal delay between valid clicks directly from an intuitive user interface.

**Download from [Microsoft Store](https://apps.microsoft.com/detail/9PDGM7NL2FF2?hl=en-us&gl=CH&ocid=pdpshare)!**

![logo](DoubleClickFix/app.ico)

---

## ✨ Features
- **Customizable Delay**: Adjust the minimal delay between two clicks via a user-friendly interface.
- **Windows Tray Integration**: Double-click the tray icon to open the settings UI.
- **Startup Option**: Easily register the application to launch with Windows. Note: the app tries to do this automatically.

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
- Settings can be adjusted in the UI.

### Handling Touch Devices
- Double-clicks from touchpads or touchscreens are allowed by default. 
- If you have trouble with this, enable the **Allow 0ms Double-Click Duration** option in the UI.

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

**Disclaimer**: While DoubleClickFix is very unlikely to trigger VAC, always use third-party tools responsibly and at your own discretion. For official information, refer to Valve's [VAC documentation](https://help.steampowered.com/en/faqs/view/571A-97DA-70E9-FF74).

---

## 💡 Tips
- Check the logs in the UI for detailed information on the elapsed time between your mouse clicks and filterd out double-clicks.
- Experiment with different delay settings to optimize for your specific hardware issues and click style.

---

## 🤝 Contributions
Contributions are welcome! Feel free to open issues, submit pull requests, or suggest improvements via the [Issues tab](https://github.com/nenning/DoubleClickFix/issues).

---

## 📜 License
This project is distributed under the [MIT License](LICENSE.txt).
