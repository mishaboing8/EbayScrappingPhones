using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FirstXamarin
{
    public partial class OnlineSendRequestPage : ContentPage
    {
        public OnlineSendRequestPage()
        {
            InitializeComponent();
        }

        static List<SearchingSettings> list = new List<SearchingSettings>();

        protected override void OnAppearing()
        {
            //list.Add(new SearchingSettings());
            //searchingStack.Children.Add(list[0].mainFrame);


            plusImage.Source = ImageSource.FromResource("FirstXamarin.Resources.plusIcon.png");
            minusImage.Source = ImageSource.FromResource("FirstXamarin.Resources.minusIcon.png");

            addTagPlusImage.Source = plusImage.Source;
            removeTagPlusImage.Source = minusImage.Source;
            //removeTagPlusImage2.Source = minusImage.Source;
            //removeTagPlusImage3.Source = minusImage.Source;

            addTagPlusImage.GestureRecognizers.Add(new TapGestureRecognizer((s, e) =>
            {
                var addedStack = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        new Frame()
                        {
                            BackgroundColor = Color.Gray,
                            Padding = 0,
                            Content = new StackLayout()
                            {
                                Orientation = StackOrientation.Horizontal,
                                BackgroundColor = Color.Gray,
                                Children =
                                {
                                    new Entry()
                                    {
                                        Text = "Tag",
                                        HorizontalTextAlignment = TextAlignment.Center,
                                        WidthRequest = 100
                                    },
                                    new Frame()
                                    {
                                        BackgroundColor = Color.OrangeRed,
                                        IsClippedToBounds = true,
                                        Padding = 5,
                                        Content = new Image()
                                        {
                                            Aspect = Aspect.AspectFill,
                                            HeightRequest = 30,
                                            WidthRequest = 30,
                                            Source = minusImage.Source
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                addRemoveSearchingTags.Children.Insert(addRemoveSearchingTags.Children.Count - 2, addedStack);
            }
                ));
            plusImage.GestureRecognizers.Add(new TapGestureRecognizer((s, e) =>
            {
                var tag = new SearchingSettings();
                list.Add(tag);
                searchingStack.Children.Add(tag.mainFrame);
                }));

            minusImage.GestureRecognizers.Add(new TapGestureRecognizer((s, e) => {
                if (searchingStack.Children.Count > 1)
                {
                    searchingStack.Children.RemoveAt(searchingStack.Children.Count - 1);
                    list.RemoveAt(list.Count - 1);
                }
                }));
        }




        public static void checkDigitEntry(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            var entry = (Entry)sender;
            var reg = new Regex("\\D");
            if (reg.IsMatch(entry.Text)) entry.Text = "";
        }

        class SearchingSettings
        {
            public Frame mainFrame { private set; get; }
            public int minPrice { private set; get; }
            public int maxPrice { private set; get; }
            public string searchingTag { private set; get; }
            public int selectedSorting { private set; get; }

            public SearchingSettings()
            {
                minPrice = -1;
                maxPrice = -1;
                selectedSorting = -1;

                var minEntry = new Entry() { Text = "Min", HorizontalOptions = LayoutOptions.Start, Keyboard = Keyboard.Numeric };
                var maxEntry = new Entry() { Text = "Max", HorizontalOptions = LayoutOptions.Start, Keyboard = Keyboard.Numeric };
                var picker = new Picker() { Title = "Sorting", HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, Items = { "Default", "Price min-max", "Price max-min" } };
                var searchingTagEntry = new Entry() { Text = "Searching tag", VerticalOptions = LayoutOptions.Start, HorizontalOptions = LayoutOptions.FillAndExpand };

                minEntry.TextChanged += (s, e) => parseEntryPrice(s, e, true);
                maxEntry.TextChanged += (s, e) => parseEntryPrice(s, e, false);
                picker.SelectedIndexChanged += (s, e) => selectedSorting = picker.SelectedIndex;
                searchingTagEntry.TextChanged += (s, e) => this.searchingTag = searchingTagEntry.Text;

                mainFrame = new Frame()
                {
                    BackgroundColor = Color.LightBlue,
                    HeightRequest = 70,
                    Content = new StackLayout()
                    {
                        Orientation = StackOrientation.Vertical,
                        Children =
                        {
                            searchingTagEntry,
                            new StackLayout()
                            {
                                Orientation = StackOrientation.Horizontal,
                                Children =
                                {
                                    minEntry,
                                    maxEntry,
                                    picker
                                }
                            }
                        }
                    }
                };


                void parseEntryPrice(System.Object sender, Xamarin.Forms.TextChangedEventArgs e, bool min)
                {
                    var entry = (Entry)sender;
                    checkDigitEntry(sender, e);

                    int pr = -1;
                    if(int.TryParse(entry.Text, out pr))
                    {
                        if (min) minPrice = pr;
                        else maxPrice = pr;
                    }
                }
            }
        }

        void ResizeLabelFrame(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            var entry = (Entry)sender;
            var parent = (Frame)entry.Parent;

            parent.HeightRequest = entry.HeightRequest;
            parent.WidthRequest = entry.WidthRequest;
        }
    }
}

