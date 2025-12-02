
# Png2Ico

Png2Ico converts a PNG image into a single-image Windows icon (.ico).

Quick overview
- Converts PNG (including transparency) to ICO
- Produces a single icon image (resized/cropped to a square)
- Default output size: 128×128 (changeable with `--size`)

Download
- Download the prebuilt executable or installer from this repository's **Releases** page.

Requirements
- On Windows: the published `Png2Ico.exe` requires the .NET 9 runtime (not the SDK).

Simple usage

From PowerShell or Command Prompt:

```powershell
# Convert and write output next to the input (default)
.\Png2Ico.exe input.png

# Specify output file and size
.\Png2Ico.exe logo.png app.ico --size 64
```

Behavior notes
- If `output.ico` is omitted, the program replaces the input extension with `.ico`.
- `--size` sets the icon dimensions (square). The program crops/resizes the image to fit.

Examples
- `Png2Ico.exe avatar.png` → `avatar.ico` (128×128)
- `Png2Ico.exe banner.png site.ico --size 32` → `site.ico` (32×32)

Troubleshooting
- Error: Input file not found — verify the input path.
- If the executable fails to start, install the .NET 9 runtime from Microsoft.

Want the installer?
- Releases may include an MSI built with WiX. Run the MSI to install the program.

Questions or issues
- Open an issue on this repository if something doesn't work or you'd like additional features.
