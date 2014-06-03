using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Classes;
using CameraControl.Core.Classes;
using CameraControl.Devices.Classes;

namespace CameraControl.Actions.Enfuse
{
  public class EnfuseSettings : BaseFieldClass 
  {
    private bool _alignImages;
    public bool AlignImages
    {
      get { return _alignImages; }
      set
      {
        _alignImages = value;
        NotifyPropertyChanged("AlignImages");
      }
    }

    private bool _optimizeFiledOfView;
    public bool OptimizeFiledOfView
    {
      get { return _optimizeFiledOfView; }
      set
      {
        _optimizeFiledOfView = value;
        NotifyPropertyChanged("OptimizeFiledOfView");
      }
    }

    private bool _autoCrop;
    public bool AutoCrop
    {
      get { return _autoCrop; }
      set
      {
        _autoCrop = value;
        NotifyPropertyChanged("AutoCrop");
      }
    }

    private double _enfuseExp;
    public double EnfuseExp
    {
      get { return _enfuseExp; }
      set
      {
        _enfuseExp = value;
        if (_enfuseExp < 0)
          _enfuseExp = 0;
        if (_enfuseExp > 100)
          _enfuseExp = 100;
        NotifyPropertyChanged("EnfuseExp");
      }
    }

    private double _enfuseCont;
    public double EnfuseCont
    {
      get { return _enfuseCont; }
      set
      {
        _enfuseCont = value;
        if (_enfuseCont < 0)
          _enfuseCont = 0;
        if (_enfuseCont > 100)
          _enfuseCont = 100;
        NotifyPropertyChanged("EnfuseCont");
      }
    }

    private double _enfuseSat;
    public double EnfuseSat
    {
      get { return _enfuseSat; }
      set
      {
        _enfuseSat = value;
        if (_enfuseSat < 0)
          _enfuseSat = 0;
        if (_enfuseSat > 100)
          _enfuseSat = 100;
        NotifyPropertyChanged("EnfuseSat");
      }
    }

    private double _enfuseEnt;
    public double EnfuseEnt
    {
      get { return _enfuseEnt; }
      set
      {
        _enfuseEnt = value;
        if (_enfuseEnt < 0)
          _enfuseEnt = 0;
        if (_enfuseEnt > 100)
          _enfuseEnt = 100;
        NotifyPropertyChanged("EnfuseEnt");
      }
    }

    private double _enfuseSigma;
    public double EnfuseSigma
    {
      get { return _enfuseSigma; }
      set
      {
        _enfuseSigma = value;
        if (_enfuseSigma < 0)
          _enfuseSigma = 0;
        if (_enfuseSigma > 100)
          _enfuseSigma = 100;
        NotifyPropertyChanged("EnfuseSigma");
      }
    }

    private bool _hardMask;
    public bool HardMask
    {
      get { return _hardMask; }
      set
      {
        _hardMask = value;
        NotifyPropertyChanged("HardMask");
      }
    }

    private int _contrasWindow;
    public int ContrasWindow
    {
      get { return _contrasWindow; }
      set
      {
        _contrasWindow = value;
        if (_contrasWindow < 3)
          _contrasWindow = 3;
        NotifyPropertyChanged("ContrasWindow");
      }
    }

    private int _scale;
    public int Scale
    {
      get { return _scale; }
      set
      {
        _scale = value;
        NotifyPropertyChanged("Scale");
      }
    }

    public EnfuseSettings()
    {
      AlignImages = true;
      EnfuseExp = 100;
      EnfuseCont = 0;
      EnfuseEnt = 0;
      EnfuseSat = 20;
      EnfuseSigma = 20;
      HardMask = false;
      ContrasWindow = 5;
      Scale = 0;
    }
  }
}
