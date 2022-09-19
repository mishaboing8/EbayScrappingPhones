using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace FirstXamarin
{

    public partial class LoadedObjectsContent : ContentPage
    {
        public static bool LoadSave = false;
        public static string LoadedObjects = "";
        public readonly static string LOADED_SAVE_PATH = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LastSave.txt");
        static bool playedAnimation = false;

        public static string scrappIP = "";
        public static string scrappPort = "";


        private List<EbayProduct> products = new List<EbayProduct>();

        public LoadedObjectsContent()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {

            LoadedObjects = "";
            productFilter.SelectedIndex = 0;
            productFilter.BackgroundColor = Color.FromRgb(235, 169, 55);
            pickerFrame.BackgroundColor = Color.FromRgb(235, 169, 55);

            backButtonImage.GestureRecognizers.Add(new TapGestureRecognizer(async (s, e) =>
            {
                LoadSave = false;
                LoadedObjects = "";
                playedAnimation = false;

                Application.Current.MainPage = new MainPage();
            }));

            backButtonImage.Source = ImageSource.FromResource("FirstXamarin.Resources." + (isLightTheme() ? "white" : "black") + "BackArrow.png");

            Application.Current.RequestedThemeChanged += (s, a) => { backButtonImage.Source = ImageSource.FromResource("FirstXamarin.Resources." + (isLightTheme() ? "white" : "black") + "BackArrow.png"); };

            productFilter.SelectedIndexChanged += async (s, e) =>
            {
                int index = productFilter.SelectedIndex;

                Color c = (index == 0 ? Color.FromRgb(235, 169, 55) : index == 2 ? Color.FromRgb(225, 175, 183) : Color.SkyBlue);
                productFilter.BackgroundColor = c;
                pickerFrame.BackgroundColor = c;

                mainStack.Children.Clear();

                addProductsToStack();

                 await Task.Run(async () => ebayFrameStackAnimation(mainStack, 0));
                 playedAnimation = true;

            };

            if (!LoadSave)
            {
                try
                {
                    TcpClient client = new TcpClient(scrappIP == "" ? "127.0.0.1" : scrappIP, scrappPort == "" ? 80 : int.Parse(scrappPort));

                    using (NetworkStream stream = client.GetStream())
                    using (StreamWriter writer = new StreamWriter(stream))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        LoadedObjects = "";

                        string result = "";
                        while (result != "No more products")
                        {
                            result = reader.ReadLine();
                            if (result == "No more products")
                                break;

                            LoadedObjects += result + "\n";
                        }
                    }
                }
                catch (Exception)
                {
                    await DisplayAlert("No connection", "Sorry server is offline\nor\nthere are some connection problems", "OK");

                    Application.Current.MainPage = new MainPage();
                }
            }
            else
            {
                LoadedObjects = File.ReadAllText(LOADED_SAVE_PATH);
            }


            if (LoadedObjects != "")
            {
                foreach (var line in LoadedObjects.Split(new string[] { "\n-----------------------\n" }, StringSplitOptions.None))
                {
                    if (line == "") continue;

                    products.Add(new EbayProduct(line));
                }

                addProductsToStack();


                if (!playedAnimation)
                {
                    await Task.Run(() => ebayFrameStackAnimation(mainStack, 0));
                    playedAnimation = true;
                }
                
            }
            else
            {
                await DisplayAlert("No products", "Sorry, there are no objects to explore", "OK");
                Application.Current.MainPage = new MainPage();
            }

        }

        void addProductsToStack()
        {
            if (productFilter.SelectedIndex > 0)
            {
                mainStack.Children.Clear();

                int index = productFilter.SelectedIndex;
                products.Where(p => index == 0 ? true : index == 1 ? p.auktionKaufen : p.sofortKaufen)
                .ToList()
                .ForEach(p => mainStack.Children.Add(p.productToFrame()));
            }
            else
            {
                products.ForEach(p => mainStack.Children.Add(p.productToFrame()));
            }
        }

        private static bool isLightTheme()
        {
            return Application.Current.RequestedTheme == OSAppTheme.Light;
        }

        static StackLayout getTappedStack(string name, string price, string prodUrl)
        {
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                Uri uri = new Uri(prodUrl);

                Browser.OpenAsync(uri);
            };

            return new StackLayout()
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,

                Children =
                {
                    new Label() { Text = name, VerticalOptions = LayoutOptions.StartAndExpand},
                    new Label() { Text = price, VerticalOptions = LayoutOptions.EndAndExpand }
                },
                GestureRecognizers = { tap }

            };
        }


        async Task ebayFrameStackAnimation(StackLayout stack, int pos)
        {
            if (stack.Children.Count() <= pos) return;

            var frame = stack.Children.ElementAt(pos);

            if (frame.Y > Height)
            {
                frame.Opacity = 1;

                await ebayFrameStackAnimation(stack, pos + 1);

                return;
            }

            await frame.TranslateTo(0, -50, 100);

            frame.FadeTo(1, 500);
            frame.TranslateTo(0, 0, 700, Easing.CubicInOut);

            await ebayFrameStackAnimation(stack, pos + 1);
        }

        class EbayProduct
        {
            public string name { get; private set; }
            public float price { get; private set; }
            public string url { get; private set; }
            public string imgUrl { get; private set; }
            public bool sofortKaufen { get; private set; }
            public bool auktionKaufen { get; private set; }

            public EbayProduct(string text)
            {
                var lines = text.Split('\n');

                name = (lines[0].Length > 34 ? lines[0].Substring(0, 30) + "..." : lines[0]);
                string sPrice = lines[1];

                try
                {
                    price = float.Parse(sPrice);
                }
                catch (Exception)
                {
                    if (!sPrice.Contains(".")) price = -1;
                    else
                    {
                        int size = sPrice.Substring(sPrice.IndexOf(".")).Count();

                        if (size <= 2) sPrice += "0";
                        int iPrice = 0;
                        if (!int.TryParse(sPrice.Replace(".", ""), out iPrice)) price = -1;
                        else
                        {
                            price = iPrice;
                            price /= 100;
                        }
                    }
                }

                url = lines[3];
                imgUrl = lines[4];

                var sk = true;
                bool.TryParse(lines[5], out sk);

                var ak = true;
                bool.TryParse(lines[6], out ak);

                sofortKaufen = sk;
                auktionKaufen = ak;

            }

            public Frame productToFrame()
            {
                var image = new Image()
                {
                    Source = ImageSource.FromUri(new Uri(imgUrl)),
                    Aspect = Aspect.AspectFill,
                    Margin = 0,
                };

                var imageFrame = new Frame()
                {
                    BorderColor = Color.Orange,
                    CornerRadius = 10,
                    IsClippedToBounds = true,
                    HeightRequest = 100,
                    WidthRequest = 100,
                    Padding = new Thickness(0),
                    Content = image
                };

                return new Frame()
                {
                    //both => yellow, sofort => red, auktion => blue
                    BackgroundColor = ((sofortKaufen && auktionKaufen) ? Color.FromRgb(235, 169, 55) : sofortKaufen ? Color.FromRgb(225, 175, 183) : Color.SkyBlue),

                    CornerRadius = 10,
                    HasShadow = true,
                    Margin = new Thickness(10, 0),
                    Opacity = 0,
                    Content = new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        Margin = new Thickness(5, 0),
                        Children =
                                {
                                    imageFrame,
                                    getTappedStack(name, "EUR:" + price, url)
                                }
                    }
                };
            }

            
        }
    }
}


