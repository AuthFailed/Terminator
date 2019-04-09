using System;
using System.IO;
using System.Threading.Tasks;
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
                    MessageBox.Show("По этому пути ничего не найдено!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    button.Enabled = false;
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
                    try
                    {
                        File.Delete(line);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Не удалось удалить файл! \n Код ошибки скопирован в буфер обмена.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Clipboard.Clear();
                        Clipboard.SetText(ex.ToString());
                    }
                    if (!File.Exists(line))
                    {
                        // Если пользователь ответил согласием - удаляем файл и выводим оповещение об удалении
                        Mess($"Файл {filename} успешно удален", "");
                        TextBox.Text = null;
                        label1.Visible = false;
                        label2.Text = null;
                        button.Enabled = false;
                    }
                }
            }
        }

        // Событие загрузки формы
        async void Form1_Load(object sender, EventArgs e)
        {
            ToolTip t = new ToolTip();
            t.SetToolTip(TextBox, "Введите полный путь");
            t.SetToolTip(button1, "Открыть меню выбор файла");
            label1.Visible = false;
            for (Opacity = 0; Opacity < .95; Opacity += .03d)
            {
                await Task.Delay(5);
            }
        }
    // Диалоговое окно с выбором файла и запись пути в textbox
    void BunifuThinButton21_Click(object sender, EventArgs e)
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
        void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox.Text = TextBox.Text.Trim('"');
            // Проверка на наличие чего-либо в textbox
            if (TextBox.Text != "")
            {
                // Проверка файла на существование и выдача информации
                if (File.Exists(TextBox.Text))
                {
                    button.Enabled = true;
                    label1.Visible = true;
                    label1.Text = "Информация о файле:";
                    label2.Text = $"Создан: " + File.GetCreationTime(TextBox.Text) +
                                   "\n Открывался: " + File.GetLastAccessTime(TextBox.Text) +
                                   "\n Изменялся: " + File.GetLastWriteTime(TextBox.Text) +
                                   "\n Размер файла: " + (Math.Round(Convert.ToDouble(File.ReadAllBytes(TextBox.Text).Length) / 1048576.0, 2)) + " мбайт";
                }
                else if (Directory.Exists(TextBox.Text))
                {
                    button.Enabled = true;
                    label1.Visible = true;
                    label1.Text = "Информация о папке:";
                    DirectoryInfo FolderInfo = new DirectoryInfo(TextBox.Text);
                    label2.Text = "Создана: " + FolderInfo.CreationTime.ToString();
                }
                else
                {
                    label1.Visible = false;
                    label2.Text = null;
                    button.Enabled = false;
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

        void Button_Click(object sender, EventArgs e)
        {
            try
            {
                Del(TextBox.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Код ошибки скопирован в буфер. \n Отправьте его разработчику", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Clipboard.Clear();
                Clipboard.SetText(ex.ToString());
            }
        }

        void Button2_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string folderName = folderBrowserDialog1.SelectedPath;
                TextBox.Text = folderName;
            }
        }

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            if (button.Enabled == true)
            {
                button.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            }
            else
            {
                button.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            }
        }
    }
}
