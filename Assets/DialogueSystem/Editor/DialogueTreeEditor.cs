using Simple.DialogueTree.Editor.Views;
using Simple.DialogueTree.Nodes;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Simple.DialogueTree.Editor
{
    /// <summary>
    /// Derive from <a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/EditorWindow.html" rel="external">UnityEditor.EditorWindow</a> class to create an editor window to Edit Dialogue Tree Scriptable Objects.
    /// Requires file named "DialogueTreeEditor.uxml" to be in an Editor Resources Folder
    /// Uses Visual Elements requires a <see cref="DialogueTreeView"/> an an <a href="https://docs.unity3d.com/ScriptReference/UIElements.IMGUIContainer.html" rel="external">UnityEngine.UIElements.IMGUIContainer</a> with a name of InspectorView.
    /// </summary>
    public class DialogueTreeEditor : EditorWindow
    {
        /// <value> The <see cref="TreeView"/> associated with this view. </value>
        private DialogueTreeView m_treeView;

        /// <value> The <a href="https://docs.unity3d.com/ScriptReference/UIElements.IMGUIContainer.html" rel="external">UnityEngine.UIElements.IMGUIContainer</a> associated with the view. </value>
        private IMGUIContainer m_inspectorView;

        /// <value> The <a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/Editor.html" rel="external">UnityEditor.Editor</a> associated with this view. </value>
        private UnityEditor.Editor m_editor;

        public static System.Action UpdateDialogueTreeEditorManuallyEvent;

        /// <summary>
        /// Adds a Entry to Window/Behavior Tree/Editor
        /// Will Open the Behavior Tree Editor to Edit Behavior Trees
        /// </summary>
        [MenuItem("Window/Dialogue/Editor")]
        public static void OpenTreeEditor()
        {
            Debug.Log("OpenTreeEditor");
            GetWindow<DialogueTreeEditor>("Dialogue Tree Editor");
        }

        /// <summary>
        /// Use Unity Editor Call Back On Open Asset.
        /// </summary>
        /// <returns>True if this method handled the asset. Else return false.</returns>
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            if (Selection.activeObject as DialogueTree == null) return false;
            OpenTreeEditor();
            return true;
        }

        /// <summary>
        /// CreateGUI is called when the EditorWindow's rootVisualElement is ready to be populated.
        ///
        /// Clones a Visual Tree Located in an Editor Resources Folder DialogueTreeEditor.uxml";
        /// </summary>
        private void CreateGUI()
        {
            Debug.Log("CreateGUI");
            //VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>("DialogueTreeEditor.uxml");
            VisualTreeAsset vt = Resources.Load<VisualTreeAsset>("DialogueTreeEditor");
            vt.CloneTree(rootVisualElement);

            m_treeView = rootVisualElement.Q<DialogueTreeView>();
            m_inspectorView = rootVisualElement.Q<IMGUIContainer>("InspectorView");
            m_treeView.onNodeSelected = OnNodeSelectionChange;

            OnSelectionChange();
        }

        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnplayModeStateChanged;
            EditorApplication.playModeStateChanged += OnplayModeStateChanged;

            UpdateDialogueTreeEditorManuallyEvent -= OnSelectionChange;
            UpdateDialogueTreeEditorManuallyEvent += OnSelectionChange;
        }

        /// <summary>
        /// This function is called when the scriptable object goes out of scope.
        /// </summary>
        private void OnDisable()
        {
            Debug.Log("Desotry");
            EditorApplication.playModeStateChanged -= OnplayModeStateChanged;
        }

        private void OnDestroy()
        {
            UpdateDialogueTreeEditorManuallyEvent -= OnSelectionChange;            
        }

        /// <summary>
        /// Called whenever the selection has changed.
        ///
        /// If the Selected Object is a Behavior Tree Binds the Tree SO to the root element and populates the tree view.
        /// </summary>
        private void OnSelectionChange()
        {
            DialogueTree tree = Selection.activeObject as DialogueTree;
            if (tree == null)
            {
                if (Selection.activeGameObject)
                {
                    DialogueTreeRunner treeRunner = Selection.activeGameObject.GetComponent<DialogueTreeRunner>();
                    if (treeRunner)
                    {
                        tree = treeRunner.tree;
                    }
                }
            }

            if (tree != null)
            {
                //CanOpenAssetInEditor
                if (Application.isPlaying || AssetDatabase.OpenAsset(tree.GetInstanceID()))
                {
                    SerializedObject so = new SerializedObject(tree);
                    rootVisualElement.Bind(so);
                    if (m_treeView != null)
                        m_treeView.PopulateView(tree);

                    return;
                }
            }

            rootVisualElement.Unbind();

            TextField textField = rootVisualElement.Q<TextField>("DialogueTreeName");
            if (textField != null)
            {
                textField.value = string.Empty;
            }
        }

        /// <summary>
        /// Method registered to <a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/EditorApplication-playModeStateChanged.html" rel="external">UnityEditor.EditorApplication.playModeStateChanged</a>
        /// </summary>
        /// <param name="obj">The <a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/PlayModeStateChange.html" rel="external">UnityEditor.PlayModeStateChange</a> object.</param>
        private void OnplayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                // Occurs during the next update of the Editor application if it is in edit mode and was previously in play mode.
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                // Occurs when exiting edit mode, before the Editor is in play mode.
                case PlayModeStateChange.ExitingEditMode:
                    break;
                // Occurs during the next update of the Editor application if it is in play mode and was previously in edit mode.
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                // Occurs when exiting play mode, before the Editor is in edit mode.
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        /// <summary>
        /// Used to Observer the tree view for when a Node is Selected.
        /// Causes the Node to display in the Inspector View.
        /// </summary>
        /// <param name="node">The Selected Node</param>
        private void OnNodeSelectionChange(DialogueNode node)
        {
            m_inspectorView.Clear();
            DestroyImmediate(m_editor);
            m_editor = UnityEditor.Editor.CreateEditor(node);
            m_inspectorView.onGUIHandler = () =>
            {
                if (m_editor.target)
                    m_editor.OnInspectorGUI();
            };
        }
    }
}