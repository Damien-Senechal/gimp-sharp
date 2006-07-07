// The PhotoshopActions plug-in
// Copyright (C) 2006 Maurits Rijk
//
// SelectLayerEvent.cs
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
//

using System;

using Gtk;

namespace Gimp.PhotoshopActions
{
  public class SelectLayerEvent : ActionEvent
  {
    string _mode;

    public SelectLayerEvent(ActionEvent srcEvent, string mode) : base(srcEvent)
    {
      _mode = mode;
    }

    public override string EventForDisplay
    {
      get {return base.EventForDisplay + " layer";}
    }

    protected override void FillParameters(TreeStore store, TreeIter iter)
    {
      store.AppendValues(iter, "Mode: " + _mode);
    }

    override public bool Execute()
    {
      switch (_mode)
	{
	case "Frwr":
	  break;
	default:
	  Console.WriteLine("SelectLayerEvent, unknown mode: " + _mode);
	  break;
	}

      return true;
    }
  }
}
