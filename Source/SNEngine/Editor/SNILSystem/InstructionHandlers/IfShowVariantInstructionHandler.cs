using System; 
  
using System.Collections.Generic;  
using System.Linq;  
using SiphoinUnityHelpers.XNodeExtensions;  
using SNEngine.Editor.SNILSystem;
using SNEngine.Editor.SNILSystem.NodeCreation;
using SNEngine.Graphs;  
using UnityEditor;  
using UnityEngine;  
  
namespace SNEngine.Editor.SNILSystem.InstructionHandlers 
{  
    public class IfShowVariantInstructionHandler : BaseInstructionHandler, IBlockInstructionHandler  
    {  
        public override bool CanHandle(string instruction)  
        {  
            return instruction.Trim().Equals("If Show Variant", StringComparison.OrdinalIgnoreCase);  
        }  
  
        public override InstructionResult Handle(string instruction, InstructionContext context) 
        {  
            return InstructionResult.Error("IF Show Variant requires block parsing - this instruction needs to be handled specially in the main processing loop");  
        }  
  
        public InstructionResult HandleBlock(string[] lines, ref int currentLineIndex, InstructionContext context) 
        {  
            if (context.Graph == null)  
            {  
                return InstructionResult.Error("Graph not initialized. 'name:' instruction must be processed before node instructions.");  
            }  
  
            var dialogueGraph = (DialogueGraph)context.Graph;  
  
            // Current line is "If Show Variant", move to next line to parse variants

            // We'll produce a BlockHandlerResult containing instructions and any function calls discovered inside the block
            var result = new BlockHandlerResult();

            var templates = SNILTemplateManager.GetNodeTemplates();

            int i = currentLineIndex; // points to "If Show Variant"
            int lineCount = lines.Length;

            // Move to next non-empty, non-comment line
            i++;
            while (i < lineCount && (string.IsNullOrWhiteSpace(lines[i]) || lines[i].TrimStart().StartsWith("//") || lines[i].TrimStart().StartsWith("#"))) i++;

            // Expect Variants: section
            if (i >= lineCount || !lines[i].Trim().StartsWith("Variants", StringComparison.OrdinalIgnoreCase))
            {
                return InstructionResult.Error("'If Show Variant' block must contain a 'Variants:' section.");
            }

            // Skip the 'Variants:' header
            i++;

            // Collect variants until we hit a line that ends with ':' or 'endif'
            var variants = new List<string>();
            while (i < lineCount)
            {
                var t = lines[i].Trim();
                if (string.IsNullOrEmpty(t) || t.StartsWith("//") || t.StartsWith("#")) { i++; continue; }
                if (t.EndsWith(":") || t.Equals("endif", StringComparison.OrdinalIgnoreCase)) break;
                variants.Add(t);
                i++;
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

            // Set _variants parameter
            var svParams = new Dictionary<string, string> { { "_variants", string.Join(", ", variants) } };
            SNILParameterApplier.ApplyParametersToNode(showNode, svParams, "ShowVariantsNode");

            AssetDatabase.AddObjectToAsset(showNode, dialogueGraph);
            context.Nodes.Add(showNode);

            // Connect to previous node in main flow
            NodeConnectionUtility.ConnectNodeToLast(dialogueGraph, showNode, context);

            // Create a single Compare + If for the ShowVariants node (compare selectedIndex == 0)
            var compareTypeSingle = SNILTypeResolver.GetNodeType("CompareIntegersNode");
            var ifTypeSingle = SNILTypeResolver.GetNodeType("IfNode");

            BaseNode compareNodeSingle = null;
            BaseNode ifNodeSingle = null;

            if (compareTypeSingle != null && ifTypeSingle != null)
            {
                compareNodeSingle = dialogueGraph.AddNode(compareTypeSingle) as BaseNode;
                compareNodeSingle.name = NodeFormatter.ToTitleCase("Compare selected == 0");
                // Place compare slightly left and down (match example: x=472, y=-184)
                compareNodeSingle.position = new Vector2(showNode.position.x - 28, showNode.position.y - 184);

                var compParamsSingle = new Dictionary<string, string> { { "b", "0" }, { "type", "Equals" } };
                SNILParameterApplier.ApplyParametersToNode(compareNodeSingle, compParamsSingle, "CompareIntegersNode");
                AssetDatabase.AddObjectToAsset(compareNodeSingle, dialogueGraph);
                context.Nodes.Add(compareNodeSingle);

                // Connect ShowVariants._selectedIndex -> compare._a
                var selOutSingle = showNode.GetOutputPort("_selectedIndex");
                var aInSingle = compareNodeSingle.GetInputPort("_a");
                if (selOutSingle != null && aInSingle != null) selOutSingle.Connect(aInSingle);

                // Create the If node placed to the right of ShowVariants so its true/false outputs look like branches
                ifNodeSingle = dialogueGraph.AddNode(ifTypeSingle) as BaseNode;
                ifNodeSingle.name = NodeFormatter.ToTitleCase("If variant is 0");
                // Place If to the right and slightly above (match example: x=792, y=40)
                ifNodeSingle.position = new Vector2(showNode.position.x + 292, showNode.position.y + 40);
                AssetDatabase.AddObjectToAsset(ifNodeSingle, dialogueGraph);
                context.Nodes.Add(ifNodeSingle);

                // Connect compare._result -> if._condition
                var compOutSingle = compareNodeSingle.GetOutputPort("_result");
                var condInSingle = ifNodeSingle.GetInputPort("_condition");
                if (compOutSingle != null && condInSingle != null) compOutSingle.Connect(condInSingle);

                // Also connect ShowVariants._exit -> If._enter so the main flow continues into the If node
                var showExitOut = showNode.GetOutputPort("_exit");
                var ifEnterIn = ifNodeSingle.GetInputPort("_enter");
                if (showExitOut != null && ifEnterIn != null) showExitOut.Connect(ifEnterIn);

                // Make If node the last node in the main flow for subsequent attachments
                context.LastNode = ifNodeSingle;
            }

            // Parse subsequent sections (True:, False:, or variant-labeled sections) until 'endif'
            while (i < lineCount)
            {
                var raw = lines[i];
                var trimmed = raw.Trim();

                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("//") || trimmed.StartsWith("#")) { i++; continue; }

                if (trimmed.Equals("endif", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                if (trimmed.EndsWith(":"))
                {
                    var sectionName = trimmed.Substring(0, trimmed.Length - 1).Trim();
                    i++;

                    // Collect lines for this section
                    var sectionLines = new List<string>();
                    while (i < lineCount)
                    {
                        var s = lines[i].Trim();
                        if (string.IsNullOrEmpty(s) || s.StartsWith("//") || s.StartsWith("#")) { i++; continue; }
                        if (s.EndsWith(":") || s.Equals("endif", StringComparison.OrdinalIgnoreCase)) break;
                        sectionLines.Add(lines[i]);
                        i++;
                    }

                    // Map section to branch: True -> true, False -> false, variant name -> true if index 0 else false
                    bool isTrueBranch = false;
                    if (sectionName.Equals("True", StringComparison.OrdinalIgnoreCase))
                    {
                        isTrueBranch = true;
                    }
                    else if (sectionName.Equals("False", StringComparison.OrdinalIgnoreCase))
                    {
                        isTrueBranch = false;
                    }
                    else
                    {
                        var idx = variants.FindIndex(v => v.Equals(sectionName, StringComparison.OrdinalIgnoreCase));
                        if (idx == 0) isTrueBranch = true; else isTrueBranch = false;
                    }

                    // Save main flow last node so branch node creation doesn't alter the main flow (restore after processing branch)
                    var savedMainLast = context.LastNode;

                    // Base positions for the branch first node (relative to the If node; X is computed dynamically so nodes can stack/align like other systems)
                    var branchBaseY = ifNodeSingle.position.y + (isTrueBranch ? -120f : 216f);

                    // Track index inside the branch so we can place nodes using standard spacing (250px per step)
                    int branchIndex = 0;

                    // Parse each line in the section and create nodes or call handlers directly
                    BaseNode prevBranchNode = null;
                    foreach (var secLine in sectionLines)
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
                                    // Position the End node inside the branch
                                    endNode.position = new Vector2(ifNodeSingle.position.x + (branchIndex + 1) * 250f, branchBaseY);

                                    // Attach to branch entry if this is the first branch node
                                    if (ifNodeSingle != null && prevBranchNode == null)
                                    {
                                        var branchOut = ifNodeSingle.GetOutputPort(isTrueBranch ? "_true" : "_false");
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
                                    // Attach to the correct If branch
                                    if (ifNodeSingle != null && prevBranchNode == null)
                                    {
                                        var branchOut = ifNodeSingle.GetOutputPort(isTrueBranch ? "_true" : "_false");
                                        var firstEnter = callNode.GetInputPort("_enter");
                                        if (branchOut != null && firstEnter != null) branchOut.Connect(firstEnter);

                                        // Position the first branch node using standard spacing relative to the If node
                                        callNode.position = new Vector2(ifNodeSingle.position.x + (branchIndex + 1) * 250f, branchBaseY);
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

                                        callNode.position = new Vector2(ifNodeSingle.position.x + (branchIndex + 1) * 250f, branchBaseY);
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
                                node.position = new Vector2(ifNodeSingle.position.x + (branchIndex + 1) * 250f, branchBaseY);
                                branchIndex++;

                                SNILParameterApplier.ApplyParametersToNode(node, parameters, template.Key);
                                AssetDatabase.AddObjectToAsset(node, dialogueGraph);

                                // Attach to branch entry if it's the first node
                                if (ifNodeSingle != null && prevBranchNode == null)
                                {
                                    var branchOut = ifNodeSingle.GetOutputPort(isTrueBranch ? "_true" : "_false");
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
                            SNILDebug.LogWarning($"Unrecognized instruction inside If Show Variant block: {sTrim}");
                        }
                    }

                    // restore the main flow last node so branch processing doesn't interfere with subsequent top-level nodes
                    context.LastNode = savedMainLast;

                    continue; // continue main loop from the current i (we've already advanced)
                }

                // If we get here, it's an unexpected line outside a labeled section - skip it
                i++;
            }

            // Update the caller's currentLineIndex to the last line we consumed
            currentLineIndex = i;

            SNILDebug.Log($"IfShowVariant created nodes for variants [{string.Join(", ", variants)}]");

            return InstructionResult.Ok();
        }
    }
} 
