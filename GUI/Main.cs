using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace GUI
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_DragDrop(object sender, DragEventArgs e)
        {
            string[] _Files = e.Data.GetData(DataFormats.FileDrop) as string[];
            string _Path = null;

            foreach (string _File in _Files)
                _Path = Path.GetFullPath(_File);

            ListViewItem _List_View_Item = new ListViewItem
            {
                Text = _Path
            };

            listView1.Items.Insert(0x0, _List_View_Item);
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listView1.AutoResizeColumn(0x1, ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void Main_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0x0)
                foreach (ListViewItem _Item in listView1.SelectedItems)
                    _Item.Remove();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog _Save_File_Dialog = new SaveFileDialog())
            {
                _Save_File_Dialog.Filter = "Executable (*.exe)|*.exe";
                _Save_File_Dialog.ShowDialog();

                string _Startup_Path = Application.StartupPath + "\\Temp";
                string _Files = "";

                Directory.CreateDirectory(_Startup_Path);

                for (int i = 0x0; i < listView1.Items.Count; i++)
                {
                    string _New_File_Name = listView1.Items[i].SubItems[0x0].Text.Remove(0x0, listView1.Items[i].SubItems[0x0].Text.LastIndexOf('\\') + 0x1).Replace(" ", string.Empty);
                    string _New_Path = _Startup_Path + "\\" + _New_File_Name;

                    File.Copy(listView1.Items[i].SubItems[0x0].Text, _New_Path);

                    _Files += string.Format("Section \"{0}\"\r\nSetOutPath \"${1}\"\r\nSetOverwrite on\r\nFile \"{2}\"\r\nExec ${3}\\{4}\r\nSectionEnd\r\n\r\n", "file" + i, "Temp", _New_Path, "Temp", _New_Path.Remove(0x0, _New_Path.LastIndexOf('\\') + 0x1));
                }

                File.AppendAllText("source.txt", "OutFile \"out.exe\"\r\nSilentInstall silent\r\nRequestExecutionLevel user\r\n" + _Files);
                Make();
                File.Copy(Application.StartupPath + "\\out.exe", _Save_File_Dialog.FileName);
                Directory.Delete(_Startup_Path, true);
                File.Delete("source.txt");
                File.Delete("out.exe");
                MessageBox.Show("Done");
            }
        }

        private void Make()
        {
            using (Process _Process = new Process())
            {
                _Process.StartInfo.CreateNoWindow = true;
                _Process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                _Process.StartInfo.FileName = Application.StartupPath + "\\NSIS\\makensis.exe";
                _Process.StartInfo.UseShellExecute = false;
                _Process.StartInfo.RedirectStandardOutput = true;
                _Process.StartInfo.Arguments = "source.txt";
                _Process.Start();
                _Process.BeginOutputReadLine();
                _Process.WaitForExit();
            }
        }
    }
}