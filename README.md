# EyeTribeGaze
A simple C# application that visualizes the user’s gaze point in real time using the EyeTribe eye tracker.

## Overview
The app connects to the EyeTribe Server and displays a small red circle on a transparent, click-through overlay window showing where the user is looking on screen.

## Requirements
- EyeTribe Eye Tracker
- EyeTribe Server (must be running)
- .NET Framework 4.6.1 or higher
- Windows OS

## Setup
1. **Install and start the EyeTribe Server**
   Download from the [EyeTribe SDK Insallers](https://github.com/eyetribe/sdk-installers/releases).

2. **Clone this repository**
   ```bash
   git clone https://github.com/yourusername/EyeTribeGaze.git
   cd EyeTribeGaze

3. **Open the solution in Visual Studio**

4. **Add the EyeTribe.ClientSdk dependency** (choose one of the following):
   - Recommended (NuGet)
     ```bash
     install-Package EyeTribe.ClientSdk
   - Alternative (manual reference):
     Add a reference to EyeTribe.ClientSdk.dll from the [EyeTribe C# SDK](https://github.com/EyeTribe/tet-csharp-client).

5. Build and run the project

## Usage
- A red circle indicates the current gaze position
- Press Esc to close the overlay window.

## Notes
- The overlay window is transparent and does **not** block mouse clicks.
- Repaint interval is set to approximately 60Hz.
- If the overlay fails to connect, ensure the EyeTribe Server is running.

## References
- [EyeTribe SDK Installers](https://github.com/EyeTribe/sdk-installers)
- [EyeTribe C# Client](https://github.com/EyeTribe/tet-csharp-client)
- [EyeTribe C# Samples](https://github.com/EyeTribe/tet-csharp-samples)

