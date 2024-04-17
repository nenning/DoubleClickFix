Contains a workaround for broken mouses that sometimes send a double-click instead of a single click.
You can fine-tune the minimal delay between two click in the user interface, which can be opened by double-clicking the Windows tray icon. 

Configuration
- app.config contains the minimum delay in milliseconds that has to be between two mouse clicks. If the time difference is less than this, the double-click is ignored and only a single click is considered.
 
Scripts
- batch files to register or unregister the application to launch at Windows startup.
