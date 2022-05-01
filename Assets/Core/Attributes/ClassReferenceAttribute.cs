using System;
using UnityEngine;

[Serializable]
public class ClassReferenceAttribute : PropertyAttribute {
	public Type type;

	public ClassReferenceAttribute(Type type) {
		this.type = type;
	}
}