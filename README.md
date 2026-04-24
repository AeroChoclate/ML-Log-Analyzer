# ML Log Analyzer

A WPF desktop application for analyzing MelonLoader logs from Unity games using AI.

## Features

- Load and analyze MelonLoader log files
- AI-powered error analysis using OpenRouter API
- Supports multiple free AI models
- Dark theme UI using WPF-UI library
- Save analysis results to text files

## Requirements

- .NET 8.0 SDK
- Windows 10/11
- OpenRouter API key (free tier available)

## Setup

1. Clone the repository
2. Open the solution in Visual Studio
3. Build and run the application
4. Enter your OpenRouter API key in Settings
5. Click "Save Settings"

## Usage

1. Click "Open Log" to select a `.log` or `.txt` file
2. Select an AI model from the dropdown
3. Click "Start Analysis" to analyze the log
4. View the analysis in the right panel
5. Click "Save Result" to export the analysis
6. Click "Close" to exit the application

## Available Models

- z-ai/glm-4.5-air
- google/gemma-3-27b-it
- nvidia/nemotron-3-super-120b-a12b
- minimax/minimax-m2.5

## Tech Stack

- .NET 8.0
- WPF
- WPF-UI (Fluent design library)
- OpenRouter AI API

## License

MIT