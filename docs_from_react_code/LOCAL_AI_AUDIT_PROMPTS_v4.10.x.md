Local AI Audit — Prompts, System Messages & JSON Shapes (v4.10.x)

Date: 2025-10-29
Author: Automated audit (artifact for Phase A)

Purpose
-------
This document centralizes all prompt templates, system messages, and expected JSON shapes discovered during the Phase A audit. It is intended as the canonical source when porting AI calls from OpenAI to local models (llama.cpp) or other inference engines (whisper.cpp, Vosk). Keep this file under version control and update whenever prompts or expected shapes change.

Files audited
-----------
- `src/features/smart-audio/utils/audioUtils.ts`
- `src/features/voice/services/voiceCommandProcessor.ts`

Summary of findings
-------------------
1) STT integration
  - `transcribeAudio()` uses OpenAI Whisper via `openai.audio.transcriptions.create()`.
  - Default response format used: `verbose_json` which contains `text`, `segments` (with start/end, avg_logprob), `language`, `duration`.
  - Local STT target shape (TranscriptionResult):
    {
      text: string,
      confidence: number,
      language?: string,
      duration?: number,
      words?: Array<{ word: string; start: number; end: number; confidence: number; }>
    }

2) Entity extraction prompt (extractEntitiesFromTranscription)
  - Context variants (inspection, component, general): short instruction strings used to seed the user prompt.
  - System message (explicit):
    "You are an AI assistant that extracts structured information from transcribed audio notes. Always respond with valid JSON."
  - User prompt template (pseudo):
    "<context prompt>\n\nTranscribed text: \"${transcription}\"\n\nPlease extract:\n1. Keywords (important terms)\n2. Entities (categorized information)\n3. Brief summary\n\nFormat as JSON with keys: keywords (array), entities (array of {type, value, confidence}), summary (string)"
  - Model parameters: `temperature: 0.3`, `max_tokens: 500`, model originally `gpt-3.5-turbo`.
  - Expected JSON output shape returned by `extractEntitiesFromTranscription()` (normalized):
    {
      keywords: string[],
      entities: Array<{ type: 'component'|'condition'|'measurement'|'location'|'issue' , value: string, confidence: number }>,
      summary: string
    }

3) Voice command suggestion prompt (generateVoiceCommandSuggestions)
  - System message: "You are a helpful assistant that suggests natural voice commands for an inspection app."
  - User prompt: includes `Current context: ${currentContext}` and `Recent commands: ${recentCommands.join(', ')}` and asks "Generate 5 helpful voice command suggestions...".
  - Model params: `temperature: 0.7`, `max_tokens: 200`.
  - Expected output: newline-separated suggestions; code parses top 5 lines.

4) Advanced note analysis (analyzeNoteWithAI)
  - System message: "You are an expert AI assistant for reserve study inspection applications. Analyze transcriptions thoroughly and provide structured JSON responses. Always respond with valid JSON only."
  - The user prompt contains a detailed JSON schema definition and strict guidelines. Model used in source: `gpt-4`, `temperature: 0.2`, `max_tokens: 2000`.
  - The expected response is a large structured JSON matching `AINoteAnalysis` interface (see below). The code runs a `JSON.parse(response)` and later calls `enhanceAnalysisResponse()` to add IDs and normalize fields.

5) AINoteAnalysis JSON schema (canonicalized from code)
  - Top-level keys: categories, entities, actionItems, followUpReminders, summary, searchTerms, metadata
  - categories: array of { primary: string, secondary?: string, confidence: number, tags: string[] }
  - entities: array of { type: 'person'|'date'|'location'|'issue'|'component'|'condition'|'measurement'|'priority', value: string, confidence: number, context?: string, position?: {start:number,end:number} }
  - actionItems: array of { id, description, assignee?, priority: 'low'|'medium'|'high'|'urgent', dueDate?, status, confidence, relatedEntities }
  - followUpReminders: array of similar structured objects
  - summary: { title, summary, keyPoints[], sentiment: 'positive'|'neutral'|'negative'|'urgent', wordCount, estimatedReadingTime }
  - metadata: { processingTime:number, aiModel: string, confidence:number }

6) AI-integration call sites and expected consumption shapes
  - `voiceCommandProcessor.enhanceWithAI()` dynamically imports `extractEntitiesFromTranscription()` and expects the returned object to have `entities` array where `e.type` values include 'component','condition','measurement','location','issue'. The code maps these into `ProcessedVoiceCommand.extractedEntities`.
  - `voiceCommandProcessor.processVoiceCommand()` expects `ProcessedVoiceCommand` objects with keys such as type (VoiceCommandType), confidence, componentName, notes, etc. See `voiceCommandProcessor.ts` for full `ProcessedVoiceCommand` interface.

7) IPC contracts (existing renderer ↔ main)
  - Commands (main exposes these via ipcMain handlers):
    - `voice-recognition-start` (handler) — called with options { language?, continuous? } → returns { success: boolean, error?: string }
    - `voice-recognition-stop` (handler) → returns { success: boolean }
    - `voice-recognition-available` (handler) → returns { sherpa: boolean, vosk?: boolean, mic: boolean, available: boolean }
    - `open-media-settings` (handler) → returns { success: boolean }
  - Events (main → renderer via event.sender.send):
    - `voice-recognition-partial` — payload: string (partial transcript)
    - `voice-recognition-result` — payload: string (final transcript)
    - `voice-recognition-error` — payload: string (error message)

  - Keep these event names stable — the renderer `useVoiceRecognition` hook listens for them.

8) Transcription and audio formats
  - Mic capture in `electron/src/index.ts` uses PCM16LE at 16000 Hz (mic library config). Vosk and Sherpa code expect PCM16LE or Float32 arrays accordingly.
  - When porting to other local engines (whisper.cpp, Vosk, Silero), ensure conversion helpers exist to produce the expected sample format.

Recommendations for porting to local models
-----------------------------------------
1. Centralize prompts and templates
   - Move all prompt templates and system message strings into a single module `src/features/smart-audio/prompts.ts` or `prompts.json`. This makes it easy to adapt to local LLMs (llama.cpp) and to update few-shot examples.

2. Enforce JSON output from local LLMs
   - Local models (llama.cpp) may not reliably return valid JSON. Use simple deterministic templates and small output validators: if JSON.parse fails, re-run with lower temperature or fallback to a rule-based extraction path.
   - Consider using a tiny post-processor or regex-based sanitizer to extract JSON blocks from model output.

3. Create typed contracts and unit tests
   - Add unit tests that feed example transcripts to the prompt templates and assert the parsed JSON matches the expected TypeScript interfaces (e.g., `ExtractEntitiesResult`, `AINoteAnalysis`).

4. Add a small set of few-shot examples for intent/entity extraction in `prompts.ts` to improve local LLM accuracy.

5. Add model-adapter layer
   - Implement a thin adapter interface for AI providers: { inferEntities(transcript, context): Promise<ExtractEntitiesResult>, analyzeNote(transcript, context): Promise<AINoteAnalysis>, suggestCommands(context): Promise<string[]> }.
   - This allows swapping `OpenAI` → `llama.cpp` → `HF` easily.

Immediate artifacts created
--------------------------
- `docs/LOCAL_AI_AUDIT_PROMPTS_v4.10.x.md` (this file) — canonical prompt & schema audit

Next steps (Phase A follow-ups)
-------------------------------
1. Move the prompt strings into `src/features/smart-audio/prompts.ts` and update calls in `audioUtils.ts` to import from that module.
2. Create an adapter implementation that maps the `extractEntitiesFromTranscription` prompt to a local LLM runner (llama.cpp wrapper) with JSON validation and retry fallback.
3. Add example transcripts and expected JSON outputs to `test-assets/ai-examples/` for unit tests.

End of audit artifact.
