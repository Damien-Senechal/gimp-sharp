// The PhotoshopActions plug-in
// Copyright (C) 2006 Maurits Rijk
//
// ReplaceColorEvent.cs
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
  public class ReplaceColorEvent : ActionEvent
  {
    double _luminance;
    int _hue, _saturation, _lightness;

    public ReplaceColorEvent()
    {
    }
    
    public override bool IsExecutable
    {
      get 
	{
	  return false;
	}
    }

    override public ActionEvent Parse(ActionParser parser)
    {
      int unknownFzns = parser.ReadLong("Fzns");

      // Minimum
      parser.ParseToken("Mnm");
      
      Objc objc = parser.ParseObjc();

      _luminance = parser.ReadDouble("Lmnc");

      double unknownA = parser.ReadDouble("A");
      double unknownB = parser.ReadDouble("B");

      // Maximum
      parser.ParseToken("Mxm");

      objc = parser.ParseObjc();

      _luminance = parser.ReadDouble("Lmnc");

      unknownA = parser.ReadDouble("A");
      unknownB = parser.ReadDouble("B");

      _hue = parser.ReadLong("H");
      _saturation = parser.ReadLong("Strt");
      _lightness = parser.ReadLong("Lght");

      return this;
    }
  }
}