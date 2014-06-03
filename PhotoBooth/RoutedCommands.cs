using System.Windows.Input;

namespace PhotoBooth
{
    public class RoutedCommands
    {
        public static RoutedUICommand DesignCard = new RoutedUICommand("Design Card", "DesignCard", typeof(RoutedCommands));
        public static RoutedUICommand InitializeCamera = new RoutedUICommand("Initialize Camera", "InitializeCamera", typeof(RoutedCommands));
        public static RoutedUICommand OpenPhotoBooth = new RoutedUICommand("Open PhotoBooth", "PhotoBooth", typeof(RoutedCommands));
        public static RoutedUICommand PrinterSetup = new RoutedUICommand("Printer Setup", "PrinterSetup", typeof(RoutedCommands));
        public static RoutedUICommand ShowCardView = new RoutedUICommand("Card View", "CardView", typeof(RoutedCommands));
        public static RoutedUICommand TakePictureSet = new RoutedUICommand("Take Picture Set", "TakePictureSet", typeof(RoutedCommands));
    }
}
