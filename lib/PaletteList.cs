// GIMP# - A C# wrapper around the GIMP Library
// Copyright (C) 2004-2018 Maurits Rijk
//
// PaletteList.cs
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place - Suite 330,
// Boston, MA 02111-1307, USA.
//

using System;
using System.Runtime.InteropServices;

namespace Gimp
{
  public sealed class PaletteList : DataObjectList<Palette>
  {
    public PaletteList(string filter = null) : base(filter)
    {
    }

    protected override IntPtr GetList(string filter, out int numDataObjects)
      => gimp_palettes_get_list(filter, out numDataObjects);

    protected override Palette CreateT(string name) => new Palette(name, false);

    static public void Refresh()
    {
      gimp_palettes_refresh();
    }

    [DllImport("libgimp-2.0-0.dll")]
    static extern void gimp_palettes_refresh();
    [DllImport("libgimp-2.0-0.dll")]
    static extern IntPtr gimp_palettes_get_list(string filter,
						out int num_palettes);
  }
}
