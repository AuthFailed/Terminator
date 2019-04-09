using System;
using System.IO;
using System.Windows.Forms;

namespace New_app
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Метод для выдачи готового оповещения 
        void Mess(string line1, string line2)
        {
            MessageBox.Show(line1, line2, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Метод удаления файла
        void Del(string line)
        {
            // При отсутствии файла выводим сообщение об ошибке и очищаем поля от информации
            if (!File.Exists(line))
            {
                // Проверяем, а может это папка, а не файл?
                if (!Directory.Exists(line))
                {
                    // Если это и не файл, и не папка, то выдаем сообщение об ошибке
                    MessageBox.Show("По этому пути ничего не найдено", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    TextBox.Text = null;
                    label2.Visible = false;
                    label2.Text = null;
                }
                else
                {
                    // Если все-таки находим папку по такому пути, то спрашиваем пользователя о его решении
                    DialogResult result = MessageBox.Show("Вы уверены?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string foldername = new DirectoryInfo(TextBox.Text).Name;
                        Directory.Delete((line),true); //true - если директория не пуста удаляем все ее содержимое
                        if (!Directory.Exists(line))
                        {
                            // Если пользователь ответил согласием - удаляем папку и выводим оповещение об удалении
                            Mess($"Папка {foldername} успешно удалена", "");
                            TextBox.Text = null;
                            label1.Visible = false;
                            label2.Text = null;
                            button.Enabled = false;
                        }
                    }
                }
            }
            else
            {
                // При наличии файла спрашиваем у пользователя о его решении
                DialogResult result = MessageBox.Show("Вы уверены?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string filename = new FileInfo(TextBox.Text).Name;
                    File.Delete(line);
                    if (!File.Exists(line))
                    {
                        // Если пользователь ответил согласием - удаляем файл и выводим оповещение об удалении
                        Mess($"Файл {filename} успешно удален", null);
                        TextBox.Text = null;
                        label1.Visible = false;
                        label2.Text = null;
                        button.Enabled = false;
                    }
                }
                else
                {
                    //Если пользователь отказался удалять файл
                    Mess("Отмена операции", "Внимание!");
                }
            }
        }

        // Событие загрузки формы
        private void Form1_Load(object sender, EventArgs e)
        {
            ToolTip t = new ToolTip();
            t.SetToolTip(TextBox, "Введите полный путь");
            t.SetToolTip(button1, "Открыть меню выбор файла");
            label1.Visible = false;
        }

        // Диалоговое окно с выбором файла и запись пути в textbox
        private void BunifuThinButton21_Click(object sender, EventArgs e)
        {
            OpenFileDialog myFile = new OpenFileDialog
            {
                Title = "Выбор файла"
            };
            if (myFile.ShowDialog() == DialogResult.OK)
            {
                TextBox.Text = Convert.ToString(myFile.FileName);
            }
        }

        //Информация о файле при вставке пути на файл вручную
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            // Проверка на наличие чего-либо в textbox
            if (TextBox.Text != null)
            {
                TextBox.Text = TextBox.Text.Trim('"');
                button.Enabled = true;
                // Проверка файла на существование и выдача информации
                if(File.Exists(TextBox.Text))
                {
                    label1.Visible = true;
                    label1.Text = "Информация о файле:";
                    label2.Text = $"Создан: " + File.GetCreationTime(TextBox.Text) +
                                   "\n Открывался: " + File.GetLastAccessTime(TextBox.Text) +
                                   "\n Изменялся: " + File.GetLastWriteTime(TextBox.Text) +
                                   "\n Размер файла: " + (Math.Round(Convert.ToDouble(File.ReadAllBytes(TextBox.Text).Length) / 1048576.0, 2)) + " мбайт";
                }
                else if(Directory.Exists(TextBox.Text))
                {
                    label1.Visible = true;
                    label1.Text = "Информация о папке:";
                    DirectoryInfo FolderInfo = new DirectoryInfo(TextBox.Text);
                    label2.Text = "Создана: " + FolderInfo.CreationTime.ToString();
                }
                else
                {
                    label1.Visible = false;
                    label2.Text = null;
                }
            }
            //Если textbox стал пустым - скрываем лишнюю информацию
            else
            {
                label1.Visible = false;
                label2.Text = null;
                button.Enabled = false;
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Del(TextBox.Text);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string folderName = folderBrowserDialog1.SelectedPath;
                TextBox.Text = folderName;
            }
        }
    }
}
