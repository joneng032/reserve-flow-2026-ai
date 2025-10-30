# Reserve Flow — Migration Map (React/Capacitor -> .NET MAUI)

This document is an automated inventory and high-level mapping of the existing React/Capacitor codebase (found under `F:\github\reserve_flow_2026`) into planned .NET MAUI artifacts. Use this as the canonical file-by-file migration checklist. Each entry has: source path, purpose, and recommended MAUI target (page/service/resource).

---

## How to read this file
- Source: path inside the React/Capacitor project
- Purpose: short description derived from code/docs
- MAUI mapping: recommended C#/.NET MAUI target (XAML Page, ViewModel, IService, Resource)
- Priority: (P0,P1,P2) — P0 = port early (core flows), P1 = port soon, P2 = later/optional

---

## Top-level priorities (recommended)
- P0: Smart Audio (recording, live transcription), Data model + local DB, Record/playback UI
- P0: Voice flows (useVoiceRecognition, ContinuousVoiceMode), IAudioService + ISpeechService
- P0: Storage/sync (db.ts, syncService) -> IStorageService + ISyncService
- P1: Maps & spatial features (MapView, map pins)
- P1: IoT BLE device manager (deviceManager, bluetoothService)
- P2: Analytics dashboards, advanced visualizations and rarely used admin pages

---

## Files mapped (selected, grouped)

### App shell & entry
- `src/main.tsx` — entry for React app. MAUI: `App.xaml` + `MauiProgram.cs` (already present). Priority: P0
- `src/App.tsx`, `src/components/AppContent.tsx` — app-level layout and routing. MAUI: `AppShell.xaml` and Shell routes + `MainPage.xaml` (map content into Shell sections). Priority: P0

### Pages (high value)
- `src/pages/FieldInspection.tsx` — Field inspection page (voice + media + map). MAUI: `Pages/FieldInspectionPage.xaml` + `FieldInspectionViewModel`. P0
- `src/pages/ProjectDetail.tsx` — Project detail screen. MAUI: `Pages/ProjectDetailPage.xaml` + `ProjectDetailViewModel`. P1
- `src/pages/Dashboard.tsx` — Dashboard overview. MAUI: `Pages/DashboardPage.xaml` + `DashboardViewModel`. P1
- `src/pages/Login.tsx` — Auth UI. MAUI: `Pages/LoginPage.xaml` + `AuthViewModel` (use secure storage). P1

### Smart Audio & Voice flows (P0)
- `src/features/smart-audio/SmartAudioRecorder.tsx` — recording UI and logic. MAUI: `Views/RecordingPage.xaml` + `RecordingViewModel`; implement `IAudioService` for platform recording and file persistence.
- `src/features/smart-audio/hooks/useAudioRecording.*` — recording hooks/tests. MAUI: move to `IAudioService` + `RecordingViewModel` tests. P0
- `src/features/voice/hooks/useVoiceRecognition.ts` — voice recognition hook. MAUI: `ISpeechService` abstraction using platform STT or ONNX/Vosk fallback. P0
- `src/features/voice/components/VoiceCommandButton.tsx` — voice action UI. MAUI: Button + command bound to ViewModel.

### Services & hooks -> MAUI services
- `src/services/syncService.ts` — data sync logic. MAUI: `ISyncService` using HttpClient and background sync worker; queue writes to SQLite. P0
- `src/services/authService.ts` — authentication. MAUI: `IAuthService` + secure storage (Keychain/Keystore). P1
- `src/services/projectsService.ts` / `componentsService.ts` — domain services. MAUI: `IProjectsService`, `IComponentsService` (DI). P1
- Hooks under `src/hooks/*` (useAuth, useCamera, useProjects, useSync) — map to service interfaces and ViewModels. P0-P1

### IoT, BLE & Device Integration (P1)
- `src/features/iot/deviceManager.ts` / `bluetoothService.ts` / `useIoTDevices.ts` — BLE device discovery, GATT parsing. MAUI: `IIoTService` + platform implementations (Shiny.BluetoothLE or Plugin.BLE). Provide `DeviceManager` abstraction. P1

### Maps & Spatial (P1)
- `src/components/MapView.tsx` — map component rendering pins and audio playback. MAUI: integrate `Maps` control (platform map or community toolkit) + `MapViewModel` for pins and playback. P1

### Media, Camera, Image handling (P0-P1)
- `src/components/CameraCapture.tsx`, `src/components/MediaDisplay.tsx`, `src/components/ImageAnnotator.tsx` — camera/photo UI and annotator. MAUI: `ICameraService` (platform camera wrappers) and `ImageAnnotationView`. Use SkiaSharp or native overlays for annotations. P1

### UI components & shared bits
- `src/components/*` (Button, Input, Modal, Footer, Header, etc.) — reusable UI pieces. MAUI: convert to XAML `UserControl`s or templates and bind to styles. Map design tokens to `Resources/Styles/Colors.xaml` & `Styles.xaml`. P1

### Data layer
- `src/db/db.ts` — local DB models and persistence. MAUI: `EF Core` or `sqlite-net` models and migrations; map TypeScript types to C# DTOs/Entities. P0
- `src/types/index.ts` — data shapes. Map to C# models in `Core/Models`. P0

### AI/ML & WebLLM/Whisper (P1-P2)
- `src/features/smart-audio/webLLM*`, `webllm-worker.js`, `webLLMAudioUtils.ts`, `whisperFinalPass.ts` — ML glue using WebLLM/TensorflowJS/Whisper. MAUI: evaluate ONNXRuntime Mobile or local whisper bindings (Vosk/whisper.cpp) and create `IAIService`. Provide cloud fallback for heavy models. P1

### Scripts & build integration
- `scripts/*` — build, model download, performance tooling. These are operational scripts. MAUI: adapt necessary model download scripts to equivalent CI steps or dotnet tools. P2

### Capacitor / platform configs
- `android/.../capacitor.config.json`, `ios/.../capacitor.config.json`, `electron/build/` & `public/` — platform packaging configs and service workers. MAUI: replace with platform build steps and platform-specific entitlements/provisioning. P0 (for packaging)

### Electron / Desktop specifics
- `electron/` directory (preload scripts, build pipeline). MAUI: Windows/macCatalyst packaging replaces Electron for desktop flows. Keep logic for some desktop-only glue but port UI to MAUI. P1

### Tests / E2E / CI
- `e2e/` Playwright specs; `src/test` & `__tests__` (Vitest). MAUI: move unit tests to xUnit / MSTest and E2E to Appium / device farm Playwright equivalents. CI workflow already wired. P1

### Assets & vendor models
- `public/assets/webllm-*`, `dist/assets/*` etc. Large vendor JS/wasm models will be either replaced with ONNX models or kept as download-only vendor files for any hybrid server. Plan model hosting and downloads (CI/prepare-vendor-models). P1

### Docs & specifications
- `docs/` — many architecture docs; we've copied/created MAUI docs under `docs_from_maui/`. Keep these as migration guidance. P0

---

## Example mapping (detailed) — Smart Audio Recorder (priority P0)
- Source: `src/features/smart-audio/SmartAudioRecorder.tsx`
  - Purpose: record audio, show waveform, live STT, create Note records
  - MAUI target:
    - `Views/RecordingPage.xaml` (XAML UI with waveform placeholder, record/play buttons)
    - `ViewModels/RecordingViewModel.cs` (binds to IAudioService, ISpeechService, ILocationService, IStorageService)
    - `Services/IAudioService.cs` and platform impls (`Platforms/Android/AudioRecorder.cs`, `Platforms/iOS/AudioRecorder.cs`, `Services/AudioService.cs`)
    - Data model: `Models/GeotaggedAudioAttachment.cs` (store path, duration, transcription, GPS)

## Quick migration checklist (first sprint)
1. Implement `IAudioService` + simple platform record/playback tests (Android/Windows). (3 days)
2. Port `SmartAudioRecorder` UI to `RecordingPage.xaml` + `RecordingViewModel`. (3 days)
3. Implement `ISpeechService` wrapper using platform STT for live transcription. (2 days)
4. Map `db.ts` types to C# Entities and setup SQLite with EF Core. (2 days)
5. Wire DI in `MauiProgram.cs`. Add simple unit tests (xUnit). (2 days)

---

## Notes / Risks
- WebLLM / Whisper JS workers are large and depend on browser/wasm. Plan for ONNX or server fallbacks. (Medium risk)
- BLE fragmentation: need device adapters and robust error handling. (Medium risk)
- UI parity: Tailwind utilities don't map one-to-one — prefer design token approach. (Low risk)

---

## Next actions I will take (if you confirm)
1. Generate `docs_from_maui/MIGRATION_MAP.md` (this file) — done.
2. Create a small working prototype: `RecordingPage.xaml` + `RecordingViewModel` + `IAudioService` stub — I can scaffold these in the repo and push. (Ask to proceed)
3. Produce a sprint backlog (tickets) from P0 items and create a `MIGRATION_BOARD.md` or GitHub issues. (Ask to proceed)

---

If you want me to proceed with scaffolding the Recording flow now, reply: "Scaffold Recording flow now" and I'll create the MAUI views/services and commit them.
