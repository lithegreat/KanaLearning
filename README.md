# KanaLearning

KanaLearning is a WinUI 3 desktop app built with Windows App SDK and MVVM (CommunityToolkit.Mvvm).
It runs as an unpackaged app and supports kana-to-romaji practice with bilingual UI (Chinese/English).

## Features

- Quiz mode: app shows kana, user inputs romaji, instant feedback.
- Kana coverage: hiragana, katakana, dakuten/handakuten, yoon.
- Strict answer matching: compares normalized romaji (trim + case-insensitive).
- Data input:
  - Built-in default kana dataset.
  - Import JSON or TXT files.
  - Manual single-question input.
- Language:
  - Default follows system language.
  - In-app switch between Chinese and English.

## Tech Stack

- WinUI 3 + Windows App SDK
- MVVM with CommunityToolkit.Mvvm
- .NET 10 Windows target

## Input File Formats

### TXT

- `kana=romaji`
- `kana,romaji`
- Empty lines and lines starting with `#` are ignored.

Examples:

```txt
あ=a
ガ=ga
きゃ=kya
```

### JSON

Array of objects with `kana`, `romaji`, optional `category`.

```json
[
  { "kana": "カ", "romaji": "ka", "category": "Katakana" },
  { "kana": "ぴ", "romaji": "pi", "category": "DakutenHandakuten" }
]
```

## Build

From project root:

```powershell
$Platform = if ($env:PROCESSOR_ARCHITECTURE -eq 'AMD64') { 'x64' } elseif ($env:PROCESSOR_ARCHITECTURE -eq 'ARM64') { 'ARM64' } else { 'x86' }
dotnet build .\KanaLearning.csproj -c Debug -p:Platform=$Platform
```

## Run Tests

```powershell
$Platform = if ($env:PROCESSOR_ARCHITECTURE -eq 'AMD64') { 'x64' } elseif ($env:PROCESSOR_ARCHITECTURE -eq 'ARM64') { 'ARM64' } else { 'x86' }
dotnet test .\KanaLearning.Tests\KanaLearning.Tests.csproj -c Debug -p:Platform=$Platform
```

## Project Structure

- `Models`: quiz entities and import result.
- `Services`: localization, question import, answer evaluation.
- `ViewModels`: quiz/session state and settings state.
- `Views`: quiz page and settings page.
- `Resources/Localization`: language JSON files.
- `Resources/Kana`: built-in default kana dataset.
