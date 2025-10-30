# Reserve Flow — Master Specifications (Native .NET MAUI)

## Executive Summary

This document adapts the original React/Capacitor master specifications into a native, cross-platform .NET MAUI (.NET 8/9) implementation. The goal: preserve all product functionality (Smart Audio Notes, AI-augmented inspection workflows, IoT integrations, export formats and study frameworks) while implementing them as native C#/MAUI components (MVVM, Shell navigation, DI, platform APIs). No hybrid/web wrapper will be used.

Key principles:
- Native performance and platform-consistent UX using .NET MAUI and platform toolkits
- Offline-first local storage with encrypted SQLite and file-based media
- Privacy-first design: explicit permissions, local processing where feasible
- Modular architecture: UI (Views/ViewModels), Services, Data layer, Platform integrations
- Testable, maintainable code using DI and clean separation of concerns

---

## Strategic Competitive Advantages (native)
- Native, single C# codebase with platform-optimized renderers for iOS/iPadOS, Android, Windows and macOS (Mac Catalyst)
- Full offline capability using SQLite (EF Core or sqlite-net) and file storage
- Native APIs for microphone, camera, location, Bluetooth and sensors for robust field performance
- Easier native access to on-device speech/ML runtimes and more predictable performance vs web-based ML
- Security and data sovereignty via platform-secure storage and optional local-only processing

---

## Scoping & Non-Goals
- Scope: Replace the React/Capacitor implementation with functionally equivalent native MAUI implementation. Maintain feature parity for Smart Audio Notes, AI-assisted workflows, export formats, IoT connectivity and study-type framework.
- Non-goal: Do not use a hybrid approach (no WebView or Capacitor/Capacitor-like wrappers). No runtime JS UI layer.

---

## High-Level Architecture

Contract (short):
- Inputs: user interactions (voice, touch), device sensors (GPS, camera, BLE), media files, offline edits
- Outputs: project data, exports (PDF/XLSX/JSON), reports, analytics metrics
- Error modes: permission denial, offline conflicts, model unavailability — all must degrade gracefully
- Success: parity with feature set and reliable native performance across target platforms

Core layers:
- Presentation: .NET MAUI Pages + MVVM ViewModels (CommunityToolkit.Mvvm preferred)
- Navigation: AppShell (`AppShell.xaml`) for routes and deep links
- Services: interface-defined services (IAudioService, ISpeechService, ILocationService, IStorageService, IIoTService, IAIService)
- Data: SQLite local DB with EF Core (or sqlite-net) + local file store for media in FileSystem.AppDataDirectory
- Platform: Platform-specific implementations via partial classes or dependency injection (e.g., AndroidSpeechService, iOSSpeechService, WindowsSpeechService)
- CI/CD: GitHub Actions building for Windows, macOS (for iOS) and Android; code-signing and packaging

Dependency and Open-Source choices (examples):
- CommunityToolkit.Maui, CommunityToolkit.Mvvm
- Microsoft.Maui.Essentials (device APIs), MAUI.Maps or Maps control
- SQLite: Microsoft.Data.Sqlite + EF Core or sqlite-net 
- BLE: Shiny.BluetoothLE or Plugin.BLE (MAUI compatible)
- On-device ML: Microsoft.ML / ONNX Runtime Mobile or native platform speech SDKs (Vosk for offline STT where needed)
- Audio handling: Plugin.Maui.Audio or platform native audio recorders for max control

---

## Feature Mapping (key items from React spec -> MAUI)

Smart Audio Notes (geotagged audio)
- Recording: native audio APIs (AVAudioSession/AVAudioRecorder on iOS, AudioRecord on Android, WASAPI/MediaCapture on Windows)
- Metadata: capture GPS from platform location API at recording start; store in DB with media reference
- Transcription: options:
  - Use on-device speech APIs (Apple Speech, Android SpeechRecognizer) for live transcription
  - For offline local models, integrate Vosk or ONNXRuntime-based STT
  - For cloud option, integrate Azure Speech (optional and clearly opt-in)
- UI: waveform, live transcription overlays, confidence indicators, accept/edit suggestions

AI/ML (photo & audio analysis)
- Photo-based recognition: run ONNX models with ONNX Runtime Mobile or Microsoft.ML on-device where performance allows; otherwise cloud fallback
- Defect detection: local lightweight models for common issues; heavier models run on server if needed
- Feedback loop: user accept/reject triggers queued telemetry for optional model improvement pipelines (explicit opt-in)

IoT & device integration
- BLE integration via Shiny or Plugin.BLE; support laser measurers and thermal cameras by parsing GATT characteristics
- Provide DeviceManager abstraction and platform implementations

Maps & spatial features
- Use MAUI maps (or platform map controls) to render pins and trails; pins play audio with transcription overlay

Data storage and Sync
- Local DB (SQLite), media files on file system; sync engine (background sync service) that uses incremental sync APIs with conflict resolution
- Export formats generated natively (PDF via PDFSharp or native OS PDF APIs, Excel via ClosedXML, CSV/JSON)

Security & Privacy
- Permissions: platform permission prompts and clear in-app settings
- Encrypted local DB (SQLCipher or platform-backed encrypted storage) for sensitive data
- GDPR/CCPA controls: data retention controls and audit logs

Accessibility & Internationalization
- Full accessibility support via platform accessibility APIs, screen readers (TalkBack, VoiceOver), large text, and contrast modes
- Localization via RESX and MAUI localization patterns

Testing & QA
- Unit tests: xUnit or NUnit for ViewModels and services
- Integration tests: in-memory DB tests, mock platform services
- UI tests: .NET MAUI UI Test (or Appium/WinAppDriver) for cross-platform UI verification
- Test data sets for performance/AI validation

Performance targets (native adaptations)
- Recording start <300ms
- GPS acquisition <3s best-case
- Live transcription latency <1s (dependent on provider)
- Media storage efficient: target <50MB/hour compressed
- Battery impact minimal: ensure recording/ML tasks scheduled and throttled

---

## Data Models (C#-style summaries)

GeotaggedAudioAttachment (C# DTO):
- Id (Guid), ProjectId, FilePath, Duration (TimeSpan), SampleRate, Channels, DeviceInfo, RecordedAt (DateTimeOffset)
- Gps: Latitude, Longitude, Accuracy, Altitude, Timestamp
- Transcription: Text, Confidence (double), Language, Segments (list with Start/End)
- ExtractedNotes: collection of { Field, Value, Confidence, UserAccepted }

ProjectMetadata, StudyType and EffortLevel map directly to C# models with strong typing, validation attributes, and mapping to DB tables.

---

## Platform-Specific Considerations

iOS/iPadOS
- Entitlements for speech recognition, background audio, location
- App Store packaging, background modes (location, audio)
- Use Apple Speech for best on-device accuracy when available

Android
- Runtime permissions for RECORD_AUDIO, ACCESS_FINE_LOCATION, BLUETOOTH, BLUETOOTH_CONNECT
- Use SpeechRecognizer or integrate Vosk or ONNX for offline
- Handle background-recording limitations depending on OS version

Windows
- Use Windows.Media APIs for audio capture and speech
- Package as MSIX / Store app or traditional installer depending on distribution strategy

macOS (Catalyst)
- Use Mac Catalyst platform APIs for audio and location where applicable

---

## Non-Functional Requirements

Reliability
- Offline-first core features with robust sync
- Conflict resolution rules and merge strategies

Security
- Encrypt sensitive storage, secure file permissions, obfuscate sensitive logic where required
- Secure telemetry and ensure opt-in for any model telemetry

Maintainability
- Single solution with platform projects via MAUI; clean service interfaces and unit coverage
- Documented public interfaces and DI registrations

Scalability
- Sync system designed for incremental batches; backend supports bulk import/export

Observability
- Structured logging via Microsoft.Extensions.Logging
- Crash/telemetry via App Center, Sentry or other provider (opt-in)

---

## Acceptance Criteria
- Feature parity with React master spec for all listed core features (recording, transcription, maps, exports, study types)
- App works fully offline for core recording, storage and export
- Native platform performance targets met on target device list
- Privacy controls and permission flows implemented and testable
- CI builds produce installable artifacts for Windows, Android, and iOS

---

## Appendix: Implementation notes & design patterns
- Use MVVM and partial classes for platform hooks
- Use DI (ServiceCollection) in MauiProgram.cs to register interfaces and implementations
- Keep heavy ML processing in background tasks (WorkManager on Android, BGTasks on iOS where allowed) or offload to server if not feasible on-device
- Prefer platform speech APIs first, then local model fallbacks; cloud options must be explicit opt-in


---

*Document created: Adapted for .NET MAUI — generated by project assistant.*
