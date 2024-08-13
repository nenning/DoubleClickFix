# Overview
Contains a workaround for broken mouses that sometimes send a double-click instead of a single click.

You can fine-tune the minimal delay between two clicks in the user interface, which can be opened by double-clicking the Windows tray icon. 

You can also register the application to launch at Windows startup.

# Installation
Download the release, unzip and run the executable (you might have to install the .NET runtime first).

Or compile it yourself. 

# Configuration
Most of the configuration is done in the user interface and stored in the `DoubleClickFix.dll.config`. When downloading a new release, make sure to configure your settings again. Currently, there's no mechanism to automatically take over previous settings.

The application tries to allow all double-clicks from touchpads and touchscreens. It does so by ignoring clicks from the device with id 0. If this is not working for you, because your touch device has a different id, you could adjust the id in the `ignoredDevice` setting to what is displayed in the application logs. If this is also not working, use the checkbox in the UI to allow double-clicks with 0ms duration.