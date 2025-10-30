Local AI Migration Plan v4.10.x — Voice Control & AI Features

Date: 2025-10-29
Author: Automated plan (draft) — local-first

Executive summary

This versioned plan (v4.10.x) updates the previous Google-focused design and adopts a local-first strategy for voice and AI features. The prototype will implement on-device STT, TTS and intent parsing using low-resource, cross-platform open-source components: Vosk + whisper.cpp for speech-to-text, llama.cpp (quantized small models) for intent/entity extraction, and Silero/Coqui or native platform TTS for audio output. A cloud fallback (Hugging Face / Replicate / later Google Vertex AI) is preserved behind a server-side proxy for heavy work, telemetry, or optional user-enabled features.

Goals

- Deliver accurate, low-latency on-device STT and intent parsing that runs on iPad M1 and typical mobile/desktop i7 integrated-GPU machines.
- Preserve privacy and offline functionality by keeping a fully functional local pipeline.
- Provide a straightforward server-side proxy for optional cloud fallbacks and staging to cloud providers later.
- Keep integration surface stable for the renderer by preserving existing IPC contracts.

Selected on-device stack (reasoned choice)

- STT (streaming + final pass):
  - Vosk: low CPU, streaming partials, mature Electron/Node/mobile bindings. Best for short-command, real-time partials.
  - whisper.cpp (ggml quantized): higher-accuracy final pass (mini/ small quantized models) for final transcript when device CPU permits.

- LLM / intent parsing:
  - llama.cpp (ggml quantized 3B recommended initially): small, fast on CPU with quantized models; ideal for intent/entity extraction locally.

- TTS:
  - Native platform TTS on iPad (AVSpeech/Apple Neural TTS) for best quality/efficiency.
  - Silero TTS (CPU-friendly) as cross-platform offline fallback for Electron/Windows.

- Cloud fallback (optional):
  - Hugging Face Inference / Replicate for rapid prototyping (free tier); later swap to Google Vertex AI/Cloud via server proxy in production.

Why this combination?

- Cross-platform: whisper.cpp, llama.cpp and Vosk have community ports and can be run on macOS/iOS, Windows, and Linux. Vosk offers robust streaming partials, and whisper.cpp + llama.cpp provide good quality for final passes and intent parsing.
- Resource-sensible: quantized ggml models and Silero are CPU-friendly and can run on M1 and i7 integrated GPUs without requiring a separate GPU server.
- Privacy/offline: everything above can run locally, preserving offline UX and user privacy, while also keeping the door open for cloud fallbacks.

Phases (v4.10.x) and tasks

Phase A — Audit & developer prep (0.5–1 day)
- Extract and centralize all prompt templates and JSON shapes from `src/features/smart-audio/utils/audioUtils.ts` and `voiceCommandProcessor.ts`.
- Identify and standardize the IPC contracts used by the renderer (`voice-recognition-partial`, `voice-recognition-result`, `voice-recognition-error`).

Phase B — Prototype local STT pipeline (2–4 days)
- Implement Vosk streaming integration in `electron/src/index.ts` as a new feature-flagged path (`USE_LOCAL_STT=true`). Wire Vosk partials to existing IPC events.
- Add whisper.cpp (mini) final-pass integration that can be invoked for higher accuracy after a phrase ends.
- Create small tests that validate partials and final text match expected contracts.

Phase C — Local LLM intent parsing prototype (2–3 days)
- Integrate llama.cpp as a local service (spawned process or bundled binary). Implement an endpoint `local-llm` in `server/` or a local IPC bridge that accepts { transcript, context } and returns { intent, parameters, replyText } in the same shape as planned in the migration contract.
- Build minimal prompt templates for entity extraction and intent classification (short, deterministic prompts to reduce token use and variability).

Phase D — TTS & UX polish (1–2 days)
- Use native TTS on macOS/iPad for production devices; use Silero TTS for Windows/Electron fallback.
- Wire TTS output to existing renderer playback code via secure IPC.

Phase E — Testing, telemetry & fallback (1–2 days)
- Add Playwright e2e tests for the happy path and a low-confidence fallback where local output triggers cloud fallback.
- Add simple telemetry counters to track confidence scores and fallback rates (persist locally and send anonymized metrics via proxy when allowed).

Phase F — Release v4.10.x & documentation (0.5–1 day)
- Update docs in `docs/` (this file + migration notes), add a short developer README for local builds and cross-compilation notes for iOS/iPad.

Integration contracts (kept stable)

- POST /api/transcribe (local or proxy)
  - Input: audio chunks (base64 PCM16LE / sampleRate / channels)
  - Output: { text: string, isFinal: boolean, confidence?: number, start?: number, end?: number }

- POST /api/llm (local LLM service or proxy)
  - Input: { transcript: string, userId?: string, sessionContext?: any, maxTokens?: number }
  - Output: { intent: string, parameters: Record<string, any>, replyText?: string, actions?: any[] }

- POST /api/tts (local/native or proxy)
  - Input: { text: string, voice?: string, audioEncoding?: string }
  - Output: { audio: string (base64) | binary, contentType: string }

Developer choices & implementation notes

- Electron integration:
  - Keep the same IPC event names and renderer hooks. Implement new local STT code paths in `electron/src/index.ts` behind `USE_LOCAL_STT` (env var) and `USE_GOOGLE_STT` may remain for cloud testing.
  - Prefer spawning small native binaries for whisper.cpp and llama.cpp to avoid complex native bindings. Use a simple JSON-over-stdin protocol and small wrapper scripts for cross-platform consistency.

- Packaging & binaries:
  - For whisper.cpp and llama.cpp, maintain a `vendor/` or `third_party/` folder with pre-built binaries for macOS (M1), Windows, and Linux for CI. Add scripts under `scripts/` to download or build these artifacts.

- Model choices & sizes:
  - Start with whisper.cpp (mini) and llama.cpp quantized 3B. Provide an easy config for upgrading to higher-quality models on dev machines.

- Tuning for latency:
  - For real-time command UX, send short chunks (500–1200ms) to Vosk for partials. Use whisper.cpp to process the entire utterance for final transcript.

Cost & operational notes

- Local-first reduces cloud spend and improves privacy. Cloud will be used only as a fallback or for heavy tasks.
- Keep server proxy budget controls in place for fallback cloud calls (rate limits, budgets, sampling).

Risks & mitigations

- Memory and latency on low-RAM devices: prefer 3B quantized models; test on iPad M1 and low-RAM i7 machines early.
- Model licensing: verify model license compatibility for distribution (Llama 2, others) before shipping binaries.

Next steps (recommended immediate work)

1. Implement local Vosk streaming in `electron/src/index.ts` behind `USE_LOCAL_STT` and wire partials/finals to existing IPC events.
2. Add wrapper to call whisper.cpp mini for final-pass transcripts when available.
3. Implement a simple local LLM service using llama.cpp that returns intent/parameters in the existing JSON shape.
4. Update docs and add a developer README explaining how to build/bundle the local binaries and set env vars for testing.

Files added/changed by this plan

- `docs/LOCAL_AI_MIGRATION_PLAN_v4.10.x.md` (this file) — active plan for local-first migration
- `docs/GOOGLE_AI_MIGRATION_PLAN.md` — now marked as deprecated and points to this file

Status

- This plan is a working draft for v4.10.x. The next concrete change will be adding Vosk and whisper.cpp integration in `electron/src/index.ts` and small local LLM service scaffolding in `server/`.

Appendix: Audit checklist (local-first specifics)

- Find code that emits or consumes `voice-recognition-*` IPC events and confirm contracts.
- Extract and normalize prompt templates from `src/features/smart-audio/utils/audioUtils.ts` for local LLM usage.
- Identify audio sample formats used by the mic capture (PCM16LE, sampleRate) and ensure wrappers convert correctly for Vosk / whisper.cpp.
- Add test audio samples (short commands and longer notes) in `test-assets/` for automated tests.

---

## Recent changes (v4.10.x)

- Worker pool: queued stream requests preserve stream semantics; per-request timeouts (WHISPER_REQUEST_TIMEOUT_MS) were added to avoid hung inflight requests.
- Main process: async temp-file cleanup for whisper final-pass, telemetry is now opt-in (`WHISPER_TELEMETRY_ENABLED='1'`), and a compact metrics IPC endpoint `whisper-pool-metrics-compact` was added for lightweight sampling.
- Playback preemption: renderer TTS now informs main process via `audio-playback-start` / `audio-playback-end` IPC so mic buffers are suppressed during playback to avoid feedback.
- Renderer: a sample camera capture component `src/features/camera/CameraCapture.tsx` and docs `docs/camera_capture.md` were added.

Refer to `docs/LOCAL_AI_IMPROVEMENTS_TRACKER_v4.10.x.md` for the full checklist, acceptance criteria, and PR tracking for remaining items.
