using System;

using Gtk;

namespace Gimp.PicturePackage
{
  public class PicturePackage : Plugin
  {
    LayoutSet _layoutSet = new LayoutSet();
    Preview _preview;

    [STAThread]
    static void Main(string[] args)
    {
      PicturePackage plugin = new PicturePackage(args);
    }

    public PicturePackage(string[] args) : base(args)
    {
    }

    override protected void Query()
    {
      InstallProcedure("plug_in_picture_package",
		       "Picture package",
		       "Picture package",
		       "Maurits Rijk",
		       "Maurits Rijk",
		       "2004",
		       "Picture Package...",
		       "RGB*, GRAY*",
		       null);

      MenuRegister("plug_in_picture_package", "<Image>/Filters/Render");
    }

    override protected bool CreateDialog()
    {
      gimp_ui_init("PicturePackage", true);

      _layoutSet.Load();

      Dialog dialog = DialogNew("Picture Package", "PicturePackage",
				IntPtr.Zero, 0, null, "PicturePackage");

      HBox hbox = new HBox(false, 12);
      hbox.BorderWidth = 12;
      dialog.VBox.PackStart(hbox, true, true, 0);

      VBox vbox = new VBox(false, 12);
      hbox.PackStart(vbox, false, false, 0);

      BuildSourceFrame(vbox);
      BuildDocumentFrame(vbox);
      BuildLabelFrame(vbox);

      Frame frame = new Frame();
      hbox.PackStart(frame, true, true, 0);

      VBox fbox = new VBox();
      fbox.BorderWidth = 12;
      frame.Add(fbox);

      _preview = new Preview();
      _preview.WidthRequest = 400;
      _preview.HeightRequest = 500;
      _preview.Layout = _layoutSet[0];		// Fix me!
      fbox.Add(_preview);

      dialog.ShowAll();
	
      return DialogRun();
    }

    CheckButton _include;
    Button _choose;

    void BuildSourceFrame(VBox vbox)
    {
      GimpFrame frame = new GimpFrame("Source");
      vbox.PackStart(frame, false, false, 0);

      GimpTable table = new GimpTable(2, 3, false);
      table.ColumnSpacing = 6;
      table.RowSpacing = 6;
      frame.Add(table);

      OptionMenu use = new OptionMenu();
      Menu menu = new Menu();
      menu.Append(new MenuItem("File"));
      menu.Append(new MenuItem("Folder"));
      menu.Append(new MenuItem("Frontmost Document"));
      use.Menu = menu;
      use.SetHistory(2);
      table.AttachAligned(0, 0, "_Use:", 0.0, 0.5, use, 1, false);
      use.Changed += new EventHandler(OnUseChanged);

      _include = new CheckButton("_Include All Subfolders");
      table.Attach(_include, 1, 2, 1, 2);

      _choose = new Button("Choose...");
      table.Attach(_choose, 1, 2, 2, 3, AttachOptions.Shrink,
		   AttachOptions.Fill, 0, 0);	

      SetSourceFrameSensitivity(2);
    }

    void SetSourceFrameSensitivity(int history)
    {
      if (history == 0)
	{
	_include.Sensitive = false;
	_choose.Sensitive = true;
	}
      else if (history == 1)
	{
	_include.Sensitive = true;
	_choose.Sensitive = true;
	}
      else
	{
	_include.Sensitive = false;
	_choose.Sensitive = false;
	}
    }

    void OnUseChanged (object o, EventArgs args) 
    {
      SetSourceFrameSensitivity((o as OptionMenu).History);
    }

    void BuildDocumentFrame(VBox vbox)
    {
      Frame frame = new GimpFrame("Document");
      vbox.PackStart(frame, false, false, 0);

      GimpTable table = new GimpTable(5, 3, false);
      table.ColumnSpacing = 6;
      table.RowSpacing = 6;
      frame.Add(table);

      OptionMenu size = new OptionMenu();
      Menu menu = new Menu();
      menu.Append(new MenuItem("8.0 x 10.0 inches"));
      size.Menu = menu;
      table.AttachAligned(0, 0, "_Page Size:", 0.0, 0.5, size, 2, false);

      OptionMenu layout = new OptionMenu();
      menu = new Menu();
      foreach (Layout l in _layoutSet)
	{
	menu.Append(new MenuItem(l.Name));
	}
      layout.Menu = menu;
      layout.Changed += new EventHandler(OnLayoutChanged);
      table.AttachAligned(0, 1, "_Layout:", 0.0, 0.5,
			  layout, 2, false);

      Entry resolution = new Entry();
      resolution.WidthChars = 4;
      table.AttachAligned(0, 2, "_Resolution:", 0.0, 0.5, resolution, 1, true);
	
      OptionMenu units = new OptionMenu();
      menu = new Menu();
      menu.Append(new MenuItem("pixels/inch"));
      menu.Append(new MenuItem("pixels/cm"));
      menu.Append(new MenuItem("pixels/mm"));
      units.Menu = menu;
      table.Attach(units, 2, 3, 2, 3);	

      OptionMenu mode = new OptionMenu();
      menu = new Menu();
      menu.Append(new MenuItem("Grayscale"));
      menu.Append(new MenuItem("RGB Color"));
      mode.Menu = menu;
      mode.SetHistory(1);
      table.AttachAligned(0, 3, "_Mode:", 0.0, 0.5, mode, 2, false);

      CheckButton flatten = new CheckButton("Flatten All Layers");
      table.Attach(flatten, 0, 2, 4, 5);
    }

    void OnLayoutChanged (object o, EventArgs args) 
    {
      int nr = (o as OptionMenu).History;
      _preview.Layout = _layoutSet[nr];
      _preview.QueueDraw();
    }

    void BuildLabelFrame(VBox vbox)
    {
      GimpFrame frame = new GimpFrame("Label");
      vbox.PackStart(frame, false, false, 0);

      GimpTable table = new GimpTable(3, 3, false);
      table.ColumnSpacing = 6;
      table.RowSpacing = 6;
      frame.Add(table);

      OptionMenu content = new OptionMenu();
      Menu menu = new Menu();
      menu.Append(new MenuItem("None"));
      menu.Append(new MenuItem("Custom Text"));
      menu.Append(new MenuItem("Filename"));
      menu.Append(new MenuItem("Copyright"));
      menu.Append(new MenuItem("Caption"));
      menu.Append(new MenuItem("Credits"));
      menu.Append(new MenuItem("Title"));
      content.Menu = menu;
      table.AttachAligned(0, 0, "Content:", 0.0, 0.5,
			  content, 1, false);

      Entry entry = new Entry();
      table.AttachAligned(0, 1, "Custom Text:", 0.0, 0.5,
			  entry, 1, true);
#if false
      GimpFontSelectWidget font = new GimpFontSelectWidget(null, 
							   "Monospace");
      table.AttachAligned(0, 2, "Font:", 0.0, 0.5,
			  font, 1, true);
#endif
      RGB rgb = new RGB(0, 0, 0);

      GimpColorButton color = new GimpColorButton("", 16, 16, rgb.GimpRGB,
						  ColorAreaType.COLOR_AREA_FLAT);
      table.AttachAligned(0, 2, "Color:", 0.0, 0.5,
			  color, 1, true);

      OptionMenu position = new OptionMenu();
      menu = new Menu();
      menu.Append(new MenuItem("Centered"));
      menu.Append(new MenuItem("Top Left"));
      menu.Append(new MenuItem("Bottom Left"));
      menu.Append(new MenuItem("Top Right"));
      menu.Append(new MenuItem("Bottom Right"));
      position.Menu = menu;
      table.AttachAligned(0, 3, "Position:", 0.0, 0.5,
			  position, 1, false);

      OptionMenu rotate = new OptionMenu();
      menu = new Menu();
      menu.Append(new MenuItem("None"));
      menu.Append(new MenuItem("45 Degrees Right"));
      menu.Append(new MenuItem("90 Degrees Right"));
      menu.Append(new MenuItem("45 Degrees Left"));
      menu.Append(new MenuItem("90 Degrees Left"));
      rotate.Menu = menu;
      table.AttachAligned(0, 4, "Rotate:", 0.0, 0.5,
			  rotate, 1, false);
    }

    override protected void DoSomething(Image image)
    {
      Image clone = image.Duplicate();
      clone.Rotate(RotationType.ROTATE_90);
      Console.WriteLine("DoSomething: " + clone.Width);
      Display.DisplaysFlush();
    }
  }
  }