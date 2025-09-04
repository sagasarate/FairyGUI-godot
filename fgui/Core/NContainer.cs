using FairyGUI;
using Godot;
using System;

namespace FairyGUI
{
	public partial class NContainer : Control, IDisplayObject
	{
		// Called when the node enters the scene tree for the first time.
		public GObject gOwner { get; set; }
		public IDisplayObject parent { get { return GetParent() as IDisplayObject; } }
		public Control node { get { return this; } }
		public bool visible { get { return Visible; } set { Visible = value; } }
		public float skewX { get; set; }
		public float skewY { get; set; }
		public BlendMode blendMode { get; set; }
		public event System.Action<double> onUpdate;
		public NContainer(GObject owner)
		{
			gOwner = owner;
			MouseFilter = MouseFilterEnum.Ignore;
		}
		public override void _Process(double delta)
		{
			if (onUpdate != null)
				onUpdate(delta);
		}		
	}
}