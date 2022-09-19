using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.IO;
using System.Net;
using Xamarin.Forms.Shapes;
using Path = System.IO.Path;
using Lottie.Forms;

namespace FirstXamarin
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public static bool LoadSave = false;
        public static string LoadedObjects = "";
        public readonly static string LOADED_SAVE_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LastSave.txt");

        private StackLayout buttonStack(){
            return new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Padding = new Thickness(30, 0),
                Children = {
                         new Frame
                         {
                             Content = LoadOnlineStack(),
                             CornerRadius = 10,
                             HorizontalOptions = LayoutOptions.CenterAndExpand,
                             BackgroundColor = Color.Coral,
                             WidthRequest = 120
                         },
                         new Frame
                         {
                            Content = LoadObjectsFromSaveButton(),
                            CornerRadius = 10,
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            BackgroundColor = Color.Coral,
                            WidthRequest = 120
                         },
                    }
            };
        }

        protected override async void OnAppearing()
        {
            TopArrowImage.Source = ImageSource.FromResource("FirstXamarin.Resources.orangeArrow.png");
            BottomArrowImage.Source = ImageSource.FromResource("FirstXamarin.Resources.orangeArrow.png");

            double stackY = 0;

            stackY = Height / 2 - TopArrowImage.Height - apearingAnimationStack.Height;
            new Animation
                    {
                        { 0 , 0.5, new Animation(v => TopArrowImage.TranslationY = v, 0, stackY == 0 ? 20 : stackY) },
                        { 0.5, 1, new Animation(v => TopArrowImage.TranslationY = v, stackY == 0 ? 20 : stackY, 0)}
                    }.Commit(this, "TopArrowAnimation", 16, 4000, null, (v, c) => TopArrowImage.TranslationY = 0, () => true);

            new Animation
                    {
                        { 0 , 0.5, new Animation(v => BottomArrowImage.TranslationY = v, 0, -(stackY == 0 ? 20 : stackY)) },
                        { 0.5, 1, new Animation(v => BottomArrowImage.TranslationY = v, -(stackY == 0 ? 20 : stackY), 0)}
                    }.Commit(this, "BottomArrowAnimation", 16, 4000, null, (v, c) => BottomArrowImage.TranslationY = 0, () => true);
        }

        void RemoveAnimationAddButtons(object sender, EventArgs eventArgs)
        {
            mainCatchStack.Children.RemoveAt(1);
            mainCatchStack.Children.Insert(1, buttonStack());
        }


        private Image ArrowImage()
        {
            var result = new Image
            {
                HeightRequest = 200,
                WidthRequest = 200,
                Source = ImageSource.FromResource("FirstXamarin.Resources.orangeArrow.png"),
            };
            return result;
        }

        private Button LoadObjectsFromSaveButton()
        {
            var button = new Button() { Text = "Try last save", TextColor = Color.WhiteSmoke, FontAttributes = FontAttributes.Bold };
            button.Clicked += async (s, e) =>
            {
                LoadedObjectsContent.LoadSave = true;

                this.AbortAnimation("TopArrowAnimation");
                this.AbortAnimation("BottomArrowAnimation");

                Application.Current.MainPage = new LoadedObjectsContent();
            };
            return button;


        }

        private Button LoadOnlineStack()
        {
            var button = new Button() { Text = "Try online  ", TextColor = Color.WhiteSmoke, FontAttributes = FontAttributes.Bold };
            button.Clicked += async (s, e) => {
                LoadedObjectsContent.LoadSave = false;

                var parentList = ((StackLayout)((Frame)((Button)s).Parent).Parent).Children;
                parentList.Clear();

                parentList.Add(ServerConnectPropertiesFrame());

                //this.AbortAnimation("TopArrowAnimation");
                //this.AbortAnimation("BottomArrowAnimation");

                //Application.Current.MainPage = new LoadedObjectsContent();
            };

            return button;
        }

        private Frame ServerConnectPropertiesFrame()
        {
            var enterIpEntry = new Entry()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = "Enter IP",
            };

            enterIpEntry.TextChanged += (s, e) => LoadedObjectsContent.scrappIP = enterIpEntry.Text;

            var enterPortEntry = new Entry()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Text = "Port",
                WidthRequest = 100
            };

            enterPortEntry.TextChanged += (s, e) => LoadedObjectsContent.scrappPort = enterPortEntry.Text;

            var acceptFrame = new Frame()
            {
                BackgroundColor = Color.GreenYellow, BorderColor = Color.Gray,
                CornerRadius = 10,

                HorizontalOptions = LayoutOptions.End,
                Padding = 5,

                IsClippedToBounds = true,

                GestureRecognizers =
                {
                    new TapGestureRecognizer((s, e) => {
                        this.AbortAnimation("TopArrowAnimation");
                        this.AbortAnimation("BottomArrowAnimation");

                        Application.Current.MainPage = new LoadedObjectsContent();
                    })
                },

                Content = new Image()
                {
                    HeightRequest = 30,
                    WidthRequest = 30,

                    Source = ImageSource.FromResource("FirstXamarin.Resources.acceptCheck128.png"),
                    Aspect = Aspect.AspectFill
                }
            };

            var declineFrame = new Frame()
            {
                BackgroundColor = Color.OrangeRed,
                BorderColor = Color.Gray,
                CornerRadius = 10,

                HorizontalOptions = LayoutOptions.Start,
                Padding = 5,

                IsClippedToBounds = true,
                GestureRecognizers =
                {
                    new TapGestureRecognizer((s, e) => RemoveAnimationAddButtons(s, (EventArgs)e))
                },

                Content = new Image()
                {
                    Source = ImageSource.FromResource("FirstXamarin.Resources.declineCheck128.png"),
                    Aspect = Aspect.AspectFit,
                    HeightRequest = 30,
                    WidthRequest = 30,

                }
            };

            var frame = new Frame(){
                CornerRadius = 10,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                BackgroundColor = Color.Coral,
                WidthRequest = 240,

                Content = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    Children =
                    {
                        enterIpEntry,
                        new StackLayout()
                        {
                            Orientation = StackOrientation.Horizontal,
                            Children =
                            {
                                declineFrame,
                                enterPortEntry,
                                acceptFrame
                            }
                        }
                    }
                }
            };


            return frame;
        }
    }
}

