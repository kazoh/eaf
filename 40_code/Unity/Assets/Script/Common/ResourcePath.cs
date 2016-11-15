using UnityEngine;
using System.Collections;

/// <summary>
/// 리소스의 경로를 관리하는 클래스.
/// </summary>
public class ResourcePath {

	public enum ResourceType
    {
        Character,
    }

	public static string GetPath(ResourceType type, string name)
    {
		return string.Format("{0}/{1}",type.ToString(), name);
    }
}
