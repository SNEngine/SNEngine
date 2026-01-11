## Switch Show Variants (block instruction)

You can define a multi-way conditional branch based on a `Show Variants` node using a `Switch Show Variant` block. The block displays a set of options to the player and contains labeled case sections with instruction bodies that execute for the selected option.

Syntax example:

```
Switch Show Variant
Cases:
Case A:
Nagatoro says A
End
Case B:
Nagatoro says B
End
Case C:
Nagatoro says C
End
Case D:
Nagatoro says D
End
endcase
```

Key details:
- The `Cases:` header starts the case definitions section
- Each case is defined with `Case [value]:` followed by the instruction body
- The compiler creates nodes directly from the block via the instruction handler:
  - `ShowVariantsNode` (with `_variants` applied from the case values)
  - a `SwitchIntNode` that receives `selectedIndex` from the ShowVariants node
  - case output ports are created dynamically based on the number of cases
- Case bodies are created as node sequences; each case receives its own independent sequence (no cross-case wiring) and the first node of a case is connected to the corresponding switch case output.
- `End` inside a case is supported: handlers will create `ExitNode` inside that case when `End` appears in a case body.

Implementation notes:
- Block parsing for `Switch Show Variant` is handled by `SwitchShowVariantInstructionHandler` (it parses the `Cases:` list, creates the `ShowVariants` node, creates the `SwitchInt` node, and then builds case bodies using template matching and call handlers).