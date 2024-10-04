using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

[InitializeOnLoad] //エディター起動時に初期化されるように
public static class CompilationPipelineExample
{
    private const bool ShowLog = false;

    //=================================================================================
    //初期化
    //=================================================================================

    /// <summary>
    /// コンストラクタ(InitializeOnLoad属性によりエディター起動時に呼び出される)
    /// </summary>
    static CompilationPipelineExample()
    {
        //CompilationPipelineの各イベントにメソッド登録
        CompilationPipeline.assemblyCompilationStarted += OnAssemblyCompilationStarted;
        CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
    }

    //=================================================================================
    //各コンパイル開始と終了の検知
    //=================================================================================

    //各コンパイル開始時に呼ばれる
    private static void OnAssemblyCompilationStarted(object obj)
    {
        if (ShowLog) Debug.Log(obj + "のコンパイル開始");
    }

    //各コンパイル終了時に呼ばれる
    private static void OnAssemblyCompilationFinished(object obj, CompilerMessage[] messages)
    {
        if (ShowLog) Debug.Log(obj + "のコンパイル終了");

        //警告やエラー等のメッセージがあればログで表示
        foreach (CompilerMessage compilerMessage in messages)
        {
            if (ShowLog) Debug.Log(compilerMessage.type.ToString() + "\n" + compilerMessage.message);
        }
    }
}