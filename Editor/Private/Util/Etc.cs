#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SimpleAndLazy.Editor
{
	internal class Etc
	{
		public static void CopyToClipboard( string s )
		{
			TextEditor te = new TextEditor();
			te.text = s;
			te.SelectAll();
			te.Copy();
		}

		public static string GetProjectPath()
		{
			return Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
		}

		public static string GetSelectedPathOrFallback()
		{
			string path = "Assets";

			foreach ( UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets) )
			{
				path = AssetDatabase.GetAssetPath(obj);
				if ( path.StartsWith("Packages") )
				{
					var splited = path.Split('/');
					var tails = "";
					for ( int i = 2; i < splited.Length; i++ )
					{
						tails = Path.Combine(tails, splited[i]);
					}

					var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(path);
					var absolutePath = packageInfo.resolvedPath;
					path = Path.Combine(absolutePath, tails);
					break;
				}

				if ( !string.IsNullOrEmpty(path) && File.Exists(path) )
				{
					path = Path.GetDirectoryName(path);
					break;
				}
			}

			return path;
		}
	}
}
#endif