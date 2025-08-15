// ------------------------------------------------------------
// File: MainWindow.xaml.cs
// Purpose: Main window logic for Calculator app with live comma formatting.
// ------------------------------------------------------------

using System;
using System.Data;
using System.Windows;
using System.Windows.Media;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        // Stores raw input string without commas
        private string input = "";

        // Stores last evaluated expression
        private string lastExpression = "";

        public MainWindow()
        {
            InitializeComponent();
            Loaded += (s, e) => AttachButtonEvents();
            UpdateDisplay("0");
        }

        private void AttachButtonEvents()
        {
            AttachClickEventRecursive(this);
        }

        private void AttachClickEventRecursive(DependencyObject parent)
        {
            if (parent == null) return;

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is View.Button.CircleButton circleButton)
                {
                    circleButton.Click += Button_Click;
                }
                else if (child is View.Button.YellowCircleButton yellowButton)
                {
                    yellowButton.Click += Button_Click;
                }
                else
                {
                    AttachClickEventRecursive(child);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is View.Button.CircleButton btn)
            {
                HandleInput(btn.ButtonContent);
            }
            else if (sender is View.Button.YellowCircleButton ybtn)
            {
                HandleInput(ybtn.ButtonContent);
            }
        }

        private void HandleInput(string content)
        {
            switch (content)
            {
                case "AC":
                    input = "";
                    lastExpression = "";
                    UpdateDisplay("0");
                    history.Text = "";
                    break;

                case "+/-":
                    if (double.TryParse(input, out double val))
                    {
                        val = -val;
                        input = val.ToString();
                        UpdateDisplay(input);
                    }
                    break;

                case "%":
                    if (double.TryParse(input, out double percentVal))
                    {
                        percentVal /= 100;
                        input = percentVal.ToString();
                        UpdateDisplay(input);
                    }
                    break;

                case "=":
                    try
                    {
                        string expression = input.Replace("x", "*");
                        var result = new DataTable().Compute(expression, null);

                        history.Text = input + " =";

                        double d = Convert.ToDouble(result);

                        if (Math.Abs(d) >= 1_000_000_000 || (Math.Abs(d) > 0 && Math.Abs(d) < 0.0001))
                        {
                            input = d.ToString("0.#####e+0");
                        }
                        else if (d % 1 == 0)
                        {
                            input = d.ToString("F0");
                        }
                        else
                        {
                            input = d.ToString("G9");
                        }
                        UpdateDisplay(input);
                    }
                    catch
                    {
                        UpdateDisplay("Error");
                        input = "";
                    }
                    break;

                default:
                    if ("0123456789.".Contains(content) || content == "00")
                    {
                        // Remove commas before adding new digit
                        input = input.Replace(",", "");

                        if (content == ".")
                        {
                            string[] parts = input.Split(new char[] { '+', '-', 'x', '/', '%' }, StringSplitOptions.RemoveEmptyEntries);
                            string currentNumber = parts.Length > 0 ? parts[^1] : "";

                            if (currentNumber.Contains("."))
                                return;
                        }

                        input += content;
                        UpdateDisplay(input);
                    }
                    else if ("+-x/%".Contains(content.ToLower()))
                    {
                        if (input.Length > 0)
                        {
                            input = input.Replace(",", "");

                            if (IsOperator(input[^1]))
                            {
                                input = input.Substring(0, input.Length - 1) + content.ToLower();
                            }
                            else
                            {
                                input += content.ToLower();
                            }
                            UpdateDisplay(input);
                        }
                    }
                    break;
            }
        }

        private bool IsOperator(char c)
        {
            return "+-x/%".Contains(c);
        }

        // Formats a number with commas (keeps decimal part)
        private string FormatNumberWithCommas(string number)
        {
            if (string.IsNullOrEmpty(number)) return number;

            if (number.Contains("."))
            {
                var parts = number.Split('.');
                if (long.TryParse(parts[0].Replace(",", ""), out long intPart))
                {
                    return intPart.ToString("N0") + "." + parts[1];
                }
                return number;
            }
            else
            {
                if (long.TryParse(number.Replace(",", ""), out long intPart))
                {
                    return intPart.ToString("N0");
                }
                return number;
            }
        }

        private void UpdateDisplay(string text)
        {
            string formatted = text;
            char[] ops = { '+', '-', 'x', '/', '%' };

            int lastOpIndex = text.LastIndexOfAny(ops);
            if (lastOpIndex >= 0)
            {
                string beforeOp = text.Substring(0, lastOpIndex);
                string op = text[lastOpIndex].ToString();
                string afterOp = text.Substring(lastOpIndex + 1);

                formatted = FormatNumberWithCommas(beforeOp) + op + FormatNumberWithCommas(afterOp);
            }
            else
            {
                formatted = FormatNumberWithCommas(text);
            }

            calculator.Text = formatted;
        }
    }
}