using System;
namespace FirstXamarin
{
    public class XamlContents
    {
        public static readonly string SEARCHING_FRAME = "<Frame BackgroundColor=\"LightBlue\" HeightRequest=\"70\">\n                    <StackLayout Orientation=\"Vertical\">\n                        <Entry Text=\"Searching tag\" VerticalOptions=\"Start\" HorizontalOptions=\"FillAndExpand\"/>\n                        <StackLayout Orientation=\"Horizontal\">\n                            <Entry Text=\"Min\" HorizontalOptions=\"Start\" TextChanged=\"checkDigitEntry\" Keyboard=\"Numeric\"/>\n                            <Entry Text=\"Max\" HorizontalOptions=\"Start\" TextChanged=\"checkDigitEntry\" Keyboard=\"Numeric\"/>\n\n                            <Picker Title=\"Sorting\" HorizontalOptions=\"FillAndExpand\" HorizontalTextAlignment=\"Center\">\n</Picker>\n                        </StackLayout>\n                    </StackLayout>\n                </Frame>";
        private XamlContents() { }
    }
}

