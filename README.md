# KanaLearning

KanaLearning is a desktop app for practicing kana to romaji typing.
It supports Chinese and English UI and runs as a portable app on Windows x64.

## What You Can Do

- Practice kana questions with instant right/wrong feedback.
- Cover hiragana, katakana, dakuten/handakuten, and yoon.
- Import your own questions from TXT or JSON files.
- Add a single question manually.
- Switch UI language between Chinese and English.

## Quick Start

1. Go to GitHub Releases and download the latest zip package.
2. Extract the zip.
3. Run KanaLearning.exe.

No installer is required.

## How To Use

1. Start the app and choose quiz mode.
2. Enter the romaji for the kana shown.
3. Submit to get instant feedback.
4. Continue to the next question.
5. Open settings to switch language if needed.

## Import File Format

TXT format (one question per line):

- kana=romaji
- kana,romaji
- Empty lines and lines starting with # are ignored.

Example:

あ=a
ガ=ga
きゃ=kya

JSON format:

- Use an array of objects.
- Required fields: kana, romaji.
- Optional field: category.

Example:

[
  { "kana": "カ", "romaji": "ka", "category": "Katakana" },
  { "kana": "ぴ", "romaji": "pi", "category": "DakutenHandakuten" }
]

## Notes

- Target platform: Windows x64.
- If launch is blocked by system security, right-click the file and allow run.
