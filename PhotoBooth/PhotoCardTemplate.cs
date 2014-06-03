using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PhotoBooth
{
    public abstract class PhotoCardTemplate : DependencyObject
    {
        public PhotoCardTemplate()
        {
            this.Images = new ObservableCollection<ImageSource>();
            this.Images.CollectionChanged += Images_CollectionChanged;
        }

        public bool CanPrint
        {
            get { return this.ImagesRequired <= this.Images.Count; }
        }

        public ColorConversion ColorConversion
        {
            get { return (ColorConversion)GetValue(ColorConversionProperty); }
            set { SetValue(ColorConversionProperty, value); }
        }

        public abstract PhotoCard CreateCard();

        public UserControl CreateDesigner()
        {
            UserControl designer = this.CreateDesignerControl();
            if (designer != null)
            {
                designer.DataContext = this;
            }

            return designer;
        }

        public string DisplayName { get; set; }

        public ObservableCollection<ImageSource> Images
        {
            get;
            private set;
        }

        public int ImagesRequired
        {
            get;
            protected set;
        }

        protected abstract UserControl CreateDesignerControl();

        private void Images_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewStartingIndex < 0)
            {
                return;
            }

            for (int index = e.NewStartingIndex; index < e.NewStartingIndex + e.NewItems.Count; index++)
            {
                ImageSource imageSrc = null;
                if (this.Images.Count > index)
                {
                    ImageSource convertedSource = null;
                    if (this.ColorConversion != PhotoBooth.ColorConversion.None)
                    {
                        convertedSource = ColorUtility.Convert(this.Images[index], this.ColorConversion);
                    }
                    if (convertedSource != null)
                    {
                        imageSrc = convertedSource;
                    }
                    else
                    {
                        imageSrc = this.Images[index];
                    }
                }

                this.OnImageUpdated(index, imageSrc);
            }
        }

        protected virtual void OnImageUpdated(int imageIndex, ImageSource imageSrc)
        {
        }

        // Using a DependencyProperty as the backing store for ColorConversion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorConversionProperty =
            DependencyProperty.Register("ColorConversion", typeof(ColorConversion), typeof(PhotoCardTemplate), new PropertyMetadata(ColorConversion.None));
    }

    public abstract class PhotoCardTemplate<T> : PhotoCardTemplate where T : PhotoCard, new()
    {
        public override PhotoCard CreateCard()
        {
            return new T();
        }
    }
}
