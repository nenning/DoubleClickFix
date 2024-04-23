TODO better description texts
Contains a workaround for broken mouses that sometimes send a double-click instead of a single click.
You can fine-tune the minimal delay between two click in the user interface, which can be opened by double-clicking the Windows tray icon. 
You can also register the application to launch at Windows startup

Configuration
- app.config contains the minimum delay in milliseconds that has to be between two mouse clicks. If the time difference is less than this, the double-click is ignored and only a single click is considered. This value can be changed through the application.