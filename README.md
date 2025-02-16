# Unity-Extended-Debug-Framework
An all-in-one extendable console, GUI menu &amp; on-screen widget framework for debugging Unity projects.

![image](https://github.com/user-attachments/assets/c0a5400e-547c-4310-86c5-fabc02917a2d)
*(note: Some commands shown in preview are specific to that project and do not come packaged)*

Created as a successor to:
https://github.com/Swagguy47/Unity-Debugging-Framework

## Notable Features:
- Works in most projects
> *Created to utilize Unity's built-in UI & swap between both input methods, meaning no external dependancies are required.*
- Customizable Commands
> *Execute any function in your scripts and pass-in unlimited parameters such as booleans, floats, strings or integers. All detected & added to registry on startup. Multiple ScriptableObects can be loaded from anywhere in the project's resources.*
- Console
> *Displays Unity Logs & offers a manual way of executing commands.*
- Console Autofill
> *Suggestions based on what you have typed will appear, pressing LEFT ALT will cycle options, pressing TAB will autofill based on selection.*
> *This also describes any extra command parameters.*
- GUI Panel
> *All commands get parsed into submenus & buttons to be easily accessed through the UI, maintaining full functionality.*
- On-Screen Widgets
> *Easily add custom UI prefabs to the screen at any moment which persist across all scenes with 10 different anchor points.*
- Release Stripping
> *All debugging features become fully inacessible upon making a non-development build. No changes need to be made by developers to ensure the end-user cannot access your tools.*
- Automatic Deployment
> *No prefabs, scripts, or setup is needed for this framework to function, simply import the package to your project and enter playmode. Works in any scene & persists through destructive loading.*
- Runtime command registering
> **[This aspect is still under active development, and may not function fully as intended]**
> *Create new commands during runtime and the framework automatically repopulates all fields with the new content, useful for dyanamic pages/command collections.*
