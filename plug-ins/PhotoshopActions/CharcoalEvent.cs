// The PhotoshopActions plug-in
// Copyright (C) 2006-2016 Maurits Rijk
//
// CharcoalEvent.cs
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

using System.Collections;

namespace Gimp.PhotoshopActions
{
  public class CharcoalEvent : ActionEvent
  {
    [Parameter("ChAm")]
    int _charcoalThickness;
    [Parameter("Dtl")]
    int _detail;
    [Parameter("LgDr")]
    int _lightDarkBalance;

    public override bool IsExecutable => false;

    protected override IEnumerable ListParameters()
    {
      yield return Format(_charcoalThickness, "ChAm");
      yield return Format(_detail, "Dtl");
      yield return Format(_lightDarkBalance, "LgDr");
    }

    override public bool Execute() => false;
  }
}
