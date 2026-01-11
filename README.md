# SNEngine

SNEngine is a visual novel engine built for Unity, designed to help developers create interactive visual novels and story-driven games. It provides a comprehensive set of systems and tools to handle dialogue, character management, backgrounds, save/load functionality, and more.

![SNEngine Promo](images/promo.png)

## Features

- **Dialogue System**: Advanced dialogue management with branching narratives
- **Character System**: Character management with expressions and animations
- **Background System**: Dynamic background management and transitions
- **Save/Load System**: Comprehensive save and load functionality
- **Audio System**: Sound and music management
- **Localization Support**: Multi-language support for global distribution
- **Visual Scripting**: Node-based visual scripting for creating game logic
- **Message System**: Customizable message boxes and UI elements
- **Input Systems**: Various input methods for player interaction
- **Asynchronous Operations**: Powered by UniTask for smooth, non-blocking operations
- **Smooth Animations**: Enhanced with DOTween for fluid animations and transitions

## Dependencies

SNEngine relies on several key third-party libraries:

- **[UniTask](https://github.com/Cysharp/UniTask)**: Provides efficient async/await integration for Unity, enabling smooth asynchronous operations throughout the engine
- **DOTween**: Premium animation engine for Unity, used extensively for UI animations, character movements, and scene transitions

## Documentation

For detailed documentation and tutorials, please visit the official documentation:

[Documentation Wiki](https://github.com/SNEngine/SNEngineDocs/wiki/)

## Installation

To install SNEngine in a new Unity project, follow the setup instructions in our documentation:

[Install and Setup Empty Project](https://github.com/SNEngine/SNEngineDocs/wiki/Install-and-Setup-empty-project)

## Project Structure

The SNEngine project is organized into several key directories:

- `CoreGame`: Core game systems and utilities
- `SNEngine`: Main engine components including dialogue, character, and background systems
- `XNode`: Visual scripting nodes and editor tools
- `Yaml`: YAML serialization and parsing utilities
- `MessagePack`: Efficient serialization for save data

## Getting Started

1. Download the latest unitypackage from the [Releases](https://github.com/SNEngine/SNEngine/releases) page
2. Before importing, use the Symbols utility from [SNEngineUtils](https://github.com/SNEngine/SNEngineUtils) (SNEngine_Symbols) to properly configure your project
3. Import the downloaded unitypackage into your Unity project
4. Refer to the [documentation](https://github.com/SNEngine/SNEngineDocs/wiki/) for setup instructions and tutorials

## License

This project is licensed under the terms found in the LICENSE file included in this repository.