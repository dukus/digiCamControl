namespace CameraControl.Core.Classes
{
    public class TransformPluginItem 
    {
        private ValuePairEnumerator _config;

        public ValuePairEnumerator Config
        {
            get { return _config; }
            set { _config = value; }
        }

        public TransformPluginItem(ValuePairEnumerator config)
        {
            _config = config;
        }

        public string Name
        {
            get { return _config["TransformPlugin"]; }
        }
    }
}
