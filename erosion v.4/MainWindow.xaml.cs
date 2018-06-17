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

using System.IO;
using DamienG.Security.Cryptography;
using System.Windows.Forms;

namespace erosion_v._4
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        String narchivo = string.Empty;
        FileInfo f;
        FileInfo fbackup;
        FileStream archivo;
        Byte eegcounter = 0;

        private int hex2dec(string hexString)
        {

            try
            {
                return Convert.ToInt32(hexString, 16);
            }
            catch (Exception e)
            {
                return 0;
            }

        }
        private string crc32(string file)
        {
            Crc32 crc32 = new Crc32();
            String hash = String.Empty;

            using (FileStream fs = File.Open(file, FileMode.Open))
                foreach (byte b in crc32.ComputeHash(fs)) hash += b.ToString("x2").ToLower();

            return hash;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void eegg(object sender, RoutedEventArgs e)
        {
            eegcounter++;
            if (eegcounter > 20) sml.Visibility=Visibility.Hidden;
            else sml.Visibility = Visibility.Visible;

        }

        //Menus archivo
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DeleteAllBackups_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int i = 0;
                foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.backup"))
                {
                    i++;
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                    log.AppendText("Backup " + file + " deleted!\n");
                }
                Backupfile.IsEnabled = true;
                System.Windows.Forms.MessageBox.Show(i + " backups deleted", "Done!", MessageBoxButtons.OK);
                log.AppendText("All backups deleted!\n");
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("Error deleting backups", "Error", MessageBoxButtons.OK);
                log.AppendText("Error deleting backups\n");
            }
        }

        private void WhatToDo_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("With this program you can change arbitrary bytes in ROMs, documents and files to see the corruption results.\n\nAlso can help you to know where are the contents of a file and find the offsets, for example:\n\nIf you want to know where are the graphics of a ROM, just try to erode the rom at different intervals of the file with the slider until the result is corrupt graphics", "What can I do with Erosion?", MessageBoxButtons.OK);
        }

        private void HowToUse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("The first of all is to load the file you want to erode(corrupt) then you can choose between making a backup of the file checking the checkbox (In the same folder of the program) or not. Remember that if you dont make a backup, eroded file will become permamently damaged!\n\nOnce the file is opened you can select the interval of the file which will be eroded inserting the offsets in hexadecimal or using the sliders.\n\nFor the corruption options, the program is divided into filters , intervals , and new values.\n\nFilters:\nAllows you to avoid eroding certain bytes. To do this you must select that unique bytes that will be changed. This allows you to for example avoid eroding sync bytes in a stream, or just erode That Kind of bytes.\n\nIntervals:\nSelect how often the bytes are changed. Very low values ​​will make the program finish much later and make the file useless. We recommend testing with +-1000 and up or down depending on the results. This range can also be random if they want to see different results with the same level of erosion.\n\nNew values:\nAllows you to choose the value of the byte that will replace the previous one. This can be fixed, random, or based on the previous one.", "How to use", MessageBoxButtons.OK);
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Erosion v.4\n\nMade by Juanmv94, All rights reserved (I think)\n\nhttp://tragicomedy-hellin.blogspot.com\n\nThere must be an Easter Egg somewhere...", "About", MessageBoxButtons.OK);
        }


        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                f = new FileInfo(openFileDialog1.FileName);
                if (f.Length > 2147483600)
                {
                    System.Windows.Forms.MessageBox.Show("Maximum file size is 2GB", "File too big!", MessageBoxButtons.OK);
                    return;
                }
                narchivo = openFileDialog1.FileName;
                fbackup = new FileInfo(f.Name + ".backup");
                if (fbackup.Exists)
                {
                    Backupfile.IsChecked = true;
                    Backupfile.IsEnabled = false;
                    log.AppendText("Backup found\n");
                }
                else
                {
                    Backupfile.IsEnabled = true;
                }


                filepath.Header = narchivo;
                if (f.Length > 67200000)
                {
                    crc.Content = "CRC32 skipped (>64Mb)";
                }
                else
                {
                    try
                    {
                        crc.Content = "CRC32: " + crc32(narchivo);
                    }
                    catch (Exception ex)
                    {
                        crc.Content = "CRC32: File busy";
                    }
                }
                sizehex.Content = "Size (hex): " + f.Length.ToString("X");
                sizedec.Content = "Size (dec): " + f.Length;
                log.AppendText("Opened File: " + narchivo + "\n-------------------------\n");
                                 startof.IsEnabled = true;
                startof.Text = "0";
                endof.IsEnabled = true;
                //endof.Text = f.Length.ToString("X");
                Startsl.IsEnabled = true;
                Endsl.IsEnabled = true;
                Startsl.Maximum = f.Length - 1;
                Endsl.Maximum = f.Length - 1;
                Endsl.Value = f.Length - 1;


                
            }
        }

        //IU genericos
        private void bytefield_TextChanged(object sender, TextChangedEventArgs e)
        {
            int n = 0;
            try
            {
                n = Int32.Parse(((System.Windows.Controls.TextBox)sender).Text);
                if (n < 0)
                {
                    n = 0;
                }
                else if (n > 255)
                {
                    n = 255;
                }
            }
            catch (Exception ex) { }
            finally
            {
                ((System.Windows.Controls.TextBox)sender).Text = n.ToString();
            }
        }
        
        //IU Botones
        private void Savetxt_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.DefaultExt = "txt";
            saveFileDialog1.Filter = "Super-geek Erosion v.4 log file | *.txt";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter st1 = new StreamWriter(saveFileDialog1.FileName);
                st1.Write(log.Text);
                st1.Close();

            }

        }
        private void Clearlog_Click(object sender, RoutedEventArgs e)
        {
            log.Clear();
        }
        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            if (!Backupfile.IsEnabled && Backupfile.IsChecked.Value)
            {
                try
                {
                    fbackup.CopyTo(narchivo,true);
                    f = new FileInfo(narchivo);
                    f.IsReadOnly = false;           //Evita crear archivo readonly cuando el backup es readonly
                    log.AppendText("File restored!\n");
                }
                catch (FileNotFoundException)
                {
                    System.Windows.Forms.MessageBox.Show("Backup file not found", "Backup Error", MessageBoxButtons.OK);
                    log.AppendText("Backup error\n");
                }
                catch (Exception)
                {
                    System.Windows.Forms.MessageBox.Show("file is Read Only", "Backup Error", MessageBoxButtons.OK);
                    log.AppendText("Backup error\n");
                }
            }

             
        }
        private void Erode_Click(object sender, RoutedEventArgs e)
        {
            if (narchivo.Equals(String.Empty))
            {
                Open_Click(sender, e);
                return;
            }

            if (( filterrange.IsChecked.Value && (Int32.Parse(bytefil2.Text) > Int32.Parse(bytefil3.Text))) || ( Interr.IsChecked.Value && (Int32.Parse(interr1.Text) > Int32.Parse(interr2.Text))) || ( valuerfrom.IsChecked.Value && (Int32.Parse(val2.Text) > Int32.Parse(val3.Text))))
            {
                System.Windows.Forms.MessageBox.Show("R1>R2", "Wrong range!", MessageBoxButtons.OK);
                return;
            }

            f = new FileInfo(narchivo);
            if (f.Exists && f.IsReadOnly)
            {
                System.Windows.Forms.MessageBox.Show("File is read-only", "Can't Erode", MessageBoxButtons.OK);
                return;
            }

            fbackup = new FileInfo(f.Name + ".backup");
            try
            {
                if (!fbackup.Exists && Backupfile.IsChecked.Value)
                {
                    f.CopyTo(f.Name + ".backup");
                    Backupfile.IsEnabled = false;
                    log.AppendText("Backup created!\n");
                }
                if (fbackup.Exists)
                {
                    Backupfile.IsEnabled = false;
                    Backupfile.IsChecked = true;

                    if (RestoreBeforeReErode.IsChecked.Value)
                    {
                        Restore_Click(sender, e);
                    }

                }
                log.AppendText("Starting eroding\n");
                erode();
            }
            catch (FileNotFoundException)
            {
                System.Windows.Forms.MessageBox.Show("File was deleted", "Error", MessageBoxButtons.OK);
                log.AppendText("Error: file not found\n");
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("Error creating backup: Read-Only filesystem", "Error", MessageBoxButtons.OK);
                log.AppendText("Error creating backup: Read-Only filesystem\n");
            }

            
        }

        
        //IU casillas
        private void Backupfile_Checked(object sender, RoutedEventArgs e)
        {
            if (Backupfile.IsChecked.Value)
            {
                RestoreBeforeReErode.IsEnabled = true;
            }
            else
            {
                RestoreBeforeReErode.IsEnabled = false;
                RestoreBeforeReErode.IsChecked = false;
            }
        }

        //IU offsets y sliders
        private void startof_TextChanged(object sender, TextChangedEventArgs e)
        {
            Startsl.Value = hex2dec(startof.Text);
            if (Startsl.Value >= f.Length)
            {
                startof.Text = f.Length.ToString("X");
            }
            if (Startsl.Value <= 0)
            {
                startof.Text = "0";
            }
            if (Startsl.Value > Endsl.Value)
            {
                startof.Text = endof.Text;
                //Startsl.Value = Endsl.Value;
            }
        }
        private void endof_TextChanged(object sender, TextChangedEventArgs e)
        {
            Endsl.Value = hex2dec(endof.Text);
            if (Endsl.Value >= f.Length)
            {
                endof.Text = f.Length.ToString("X");
            }
            if (Endsl.Value <= 0)
            {
                endof.Text = "0";
            }
            if (Startsl.Value > Endsl.Value)
            {
                endof.Text = startof.Text;
                //Endsl.Value = Startsl.Value;
            }
        }
        private void Startsl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Startsl.Value > Endsl.Value) Startsl.Value = Endsl.Value;
            startof.Text = ((Int32)Startsl.Value).ToString("X");
        }
        private void Endsl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Startsl.Value > Endsl.Value) Endsl.Value = Startsl.Value;
            endof.Text = ((Int32)Endsl.Value).ToString("X");
        }

        //IU filtro de erosion
        private void filternone_Checked(object sender, RoutedEventArgs e)
        {
            bytefil1.IsEnabled = false;
            bytefil2.IsEnabled = false;
            bytefil3.IsEnabled = false;
        }
        private void filterbyte_Checked(object sender, RoutedEventArgs e)
        {
            bytefil1.IsEnabled = true;
            bytefil2.IsEnabled = false;
            bytefil3.IsEnabled = false;
        }   
        private void filterrange_Checked(object sender, RoutedEventArgs e)
        {

            bytefil1.IsEnabled = false;
            bytefil2.IsEnabled = true;
            bytefil3.IsEnabled = true;
        }

        //IU Intervalo
        private void Interpred_Checked(object sender, RoutedEventArgs e)
        {
            interpred.IsEnabled = true;
            interr1.IsEnabled = false;
            interr2.IsEnabled = false;
        }
        private void Interr_Checked(object sender, RoutedEventArgs e)
        {
            interpred.IsEnabled = false;
            interr1.IsEnabled = true;
            interr2.IsEnabled = true;
        }

        private void rangefield_TextChanged(object sender, TextChangedEventArgs e)
        {
            int n = 1;
            try
            {
                n = Int32.Parse(((System.Windows.Controls.TextBox)sender).Text);
                if (n < 1)
                {
                    n = 1;
                }
                else if (n > 999999)
                {
                    n = 999999;
                }
            }
            catch (Exception ex) { }
            finally
            {
                ((System.Windows.Controls.TextBox)sender).Text = n.ToString();
            }
        }

        //IU new Value
        private void value_Checked(object sender, RoutedEventArgs e)
        {
            switch (((System.Windows.Controls.RadioButton)sender).Name)
            {
                case "valuespec":
                    val1.IsEnabled = true;
                    val2.IsEnabled = false;
                    val3.IsEnabled = false;
                    val4.IsEnabled = false;
                    break;
                case "valuerfrom":
                    val1.IsEnabled = false;
                    val2.IsEnabled = true;
                    val3.IsEnabled = true;
                    val4.IsEnabled = false;
                    break;
                case "valueincr":
                case "valuedecr":
                    val1.IsEnabled = false;
                    val2.IsEnabled = false;
                    val3.IsEnabled = false;
                    val4.IsEnabled = true;
                    break;
                default:
                    val1.IsEnabled = false;
                    val2.IsEnabled = false;
                    val3.IsEnabled = false;
                    val4.IsEnabled = false;
                    break;
            }

        }

        //Codigo principal del programa
        public void erode()
        {
            try
            {
                archivo = new FileStream(narchivo, FileMode.Open, FileAccess.ReadWrite);
                int pointer = (Int32)Startsl.Value;
                int end = (Int32)Endsl.Value;
                Random a = new Random();
                byte[] newval = new byte[1];
                byte[] oldval = new byte[1];
                int interval = Int32.Parse(interpred.Text);  //inicializa en intervalo fijo
                int intrand1 = Int32.Parse(interr1.Text);
                int intrand2 = Int32.Parse(interr2.Text);

                int f1 = Int32.Parse(val2.Text);
                int f2 = Int32.Parse(val3.Text);      //campos de nuevo valor de byte
                int f3 = Int32.Parse(val4.Text);


                if (valuespec.IsChecked.Value) newval[0] = byte.Parse(val1.Text);  //valor fijo

                while (pointer <= end)
                {
                    if (filterbyte.IsChecked.Value || filterrange.IsChecked.Value || valueincr.IsChecked.Value || valuedecr.IsChecked.Value || valueinv.IsChecked.Value)
                    {
                        archivo.Seek(pointer, SeekOrigin.Begin);
                        archivo.Read(oldval, 0, 1);       //Lee byte antiguo solo cuando es necesario
                    }

                    //Calculamos valor nuevo byte       ¿Optimizable?
                    if (valuerand.IsChecked.Value) a.NextBytes(newval);
                    else if (valuerfrom.IsChecked.Value) newval[0] = (Byte)(a.Next(f2 - f1 + 1) + f1);
                    else if (valueincr.IsChecked.Value) newval[0] = (Byte)(oldval[0] + f3);
                    else if (valuedecr.IsChecked.Value) newval[0] = (Byte)(oldval[0] - f3);
                    else if (valueinv.IsChecked.Value) newval[0] = (Byte)(oldval[0] ^ 255);

                    if (filternone.IsChecked.Value || (filterbyte.IsChecked.Value && (Int32.Parse(bytefil1.Text) == oldval[0])) || (filterrange.IsChecked.Value && (Int32.Parse(bytefil2.Text) <= oldval[0]) && (Int32.Parse(bytefil3.Text) >= oldval[0])))
                    {       //Condiciones Filtros
                        archivo.Seek(pointer, SeekOrigin.Begin);
                        archivo.Write(newval, 0, 1);   //reemplazamos byte
                        log.AppendText("0x" + pointer.ToString("X") + " = " + newval[0] + "\n");

                    }

                    //incrementamos pointer
                    if (!Interpred.IsChecked.Value) //si aleatorio
                    {
                        interval = a.Next(intrand2 - intrand1 + 1) + intrand1;
                    }
                    pointer += interval;

                }
                archivo.Close();
                log.AppendText("Corruption finished\n");
                System.Media.SystemSounds.Beep.Play();
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("File can't be accessed", "Error", MessageBoxButtons.OK);
                log.AppendText("Error: file can't be accessed\n");
            }
        }

    }

}
