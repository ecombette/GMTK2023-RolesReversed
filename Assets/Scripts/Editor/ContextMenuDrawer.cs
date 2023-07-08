using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[CustomEditor(typeof(UnityEngine.Object), true, isFallback = true)]
[CanEditMultipleObjects]
public class ContextMenuDrawer : Editor
{ 
	protected static bool FORCE_INIT = false;
	[DidReloadScripts]
	private static void HandleScriptReload()
	{
		FORCE_INIT = true;

		EditorApplication.delayCall = () => { EditorApplication.delayCall = () => { FORCE_INIT = false; }; };
	}

	private static GUIStyle styleHighlight;

	public bool isSubEditor;

	private readonly GUILayoutOption uiExpandWidth = GUILayout.ExpandWidth(true);
	private readonly GUILayoutOption uiWidth50 = GUILayout.Width(50);
	private readonly GUIContent labelBtnCreate = new GUIContent("Create");
	private GUIStyle styleEditBox;

	private readonly Dictionary<string, Editor> editableIndex = new Dictionary<string, Editor>();

	protected bool alwaysDrawInspector = false;
	protected bool isInitialized = false;
	protected bool hasEditable = false;

	protected struct ContextMenuData
	{
		public string menuItem;
		public MethodInfo function;
		public MethodInfo validate;

		public ContextMenuData(string item)
		{
			menuItem = item;
			function = null;
			validate = null;
		}
	}

	protected Dictionary<string, ContextMenuData> contextData = new Dictionary<string, ContextMenuData>();


	#region Initialization
	private void OnEnable()
	{
		InitInspector();
	}

	protected virtual void InitInspector(bool force)
	{
		if (force)
			isInitialized = false;
		InitInspector();
	}

	protected virtual void InitInspector()
	{
		if (isInitialized && FORCE_INIT == false)
			return;

		if(styleEditBox != null && EditorStyles.helpBox != null)
			styleEditBox = new GUIStyle(EditorStyles.helpBox) { padding = new RectOffset(5, 5, 5, 5) };

		FindContextMenu();
	}

	private IEnumerable<MethodInfo> GetAllMethods(Type t)
	{
		if (t == null)
			return Enumerable.Empty<MethodInfo>();
		var binding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		return t.GetMethods(binding).Concat(GetAllMethods(t.BaseType));
	}

	private void FindContextMenu()
	{
		contextData.Clear();

		// Get context menu
		Type targetType = target.GetType();
		Type contextMenuType = typeof(ContextMenu);
		MethodInfo[] methods = GetAllMethods(targetType).ToArray();
		for (int index = 0; index < methods.GetLength(0); ++index)
		{
			MethodInfo methodInfo = methods[index];
			foreach (ContextMenu contextMenu in methodInfo.GetCustomAttributes(contextMenuType, false))
			{
				if (contextData.ContainsKey(contextMenu.menuItem))
				{
					var data = contextData[contextMenu.menuItem];
					if (contextMenu.validate)
						data.validate = methodInfo;
					else
						data.function = methodInfo;
					contextData[data.menuItem] = data;
				}
				else
				{
					var data = new ContextMenuData(contextMenu.menuItem);
					if (contextMenu.validate)
						data.validate = methodInfo;
					else
						data.function = methodInfo;
					contextData.Add(data.menuItem, data);
				}
			}
		}
	}
	#endregion

	protected bool InspectorGUIStart(bool force = false)
	{
		// Not initialized, try initializing
		if (hasEditable && editableIndex.Count == 0)
			InitInspector();

		// No sortable arrays or list index unintialized
		bool cannotDrawEditable = (hasEditable == false || editableIndex.Count == 0);
		if (cannotDrawEditable && force == false)
		{
			if (isSubEditor)
				DrawPropertiesExcluding(serializedObject, "m_Script");
			else
				base.OnInspectorGUI();

			DrawContextMenuButtons();
			return false;
		}

		serializedObject.Update();
		return true;
	}

	protected virtual void DrawInspector()
	{
		DrawPropertiesAll();
	}

	public override void OnInspectorGUI()
	{
		if (InspectorGUIStart(alwaysDrawInspector) == false)
			return;

		EditorGUI.BeginChangeCheck();

		DrawInspector();

		if (EditorGUI.EndChangeCheck())
		{
			serializedObject.ApplyModifiedProperties();
			InitInspector(true);
		}

		DrawContextMenuButtons();
	}

	protected enum IterControl
	{
		Draw,
		Continue,
		Break
	}

	protected void IterateDrawProperty(SerializedProperty property, Func<IterControl> filter = null)
	{
		if (property.NextVisible(true))
		{
			// Remember depth iteration started from
			int depth = property.Copy().depth;
			do
			{
				// If goes deeper than the iteration depth, get out
				if (property.depth != depth)
					break;
				if (isSubEditor && property.name.Equals("m_Script"))
					continue;

				if (filter != null)
				{
					var filterResult = filter();
					if (filterResult == IterControl.Break)
						break;
					if (filterResult == IterControl.Continue)
						continue;
				}
			} while (property.NextVisible(false));
		}
	}

	#region Helper functions
	/// <summary>
	/// Draw the default inspector, with the sortable arrays
	/// </summary>
	public void DrawPropertiesAll()
	{
		SerializedProperty iterProp = serializedObject.GetIterator();
		IterateDrawProperty(iterProp);
	}

	/// <summary>
	/// Draw the default inspector, except for the given property names
	/// </summary>
	/// <param name="propertyNames"></param>
	public void DrawPropertiesExcept(params string[] propertyNames)
	{
		SerializedProperty iterProp = serializedObject.GetIterator();

		IterateDrawProperty(iterProp,
			filter: () =>
			{
				if (propertyNames.Contains(iterProp.name))
					return IterControl.Continue;
				return IterControl.Draw;
			});
	}

	/// <summary>
	/// Draw the default inspector, starting from a given property
	/// </summary>
	/// <param name="propertyStart">Property name to start from</param>
	public void DrawPropertiesFrom(string propertyStart)
	{
		bool canDraw = false;
		SerializedProperty iterProp = serializedObject.GetIterator();
		IterateDrawProperty(iterProp,
			filter: () =>
			{
				if (iterProp.name.Equals(propertyStart))
					canDraw = true;
				if (canDraw)
					return IterControl.Draw;
				return IterControl.Continue;
			});
	}

	/// <summary>
	/// Draw the default inspector, up to a given property
	/// </summary>
	/// <param name="propertyStop">Property name to stop at</param>
	public void DrawPropertiesUpTo(string propertyStop)
	{
		SerializedProperty iterProp = serializedObject.GetIterator();
		IterateDrawProperty(iterProp,
			filter: () =>
			{
				if (iterProp.name.Equals(propertyStop))
					return IterControl.Break;
				return IterControl.Draw;
			});
	}

	/// <summary>
	/// Draw the default inspector, starting from a given property to a stopping property
	/// </summary>
	/// <param name="propertyStart">Property name to start from</param>
	/// <param name="propertyStop">Property name to stop at</param>
	public void DrawPropertiesFromUpTo(string propertyStart, string propertyStop)
	{
		bool canDraw = false;
		SerializedProperty iterProp = serializedObject.GetIterator();
		IterateDrawProperty(iterProp,
			filter: () =>
			{
				if (iterProp.name.Equals(propertyStop))
					return IterControl.Break;

				if (iterProp.name.Equals(propertyStart))
					canDraw = true;

				if (canDraw == false)
					return IterControl.Continue;

				return IterControl.Draw;
			});
	}

	public void DrawContextMenuButtons()
	{
		if (contextData.Count == 0) return;

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Context Menu", EditorStyles.boldLabel);
		foreach (KeyValuePair<string, ContextMenuData> kv in contextData)
		{
			bool enabledState = GUI.enabled;
			bool isEnabled = true;
			if (kv.Value.validate != null)
				isEnabled = (bool)kv.Value.validate.Invoke(target, null);

			GUI.enabled = isEnabled;
			if (GUILayout.Button(kv.Key) && kv.Value.function != null)
			{
				kv.Value.function.Invoke(target, null);
			}
			GUI.enabled = enabledState;
		}
	}
	#endregion
}
