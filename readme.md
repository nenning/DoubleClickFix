Contains a workaround for broken mouses that sometimes send a double-click instead of a single click.

Parameters
 -i (or -interactive): interactive mode to display the timespan between two mouse clicks
 
Configuration
- app.config contains the minimum delay in milliseconds that has to be between two mouse clicks. Of the time difference is less than this, the double-click is ignored and only a single click is considered.
 
Scripts
- registry files to register or unregister the application to launch at Windows startup.
