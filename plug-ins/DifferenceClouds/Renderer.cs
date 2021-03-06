// The Difference Clouds plug-in
// Copyright (C) 2006-2016 Maurits Rijk
//
// Renderer.cs
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

namespace Gimp.DifferenceClouds
{
  public class Renderer : BaseRenderer
  {
    readonly Random _random;
    readonly double _turbulence;

    readonly Progress _progressBar;
    uint _count;

    int _progress;
    int _maxProgress;
    int _bpp;
    bool _hasAlpha;

    IndexedColorsMap _indexedColorsMap;

    public Renderer(VariableSet variables) : 
      base(variables)
    {
      _random = new Random((int) GetValue<UInt32>("seed"));
      _turbulence = GetValue<double>("turbulence");

      _progressBar = new Progress(_("Difference Clouds..."));
    }

    public void Render(Image image, Drawable drawable)
    {
      Tile.CacheDefault(drawable);

      var activeLayer = image.ActiveLayer;
      var newLayer = new Layer(activeLayer)
	{
	  Name = "_DifferenceClouds_", 
	  Visible = false,
	  Mode = activeLayer.Mode, 
	  Opacity = activeLayer.Opacity
	};

      // Initialization steps
      _bpp = drawable.Bpp;
      var pf = new PixelFetcher(drawable, true);
      _progress = 0;
      _hasAlpha = newLayer.HasAlpha;

      var rectangle = drawable.MaskBounds;
      _maxProgress = rectangle.Area;

      if (rectangle.Width > 0 && rectangle.Height > 0)
	{
	  //
	  // This first time only puts in the seed pixels - one in each
	  // corner, and one in the center of each edge, plus one in the
	  // center of the image.
	  //
	  InitSeedPixels(pf, rectangle);

	  //
	  // Now we recurse through the images, going further each time.
	  //
	  int depth = 1;
	  while (!DoDifferenceClouds(pf, rectangle.X1, rectangle.Y1, 
				     rectangle.X2 - 1, rectangle.Y2 - 1, 
				     depth, 0))
	    {
	      depth++;
	    }
	}
      
      pf.Dispose();

      drawable.Flush();
      drawable.MergeShadow(true);
      
      DoDifference(drawable, newLayer);
      
      drawable.Update(rectangle);
    }

    void DoDifference(Drawable sourceDrawable, Drawable toDiffDrawable)
    {
      _indexedColorsMap = new IndexedColorsMap();

      var rectangle = sourceDrawable.MaskBounds;
      var srcPR = new PixelRgn(sourceDrawable, rectangle, true, true);
      var destPR = new PixelRgn(toDiffDrawable, rectangle, false, false);

      var iterator = new RegionIterator(srcPR, destPR);
      iterator.ForEach((src, dest) => src.Set(MakeAbsDiff(dest, src)));

      sourceDrawable.Flush();
      sourceDrawable.MergeShadow(false);
      sourceDrawable.Update(rectangle);
    }

    Pixel MakeAbsDiff(Pixel dest, Pixel src)
    {
      int tmpVal = 0;
      for (int i = 0; i < _bpp; i++)
	{
	  tmpVal += src[i];
	}
      tmpVal /= _bpp;

      var pixel = new Pixel(_bpp)
	{Color = dest.Color - _indexedColorsMap[tmpVal]};
        
      if (_hasAlpha)
	{
	  pixel.Alpha = 255;
	}
      return pixel;
    }   

    void InitSeedPixels(PixelFetcher pf, Rectangle rectangle)
    {
      int x1 = rectangle.X1;
      int y1 = rectangle.Y1;
      int x2 = rectangle.X2 - 1;
      int y2 = rectangle.Y2 - 1;

      int xm = (x1 + x2) / 2;
      int ym = (y1 + y2) / 2;

      pf[y1, x1] = RandomRGB();
      pf[y1, x2] = RandomRGB();
      pf[y2, x1] = RandomRGB();
      pf[y2, x2] = RandomRGB();
      pf[ym, x1] = RandomRGB();
      pf[ym, x2] = RandomRGB();
      pf[y1, xm] = RandomRGB();
      pf[y2, xm] = RandomRGB();

      _progress += 8;
    }

    bool DoDifferenceClouds(PixelFetcher pf, int x1, int y1, int x2, int y2, 
			    int depth, int scaleDepth)
    {
      int xm = (x1 + x2) / 2;
      int ym = (y1 + y2) / 2;

      if (depth == 0)
	{
	  if (x1 == x2 && y1 == y2)
	    {
	      return false;
	    }

	  var tl = pf[y1, x1];
	  var tr = pf[y1, x2];
	  var bl = pf[y2, x1];
	  var br = pf[y2, x2];

	  int ran = (int)((256.0 / (2.0 * scaleDepth)) * _turbulence);

	  if (xm != x1 || xm != x2)
	    {
	      // Left
	      pf[ym, x1] = AddRandom((tl + bl) / 2, ran);
	      _progress++;

	      if (x1 != x2)
		{
		  // Right
		  pf[ym, x2] = AddRandom((tr + br) / 2, ran);
		  _progress++;
		}
	    }

	  if (ym != y1 || ym != y2)
	    {
	      if (x1 != xm || ym != y2)
		{
		  // Bottom
		  pf[y2, xm] = AddRandom((bl + br) / 2, ran);
		  _progress++;
		}

	      if (y1 != y2)
		{
		  // Top
		  pf[y1, xm] = AddRandom((tl + tr) / 2, ran);
		  _progress++;
		}
	    }

	  if (y1 != y2 || x1 != x2)
	    {
	      // Middle pixel
	      pf[ym, xm] = AddRandom((tl + tr + bl + br) / 4, ran);
	      _progress++;
	    }

	  _count++;

	  if (_count % 2000 == 0)
	    {
	      _progressBar.Update((double)_progress / (double) _maxProgress);
	    }

	  return (x2 - x1 < 3) && (y2 - y1 < 3);
	}
      else if (x1 < x2 || y1 < y2)
	{
	  depth--;
	  scaleDepth++;

	  // Top left
	  DoDifferenceClouds(pf, x1, y1, xm, ym, depth, scaleDepth);
	  // Bottom left
	  DoDifferenceClouds(pf, x1, ym, xm ,y2, depth, scaleDepth);
	  // Top right
	  DoDifferenceClouds(pf, xm, y1, x2 , ym, depth, scaleDepth);
	  // Bottom right
	  return DoDifferenceClouds(pf, xm, ym, x2, y2, depth, scaleDepth);
	}
      else
	{
	  return true;
	}
    }

    Pixel RandomRGB()
    {
      var pixel = new Pixel(_bpp);

      for (int i = 0; i < _bpp; i++)
	{
	  pixel[i] = _indexedColorsMap[_random.Next(256), i];
	} 
      
      if (_hasAlpha)
	{
	  pixel.Alpha = 255;
	}
      return pixel;
    }

    Pixel AddRandom(Pixel pixel, int amount)
    {
      pixel.AddNoise(amount / 2);
      return pixel;
    }
  }
}
