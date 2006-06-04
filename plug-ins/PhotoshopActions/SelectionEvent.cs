// The PhotoshopActions plug-in
// Copyright (C) 2006 Maurits Rijk
//
// SelectionEvent.cs
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

namespace Gimp.PhotoshopActions
{
  public class SelectionEvent : ActionEvent
  {
    double _top, _left, _bottom, _right;

    public SelectionEvent()
    {
    }
    
    override public ActionEvent Parse(ActionParser parser)
    {
      parser.ParseToken("T");

      string token = parser.ReadFourByteString();
      if (token == "enum")
	{
	  parser.ParseToken("Ordn");
	  parser.ParseToken("Al");
	}
      else if (token == "Objc")
	{
	  string classID = parser.ReadUnicodeString();
	  string classID2 = parser.ReadTokenOrString();
	  
	  parser.ParseInt32(4);

	  string units;
	  _top = parser.ReadDouble("Top", out units);
	  _left = parser.ReadDouble("Left", out units);
	  _bottom = parser.ReadDouble("Btom", out units);
	  _right = parser.ReadDouble("Rght", out units);
	}
      else
	{
	  Console.WriteLine("Unknown SelectionEvent: " + token);
	}
      return this;
    }
  }
}