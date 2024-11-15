using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

public class GraphConfigurationTest
{
    [SetUp]
    public void Setup()
    {
        EditorSceneManager.OpenScene("Assets\\Wave\\Demo\\Stage\\Demo-Stage-ICE.unity");
    }
    

    [TearDown]
    public void Teardown()
    {
        EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
    }
    // A Test behaves as an ordinary method
    [Test]
    public void GraphBuild()
    {
        var debugger = Object.FindFirstObjectByType<QuantumRunnerLocalDebug>();
        Assert.True(debugger != null, "QuantumRunnerLocalDebug not found");
        QuantumRunner.Init();
    }
}
