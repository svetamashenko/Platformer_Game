using System;

namespace Assets.PixelCrew.Model.Data.Properties
{
    [Serializable] public class IntProperty : ObservableProperty<int> {
        public IntProperty(int value) : base()
        {
            Value = value;
        }
        public IntProperty() : base()
        {
        }
    }
}