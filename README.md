# IAMAI Unity Project Build Instructions

## Prerequisites
- **Unity 6** (Download via [Unity Hub](https://unity.com/download))

## Setup Instructions
### 1. Clone the Repository
```bash
git clone -b ExampleScene https://github.com/iamai-core/iamai-unity.git
```

### 2. Open the Project in Unity
- Launch Unity.
- Open the cloned project from the **Unity Hub**.

### 3. Load the Demo Scene
- In Unity's **Content Browser**, go to the `Scenes` folder.
- Select the demo scene you want to load.

### 4. Install Required Plugin
- Open **Window > Package Manager**.
- If the required plugin is missing:
  - Click the **+ (Add)** button in the top-left corner.
  - Select **"Add package from git URL..."**.
  - Enter the following URL:

```
https://github.com/iamai-core/iamai-unity.git#Plugin
```

- Click **Install** or press **Enter**.

## Build Instructions
### 1. Open Build Settings
- Go to **File > Build Profiles...** (or press **Ctrl + Shift + B**).

### 2. Configure Scenes
- In the **Scenes in Build** section:
  - Click **Add Open Scenes** if your desired scene is not already added.
  - Remove any outdated or unused scenes.

### 3. Select Platform and Build
- In the **Platform** section, select **Windows**.
- Click **Build**.
- When prompted, select a folder for your build output.
- Click **Yes** on any confirmation pop-ups.

## Post-Build Setup
### 1. Add Required DLL Files
- In the build folder, navigate to:
```
[YourBuildFolder] > IAMAI Unity_Data > Plugins > x86_64
```
- Add the following `.dll` files to this folder:
  - `cublas64_12.dll`
  - `cublasLt64_12.dll`
  - `cudart64_12.dll`

### 2. Add the LLM Model
- In the same `x86_64` folder:
  - Create a new folder called **`models`**.
  - Add your LLM model file to this folder.

### 3. Final Step: Zip the Build Folder
- Compress the entire build folder into a `.zip` file for distribution.

---

