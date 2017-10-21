using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Capture.Workflow.Core.Annotations;

namespace Capture.Workflow.Core.Classes
{
    public class CustomPropertyCollection: INotifyPropertyChanged
    {
        [XmlElement("Property")]
        public ObservableCollection<CustomProperty> Items { get; set; }

        public CustomProperty this[string name]
        {
            get
            {
                foreach (var item in Items)
                {
                    if (item.Name == name)
                        return item;
                }
                return new CustomProperty() {Name = name};
            }
        }

        public void Add(CustomProperty property)
        {
            Items.Add(property);
        
            
        }

        private void Property_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(sender, e);
        }

        public CustomPropertyCollection()
        {
            Items = new ObservableCollection<CustomProperty>();
            Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    ((CustomProperty) item).PropertyChanged += Property_PropertyChanged;
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    ((CustomProperty)item).PropertyChanged -= Property_PropertyChanged;
                }
            }
        }

        public void CopyValuesFrom(CustomPropertyCollection propertyCollection)
        {
            foreach (var property in propertyCollection.Items)
            {
                this[property.Name].Value = property.Value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs property)
        {
            PropertyChanged?.Invoke(sender, property);
        }

    }
}
