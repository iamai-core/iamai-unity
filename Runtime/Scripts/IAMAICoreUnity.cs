using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;


namespace iamai_core_lib
{
    public class AI : IDisposable
    {
        private IntPtr ctx;
        private IntPtr dllHandle;
        private bool disposed = false;
        private const string DLL_PATH = "iamai-core.dll";

        // Win32 API functions
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        // Function delegate types
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr InitDelegate([MarshalAs(UnmanagedType.LPStr)] string modelPath);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool GenerateDelegate(IntPtr context, [MarshalAs(UnmanagedType.LPStr)] string prompt,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder output, int maxLength);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetMaxTokensDelegate(IntPtr context, int maxTokens);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetThreadsDelegate(IntPtr context, int nThreads);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetBatchSizeDelegate(IntPtr context, int batchSize);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void FreeDelegate(IntPtr context);

        // Function delegates
        private InitDelegate _init;
        private GenerateDelegate _generate;
        private SetMaxTokensDelegate _setMaxTokens;
        private SetThreadsDelegate _setThreads;
        private SetBatchSizeDelegate _setBatchSize;
        private FreeDelegate _free;

        public AI(string modelName)
        {
            // Get the current directory and navigate to the DLL location
            string exePath = Directory.GetCurrentDirectory();
            string projectRoot = Path.Combine(exePath);
            string dllDirectory = Path.Combine(projectRoot, "Library\\PackageCache\\com.iamai-core.iamai-unity\\Runtime\\DLLs");
            string dllPath = Path.Combine(dllDirectory, DLL_PATH);
            string modelDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string modelPath = Path.Combine(modelDir, "iamai", "models", modelName);

            if (!Directory.Exists(dllDirectory))
            {
                throw new DirectoryNotFoundException($"DLL directory not found: {dllDirectory}");
            }

            Console.WriteLine($"Loading DLL from: {dllPath}");
            SetDllDirectory(dllDirectory);

            // Load the DLL
            dllHandle = LoadLibrary(dllPath);
            if (dllHandle == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new InvalidOperationException($"Failed to load DLL. Error code: {errorCode}");
            }

            // Get function pointers
            _init = GetDelegate<InitDelegate>("Init");
            _generate = GetDelegate<GenerateDelegate>("Generate");
            _setMaxTokens = GetDelegate<SetMaxTokensDelegate>("SetMaxTokens");
            _setThreads = GetDelegate<SetThreadsDelegate>("SetThreads");
            _setBatchSize = GetDelegate<SetBatchSizeDelegate>("SetBatchSize");
            _free = GetDelegate<FreeDelegate>("Free");

            // Initialize the model
            ctx = _init(modelPath);
            if (ctx == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to initialize model");
            }
        }

        private T GetDelegate<T>(string procName) where T : Delegate
        {
            IntPtr procAddress = GetProcAddress(dllHandle, procName);
            if (procAddress == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new InvalidOperationException(
                    $"Failed to get proc address for {procName}. Error code: {errorCode}");
            }
            return Marshal.GetDelegateForFunctionPointer<T>(procAddress);
        }

        public string Generate(string prompt, int maxLength = 4096)
        {
            StringBuilder output = new StringBuilder(maxLength);
            bool success = _generate(ctx, prompt, output, maxLength);

            if (!success)
            {
                throw new InvalidOperationException("Generation failed");
            }

            return output.ToString();
        }

        public void SetMaxTokens(int maxTokens)
        {
            _setMaxTokens(ctx, maxTokens);
        }

        public void SetThreads(int nThreads)
        {
            _setThreads(ctx, nThreads);
        }

        public void SetBatchSize(int batchSize)
        {
            _setBatchSize(ctx, batchSize);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (ctx != IntPtr.Zero)
                {
                    _free(ctx);
                    ctx = IntPtr.Zero;
                }
                if (dllHandle != IntPtr.Zero)
                {
                    FreeLibrary(dllHandle);
                    dllHandle = IntPtr.Zero;
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AI()
        {
            Dispose(false);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var ai = new AI(@"Llama-3.2-1B-Instruct-Q4_K_M.gguf"))
                {
                    ai.SetMaxTokens(256);
                    string response = ai.Generate("Tell me a story about a robot.");
                    Console.WriteLine(response);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
