using System;
using System.Windows;
using System.Windows.Controls;

namespace OnlineJournal.CustomControls
{
    public partial class BindablePasswordBox : UserControl
    {
        public static readonly DependencyProperty PasswordProperty;

        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        static BindablePasswordBox()
        {
            PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(BindablePasswordBox));
        }

        public BindablePasswordBox()
        {
            InitializeComponent();
            txtPassword.PasswordChanged += OnPasswordChanged;
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = txtPassword.Password;
        }
    }
}
