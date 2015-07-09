using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;

namespace ExpenseIt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        private UnicodeEncoding uniEnoding = new UnicodeEncoding();
        private RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

        private byte[]  globalData;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var data = uniEnoding.GetBytes(InputText.Text);
            var encryptedData = await Encrypt(data, RSA.ExportParameters(false), false);
            globalData = encryptedData;
            
            //OutputText.Text = uniEnoding.GetString(encryptedData);
            
            StringBuilder sb = new StringBuilder();
            foreach(var b in encryptedData)
            {
                sb.Append(b + " ");
            }
            OutputText.Text = sb.ToString();            
        }



        static public Task<byte[]> Encrypt(byte[] data, RSAParameters RSAKey, bool doOAEPadding)
        {
            return Task.Run(() =>
            {
                try
                {
                    byte[] encryptedaData;
                    using (RSACryptoServiceProvider RSAprovider = new RSACryptoServiceProvider())
                    {
                        RSAprovider.ImportParameters(RSAKey);
                        encryptedaData = RSAprovider.Encrypt(data, doOAEPadding);
                        return encryptedaData;
                    }
                }
                catch (CryptographicException)
                {
                    return null;
                }
            });
        }


        static public Task<byte[]> Decrypt(byte[] data, RSAParameters RSAKey, bool doOAEPadding)
        {
            return Task.Run(() =>
            {
                try
                {
                    byte[] decryptedData;
                    using (RSACryptoServiceProvider RSAProvider = new RSACryptoServiceProvider())
                    {
                        RSAProvider.ImportParameters(RSAKey);
                        decryptedData = RSAProvider.Decrypt(data, doOAEPadding);
                        return decryptedData;
                    }
                }
                catch (CryptographicException)
                {
                    return null;
                }
            });
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string text = OutputText.Text;
            byte[] data = new byte[128];
            string[] tokens = text.Split();
            
            for(int i = 0; i < data.Length; i++)
            {
                try
                {
                    data[i] = byte.Parse(tokens[i]);
                }
                catch(FormatException)
                {
                    DecryptedText.Text = "Invalid code was entered";
                }
            }
            
            var res = await Decrypt(data, RSA.ExportParameters(true), false);
            
            if (res != null)
            {
                DecryptedText.Text = uniEnoding.GetString(res);
            }
            else
            {
                var r = await Decrypt(globalData, RSA.ExportParameters(true), false);
                if(r != null)
                {
                    //DecryptedText.Text = uniEnoding.GetString(r); 
                }
                else
                {
                    DecryptedText.Text = "An error occurred";
                }
            }
            
        }
    }
}
