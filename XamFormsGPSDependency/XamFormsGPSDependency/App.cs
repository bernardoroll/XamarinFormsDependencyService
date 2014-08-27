using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

using XamFormsGPSDependency.Pages;

namespace XamFormsGPSDependency
{
    public class App
    {
        public static Page GetMainPage()
        {
            return new NavigationPage(new WhereAmIPage());

            //return new ContentPage
            //{
            //    Content = new Label
            //    {
            //        Text = "Hello, Forms !",
            //        VerticalOptions = LayoutOptions.CenterAndExpand,
            //        HorizontalOptions = LayoutOptions.CenterAndExpand,
            //    },
            //};
        }
    }
}
