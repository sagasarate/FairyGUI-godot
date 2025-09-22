using FairyGUI;
using Godot;
using System;

namespace FairyGUI
{
	public partial class NContainer : Control, IDisplayObject
	{
		// Called when the node enters the scene tree for the first time.
		protected Control _mask;
		public GObject gOwner { get; set; }
		public IDisplayObject parent { get { return GetParent() as IDisplayObject; } }
		public Control node { get { return this; } }
		public bool visible { get { return Visible; } set { Visible = value; } }
		public float skewX { get; set; }
		public float skewY { get; set; }
		public Vector2 position { get { return Position; } set { Position = value; } }
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

		public Control mask
		{
			get { return _mask; }
			set
			{
				if (_mask != value)
				{
					if (value == null)
					{
						if (_mask is NImage image)
						{
							image.maskOwner = null;
							image.QueueRedraw();
						}
						else if (_mask is NShape shape)
						{
							shape.maskOwner = null;
							shape.QueueRedraw();
						}
					}
					_mask = value;
					if (_mask != null)
					{
						ClipChildren = ClipChildrenMode.Only;
						TextureRepeat = _mask.TextureRepeat;
						if (_mask is NImage image)
						{
							image.maskOwner = this;
							image.QueueRedraw();
						}
						else if (_mask is NShape shape)
						{
							shape.maskOwner = this;
							shape.QueueRedraw();
						}
					}
					else
					{
						ClipChildren = ClipChildrenMode.Disabled;
					}
					QueueRedraw();
				}
			}
		}
		public bool reversedMask
		{
			get
			{
				if (_mask is NImage image)
				{
					return image.reverseMask;
				}
				else if (_mask is NShape shape)
				{
					return shape.reverseMask;
				}
				return false;
			}
			set
			{
				if (_mask is NImage image)
				{
					image.reverseMask = value;
				}
				else if (_mask is NShape shape)
				{
					shape.reverseMask = value;
				}
			}
		}
		public override void _Draw()
		{
			if (_mask != null)
			{
				if (_mask is NImage image)
				{
					Transform2D trans = image.GetTransform();
					image.UpdateMesh();
					DrawMesh(image.mesh, image.drawTexture, trans);
					if (image.outBoundMesh != null)
						DrawMesh(image.outBoundMesh, null, trans);
				}
				else if (_mask is NShape shape)
				{
					Transform2D trans = shape.GetTransform();
					shape.UpdateMesh();
					DrawMesh(shape.mesh, shape.texture?.nativeTexture, trans);
				}
			}
		}
	}
}