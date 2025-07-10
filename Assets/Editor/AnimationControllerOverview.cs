using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Text;

[CustomEditor(typeof(AnimatorController))]
public class AnimatorControllerInspectorButton : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Copy Animator Controller Overview (with Transitions) to Clipboard"))
        {
            var controller = target as AnimatorController;
            if (controller == null) return;

            StringBuilder sb = new StringBuilder();

            foreach (var layer in controller.layers)
            {
                sb.AppendLine($"Layer: {layer.name}");
                var stateMachine = layer.stateMachine;
                foreach (var state in stateMachine.states)
                {
                    sb.AppendLine($"  State: {state.state.name}");
                    foreach (var transition in state.state.transitions)
                    {
                        sb.AppendLine($"    -> {transition.destinationState.name}");
                        sb.AppendLine($"      hasExitTime: {transition.hasExitTime}");
                        sb.AppendLine($"      exitTime: {transition.exitTime}");
                        sb.AppendLine($"      duration: {transition.duration}");
                        sb.AppendLine($"      hasFixedDuration: {transition.hasFixedDuration}");
                        sb.AppendLine($"      conditions:");
                        foreach (var cond in transition.conditions)
                        {
                            sb.AppendLine($"        - parameter: {cond.parameter}, mode: {cond.mode}, threshold: {cond.threshold}");
                        }
                    }
                }
                // Any State transitions
                foreach (var transition in stateMachine.anyStateTransitions)
                {
                    sb.AppendLine($"  AnyState -> {transition.destinationState.name}");
                    sb.AppendLine($"    hasExitTime: {transition.hasExitTime}");
                    sb.AppendLine($"    exitTime: {transition.exitTime}");
                    sb.AppendLine($"    duration: {transition.duration}");
                    sb.AppendLine($"    hasFixedDuration: {transition.hasFixedDuration}");
                    sb.AppendLine($"    conditions:");
                    foreach (var cond in transition.conditions)
                    {
                        sb.AppendLine($"      - parameter: {cond.parameter}, mode: {cond.mode}, threshold: {cond.threshold}");
                    }
                }
            }

            EditorGUIUtility.systemCopyBuffer = sb.ToString();
            EditorUtility.DisplayDialog("Animator Controller Overview", "Overview with transition parameters copied to clipboard!", "OK");
        }
    }
}
