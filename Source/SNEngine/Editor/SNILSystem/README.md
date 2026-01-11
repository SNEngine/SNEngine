# SNIL System Documentation

## Overview

The SNIL (Script Novel Intermediate Language) system is a powerful, modular text-based scripting solution for creating visual novel dialogues in Unity. It allows developers to define complex dialogue flows using simple text-based scripts that are automatically converted into node-based dialogue graphs through a plugin-based architecture.

## Key Features

- **Modular Architecture**: Plugin-based instruction handling system with dedicated validators and processors
- **Text-based dialogue scripting**: Write dialogues using simple, readable text files
- **Node-based execution**: Automatically converts text scripts to Unity node graphs
- **Flexible parameter system**: Supports complex parameter passing to nodes
- **Resource management**: Built-in system for handling characters, backgrounds, and assets
- **Multi-script support**: Create multiple dialogues in a single file
- **Function system**: Reusable code blocks for common operations
- **Comprehensive validation**: Detailed error checking with multiple validation layers
- **Extensible design**: Easy to add new node types and instruction handlers

## Architecture

The SNIL system follows a modular, plugin-based architecture:

### Core Components

#### 1. Instruction Handler System
The system uses a dynamic instruction handler architecture:
- `IInstructionHandler` interface for all instruction handlers
- `InstructionHandlerManager` for registering and managing handlers
- Dedicated handlers for different instruction types:
  - `NameInstructionHandler` - handles `name:` directives
  - `StartInstructionHandler` - creates Start nodes
  - `EndInstructionHandler` - creates End nodes
  - `FunctionDefinitionInstructionHandler` - handles function definitions
  - `FunctionEndInstructionHandler` - handles function endings
  - `CallInstructionHandler` - handles function calls
  - `SetVariableInstructionHandler` - handles `set [name] = [value]` instructions
  - `GenericNodeInstructionHandler` - handles template-based nodes

#### 2. Validation System
Multi-layered validation architecture:
- `SNILSyntaxValidator` - main validation coordinator
- `EmptyFileValidator` - checks for empty/null files
- `NameDirectiveValidator` - validates `name:` directive and structure
- `FunctionValidator` - validates function definitions and structure
- `SetVariableInstructionValidator` - validates `set [name] = [value]` instructions
- `InstructionValidator` - validates individual instructions against templates
- `InstructionValidatorManager` - manages specialized validators

#### 3. Compilation System
- `SNILInstructionBasedCompiler` - main compilation engine
- `SNILCompiler` - high-level interface
- Modular compilation with error propagation
- Validation occurs before processing

#### 4. Node Connection System
- `NodeConnectionUtility` - handles sequential node connections
- Automatic `_enter` and `_exit` port connections
- Proper function body connection through `_operations` port

### File Organization

```
SNILSystem/
├── FunctionSystem/           # Function parsing and handling
├── Importers/               # Script import logic
│   ├── SNILScriptImporter.cs
│   ├── SNILGraphCreator.cs
│   ├── SNILScriptProcessor.cs
│   └── SNILScriptValidator.cs
├── InstructionHandlers/     # Instruction handling system
│   ├── IInstructionHandler.cs
│   ├── BaseInstructionHandler.cs
│   ├── NameInstructionHandler.cs
│   ├── StartInstructionHandler.cs
│   ├── EndInstructionHandler.cs
│   ├── FunctionDefinitionInstructionHandler.cs
│   ├── FunctionEndInstructionHandler.cs
│   ├── CallInstructionHandler.cs
│   ├── SetVariableInstructionHandler.cs
│   ├── GenericNodeInstructionHandler.cs
│   ├── FunctionBodyCreator.cs
│   └── InstructionHandlerManager.cs
├── NodeCreation/            # Node creation logic
│   ├── NodeCreator.cs
│   ├── NodePositioner.cs
│   ├── NodeConnector.cs
│   └── NodeFormatter.cs
├── Parsers/                 # Script parsing
├── ResourceFinder/          # Asset finding utilities
├── Utilities/              # General utilities
│   ├── SNILParameterApplier.cs
│   ├── SNILTypeResolver.cs
│   ├── SNILTemplateMatcher.cs
│   └── SNILTemplateManager.cs
├── Validators/             # Validation system
│   ├── IInstructionValidator.cs
│   ├── BaseInstructionValidator.cs
│   ├── TemplateBasedInstructionValidator.cs
│   ├── InstructionValidatorManager.cs
│   ├── EmptyFileValidator.cs
│   ├── NameDirectiveValidator.cs
│   ├── FunctionValidator.cs
│   ├── SetVariableInstructionValidator.cs
│   ├── InstructionValidator.cs
│   ├── ScriptLineExtractor.cs
│   └── SNILSyntaxValidator.cs
├── Workers/                # Node worker system
├── SNILCompiler.cs         # Main compiler interface
└── SNILInstructionBasedCompiler.cs  # Core compilation engine
```

## Basic Script Structure

A basic SNIL script consists of:

```
name: DialogueName
Start
[dialogue content]
End
```

Example:
```
name: Greeting
Start
Nagatoro says Hello there!
Player says Hi Nagatoro!
Nagatoro says How are you doing today?
End
```

## Displayed Instruction

You can use the `Displayed` instruction to combine showing a character and displaying their dialogue in one step. This creates both a `ShowCharacterNode` and a `DialogNode` automatically.

Syntax:
```
Displayed {character} says {text}
```

Or with emotion:
```
Displayed {character} says {text} with emotion {emotion}
```

Example:
```
name: ExampleDialogue
Start
Displayed Nagatoro says Hello there!
Displayed Player says Hi Nagatoro! with emotion Happy
End
```

Key details:
- The instruction creates two nodes: `ShowCharacterNode` and `DialogNode`
- The character is shown with the specified emotion (defaults to "Default" if not specified)
- The character's dialogue is displayed after they appear
- Both nodes are automatically connected in sequence

## If Show Variants (block instruction)

You can define a conditional branch based on a `Show Variants` node using an `If Show Variant` block. The block displays a set of options to the player and contains labeled sections (e.g., `True:`, `False:` or variant-name sections) with instruction bodies that execute for the selected option.

Syntax examples:

Simple form:
```
If Show Variant
Variants:
Option A
Option B
True:
Print A
False:
Print B
endif
```

With `End` inside branch bodies (allowed):
```
If Show Variant
Variants:
A
B
True:
Nagatoro says (True default branch)
End
False:
Nagatoro says (False default branch)
End
endif
```

Key details:
- The `Variants:` header lists options (one per line). Extra identical names are permitted but may be confusing; prefer unique variant labels.
- Labeled sections can be one of:
  - `True:` / `False:` — maps to logical branches in the generated `If` node
  - a variant name (e.g. `A:`) — maps to the branch that corresponds to that variant
- The compiler will create nodes directly from the block via the instruction handler:
  - `ShowVariantsNode` (with `_variants` applied from the list)
  - a single `CompareIntegersNode` that compares `selectedIndex` to `0` (used to drive a single `If` split)
  - an `IfNode` with `_true` and `_false` outputs wired to the parsed branch bodies
- Branch bodies are created as node sequences; each branch receives its own independent sequence (no cross-branch wiring) and the first node of a branch is connected to the corresponding `If` output (`_true` or `_false`).
- `End` inside a branch is supported: handlers will create `ExitNode` inside that branch when `End` appears in a branch body. `End` inside branches does not override the top-level script termination semantics.

Validator notes:
- The validator enforces that a script *as a whole* ends with a *top-level* `End` or `Jump To` instruction. However, the validator also accepts scripts that end with a top-level `If Show Variant` block **provided that every branch inside that block itself terminates with `End` or `Jump To`** (so you can place `End` inside `True:` / `False:` branches and omit a final top-level `End`).

Implementation notes:
- Block parsing for `If Show Variant` is handled by `IfShowVariantInstructionHandler` (it parses the `Variants:` list, creates the `ShowVariants` node, creates the `Compare` + `If` nodes, and then builds branch bodies using template matching and call handlers).
- The legacy `SNILBlockParser` behavior has been deprecated and block instructions are now handled through `IBlockInstructionHandler` implementations.

Examples and tests:
- See `Assets/.../SNILSystem/Examples/test_if_show_variant.snil` for a minimal example demonstrating `End` inside branches and the generated structure.

## Adding New Node Types

The system supports easy addition of new node types through the instruction handler architecture:

### Step 1: Create Instruction Handler

Create a new handler that implements `IInstructionHandler`:

```csharp
public class CustomInstructionHandler : BaseInstructionHandler
{
    public override bool CanHandle(string instruction)
    {
        // Check if this handler can process the instruction
        return instruction.StartsWith("CustomCommand", StringComparison.OrdinalIgnoreCase);
    }

    public override InstructionResult Handle(string instruction, InstructionContext context)
    {
        // Create and configure the node
        if (context.Graph == null)
        {
            return InstructionResult.Error("Graph not initialized.");
        }

        var dialogueGraph = (DialogueGraph)context.Graph;
        var nodeType = SNILTypeResolver.GetNodeType("CustomNode");
        
        if (nodeType != null)
        {
            var node = dialogueGraph.AddNode(nodeType) as BaseNode;
            if (node != null)
            {
                // Configure the node
                node.name = "Custom";
                node.position = new Vector2(context.Nodes.Count * 250, 0);

                // Apply parameters
                var parameters = ParseParameters(instruction);
                SNILParameterApplier.ApplyParametersToNode(node, parameters, "CustomNode");

                // Add to context
                context.Nodes.Add(node);
                NodeConnectionUtility.ConnectNodeToLast(dialogueGraph, node, context);

                return InstructionResult.Ok(node);
            }
        }

        return InstructionResult.Error("Failed to create CustomNode.");
    }
}
```

### Step 2: Register the Handler

Register your handler with the manager:

```csharp
// In InstructionHandlerManager.RegisterDefaultHandlers()
RegisterHandler(new CustomInstructionHandler());
```

### Step 3: Use Your New Node

Now you can use your new instruction in SNIL scripts:

```
name: MyDialogue
Start
CustomCommand parameter1=value1 parameter2=value2
End
```

## Core Node Types

### Dialogue Nodes
```
[Character name] says [dialogue text]
```

### Start and End Nodes
```
Start
[dialogue content]
End
```

### Jump Nodes
```
Jump To [dialogue name]
```

## Variable System

SNIL supports variables for storing and manipulating data during dialogue execution. The system provides both variable declaration/creation and value assignment through dedicated instructions.

### Variable Declaration and Assignment

Variables can be created and assigned values using the `set` instruction:

```
set [variable name] = [value]
```

Examples:
```
set playerScore = 100
set playerName = "John"
set isGameActive = true
set playerHealth = 75.5
```

The system automatically determines the variable type based on the assigned value:
- Integer numbers (e.g., `100`) create integer variables
- Floating point numbers (e.g., `75.5`) create float variables
- Text in quotes (e.g., `"John"`) creates string variables
- Boolean values (`true`/`false`) create boolean variables

### Variable Usage in Graphs

When a `set` instruction is processed:
1. If a variable with the given name already exists, its value is updated
2. If no variable exists, a new one is created with the specified name and type
3. The system creates a `Set[Type]Node` that connects to the target variable
4. Variables are positioned in the left area of the graph (x=488) with vertical spacing
5. Set nodes are positioned in a horizontal chain (y=88) with increasing x coordinates
6. Connections are established between variables and their corresponding Set nodes

Example with variables:
```
name: VariableExample
Start
set playerScore = 0
set playerName = "Hero"
set isGameActive = true
Nagatoro says Hello [Property=playerName]! Your score is [Property=playerScore].
set playerScore = 100
End
```

## Function System

The system supports functions with proper body creation and connection. Functions can be defined anywhere in the file and are available throughout that same file once registered during compilation.

### Defining Functions
```
function functionName
// function body with SNIL commands
end
```

### Calling Functions
```
call functionName
```

### Function Scope and Availability

**Within a single file**: All functions defined in a file are available to all scripts in that same file, regardless of their order in the file. The compiler registers all functions before processing the main script content.

**Across files**: Functions defined in one file are NOT available in other files. Each SNIL file has its own function scope.

**Multi-script files**: When using `---` separators to define multiple scripts in one file, each script part has access to functions defined in the same file, but function calls cannot cross script boundaries within the same file.

### Function Body Processing
- Functions create `GroupCallsNode` for the function call
- Function bodies are created as connected nodes within the function
- Proper Y-axis positioning above main flow for visual separation
- Connection through `_operations` port to function body
- Functions can be called before their definition within the same file

### Function Definition Order
Functions can be called before they are defined in the same file:
```
name: FunctionCallBeforeDefinition
Start
Show Background beachBackground
call greetNagatoro  # Function called before definition
Nagatoro says Thanks for calling me!
End

function greetNagatoro
Wait 2 seconds
Nagatoro says You called me!
Player says Yes, hello!
end
```

### Import Behavior
**Single file import**: All functions in the file are registered before processing any script content, allowing calls to functions defined later in the file.

**Folder import**: Each file is processed independently, so functions in one file are not accessible from other files in the same import folder.

**Multi-script files**: Functions defined in a single file with multiple scripts (separated by `---`) are available to all script parts within that same file.

## Multi-Script Support

You can create multiple dialogues in one file using separators:

```
name: FirstDialogue
Start
Nagatoro says Hello!
Jump To SecondDialogue

---

name: SecondDialogue
Start
Player says Hi back!
End
```

## Validation Architecture

The system uses a multi-layered validation approach:

1. **Empty File Validation**: Checks for null/empty input
2. **Name Directive Validation**: Ensures proper script structure
3. **Function Validation**: Validates function definitions
4. **Instruction Validation**: Validates individual instructions against templates
5. **Template-Based Validation**: Checks against registered node templates

Each validation layer can fail independently, stopping the import process and providing detailed error messages.

## Error Handling

The system provides comprehensive error handling:
- Detailed error messages with line numbers
- Multiple error reporting (doesn't stop at first error)
- Proper error propagation through the compilation pipeline
- Clear distinction between validation and runtime errors

## Import Process Flow

1. **Validation Phase**: All validators check the script
2. **Function Registration**: All functions are registered before processing
3. **Instruction Processing**: Each instruction is handled by appropriate handler
4. **Node Connection**: Sequential connections are established
5. **Post-Processing**: Cross-references are resolved

## Best Practices

1. **Handler Design**: Create focused handlers for specific instruction types
2. **Validation**: Implement proper validation before processing
3. **Error Reporting**: Provide clear, actionable error messages
4. **Modularity**: Keep handlers small and focused
5. **Testing**: Validate handlers with various input scenarios
6. **Documentation**: Document new instruction formats clearly

## Extensibility

The system is designed for easy extensibility:
- New instruction handlers can be added without modifying core code
- Validators can be extended for domain-specific validation
- The context system allows for state management across handlers
- Template-based validation supports new node types seamlessly

## Troubleshooting

### Common Issues

- **Instruction not recognized**: Check if appropriate handler is registered
- **Validation errors**: Verify script structure and syntax
- **Node connection issues**: Ensure proper context management
- **Function body not created**: Verify function definition syntax

### Debugging Tips

- Enable detailed logging for instruction processing
- Check handler registration in `InstructionHandlerManager`
- Verify template files exist for template-based nodes
- Use validation separately to isolate issues