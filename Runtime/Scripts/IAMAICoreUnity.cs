using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace iamai_core_lib {
	public class AI : IDisposable {
		private IntPtr ctx;
		private IntPtr whisperCtx;
		private IntPtr iamaiDllHandle;
		private IntPtr whisperDllHandle;
		private bool disposed = false;
		private const string IAMAI_DLL_PATH = "iamai-core.dll";
		private const string Whisper_DLL_PATH = "whisper-interface.dll";
		private readonly object transcribeLock = new object();


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
		private delegate IntPtr FullInitDelegate([MarshalAs(UnmanagedType.LPStr)] string modelPath, int ctxSize, int maxTokens, int batchSize, int threads, int top_k, float top_p, float temperature, uint seed);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate bool GenerateDelegate(IntPtr context, [MarshalAs(UnmanagedType.LPStr)] string prompt,
			[MarshalAs(UnmanagedType.LPStr)] StringBuilder output, int maxLength);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void SetMaxTokensDelegate(IntPtr context, int maxTokens);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void SetPromptFormatDelegate(IntPtr context, [MarshalAs(UnmanagedType.LPStr)] string format);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void ClearPromptFormatDelegate(IntPtr context);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void SetThreadsDelegate(IntPtr context, int nThreads);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void SetBatchSizeDelegate(IntPtr context, int batchSize);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void FreeDelegate(IntPtr context);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr WhisperInitDelegate([MarshalAs(UnmanagedType.LPStr)] string modelPath, int threads);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void SetLanguageDelegate(IntPtr context, [MarshalAs(UnmanagedType.LPStr)] string language);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void SetTranslateDelegate(IntPtr context, bool translate);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr TranscribeDelegate(IntPtr context, IntPtr translate, int samples);
		// Function delegates
		private InitDelegate _init;
		private FullInitDelegate _fullInit;
		private WhisperInitDelegate _whisperInit;
		private GenerateDelegate _generate;
		private SetMaxTokensDelegate _setMaxTokens;
		private SetThreadsDelegate _whisperSetThreads;
		private SetPromptFormatDelegate _setPrompt;
		private ClearPromptFormatDelegate _clearPrompt;
		private FreeDelegate _free;
		private FreeDelegate _whisperfree;
		private SetLanguageDelegate _whisperSetLanguage;
		private SetTranslateDelegate _whisperSetTranslate;
		private TranscribeDelegate _whisperTranscribe;

		string m_iamaiModel = "";
		string m_whisperModel = "";

		int m_size = 2048;
		int m_iamaiTokens = 512;
		int m_iamaiBatch = 512;
		int m_iamaiThreads = 1;
		int m_whisperThreads = 1;
		int m_top_K = 50;
		float m_top_P = 0.9f;
		float m_Temperature = 0.5f;

		uint m_seed = 4294967295;

		#region Initialize
		public AI(string IamaiModel, string WhisperModel) {
			m_iamaiModel = IamaiModel;
			m_whisperModel = WhisperModel;
		}

		public AI(string modelName) {
			m_iamaiModel = modelName;
		}

		public AI(string modelName, int ctxSize = 8192, int batchSize = 1, int maxTokens = 512, int threads = 1, int top_k = 50, float top_p = 0.9f, float temperature = 0.5f, uint seed = 4294967295	) {
			m_iamaiModel = modelName;
			m_size = ctxSize;
			m_iamaiTokens = maxTokens;
			m_iamaiBatch = batchSize;
			m_iamaiThreads = threads;
			m_top_K = top_k;
			m_top_P = top_p;
			m_Temperature = temperature;
			m_seed = seed;
		}

		public AI(string WhisperModel, int threads) {
			m_whisperModel = WhisperModel;
			m_whisperThreads = threads;
		}

		public async Task Activate() {
			await Task.Run(() => {
				// Get the current directory and navigate to the DLL location
				string exePath = Directory.GetCurrentDirectory();
				string projectRoot = Path.Combine(exePath);
				#if UNITY_EDITOR
				string dllDirectory = Path.Combine(projectRoot, "Library\\PackageCache\\com.iamai-core.iamai-unity\\Runtime\\DLLs");
				#else
				string dllDirectory = Path.Combine(projectRoot, Application.dataPath, "Plugins\\x86_64");
				#endif
				string iamaiDllPath = Path.Combine(dllDirectory, IAMAI_DLL_PATH);
				string whisperDllPath = Path.Combine(dllDirectory, Whisper_DLL_PATH);
				string modelDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				#if UNITY_EDITOR
				string iamaiModelPath = Path.Combine(modelDir, "iamai", "models", m_iamaiModel);
				string whisperModelPath = Path.Combine(modelDir, "iamai", "models", m_whisperModel);
				#else
				string iamaiModelPath = Path.Combine(projectRoot, Application.dataPath, "Plugins\\x86_64\\models", m_iamaiModel);
				string whisperModelPath = Path.Combine(projectRoot, Application.dataPath, "Plugins\\x86_64\\models", m_whisperModel);
				#endif

				if (!Directory.Exists(dllDirectory)) {
					throw new DirectoryNotFoundException($"DLL directory not found: {dllDirectory}");
				}

				Console.WriteLine($"Loading DLL from: {iamaiDllPath}");
				SetDllDirectory(dllDirectory);

				// Load the DLL
				iamaiDllHandle = LoadLibrary(iamaiDllPath);
				whisperDllHandle = LoadLibrary(whisperDllPath);
				if (iamaiDllHandle == IntPtr.Zero) {
					int errorCode = Marshal.GetLastWin32Error();
					throw new InvalidOperationException($"Failed to load DLL. Error code: {errorCode}");
				}

				if (whisperDllHandle == IntPtr.Zero) {
					int errorCode = Marshal.GetLastWin32Error();
					throw new InvalidOperationException($"Failed to load DLL. Error code: {errorCode}");
				}
				if (!string.IsNullOrEmpty(m_iamaiModel)) {
					// Get function pointers
					_init = GetDelegate<InitDelegate>("Init", iamaiDllHandle);
					_fullInit = GetDelegate<FullInitDelegate>("FullInit", iamaiDllHandle);
					_generate = GetDelegate<GenerateDelegate>("Generate", iamaiDllHandle);
					_setMaxTokens = GetDelegate<SetMaxTokensDelegate>("SetMaxTokens", iamaiDllHandle);
					_free = GetDelegate<FreeDelegate>("Free", iamaiDllHandle);
					_setPrompt = GetDelegate<SetPromptFormatDelegate>("SetPromptFormat", iamaiDllHandle);
					_clearPrompt = GetDelegate<ClearPromptFormatDelegate>("ClearPromptFormat", iamaiDllHandle);
				}

				if (!string.IsNullOrEmpty(m_whisperModel)) {
					//get whisper function pointers
					_whisperInit = GetDelegate<WhisperInitDelegate>("Init", whisperDllHandle);
					_whisperfree = GetDelegate<FreeDelegate>("Free", whisperDllHandle);
					_whisperSetThreads = GetDelegate<SetThreadsDelegate>("setThreads", whisperDllHandle);
					_whisperSetLanguage = GetDelegate<SetLanguageDelegate>("setLanguage", whisperDllHandle);
					_whisperSetTranslate = GetDelegate<SetTranslateDelegate>("setTranslate", whisperDllHandle);
					_whisperTranscribe = GetDelegate<TranscribeDelegate>("Transcrible", whisperDllHandle);
				}
				// Initialize the model
				if (!string.IsNullOrEmpty(m_iamaiModel)) {
					if (m_size > 0) {
						ctx = _fullInit(iamaiModelPath, m_size, m_iamaiTokens, m_iamaiBatch, m_iamaiThreads, m_top_K, m_top_P, m_Temperature, m_seed);
					} else {
						ctx = _init(iamaiModelPath);
					}
					if (ctx == IntPtr.Zero) {
						throw new InvalidOperationException("Failed to initialize model");
					}
				}
				if (!string.IsNullOrEmpty(m_whisperModel)) {
					whisperCtx = _whisperInit(whisperModelPath, m_whisperThreads);
				}
				disposed = false;
			});
		}

		private T GetDelegate<T>(string procName, IntPtr dllHandle) where T : Delegate {
			IntPtr procAddress = GetProcAddress(dllHandle, procName);
			if (procAddress == IntPtr.Zero) {
				int errorCode = Marshal.GetLastWin32Error();
				throw new InvalidOperationException(
					$"Failed to get proc address for {procName}. Error code: {errorCode}");
			}
			return Marshal.GetDelegateForFunctionPointer<T>(procAddress);
		}

		#endregion

		#region llama functions
		public async Task<string> GenerateAsync(string prompt, int maxLength = 4096) {
			return await Task.Run(() => {
				return Generate(prompt, maxLength);
			});
		}

		public string Generate(string prompt, int maxLength = 4096) {
			if (ctx == IntPtr.Zero) throw new InvalidOperationException("AI context not initialized.");
    		if (string.IsNullOrEmpty(prompt)) throw new ArgumentNullException(nameof(prompt));
			StringBuilder output = new StringBuilder(maxLength);
			bool success = _generate(ctx, prompt, output, maxLength);

			if (!success) {
				throw new InvalidOperationException("Generation failed");
			}

			return output.ToString();
		}

		public void SetMaxTokens(int maxTokens) {
			_setMaxTokens(ctx, maxTokens);
		}

		public void setPromptFormat(string promptFormat) {
			_setPrompt(ctx, promptFormat);
		}

		public void clearPromptFormat(){
			_clearPrompt(ctx);
		}
		#endregion

		#region whisper functions 
		//this region is for Whisper AI/Transcribe audio files
		//set threads
		public void setWhisperThreads(int threads) {
			_whisperSetThreads(whisperCtx, threads);
		}
		//set language
		public void setWhisperLanguage(string language) {
			_whisperSetLanguage(whisperCtx, language);
		}
		//set translate
		public void setWhisperTranslate(bool translate) {
			_whisperSetTranslate(whisperCtx, translate);
		}
		public async Task<string> WhisperAsyncTranscribe(float[] data, int samples) {
			return await Task.Run(() => {
				return WhisperTranscribe(data, samples);
			});
		}
		//transcribe pcm32 
		public string WhisperTranscribe(float[] data, int samples) {
			if (whisperCtx == IntPtr.Zero || data == null || samples <= 0) {
				Debug.LogError("Invalid params passed to _transcribe");
				return "";
			}

			lock (transcribeLock) {
				unsafe {
					fixed (float* dataPtr = data) {
						IntPtr resultPtr = _whisperTranscribe(whisperCtx, (IntPtr)dataPtr, samples);
						return resultPtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(resultPtr) : "";
					}
				}
			}
		}
		// some way to get audio based on iamaivoiceinput files
		#endregion
		protected virtual void Dispose(bool disposing) {
			if (!disposed) {
				if (ctx != IntPtr.Zero) {
					_free(ctx);
					ctx = IntPtr.Zero;
				}
				if (whisperCtx != IntPtr.Zero) {
					_whisperfree(ctx);
					whisperCtx = IntPtr.Zero;
				}
				if (iamaiDllHandle != IntPtr.Zero) {
					FreeLibrary(iamaiDllHandle);
					iamaiDllHandle = IntPtr.Zero;
				}
				if (whisperDllHandle != IntPtr.Zero) {
					FreeLibrary(whisperDllHandle);
					whisperDllHandle = IntPtr.Zero;
				}
				disposed = true;
			}
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~AI() {
			Dispose(false);
		}
	}

}
