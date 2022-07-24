using System.Collections;
using NUnit.Framework;
using SimpleAndLazy.Editor.Public;
using UnityEngine;
using UnityEngine.TestTools;

namespace SimpleAndLazy.Tests.Editor
{
    public class InvokeTest
    {
        [UnityTest]
        public IEnumerator CoEcho()
        {
            UniTerm term = new UniTerm();
            term.Name = "My Command";
            term.ShellType = ShellType.ZSH;
            term.Input = "echo hello";
            term.WorkingPath = "${projectPath}";
            var task = term.Run();
            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"InvokeTest.CoEcho {task.Result}");

            yield break;
        }
    }
}