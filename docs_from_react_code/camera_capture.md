# Renderer Camera Capture (getUserMedia)

This project recommends using the renderer `getUserMedia` API to capture photos on desktop. It's cross-platform (Windows/macOS/Linux) and avoids launching external camera apps or building native helpers.

Quick example component (already added in `src/features/camera/CameraCapture.tsx`):

- The component requests camera permission from the user, shows a live preview, and exposes a `Take photo` button that returns a DataURL via the `onCapture` prop.

Usage example (React):

```tsx
import React from 'react'
import CameraCapture from 'src/features/camera/CameraCapture'

export default function CameraPage() {
  const handleCapture = (dataUrl: string) => {
    // dataUrl is a base64 PNG; you can preview it or send it to main to save
    console.log('captured image length', dataUrl.length)
    // Example: send to main to save
    // window.electron?.ipc.invoke('save-captured-image', dataUrl)
  }

  return (
    <div>
      <h2>Take a photo</h2>
      <CameraCapture onCapture={handleCapture} />
    </div>
  )
}
```

Permissions
- Electron on Windows may require the user to enable "Allow desktop apps to access your camera" in Settings > Privacy. The app's renderer will prompt for permission on first use.
- If permissions are denied, instruct the user to open OS settings to enable camera access.

Saving images
- The renderer can send the captured DataURL to the main process via IPC for persistence. Add an IPC handler in `electron/src/index.ts` (for example `ipcMain.handle('save-captured-image', ...)`) that decodes the base64 and writes a file with `fs.promises.writeFile`.

Notes
- For richer UX (crop, rotate), implement an image editor on the captured DataURL.
- If a native camera UI is absolutely required (special hardware or Windows-only UX), consider a small native helper as a later task.

Playback / TTS integration
 - To avoid feedback while playing TTS or other audio, the renderer should inform the main process when playback starts and ends.
 - Example (renderer):

```ts
// when starting playback
await window.electron?.ipc.invoke('audio-playback-start')

// when playback finishes
await window.electron?.ipc.invoke('audio-playback-end')
```

This will suppress microphone buffers in the main process while playback is active.
