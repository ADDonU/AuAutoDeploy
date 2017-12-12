using System;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using UnityEngine;
using System.IO;

class BuildAutomation
{

    #region platforms

    [MenuItem("File/build/macOS 32bit", false, 100)]
    static void MacOSX32()
    {
        GenericBuild(
            Combine("target", "mac-32", "ummorpg.app"),
            BuildTargetGroup.Standalone,
            BuildTarget.StandaloneOSXIntel,
            BuildOptions.Development);

    }

    [MenuItem("File/build/macOS 64bit", false, 101)]
    static void MacOSX64()
    {
        GenericBuild(
            Combine("target", "mac-64", "ummorpg.app"),
            BuildTargetGroup.Standalone,
            BuildTarget.StandaloneOSXIntel64,
            BuildOptions.Development);
    }

    [MenuItem("File/build/Windows 32bit", false, 200)]
    static void Windows32()
    {
        GenericBuild(
            Combine("target", "win-32", "ummorpg.exe"),
            BuildTargetGroup.Standalone,
            BuildTarget.StandaloneWindows,
            BuildOptions.Development);
    }

    [MenuItem("File/build/Windows 64bit", false, 201)]
    static void Windows64()
    {
        GenericBuild(
            Combine("target", "win-64", "ummorpg.exe"),
            BuildTargetGroup.Standalone,
            BuildTarget.StandaloneWindows64,
            BuildOptions.Development);
    }

    [MenuItem("File/build/Linux 32bit", false, 300)]
    static void Linux32()
    {
        GenericBuild(
            Combine("target", "linux-32", "ummorpg"),
            BuildTargetGroup.Standalone,
            BuildTarget.StandaloneLinux,
            BuildOptions.Development);
    }

    [MenuItem("File/build/Linux 64bit", false, 301)]
    static void Linux64()
    {
        GenericBuild(
            Combine("target", "linux-64", "ummorpg"),
            BuildTargetGroup.Standalone,
            BuildTarget.StandaloneLinux64,
            BuildOptions.Development);
    }

    [MenuItem("File/build/Linux Server", false, 302)]
    static void LinuxServer()
    {
        GenericBuild(
            Combine("target", "linux-server", "ummorpg"),
            BuildTargetGroup.Standalone,
            BuildTarget.StandaloneLinux64,
            BuildOptions.EnableHeadlessMode);
    }


    [MenuItem("File/build/Android x86", false, 400)]
    static void Androidx86()
    {
        PlayerSettings.Android.targetDevice = AndroidTargetDevice.x86;
        GenericBuild(
            Combine("target", "android-x86", "ummorpg.apk"),
            BuildTargetGroup.Android,
            BuildTarget.Android,
            BuildOptions.Development);
    }

    [MenuItem("File/build/Android ARM", false, 401)]
    static void AndroidARM()
    {
        PlayerSettings.Android.targetDevice = AndroidTargetDevice.ARMv7;
        GenericBuild(
            Combine("target", "android-arm", "ummorpg.apk"),
            BuildTargetGroup.Android,
            BuildTarget.Android,
            BuildOptions.Development);
    }
    [MenuItem("File/build/iOS", false, 403)]
    static void AndroidIOS()
    {
        GenericBuild(
            Combine("target", "android-ios", "ummorpg"),
            BuildTargetGroup.iOS,
            BuildTarget.iOS,
            BuildOptions.Development);
    }

    [MenuItem("File/build/WebGL", false, 500)]
    static void WebGL()
    {
        GenericBuild(
            Combine("target", "webgl", "ummorpg"),
            BuildTargetGroup.WebGL,
            BuildTarget.WebGL,
            BuildOptions.Development);
    }

    #endregion

    #region common


    private static String Combine(params string[] paths)
    {
        return paths.Aggregate(Path.Combine);
    }

    private static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }



    static void GenericBuild(string path, BuildTargetGroup targetGroup, BuildTarget build_target, BuildOptions build_options)
    {
        string[] scenes = FindEnabledEditorScenes();

        EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, build_target);
        string res = BuildPipeline.BuildPlayer(scenes, path, build_target, build_options);
        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }
    }

    #endregion

}