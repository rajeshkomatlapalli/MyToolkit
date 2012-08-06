using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Microsoft.Phone.Controls;

namespace MyToolkit.Controls
{
	public class ExtendedListPicker : ListPicker
	{
		public static readonly DependencyProperty MySelectedItemsProperty =
			DependencyProperty.Register("TypedSelectedItems", typeof(IList), typeof(ExtendedListPicker),
			new PropertyMetadata(default(IEnumerable), PropertyChangedCallback));

		private static void PropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var ctrl = (ExtendedListPicker)obj;
			ctrl.Update();
		}

		private bool bindingRegistered = false;
		private void Update()
		{
			if (!bindingRegistered)
			{
				var binding = new Binding("SelectedItems");
				binding.Source = new Helper(this);
				binding.Mode = BindingMode.TwoWay;
				SetBinding(SelectedItemsProperty, binding);
				bindingRegistered = true;
			}

			SelectedItems = TypedSelectedItems;
		}

		public class Helper
		{
			private readonly ExtendedListPicker picker;
			public Helper(ExtendedListPicker p)
			{
				picker = p;
			}

			public IList SelectedItems
			{
				get { return picker.TypedSelectedItems; }
				set
				{
					if (picker.TypedSelectedItems != null && value != null)
					{
						var elementType = picker.TypedSelectedItems.GetType().GetGenericArguments()[0];
						var type = value.GetType().GetGenericTypeDefinition().MakeGenericType(elementType);
						if (value.GetType() != type)
						{
							var method = typeof(Enumerable).GetMethod("OfType").MakeGenericMethod(elementType);
							var list = method.Invoke(null, new object[] { value });
							picker.TypedSelectedItems = (IList)Activator.CreateInstance(type, list);
						}
						else
							picker.TypedSelectedItems = value;
					}
					else
						picker.TypedSelectedItems = value;
				}
			}
		}

		public IList TypedSelectedItems
		{
			get { return (IList)GetValue(MySelectedItemsProperty); }
			set { SetValue(MySelectedItemsProperty, value); }
		}

		public new IList SelectedItems
		{
			get { return (IList)GetValue(SelectedItemsProperty); }
			set { base.SetValue(SelectedItemsProperty, value); }
		}
	}
}