namespace CameraControl.Core.Classes
{
  public class VideoType
  {
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public VideoType()
    {
      Name = "";
      Width = 0;
      Height = 0;
    }

    public VideoType(string name, int width, int heigth)
    {
      Name = name;
      Width = width;
      Height = heigth;
    }

    public override string ToString()
    {
      return string.Format("{0} " + "({1}x{2})", Name, Width, Height);
    }

    public override bool Equals(object obj)
    {
      VideoType videoType = obj as VideoType;
      if (videoType == null)
        return base.Equals(obj);
      return Name.Equals(videoType.Name);
    }



    public override int GetHashCode()
    {
      return Name.GetHashCode();
    }

  }
}
