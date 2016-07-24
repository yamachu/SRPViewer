 using Xamarin.Forms;

namespace SRPViewer
{
	public partial class SRPViewerPage : ContentPage
	{
		public SRPViewerPage()
		{
			InitializeComponent();

			this.FindByName<Button>("loginButton").Clicked += async (sender, e) => 
			{
				var result = await new SRPCommunicationModel().DoLogin("User", "Pass", "Matrix");
				switch (result)
				{
					case SRPCommunicationModel.LoginStatus.IncorrectIdPass:
						break;
					case SRPCommunicationModel.LoginStatus.IncorrectMatrix:
						break;
					case SRPCommunicationModel.LoginStatus.ReachPortal:
						// Go Next Page and Parse Gakumu Page
						break;
				}
			};
		}
	}
}

