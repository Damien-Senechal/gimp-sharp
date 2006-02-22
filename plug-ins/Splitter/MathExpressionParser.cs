// The Splitter plug-in
// Copyright (C) 2004-2006 Maurits Rijk
//
// MathExpressionParser.cs
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
using System.Reflection;

namespace Gimp.Splitter
{
  public class MathExpressionParser
  {
    MyClassBase myobj = null;
    public MathExpressionParser()
    {
    }
    public bool Init(string expr)
    {
      Microsoft.CSharp.CSharpCodeProvider cp
	= new Microsoft.CSharp.CSharpCodeProvider();
      System.CodeDom.Compiler.ICodeCompiler ic = cp.CreateCompiler();
      System.CodeDom.Compiler.CompilerParameters cpar
	= new System.CodeDom.Compiler.CompilerParameters();
      cpar.GenerateInMemory = true;
      cpar.GenerateExecutable = false;
      cpar.ReferencedAssemblies.Add("System.dll");
      // FIXME: next line fails if GIMP not executed in plug-in directory
      cpar.ReferencedAssemblies.Add("Splitter.exe"); 
      string src = 
	"using System;"+
	"class myclass : Gimp.Splitter.MyClassBase" + 
	"{"+
	"public myclass(){}"+
	"public override double eval(double x,double y)"+
	"{"+
	"return "+ expr +";"+
	"}"+
	"}";
      System.CodeDom.Compiler.CompilerResults cr
	= ic.CompileAssemblyFromSource(cpar,src);
      foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
	Console.WriteLine(ce.ErrorText);

      if (cr.Errors.Count == 0 && cr.CompiledAssembly != null)
	{
	  Type ObjType = cr.CompiledAssembly.GetType("myclass");
	  try
	    {
	      if (ObjType != null)
		{
		  myobj = (MyClassBase)Activator.CreateInstance(ObjType);
		}
	    }
	  catch (Exception ex)
	    {
	      Console.WriteLine(ex.Message);
	    }
	  return true;
	}
      else 
	return false;
    }

    public double Eval(double x, double y)
    {
      double val = 0.0;
      if (myobj != null)
	{
	  val = myobj.eval(x,y);
	}
      return val;
    }
  }
}