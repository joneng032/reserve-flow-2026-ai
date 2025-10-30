# Reserve Flow — Master Implementation Plan (Native .NET MAUI)

This implementation plan adapts the original Capacitor roadmap to a native .NET MAUI delivery. It preserves the feature milestones and timelines but substitutes native platform steps, tooling, and CI/CD specifics for a .NET-based stack.

## Goals
- Deliver a native C#/MAUI application with feature parity for Smart Audio Notes, AI-assisted inspections, IoT device integration, and study-type frameworks.
- Prioritize a robust offline-first experience and explicit privacy controls.
- Provide a repeatable CI/CD pipeline producing MSIX/Store packages for Windows, APK/AAB for Android, and App Store-signed archives for iOS.

---

## Team & Roles (recommended)
- Platform Architect (MAUI + native platform experience)
- Lead Mobile Engineer (C#/MAUI)
- Voice & Audio Engineer (platform speech, local/offline STT)
- AI/ML Engineer (ONNX / ML models and integration)
- Integration Specialist (BLE/IoT, backend sync)
- QA Engineer (unit+UI+device farm)
- DevOps (CI/CD pipelines, code signing)

---

## Tooling & Infrastructure
- .NET SDK (match project target: .NET 8/9 depending on project constraints)
- Visual Studio 2022/2023 (Windows) and VS for Mac (macOS) with MAUI workloads
- GitHub Actions for multi-platform CI (windows-latest, macos-latest, ubuntu-latest)
- Device farm: BrowserStack / App Center / physical device matrix
- NuGet packages: CommunityToolkit.Maui, Microsoft.Maui.Essentials, Shiny/Plugin.BLE, Microsoft.ML / ONNXRuntime Mobile
- Database: SQLite with EF Core or sqlite-net

---

## Milestones & Sprints (mapped to original roadmap)

Sprint planning: two-week sprints. Each sprint produces working software and passes basic QA.

Phase 1 — Core Recording & Storage (Sprints 1-2)
- Tasks:
  - Project scaffolding: MAUI solution, common projects (App, Services, Data)
  - AppShell routing and initial Views/ViewModels
  - Implement IStorageService and local SQLite setup
  - Implement IAudioService: native audio recording + file persistence
  - Implement ILocationService: capture GPS at recording start
  - Basic UI for recording with waveform placeholder and playback
- Deliverables: working record/playback, DB storage, GPS tagging
- Tests: unit tests for data layer, integration test for storage

Phase 2 — Live Transcription & Autopopulation (Sprints 3-4)
- Tasks:
  - Integrate platform speech SDKs (iOS/Android/Windows) via ISpeechService
  - Live transcription UI, confidence display, segment mapping
  - Implement simple NLP parser for extraction of condition keywords ("crack", "rust"), as an IAIService stub
  - UI for suggestion accept/reject
- Deliverables: live transcription, auto-suggested fields, user validation flow

Phase 3 — Spatial & Map Features (Sprint 5)
- Tasks:
  - Integrate Maps control for MAUI (platform maps or community control)
  - Render audio pins with metadata and playback
  - Implement proximity search and trail visualization
- Deliverables: interactive map with pins and playback

Phase 4 — IoT Connectivity (Sprints 6-7)
- Tasks:
  - Integrate Shiny.BluetoothLE (or equivalent) and build DeviceManager abstraction
  - Support a laser distance measurer as a test device (parse GATT)
  - Auto-fill measurement fields from device data
- Deliverables: BLE device discovery, pairing, data capture and UI integration

Phase 5 — AI/ML Integration (Sprints 8-12)
- Tasks:
  - Evaluate ONNXRuntime Mobile vs Microsoft.ML for model hosting
  - Integrate models for photo classification and deficiency detection (start with lightweight model)
  - Implement feedback loop for user corrections (queued telemetry opt-in)
  - Add photo analysis UI with overlays
- Deliverables: on-device or hybrid ML inference for images, user feedback collection

Phase 6 — Export, Reporting & Study Framework (Sprints 13-14)
- Tasks:
  - Implement export pipelines: PDF (PDFSharp or native), XLSX (ClosedXML), CSV/JSON
  - Implement study-type configuration pages and report templates
  - Map data export format to WinReserve/PRA System requirements
- Deliverables: exportable reports and study templates

Phase 7 — Enterprise Features & Security (Sprints 15-16)
- Tasks:
  - Implement encryption for sensitive data (SQLCipher or platform keychain-backed encryption strategy)
  - Implement multi-tenant scoping and advanced permissions at API level
  - Audit logging and data retention controls
- Deliverables: encrypted local store, audit trail, enterprise config

Phase 8 — QA, Performance Tuning & Release (Sprints 17-20)
- Tasks:
  - Full device testing and performance tuning (battery, memory)
  - Accessibility and localization pass
  - CI/CD finalization, code signing and store submission process
- Deliverables: release-ready artifacts for all platforms

---

## CI/CD & Build Matrix
- GitHub Actions workflows:
  - Build+test on Windows (build Android and Windows packages), macOS runner for iOS builds
  - Publish artifacts (APK/AAB, MSIX, .ipa) to artifact storage
  - Optional: run UI tests on device farm
- Signing:
  - Android: keystore and Play Console setup
  - iOS: Apple Developer IDs, provisioning profiles, automatic signing from macOS runner
  - Windows: MSIX certificates for Store signing or installer

Suggested GH Action matrix: windows-latest (build Windows + Android), macos-latest (build iOS/macCatalyst), ubuntu-latest (unit test runner) with caching of NuGet and workloads.

---

## Testing Strategy
- Unit tests: xUnit for services and ViewModels; mock platform services
- Integration tests: in-memory DB tests and service composition tests
- UI tests: Appium, MAUI UITest (if available), or Playwright for Web preview only (not used for native run)
- Performance tests: automated tests for recording latency, transcription latency, memory use
- AI model validation: deterministic tests against labelled images/audio

---

## Risk & Mitigation
- Speech/STT accuracy: prefer platform-provided STT and provide local fallbacks (Vosk/ONNX) — provide user controls and fallback flows
- Background recording limitations (Android/iOS): design around foreground recording and use permitted background modes where allowed
- BLE fragmentation: maintain device compatibility list and robust error handling; implement profile-based device adapters
- ML model size/performance: keep models small and optional; provide cloud fallback for heavy inference

---

## Timeline (High-level)
(Assume 2-week sprints.)
- Sprints 1-2 (4 weeks): Core Recording & Storage
- Sprints 3-4 (4 weeks): Live Transcription & Autopopulation
- Sprint 5 (2 weeks): Maps & Spatial Features
- Sprints 6-7 (4 weeks): IoT Connectivity
- Sprints 8-12 (10 weeks): AI/ML Integration
- Sprints 13-14 (4 weeks): Export & Study Framework
- Sprints 15-16 (4 weeks): Enterprise Features & Security
- Sprints 17-20 (8 weeks): QA, tuning & release

(Adjust the schedule based on team size and parallelization opportunities.)

---

## Deliverables & Acceptance Criteria
- Prototype builds for Android, iOS (simulator), and Windows after Sprint 2
- Live transcription with accept/reject flow after Sprint 4
- Map overlay and audio pin playback after Sprint 5
- BLE measurement integration after Sprint 7
- Basic on-device ML photo analysis after Sprint 12
- Full export and study templates after Sprint 14
- Enterprise feature set and secure storage after Sprint 16
- Release artifacts and store submission steps completed by Sprint 20

---

## CI / Release Checklist (per release candidate)
- All unit and integration tests passing
- UI tests for critical flows (record, transcribe, play, export)
- Accessibility and localization checks
- Performance budgets verified on device farm
- Build artifacts signed and validated
- Release notes drafted and documentation updated

---

## Recommended Next Steps (immediate)
1. Initialize MAUI solution and repository topology. Create projects: Core (models/services), App (UI), Platform (iOS/Android/Windows specifics), Tests.
2. Implement basic DI and service contracts (IAudioService, IStorageService, ISpeechService, ILocationService).
3. Deliver Sprint 1 prototype: record/playback + DB storage + GPS metadata.
4. Setup GitHub Actions skeleton for multi-platform builds.

---

*Document created: Adapted for .NET MAUI — generated by project assistant.*
