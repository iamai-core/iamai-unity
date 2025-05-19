# IAMAI Unity Project Build Instructions

## Prerequisites
- **Unity 6** (Download via [Unity Hub](https://unity.com/download))

## Setup Instructions
### 1. Clone the Repository
```bash
git clone -b ExampleScene https://github.com/iamai-core/iamai-unity.git
```
### 2. Downlad & install Cuda
```
https://developer.nvidia.com/cuda-12-6-0-download-archive
```

### 2. Open the Project in Unity
- Open the root folder of cloned project from the **Unity Hub**.

### 3. Allow unsafe code
- Open Edit > Project settings > Player > check **Allow 'unsafe' code**.

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
- When prompted, select/create an empty folder for your build output.
- Click **Yes** on any confirmation pop-ups.

## Post-Build Setup
### 1. Add Required DLL Files
- In the build folder, navigate to:
```
[YourBuildFolder] > IAMAI Unity_Data > Plugins > x86_64
```
- Add the following `.dll` files to the `x86_64` folder:
  - `cublas64_12.dll`
  - `cublasLt64_12.dll`
  - `cudart64_12.dll`
    
- The dll files can be found in this directory
```
[Program Files] > NVIDIA GPU Computing Toolkit > CUDA > v12.6 > bin
```

### 2. Add the LLM Model
- In the same `x86_64` folder:
  - Create a new folder called **`models`**.
  - Add your LLM model file to this folder.

- Example:
  - Find an LLM model `.gguf` file from `https://huggingface.co/models` like our `llama-3.2-1b-instruct-q4_k_m.gguf`
  - This is a link to the model we used
```
https://huggingface.co/hugging-quants/Llama-3.2-1B-Instruct-Q4_K_M-GGUF/resolve/main/llama-3.2-1b-instruct-q4_k_m.gguf?download=true
```


### 3. Final Step: Zip the Build Folder
- Compress the entire build folder into a `.zip` file for distribution.

---

