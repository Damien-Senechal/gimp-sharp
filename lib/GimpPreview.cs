using System;
using System.Collections;
using System.Runtime.InteropServices;

using Gdk;
using GLib;
using Gtk;

namespace Gimp
  {
    public class GimpPreview : VBox
    {
      public GimpPreview(IntPtr ptr) : base(ptr)
      {
      }

      public bool Update
      {
	get {return gimp_preview_get_update(Handle);}
	set {gimp_preview_set_update(Handle, value);}
      }

      public void SetBounds(int xmin, int ymin, int xmax, int ymax)
      {
	gimp_preview_set_bounds(Handle, xmin, ymin, xmax, ymax);
      }

      public void GetPosition(out int x, out int y)
      {
	gimp_preview_get_position(Handle, out x, out y);
      }

      public void GetSize(out int width, out int height)
      {
	gimp_preview_get_size(Handle, out width, out height);
      }

      public void Draw()
      {
	gimp_preview_draw(Handle);
      }

      public void DrawBuffer(byte[] buffer, int rowstride)
      {
	gimp_preview_draw_buffer(Handle, buffer, rowstride);
      }

      public void Invalidate()
      {
	gimp_preview_invalidate(Handle);
      }

      public Cursor DefaultCursor
      {
	set {gimp_preview_set_default_cursor(Handle, value);}
      }

      [Signal("invalidated")]
      public event EventHandler Invalidated
      {
	add 
	    {
	    if (value.Method.GetCustomAttributes(typeof(ConnectBeforeAttribute), false).Length > 0) 
	      {
	      if (BeforeHandlers["invalidated"] == null)
		BeforeSignals["invalidated"] = new voidObjectSignal(this, "invalidated", value, typeof (System.EventArgs), 0);		 		 		 		 		 else
		  ((SignalCallback) BeforeSignals ["invalidated"]).AddDelegate (value);
	      BeforeHandlers.AddHandler("invalidated", value);
	      } 
	    else 
	      {
	      if (AfterHandlers["invalidated"] == null)
		AfterSignals["invalidated"] = new voidObjectSignal(this, "invalidated", value, typeof (System.EventArgs), 1);		 		 		 		 		 else
		  ((SignalCallback) AfterSignals ["invalidated"]).AddDelegate (value);
	      AfterHandlers.AddHandler("invalidated", value);
	      }
	    }
	remove 
	    {
	    System.ComponentModel.EventHandlerList event_list = AfterHandlers;
	    Hashtable signals = AfterSignals;
	    if (value.Method.GetCustomAttributes(typeof(ConnectBeforeAttribute), false).Length > 0) 
	      {
	      event_list = BeforeHandlers;
	      signals = BeforeSignals;
	      }
	    SignalCallback cb = signals ["invalidated"] as SignalCallback;
	    event_list.RemoveHandler("invalidated", value);
	    if (cb == null)
	      return;

	    cb.RemoveDelegate (value);

	    if (event_list["invalidated"] == null) 
	      {
	      signals.Remove("invalidated");
	      cb.Dispose ();
	      }
	    }
      }

      [DllImport("libgimpwidgets-2.0.so")]
      extern static void gimp_preview_set_update (IntPtr preview,
						  bool update);
      [DllImport("libgimpwidgets-2.0.so")]
      extern static bool gimp_preview_get_update (IntPtr preview);
      [DllImport("libgimpwidgets-2.0.so")]
      extern static void gimp_preview_set_bounds (IntPtr preview,
						  int xmin, int ymin, 
						  int xmax, int ymax);
      [DllImport("libgimpwidgets-2.0.so")]
      extern static void gimp_preview_get_position(IntPtr preview,
						   out int x, out int y);
      [DllImport("libgimpwidgets-2.0.so")]
      extern static void gimp_preview_get_size(IntPtr preview,
					       out int width, out int height);
      [DllImport("libgimpwidgets-2.0.so")]
      extern static void gimp_preview_draw(IntPtr preview);
      [DllImport("libgimpwidgets-2.0.so")]
      extern static void gimp_preview_draw_buffer(IntPtr preview,
						  byte[] buffer, 
						  int rowstride);
      [DllImport("libgimpwidgets-2.0.so")]
      extern static void gimp_preview_invalidate(IntPtr preview);
      [DllImport("libgimpwidgets-2.0.so")]
      extern static void gimp_preview_set_default_cursor(IntPtr preview,
							 Cursor cursor);
    }
  }