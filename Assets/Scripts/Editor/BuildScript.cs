using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildScript
{
    static void PerformBuild()
    {
        string[] defaultScene = { "Assets/Scenes/Level_1.unity" };
        BuildPipeline.BuildPlayer(defaultScene, "./build/game.x86_64",
            BuildTarget.StandaloneWindows64, BuildOptions.None);
    }
}
