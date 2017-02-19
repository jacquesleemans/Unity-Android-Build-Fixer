using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;

public class AndroidFixerEditorWindow : EditorWindow {

	/// <summary>
	/// All the aar Files, these can be renamed to .zip to view the context
	/// </summary>
	private FileInfo[] m_aarFiles;
	/// <summary>
	/// All the XML files, we are mostly interested in min
	/// </summary>
	private FileInfo[] m_xmlFiles;
	/// <summary>
	/// All the JAR files
	/// </summary>
	private FileInfo[] m_jarFiles;

	private bool m_showArrFiles = true;
	private bool m_showJarFiles = true;
	private bool m_showXmlFiles = true;

	private int m_minSDKVersion = 15;

	[MenuItem("XGameDev/Android Fixer")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(AndroidFixerEditorWindow));
	}

	void Update()
	{
		Repaint();
	}

	void OnGUI()
	{
		if (GUILayout.Button("Check Android"))
		{
			string projectPath = Application.dataPath;

			getAllFilesInDir (projectPath, "*.aar", ref m_aarFiles);
			getAllFilesInDir (projectPath, "*.jar", ref m_jarFiles);
			getAllFilesInDir (projectPath, "AndroidManifest.xml", ref m_xmlFiles);
		}

		GUILayout.Space (16);

		m_showArrFiles = EditorGUILayout.Toggle("Show Aar Files", m_showArrFiles);

		GUILayout.Space (16);

		if (m_showArrFiles) {

			if (GUILayout.Button (string.Format("Check conflicts",m_minSDKVersion))) {
				checkAarFiles ();
			}

			if (m_aarFiles != null) {
				for (int i = 0; i < m_aarFiles.Length; i++) {
					EditorGUILayout.LabelField (m_aarFiles [i].Name, EditorStyles.boldLabel);
				}
			}
			GUILayout.Space (16);
		}

		m_showJarFiles = EditorGUILayout.Toggle("Show JAR Files", m_showJarFiles);

		GUILayout.Space (16);

		if (m_showJarFiles) {

			if (GUILayout.Button (string.Format("Check conflicts",m_minSDKVersion))) {
				checkJarFiles ();
			}

			if (m_jarFiles != null) {
				for (int i = 0; i < m_jarFiles.Length; i++) {
					EditorGUILayout.LabelField (m_jarFiles [i].Name, EditorStyles.boldLabel);
				}
			}
			GUILayout.Space (16);
		}

		m_showXmlFiles = EditorGUILayout.Toggle("Show AndroidManifest Files", m_showXmlFiles);

		GUILayout.Space (16);

		if (m_showXmlFiles) {

			if (m_xmlFiles != null) {

				if (GUILayout.Button (string.Format("Change minSDKVersion to {0}",m_minSDKVersion))) {
					changeMinSdkVersionInXMl ();
				}

				for (int i = 0; i < m_xmlFiles.Length; i++) {

					int idx = m_xmlFiles [i].FullName.IndexOf ("Assets");

					string path = m_xmlFiles [i].Name;

					if (idx >= 0) {
						path = m_xmlFiles [i].FullName.Substring (idx);
					}

					EditorGUILayout.LabelField (path, EditorStyles.boldLabel);
					EditorGUILayout.LabelField (string.Format("minSdkVersion {0}",processXml (m_xmlFiles [i].FullName)), EditorStyles.boldLabel);
				}
			}
		}
	}

	/// <summary>
	/// Gets all files in dir.
	/// </summary>
	/// <param name="dirPath">Dir path.</param>
	/// <param name="filter">Filter.</param>
	/// <param name="fileArr">File arr.</param>
	void getAllFilesInDir(string dirPath, string filter, ref FileInfo[] fileArr){
		DirectoryInfo dir = new DirectoryInfo(dirPath);
		fileArr = dir.GetFiles(filter,SearchOption.AllDirectories);


	}

	#region AAR
	private void checkAarFiles(){
		Dictionary<string,string> files = new Dictionary<string, string> ();

		for (int i = 0; i < m_aarFiles.Length; i++) {
			int idx = m_aarFiles [i].Name.LastIndexOf ("-");

			string name = m_aarFiles [i].Name;

			if (idx >= 0) {
				name = m_aarFiles [i].Name.Substring (0,idx);
			}

			if (!files.ContainsKey (name)) {
				files.Add (name, m_aarFiles [i].FullName);
			} else {
				Debug.LogError (name + " already added");
			}
		}
	}
	#endregion

	#region JAR
	private void checkJarFiles(){
		Dictionary<string,string> files = new Dictionary<string, string> ();

		for (int i = 0; i < m_jarFiles.Length; i++) {
			int idx = m_jarFiles [i].Name.LastIndexOf ("-");

			string name = m_jarFiles [i].Name;

			if (idx >= 0) {
				name = m_jarFiles [i].Name.Substring (0,idx);
			}

			if (!files.ContainsKey (name)) {
				files.Add (name, m_jarFiles [i].FullName);
			} else {
				Debug.LogError (name + " already added");
			}
		}
	}
	#endregion

	#region XML
	/// <summary>
	/// Processes the xml.
	/// </summary>
	/// <returns>The xml.</returns>
	/// <param name="pathToXml">Path to xml.</param>
	string processXml(string pathToXml){

		//string pathToXml = m_xmlFiles[0].FullName;

		XmlDocument doc = new XmlDocument();
		doc.Load (pathToXml);

		if (doc == null) {
			return string.Empty;
		}

		XmlNode root = doc.DocumentElement;

		XmlNode usesSdkNode = root.SelectSingleNode( "uses-sdk" ); 

		if (usesSdkNode != null) {

			XmlAttribute att = usesSdkNode.Attributes ["android:minSdkVersion"];

			if (att != null) {

				int minSdkVersion = int.Parse (att.Value);

				if (minSdkVersion > m_minSDKVersion) {
					m_minSDKVersion = minSdkVersion;
				}

				return att.Value;
			} else {
				return string.Empty;
			}

			//return usesSdkNode.Attributes ["android:minSdkVersion"].Value;
		}

		return string.Empty;
	}

	private void changeMinSdkVersionInXMl(){
		
		for (int i = 0; i < m_xmlFiles.Length; i++) {

			XmlDocument doc = new XmlDocument();
			doc.Load (m_xmlFiles[i].FullName);

			XmlNode root = doc.DocumentElement;

			XmlNode usesSdkNode = root.SelectSingleNode( "uses-sdk" ); 

			if (usesSdkNode != null) {

				XmlAttribute att = usesSdkNode.Attributes ["android:minSdkVersion"];

				if (att != null) {

					att.Value = m_minSDKVersion.ToString ();
				} else {
					
				}
			}

			//Debug.Log (m_xmlFiles[i].FullName);
			//doc.Save (m_xmlFiles[i].Name);

			doc.Save (m_xmlFiles[i].FullName);
		}
	}
	#endregion
}