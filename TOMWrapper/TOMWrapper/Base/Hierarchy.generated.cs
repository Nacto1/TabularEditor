// Code generated by a template
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using TabularEditor.PropertyGridUI;
using TabularEditor.UndoFramework;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
  
    /// <summary>
	/// Base class declaration for Hierarchy
	/// </summary>
	[TypeConverter(typeof(DynamicPropertyConverter))]
	public partial class Hierarchy: TabularNamedObject, IDetailObject, IHideableObject, ITabularTableObject, IDescriptionObject, IAnnotationObject
	{
	    protected internal new TOM.Hierarchy MetadataObject { get { return base.MetadataObject as TOM.Hierarchy; } internal set { base.MetadataObject = value; } }

		public Hierarchy(Table parent) : base(parent.Handler, new TOM.Hierarchy(), false) {
			MetadataObject.Name = parent.MetadataObject.Hierarchies.GetNewName("New Hierarchy");
			parent.Hierarchies.Add(this);
			Init();
		}

		public Hierarchy(TabularModelHandler handler, TOM.Hierarchy hierarchyMetadataObject) : base(handler, hierarchyMetadataObject)
		{
		}
		public string GetAnnotation(string name) {
		    return MetadataObject.Annotations.Find(name)?.Value;
		}
		public void SetAnnotation(string name, string value, bool undoable = true) {
			if(MetadataObject.Annotations.Contains(name)) {
				MetadataObject.Annotations[name].Value = value;
			} else {
				MetadataObject.Annotations.Add(new TOM.Annotation{ Name = name, Value = value });
			}
			if (undoable) Handler.UndoManager.Add(new UndoAnnotationAction(this, name, value));
		}
		        /// <summary>
        /// Gets or sets the Description of the Hierarchy.
        /// </summary>
		[DisplayName("Description")]
		[Category("Basic"),IntelliSense("The Description of this Hierarchy.")]
		public string Description {
			get {
			    return MetadataObject.Description;
			}
			set {
				var oldValue = Description;
				if (oldValue == value) return;
				bool undoable = true;
				bool cancel = false;
				OnPropertyChanging("Description", value, ref undoable, ref cancel);
				if (cancel) return;
				MetadataObject.Description = value;
				if(undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, "Description", oldValue, value));
				OnPropertyChanged("Description", oldValue, value);
			}
		}
		private bool ShouldSerializeDescription() { return false; }
        /// <summary>
        /// Collection of localized descriptions for this Hierarchy.
        /// </summary>
        [Browsable(true),DisplayName("Descriptions"),Category("Translations and Perspectives")]
	    public new TranslationIndexer TranslatedDescriptions { get { return base.TranslatedDescriptions; } }
        /// <summary>
        /// Gets or sets the IsHidden of the Hierarchy.
        /// </summary>
		[DisplayName("Hidden")]
		[Category("Basic"),IntelliSense("The Hidden of this Hierarchy.")]
		public bool IsHidden {
			get {
			    return MetadataObject.IsHidden;
			}
			set {
				var oldValue = IsHidden;
				if (oldValue == value) return;
				bool undoable = true;
				bool cancel = false;
				OnPropertyChanging("IsHidden", value, ref undoable, ref cancel);
				if (cancel) return;
				MetadataObject.IsHidden = value;
				if(undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, "IsHidden", oldValue, value));
				OnPropertyChanged("IsHidden", oldValue, value);
				Handler.UpdateObject(this);
			}
		}
		private bool ShouldSerializeIsHidden() { return false; }
        /// <summary>
        /// Gets or sets the State of the Hierarchy.
        /// </summary>
		[DisplayName("State")]
		[Category("Metadata"),IntelliSense("The State of this Hierarchy.")]
		public TOM.ObjectState State {
			get {
			    return MetadataObject.State;
			}
			
		}
		private bool ShouldSerializeState() { return false; }
        /// <summary>
        /// Gets or sets the DisplayFolder of the Hierarchy.
        /// </summary>
		[DisplayName("Display Folder")]
		[Category("Basic"),IntelliSense("The Display Folder of this Hierarchy.")][Editor(typeof(CustomDialogEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string DisplayFolder {
			get {
			    return MetadataObject.DisplayFolder;
			}
			set {
				var oldValue = DisplayFolder;
				if (oldValue == value) return;
				bool undoable = true;
				bool cancel = false;
				OnPropertyChanging("DisplayFolder", value, ref undoable, ref cancel);
				if (cancel) return;
				MetadataObject.DisplayFolder = value;
				if(undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, "DisplayFolder", oldValue, value));
				OnPropertyChanged("DisplayFolder", oldValue, value);
				Handler.UpdateFolders(Table);
			}
		}
		private bool ShouldSerializeDisplayFolder() { return false; }
        /// <summary>
        /// Collection of localized Display Folders for this Hierarchy.
        /// </summary>
        [Browsable(true),DisplayName("Display Folders"),Category("Translations and Perspectives")]
	    public new TranslationIndexer TranslatedDisplayFolders { get { return base.TranslatedDisplayFolders; } }
        /// <summary>
        /// Gets or sets the HideMembers of the Hierarchy.
        /// </summary>
		[DisplayName("Hide Members")]
		[Category("Other"),IntelliSense("The Hide Members of this Hierarchy.")]
		public TOM.HierarchyHideMembersType HideMembers {
			get {
			    return MetadataObject.HideMembers;
			}
			set {
				var oldValue = HideMembers;
				if (oldValue == value) return;
				bool undoable = true;
				bool cancel = false;
				OnPropertyChanging("HideMembers", value, ref undoable, ref cancel);
				if (cancel) return;
				MetadataObject.HideMembers = value;
				if(undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, "HideMembers", oldValue, value));
				OnPropertyChanged("HideMembers", oldValue, value);
			}
		}
		private bool ShouldSerializeHideMembers() { return false; }
		[Browsable(false)]
		public Table Table
		{ 
			get 
			{ 
				TabularObject t = null;
				if(MetadataObject == null || MetadataObject.Table == null) return null;
				if(!Handler.WrapperLookup.TryGetValue(MetadataObject.Table, out t)) {
				    t = Model.Tables[MetadataObject.Table.Name];
				}
				return t as Table;
			} 
		}
    }

	/// <summary>
	/// Collection class for Hierarchy. Provides convenient properties for setting a property on multiple objects at once.
	/// </summary>
	public partial class HierarchyCollection: TabularObjectCollection<Hierarchy, TOM.Hierarchy, TOM.Table>
	{
		public Table Parent { get; private set; }

		public HierarchyCollection(TabularModelHandler handler, string collectionName, TOM.HierarchyCollection metadataObjectCollection, Table parent) : base(handler, collectionName, metadataObjectCollection)
		{
			Parent = parent;

			// Construct child objects (they are automatically added to the Handler's WrapperLookup dictionary):
			foreach(var obj in MetadataObjectCollection) {
				new Hierarchy(handler, obj) { Collection = this };
			}
		}

		[Description("Sets the Description property of all objects in the collection at once.")]
		public string Description {
			set {
				if(Handler == null) return;
				Handler.UndoManager.BeginBatch(UndoPropertyChangedAction.GetActionNameFromProperty("Description"));
				this.ToList().ForEach(item => { item.Description = value; });
				Handler.UndoManager.EndBatch();
			}
		}
		[Description("Sets the IsHidden property of all objects in the collection at once.")]
		public bool IsHidden {
			set {
				if(Handler == null) return;
				Handler.UndoManager.BeginBatch(UndoPropertyChangedAction.GetActionNameFromProperty("IsHidden"));
				this.ToList().ForEach(item => { item.IsHidden = value; });
				Handler.UndoManager.EndBatch();
			}
		}
		[Description("Sets the DisplayFolder property of all objects in the collection at once.")]
		public string DisplayFolder {
			set {
				if(Handler == null) return;
				Handler.UndoManager.BeginBatch(UndoPropertyChangedAction.GetActionNameFromProperty("DisplayFolder"));
				this.ToList().ForEach(item => { item.DisplayFolder = value; });
				Handler.UndoManager.EndBatch();
			}
		}
		[Description("Sets the HideMembers property of all objects in the collection at once.")]
		public TOM.HierarchyHideMembersType HideMembers {
			set {
				if(Handler == null) return;
				Handler.UndoManager.BeginBatch(UndoPropertyChangedAction.GetActionNameFromProperty("HideMembers"));
				this.ToList().ForEach(item => { item.HideMembers = value; });
				Handler.UndoManager.EndBatch();
			}
		}

		public override string ToString() {
			return string.Format("({0} {1})", Count, (Count == 1 ? "Hierarchy" : "Hierarchies").ToLower());
		}
	}
}
