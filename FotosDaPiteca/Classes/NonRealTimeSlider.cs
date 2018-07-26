using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace FotosDaPiteca.Classes
{
    public class NonRealtimeSlider : Slider
    {
        static NonRealtimeSlider()
        {
            var defaultMetadata = ValueProperty.GetMetadata(typeof(TextBox));

            ValueProperty.OverrideMetadata(typeof(NonRealtimeSlider), new FrameworkPropertyMetadata(
            defaultMetadata.DefaultValue,
            FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            defaultMetadata.PropertyChangedCallback,
            defaultMetadata.CoerceValueCallback,
            true,
            UpdateSourceTrigger.Explicit));
        }

        protected override void OnThumbDragCompleted(DragCompletedEventArgs e)
        {
            base.OnThumbDragCompleted(e);
            GetBindingExpression(ValueProperty)?.UpdateSource();
        }
    }
}
