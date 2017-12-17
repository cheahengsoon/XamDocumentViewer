using CrossFileHelper.Entities;
using System;
using System.IO;
using Xamarin.Forms;
using XamDocumentViewer.Standard.Abstractions;
using XamDocumentViewer.Standard.Dtos;
using XamDocumentViewer.Standard.Helpers;

namespace XamDocumentViewer.Standard
{
	public partial class MainPage : ContentPage
	{
		private Stream mFileStream;
		private string mDocumentType;
		private string mHtmlFileName;

		public MainPage()
		{
			Title = "My Document Viewer";
			InitializeComponent();
		}

		public MainPage(DocumentDetails documentDetails)
		{
			mFileStream = documentDetails.Stream;
			mDocumentType = documentDetails.Type;
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			LoadingSpinner.IsRunning = true;
			LoadingSpinner.IsVisible = true;

			if (mFileStream != null && !string.IsNullOrWhiteSpace(mDocumentType))
			{
				mHtmlFileName = "Test.html";

				DocumentHelper documentHelper = new DocumentHelper();
				await documentHelper.ConvertToHTML(mFileStream, mHtmlFileName, mDocumentType);
				mFileStream.Dispose();
				mFileStream = null;

				string htmlContent = null;
				using (Stream htmlReaderStream = await DependencyService.Get<ISaveAndLoad>().GetLocalFileInputStreamAsync(mHtmlFileName))
				{
					using (StreamReader streamReader = new StreamReader(htmlReaderStream))
					{
						htmlContent = await streamReader.ReadToEndAsync();
					}
				}

				webView.Source = new HtmlWebViewSource
				{
					Html = htmlContent
				};
			}
			else
			{
				webView.Source = new HtmlWebViewSource
				{
					Html = "<center>Choose file directly from file browser and choose open with My Document Viewer to open the file.</center>"
				};
			}
			LoadingSpinner.IsRunning = false;
			LoadingSpinner.IsVisible = false;
		}

		private async void ToolbarItem_Clicked(object sender, EventArgs e)
		{
			mDocumentType = null;
			FileData fileData = await CrossFileHelper.CrossFileHelper.Current.PickFile();
			if (fileData != null)
			{
				mFileStream = fileData.GetStream();
				//using (mFileStream = await CrossFileHelper.CrossFileHelper.Current.GetFileReadStreamAsync(fileData.FilePath))
				//{
					mHtmlFileName = "Test.html";
					DocumentHelper documentHelper = new DocumentHelper();
					await documentHelper.ConvertToHTML(mFileStream, mHtmlFileName, mDocumentType);
					mFileStream.Dispose();
					mFileStream = null;
				//}
				string htmlContent = null;
				using (Stream htmlReaderStream = await DependencyService.Get<ISaveAndLoad>().GetLocalFileInputStreamAsync(mHtmlFileName))
				{
					using (StreamReader streamReader = new StreamReader(htmlReaderStream))
					{
						htmlContent = await streamReader.ReadToEndAsync();
					}
				}
				webView.Source = new HtmlWebViewSource
				{
					Html = htmlContent
				};
			}
		}
	}
}
