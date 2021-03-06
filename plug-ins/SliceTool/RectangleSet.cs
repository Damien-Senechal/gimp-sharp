// The SliceTool plug-in
// Copyright (C) 2004-2017 Maurits Rijk
//
// RectangleSet.cs
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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gimp.SliceTool
{
  public class SelectedChangedEventArgs : EventArgs
  {
    public Rectangle OldSelected {get; internal set;}
    public Rectangle NewSelected {get; internal set;}

    public SelectedChangedEventArgs(Rectangle oldSelected, 
				    Rectangle newSelected)
    {
      OldSelected = oldSelected ?? new Rectangle(null, null, null, null);
      NewSelected = newSelected;
    }
  }

  public class RectangleSet
  {
    public delegate void SelectedRectangleChangedHandler(object sender,
							 SelectedChangedEventArgs args);

    public event SelectedRectangleChangedHandler SelectedRectangleChanged = 
      delegate {};

    readonly List<Rectangle> _set = new List<Rectangle>();
    Rectangle _selected;
    public bool Changed {get; set;}

    public Rectangle Selected 
    {
      get => _selected;

      private set 
      {
	if (value != _selected)
	  {
	    OnSelectedRectangleChanged(value);
	  }
	_selected = value;
      }
    }

    void OnSelectedRectangleChanged(Rectangle newSelected)
    {
      var args = new SelectedChangedEventArgs(Selected, newSelected);
      SelectedRectangleChanged(this, args);
    }

    void Flush()
    {
      OnSelectedRectangleChanged(Selected);
    }

    public void Add(Rectangle rectangle)
    {
      Changed = true;
      _set.Add(rectangle);
      Selected = Selected ?? rectangle;
    }

    Rectangle this[int index] => _set[index];

    public void Clear()
    {
      Changed = true;
      _set.Clear();
    }

    public void Slice(Slice slice)
    {
      var query = _set.Where(rectangle => rectangle.IntersectsWith(slice));
      query.ToList().ForEach(rectangle => Add(rectangle.Slice(slice)));
    }

    public Rectangle Find(IntCoordinate c) =>
      _set.Find(rectangle => rectangle.IsInside(c));

    public void Select(IntCoordinate c)
    {
      Selected = Find(c);
    }

    public void WriteHTML(StreamWriter w, string name, bool useGlobalExtension)
    {
      Flush();
      _set.Sort();
      
      w.WriteLine("<tr>");
      var prev = this[0];
      prev.WriteHTML(w, name, useGlobalExtension, 0);
      for (int i = 1; i < _set.Count; i++)
	{
	  if (this[i].Top.Index != prev.Top.Index)
	    {
	      w.WriteLine("</tr>");
	      w.WriteLine("");
	      w.WriteLine("<tr>");
	    }
	  prev = this[i];
	  prev.WriteHTML(w, name, useGlobalExtension, i);
	}
      w.WriteLine("</tr>");
    }
    
    public void WriteSlices(Image image, string path, string name, 
			    bool useGlobalExtension)
    {
      ForEach(rectangle => rectangle.WriteSlice(image, path, name, 
						useGlobalExtension));
    }
    
    public void Save(StreamWriter w)
    {
      ForEach(rectangle => rectangle.Save(w));
      Changed = false;
    }
    
    public void Resolve(SliceSet hslices, SliceSet vslices)
    {
      ForEach(rectangle => rectangle.Resolve(hslices, vslices));
    }

    public void Cleanup(Slice slice)
    {
      var query = _set.Where(rectangle => rectangle.Normalize(slice));
      _set.RemoveAll(rectangle => query.Contains(rectangle));
      Changed = query.Count() > 0;
    }

    public void Remove(Slice slice)
    {
      var set1 = _set.Where(rectangle => slice == rectangle.Bottom || 
			    slice == rectangle.Right).ToList();
      var set2 = _set.Where(rectangle => slice == rectangle.Top || 
			    slice == rectangle.Left).ToList();

      foreach (var r1 in set1)
        {
	  foreach (var r2 in set2)
	    {
	      if (r1.Left == r2.Left && r1.Right == r2.Right)
		{
		  r1.Bottom = r2.Bottom;
		  Remove(r2);
		  break;
		}
	      else if (r1.Top == r2.Top && r1.Bottom == r2.Bottom)
		{
		  r1.Right = r2.Right;
		  Remove(r2);
		  break;
		}
	    }
        }
    }

    IEnumerator<Rectangle> GetEnumerator() => _set.GetEnumerator();

    void Remove(Rectangle rectangle)
    {
      Changed = true;
      _set.Remove(rectangle);
    }

    void ForEach(Action<Rectangle> action) => _set.ForEach(action);
  }
}
