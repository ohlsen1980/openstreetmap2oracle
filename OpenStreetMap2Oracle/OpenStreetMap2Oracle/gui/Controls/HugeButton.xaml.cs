using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Effects;

namespace OpenStreetMap2Oracle
{
	/// <summary>
	/// Interaktionslogik für HugeButton.xaml
	/// </summary>
	public partial class HugeButton : UserControl
	{
		BlurEffect blur; 
		
		public HugeButton()
		{
			this.InitializeComponent();
			blur = new BlurEffect();
			blur.Radius = 5;
			blur.KernelType = KernelType.Gaussian;
		}
		
		public string Header {
			get {
				return lblHeader.Content.ToString();
			}
			set {
				lblHeader.Content = value;
			}
		}
		
		
		private bool disabled = false;
		public bool Disabled {
			set {
				disabled = value;
				if (disabled) {
					canvasOverlay.Opacity = 0.50;
					grid.Effect = blur;
				} else {
					canvasOverlay.Opacity = 0;
					grid.Effect = null;
				}
			}
			get {
				return disabled;
			}
		}
		
		public string BodyContent {
			get {
				return lblContent.Text;
			}
			set {
				lblContent.Text = value;
			}
		}
		
		public ImageSource ImageSource {
			get {
				return imgMain.Source;
			}
			set {
				imgMain.Source = value;
			}
		}
	}
}