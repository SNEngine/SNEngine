using System;
using System.Collections.Generic;
using System.Linq;
using SiphoinUnityHelpers.XNodeExtensions;
using SiphoinUnityHelpers.XNodeExtensions.NodesControlExecutes.Switch;
using SNEngine.Editor.SNILSystem;
using SNEngine.Editor.SNILSystem.NodeCreation;
using SNEngine.Graphs;
using UnityEditor;
using UnityEngine;
using XNode;

namespace SNEngine.Editor.SNILSystem.InstructionHandlers
{
    public class SwitchShowVariantInstructionHandler : BaseInstructionHandler, IBlockInstructionHandler
    {
        public override bool CanHandle(string instruction)
        {
            return instruction.Trim().Equals("Switch Show Variant", StringComparison.OrdinalIgnoreCase);
        }

        public override InstructionResult Handle(string instruction, InstructionContext context)
        {
            return InstructionResult.Error("Switch Show Variant requires block parsing - this instruction needs to be handled specially in the main processing loop");
        }

        public InstructionResult HandleBlock(string[] lines, ref int currentLineIndex, InstructionContext context)
        {
            if (context.Graph == null)
            {
                return InstructionResult.Error("Graph not initialized. 'name:' instruction must be processed before node instructions.");
            }

            var dialogueGraph = (DialogueGraph)context.Graph;

            // Current line is "Switch Show Variant", move to next line to parse cases

            // We'll produce a BlockHandlerResult containing instructions and any function calls discovered inside the block
            var result = new BlockHandlerResult();

            var templates = SNILTemplateManager.GetNodeTemplates();

            int i = currentLineIndex; // points to "Switch Show Variant"
            int lineCount = lines.Length;

            // Move to next non-empty, non-comment line
            i++;
            while (i < lineCount && (string.IsNullOrWhiteSpace(lines[i]) || lines[i].TrimStart().StartsWith("//") || lines[i].TrimStart().StartsWith("#"))) i++;

            // Expect Cases: section
            if (i >= lineCount || !lines[i].Trim().Equals("Cases:", StringComparison.OrdinalIgnoreCase))
            {
                return InstructionResult.Error("'Switch Show Variant' block must contain a 'Cases:' section.");
            }

            // Skip the 'Cases:' header
            i++;

            // Collect cases until we hit a line that starts with "Case " or "endcase"
            var caseValues = new List<string>();
            var caseLines = new Dictionary<string, List<string>>();

            while (i < lineCount)
            {
                var t = lines[i].Trim();
                if (string.IsNullOrEmpty(t) || t.StartsWith("//") || t.StartsWith("#")) { i++; continue; }

                if (t.StartsWith("Case ", StringComparison.OrdinalIgnoreCase))
                {
                    // Extract case value
                    var caseValue = t.Substring(5).TrimEnd(':').Trim(); // Remove "Case " and trailing ":"
                    caseValues.Add(caseValue);

                    // Collect lines for this case until we hit another "Case" or "endcase"
                    i++; // Move to next line after "Case X:"
                    var caseBodyLines = new List<string>();

                    while (i < lineCount)
                    {
                        var currentLine = lines[i].Trim();
                        if (string.IsNullOrEmpty(currentLine) || currentLine.StartsWith("//") || currentLine.StartsWith("#"))
                        {
                            caseBodyLines.Add(lines[i]);
                            i++;
                            continue;
                        }

                        if (currentLine.StartsWith("Case ", StringComparison.OrdinalIgnoreCase) ||
                            currentLine.Equals("endcase", StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }

                        caseBodyLines.Add(lines[i]);
                        i++;
                    }

                    caseLines[caseValue] = caseBodyLines;
                    continue;
                }

                if (t.Equals("endcase", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                // If we encounter a line that doesn't belong, it's an error
                return InstructionResult.Error($"Unexpected line in Switch Show Variant block: {t}. Expected 'Case X:' or 'endcase'.");
            }

            if (caseValues.Count == 0)
            {
                return InstructionResult.Error("'Switch Show Variant' block must contain at least one 'Case' section.");
            }

            // Create ShowVariants node in the graph directly
            var showType = SNILTypeResolver.GetNodeType("ShowVariantsNode");
            if (showType == null)
            {
                return InstructionResult.Error("ShowVariantsNode type not found.");
            }

            var showNode = dialogueGraph.AddNode(showType) as BaseNode;
            showNode.name = NodeFormatter.ToTitleCase("Show Variants");
            // Place using standard horizontal spacing so X is not fixed (allows consistent placement with other nodes)
            showNode.position = new Vector2(context.Nodes.Count * 250, 0);

            // Set _variants parameter - use case values directly as variants
            var variants = caseValues.ToList();
            var svParams = new Dictionary<string, string> { { "_variants", string.Join(", ", variants) } };
            SNILParameterApplier.ApplyParametersToNode(showNode, svParams, "ShowVariantsNode");

            AssetDatabase.AddObjectToAsset(showNode, dialogueGraph);
            context.Nodes.Add(showNode);

            // Connect to previous node in main flow
            NodeConnectionUtility.ConnectNodeToLast(dialogueGraph, showNode, context);

            // Create SwitchInt node to handle the selection
            var switchType = SNILTypeResolver.GetNodeType("SwitchIntNode");
            if (switchType == null)
            {
                return InstructionResult.Error("SwitchIntNode type not found.");
            }

            var switchNode = dialogueGraph.AddNode(switchType) as SwitchIntNode;
            switchNode.name = NodeFormatter.ToTitleCase("Switch Show Variant");
            // Place switch node to the right of the show node
            switchNode.position = new Vector2(showNode.position.x + 250, showNode.position.y);

            AssetDatabase.AddObjectToAsset(switchNode, dialogueGraph);
            context.Nodes.Add(switchNode);

            // Connect ShowVariants._selectedIndex -> SwitchInt._value
            var selOut = showNode.GetOutputPort("_selectedIndex");
            var valueIn = switchNode.GetInputPort("_value");
            if (selOut != null && valueIn != null) selOut.Connect(valueIn);

            // Connect ShowVariants._exit -> SwitchInt._enter (so the main flow continues into the Switch node)
            var showExitOut = showNode.GetOutputPort("_exit");
            var switchEnterIn = switchNode.GetInputPort("_enter");
            if (showExitOut != null && switchEnterIn != null) showExitOut.Connect(switchEnterIn);

            // Set up the switch cases - map case values to indices using reflection since _cases is private
            var casesField = typeof(SwitchIntNode).GetField("_cases", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (casesField != null)
            {
                var casesList = new List<int>();
                for (int idx = 0; idx < caseValues.Count; idx++)
                {
                    casesList.Add(idx);
                }
                casesField.SetValue(switchNode, casesList);

                // After setting the cases, we need to ensure the dynamic ports are created
                // Since SyncPorts is in the editor, we need to manually create the ports
                // The SwitchNode creates ports based on the _cases list in its GetPortName method
                for (int idx = 0; idx < caseValues.Count; idx++)
                {
                    string portName = "case " + idx;
                    if (!switchNode.HasPort(portName))
                    {
                        switchNode.AddDynamicOutput(
                            typeof(NodeControlExecute),
                            Node.ConnectionType.Multiple,
                            Node.TypeConstraint.None,
                            portName
                        );
                    }
                }
            }
            else
            {
                SNILDebug.LogError("Could not find _cases field in SwitchIntNode using reflection");
                return InstructionResult.Error("Internal error: Could not configure switch cases");
            }

            // Ensure the switch node has the correct number of case ports
            // The ports will be created dynamically based on the number of cases

            // Process each case branch
            for (int caseIdx = 0; caseIdx < caseValues.Count; caseIdx++)
            {
                var caseValue = caseValues[caseIdx];
                var caseBody = caseLines[caseValue];

                // Save main flow last node so branch node creation doesn't alter the main flow (restore after processing branch)
                var savedMainLast = context.LastNode;

                // Base positions for the branch first node (relative to the Switch node; X is computed dynamically so nodes can stack/align like other systems)
                // Position cases with good spacing, starting from a position relative to the switch node
                var branchBaseY = switchNode.position.y - 120 + (caseIdx * 200); // Space out branches vertically with 200px spacing

                // Track index inside the branch so we can place nodes using standard spacing (250px per step)
                int branchIndex = 0;

                // Parse each line in the case body and create nodes or call handlers directly
                BaseNode prevBranchNode = null;
                foreach (var secLine in caseBody)
                {
                    var sTrim = secLine.Trim();

                    // Allow 'End' inside branch bodies (create an ExitNode there)
                    if (sTrim.Equals("End", StringComparison.OrdinalIgnoreCase))
                    {
                        var endHandler = InstructionHandlerManager.Instance.GetHandlerForInstruction("End");
                        if (endHandler != null)
                        {
                            var prevLast = context.LastNode;
                            var endRes = endHandler.Handle("End", context);
                            if (!endRes.Success)
                            {
                                SNILDebug.LogError(endRes.ErrorMessage);
                            }
                            else if (endRes.Data is BaseNode endNode)
                            {
                                // Position the End node inside the branch with increased X spacing
                                endNode.position = new Vector2(switchNode.position.x + (branchIndex + 1) * 300f, branchBaseY);

                                // Attach to branch entry if this is the first branch node
                                if (switchNode != null && prevBranchNode == null)
                                {
                                    var branchOut = switchNode.GetOutputPort($"case {caseIdx}");
                                    var firstEnter = endNode.GetInputPort("_enter");
                                    if (branchOut != null && firstEnter != null) branchOut.Connect(firstEnter);
                                }

                                // Otherwise chain after previous branch node
                                if (prevBranchNode != null && prevBranchNode is BaseNodeInteraction prevInteraction && endNode is BaseNodeInteraction currInteraction)
                                {
                                    var outPort = prevInteraction.GetExitPort();
                                    var inPort = currInteraction.GetEnterPort();
                                    if (outPort != null && inPort != null) outPort.Connect(inPort);
                                }

                                // Ensure it's in the context nodes (End handler should have added it already)
                                if (!context.Nodes.Contains(endNode)) context.Nodes.Add(endNode);

                                prevBranchNode = endNode;
                                branchIndex++;

                                // Restore last node in the main flow so End inside branch doesn't alter it
                                context.LastNode = prevLast;
                            }
                        }

                        continue;
                    }

                    // If it's a call, reuse CallInstructionHandler to create nodes and function bodies
                    if (sTrim.StartsWith("call ", StringComparison.OrdinalIgnoreCase))
                    {
                        var callHandler = InstructionHandlerManager.Instance.GetHandlerForInstruction(sTrim);
                        if (callHandler != null)
                        {
                            var callRes = callHandler.Handle(sTrim, context);
                            if (!callRes.Success)
                            {
                                SNILDebug.LogError(callRes.ErrorMessage);
                            }
                            else if (callRes.Data is BaseNode callNode)
                            {
                                // Attach to the correct Switch branch
                                if (switchNode != null && prevBranchNode == null)
                                {
                                    var branchOut = switchNode.GetOutputPort($"case {caseIdx}");
                                    var firstEnter = callNode.GetInputPort("_enter");
                                    if (branchOut != null && firstEnter != null) branchOut.Connect(firstEnter);

                                    // Position the first branch node using standard spacing relative to the Switch node
                                    callNode.position = new Vector2(switchNode.position.x + (branchIndex + 1) * 300f, branchBaseY);
                                    branchIndex++;
                                }
                                else if (prevBranchNode != null)
                                {
                                    // Chain sequentially inside branch and position to the right according to branchIndex
                                    if (prevBranchNode is BaseNodeInteraction prevInteraction && callNode is BaseNodeInteraction currInteraction)
                                    {
                                        var outPort = prevInteraction.GetExitPort();
                                        var inPort = currInteraction.GetEnterPort();
                                        if (outPort != null && inPort != null) outPort.Connect(inPort);
                                    }

                                    callNode.position = new Vector2(switchNode.position.x + (branchIndex + 1) * 300f, branchBaseY);
                                    branchIndex++;
                                }

                                prevBranchNode = callNode;
                            }

                            continue;
                        }
                    }

                    // Otherwise try to match template and create a node
                    bool matched = false;
                    foreach (var template in templates)
                    {
                        var parameters = SNILTemplateMatcher.MatchLineWithTemplate(sTrim, template.Value.Template);
                        if (parameters != null)
                        {
                            var nodeType = SNILTypeResolver.GetNodeType(template.Key);
                            if (nodeType == null)
                            {
                                SNILDebug.LogWarning($"Node type for template {template.Key} not found.");
                                matched = true; // considered handled
                                break;
                            }

                            var node = dialogueGraph.AddNode(nodeType) as BaseNode;
                            node.name = NodeFormatter.FormatNodeDisplayName(template.Key);

                            // Place first node of branch using the branchIndex spacing, otherwise chain using branchIndex
                            node.position = new Vector2(switchNode.position.x + (branchIndex + 1) * 300f, branchBaseY);
                            branchIndex++;

                            SNILParameterApplier.ApplyParametersToNode(node, parameters, template.Key);
                            AssetDatabase.AddObjectToAsset(node, dialogueGraph);

                            // Attach to branch entry if it's the first node
                            if (switchNode != null && prevBranchNode == null)
                            {
                                var branchOut = switchNode.GetOutputPort($"case {caseIdx}");
                                var firstEnter = node.GetInputPort("_enter");
                                if (branchOut != null && firstEnter != null) branchOut.Connect(firstEnter);
                            }

                            // Chain sequentially inside branch
                            if (prevBranchNode != null && prevBranchNode is BaseNodeInteraction prevInteraction && node is BaseNodeInteraction currInteraction)
                            {
                                var outPort = prevInteraction.GetExitPort();
                                var inPort = currInteraction.GetEnterPort();
                                if (outPort != null && inPort != null) outPort.Connect(inPort);
                            }

                            // Add to context nodes (so they get saved)
                            context.Nodes.Add(node);
                            prevBranchNode = node;
                            matched = true;
                            break;
                        }
                    }

                    if (!matched)
                    {
                        SNILDebug.LogWarning($"Unrecognized instruction inside Switch Show Variant block: {sTrim}");
                    }
                }

                // restore the main flow last node so branch processing doesn't interfere with subsequent top-level nodes
                context.LastNode = savedMainLast;
            }

            // Make Switch node the last node in the main flow for subsequent attachments
            context.LastNode = switchNode;

            // Update the caller's currentLineIndex to the last line we consumed
            currentLineIndex = i;

            SNILDebug.Log($"SwitchShowVariant created nodes for cases [{string.Join(", ", caseValues)}]");

            return InstructionResult.Ok();
        }
    }
}