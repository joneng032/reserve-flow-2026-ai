# Installing a Vosk model for local STT testing

This project includes a feature-flagged local STT path (Vosk) implemented in `electron/src/index.ts`. To test the local path you must install a Vosk model into a folder the Electron process can read.

This document describes a safe, repeatable way to download and install a Vosk model into `vendor/vosk-model` and how to place it for Electron runtime use.

## Recommended model

For development and testing the English "small" model is a good starting point:

- Example (English small):
  - https://alphacephei.com/vosk/models/vosk-model-small-en-us-0.15.zip

If the URL changes, browse the official models index: https://alphacephei.com/vosk/models

## Provided helper scripts

Two helper scripts are available in the `scripts/` folder:

- PowerShell (Windows): `scripts/install-vosk-model.ps1`
- POSIX shell (macOS / Linux / WSL): `scripts/install-vosk-model.sh`

Both scripts accept a model ZIP URL and an optional output directory (defaults to `vendor/vosk-model`). They will download, extract, and flatten the archive if it contains a single top-level folder.

### PowerShell example (pwsh.exe)

Open PowerShell in the workspace root and run:

.
```
.\scripts\install-vosk-model.ps1 -Url 'https://alphacephei.com/vosk/models/vosk-model-small-en-us-0.15.zip' -OutDir 'vendor/vosk-model'
```

Or set the environment variable first:

```
$env:VOSK_MODEL_URL = 'https://alphacephei.com/vosk/models/vosk-model-small-en-us-0.15.zip'
.\scripts\install-vosk-model.ps1
```

### POSIX example (macOS / Linux / WSL)

From the workspace root:

```
./scripts/install-vosk-model.sh 'https://alphacephei.com/vosk/models/vosk-model-small-en-us-0.15.zip' vendor/vosk-model
```

## Where to place the model for Electron runtime

During development the Electron main process in `electron/src/index.ts` checks for a model directory named `vosk-model` under the app resources path. For local testing you can either:

1. Keep the model in `vendor/vosk-model` and update the path in `electron/src/index.ts` to point there (temporary).
2. Copy or link the folder into `electron/resources/vosk-model` so it mirrors the packaged runtime layout.

Example (PowerShell):

```
Copy-Item -Recurse -Path vendor\vosk-model -Destination electron\resources\vosk-model
```

Example (POSIX):

```
cp -r vendor/vosk-model electron/resources/vosk-model
```

## Notes and troubleshooting

- If the Electron process fails to initialize Vosk, confirm the model folder contains files such as `ivector` (optional), `conf`, and model files (graph, etc.).
- If download or extraction fails, try downloading the ZIP manually from the model page and extract it into `vendor/vosk-model`.
- Model files can be large — the small models are usually tens to a few hundred MB; larger models are several GB.

## Next steps

After the model is in place, start Electron with the `USE_LOCAL_STT=true` environment flag so the code takes the Vosk path. Example (PowerShell):

```
$env:USE_LOCAL_STT = 'true'
# start your dev Electron script, for example (project-specific):
# pnpm --filter electron run start
```

If you want, I can also add a small npm script (or a package.json entry) that runs the appropriate script for the current platform — tell me which you'd prefer and I will add it.

## Verify installation

After extraction, confirm the model folder contains expected files. Example PowerShell check:

```powershell
# from workspace root
Test-Path vendor\vosk-model\conf -PathType Container
Test-Path vendor\vosk-model\model -PathType Container
Get-ChildItem vendor\vosk-model -Depth 1 | Select-Object Name, Length
```

Or a quick Node.js check (run from project root):

```pwsh
node -e "const fs=require('fs'); const d='vendor/vosk-model'; console.log(fs.existsSync(d)); console.log(fs.readdirSync(d).slice(0,20))"
```

If the folder contains files like `conf`, `ivector`, and model binaries (graphs, .bin/.onnx depending on model), it's likely installed correctly.
