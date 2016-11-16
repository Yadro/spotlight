using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace spotlight
{
    public class HighlightTextBlock : TextBlock
    {
        public static DependencyProperty HighlightTextProperty =
            DependencyProperty.Register("HighlightText", typeof(string), typeof(HighlightTextBlock));

        public string HighlightText
        {
            get { return (string) GetValue(HighlightTextProperty); }
            set { SetValue(HighlightTextProperty, value); }
        }

        public override void EndInit()
        {
            base.EndInit();
            if (HighlightText == null)
                return;

            string[] alignment = HighlightText.Split(' ');
            Inlines.Clear();
            foreach (var str in alignment)
            {
                var regex = new Regex(str, RegexOptions.IgnoreCase);
                Match match = regex.Match(Text);

                Inlines.Add(Text.Substring(0, match.Index));
                Inlines.Add(new Run()
                {
                    Text = Text.Substring(match.Index, match.Length),
                    FontWeight = FontWeights.Bold
                });
                Inlines.Add(Text.Substring(match.Index + match.Length));
            }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // todo
        }
    }
}